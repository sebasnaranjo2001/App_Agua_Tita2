using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class ManejadorRegistro : MonoBehaviour
{
    public static ManejadorRegistro instance;

    [Header("Referencias de UI")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;
    public Transform contenedorLista;
    public TMP_Text textoContadorMiembros;
    public Button botonGuardar;
    public ScrollRect scrollRectRegistro;

    [Header("Configuración del Prefab")]
    public GameObject prefabMiembro;

    [Header("--- BOTONES DE COLORES (VISUAL) ---")]
    public RectTransform[] botonesColores;

    [Header("Datos en Memoria")]
    public string nombreSeleccionado;
    public List<DatosMiembro> listaDeMiembros = new List<DatosMiembro>();

    private int colorSeleccionadoTemporal = 0;

    [System.Serializable]
    public class RegistroBano { public float duracion; public string fecha; public string hora; }

    [System.Serializable]
    public class DatosMiembro
    {
        public string idUnico;
        public List<string> idsAsociados = new List<string>();
        public string nombre;
        public string edad;
        public float mejorTiempo;
        public int indiceTemaColor;
        public List<RegistroBano> historialBanos = new List<RegistroBano>();
        public string ultimoRetoAceptado = "";
    }

    [System.Serializable]
    public class ListaWrapper { public List<DatosMiembro> miembros = new List<DatosMiembro>(); }

    void Awake()
    {
        if (instance == null) { instance = this; CargarDatosDelTelefono(); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        RefrescarListaVisual();
        ConfigurarInputs();
        ValidarCamposLlenos();
        SeleccionarColorParaNuevoMiembro(0);
    }

    void OnEnable()
    {
        StartCoroutine(ResetearScrollAlInicio());
    }

    IEnumerator ResetearScrollAlInicio()
    {
        yield return new WaitForEndOfFrame();
        if (scrollRectRegistro != null) scrollRectRegistro.verticalNormalizedPosition = 1f;
    }

    public void ActualizarRanking()
    {
        CargarDatosDelTelefono();
        RefrescarListaVisual();
    }

    void ConfigurarInputs()
    {
        if (inputEdad != null)
        {
            inputEdad.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputEdad.characterLimit = 3;
            inputEdad.onValueChanged.AddListener(delegate { ValidarCamposLlenos(); });
        }

        if (inputNombre != null)
        {
            inputNombre.characterLimit = 10;
            inputNombre.onValueChanged.AddListener(delegate { ValidarNombre(); ValidarCamposLlenos(); });
        }
    }

    void ValidarNombre()
    {
        if (inputNombre.text.Length > 0)
        {
            string texto = inputNombre.text;
            inputNombre.text = char.ToUpper(texto[0]) + texto.Substring(1);
        }
    }

    public void ValidarCamposLlenos()
    {
        if (botonGuardar != null && inputNombre != null && inputEdad != null)
        {
            bool tieneNombre = !string.IsNullOrWhiteSpace(inputNombre.text);
            bool tieneEdad = !string.IsNullOrWhiteSpace(inputEdad.text);
            botonGuardar.interactable = (tieneNombre && tieneEdad);
        }
    }

    public void SeleccionarColorParaNuevoMiembro(int indiceColor)
    {
        colorSeleccionadoTemporal = indiceColor;
        if (botonesColores != null && botonesColores.Length > 0)
        {
            for (int i = 0; i < botonesColores.Length; i++)
            {
                if (botonesColores[i] != null)
                {
                    LeanTween.cancel(botonesColores[i].gameObject);
                    if (i == indiceColor) LeanTween.scale(botonesColores[i].gameObject, Vector3.one * 1.3f, 0.25f).setEase(LeanTweenType.easeOutBack);
                    else LeanTween.scale(botonesColores[i].gameObject, Vector3.one * 1.0f, 0.2f).setEase(LeanTweenType.easeOutQuad);
                }
            }
        }
    }

    public void GuardarDatos()
    {
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text)) return;

        foreach (DatosMiembro m in listaDeMiembros)
        {
            if (m.nombre.ToLower() == inputNombre.text.ToLower() && m.edad == inputEdad.text)
            {
                if (Avisos.instance != null && Avisos.instance.popMiembroDuplicado != null)
                    Avisos.instance.MostrarAvisoPopUp(Avisos.instance.popMiembroDuplicado);
                return;
            }
        }

        DatosMiembro nuevoMiembro = new DatosMiembro
        {
            idUnico = System.Guid.NewGuid().ToString(),
            idsAsociados = new List<string>(),
            nombre = inputNombre.text,
            edad = inputEdad.text,
            mejorTiempo = 0,
            indiceTemaColor = colorSeleccionadoTemporal,
            historialBanos = new List<RegistroBano>(),
            ultimoRetoAceptado = ""
        };

        listaDeMiembros.Add(nuevoMiembro);
        CrearItemEnLista(nuevoMiembro);
        GuardarEnDisco();

        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();
        inputNombre.text = "";
        inputEdad.text = "";
        SeleccionarColorParaNuevoMiembro(0);
        ActualizarTextoContador();
        ValidarCamposLlenos();

        if (Avisos.instance != null && Avisos.instance.navegador != null)
            Avisos.instance.navegador.CerrarTarjetaRegistro();
    }

    public void RefrescarListaVisual()
    {
        if (contenedorLista == null) return;
        foreach (Transform hijo in contenedorLista) { Destroy(hijo.gameObject); }
        foreach (DatosMiembro m in listaDeMiembros) { CrearItemEnLista(m); }
        ActualizarTextoContador();
    }

    void CrearItemEnLista(DatosMiembro miembro)
    {
        if (contenedorLista == null || prefabMiembro == null) return;
        GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);
        nuevoItem.transform.SetAsLastSibling();
        nuevoItem.name = miembro.nombre;

        SeleccionMiembros tarjeta = nuevoItem.GetComponent<SeleccionMiembros>();
        if (tarjeta != null)
        {
            if (tarjeta.textoNombre != null) tarjeta.textoNombre.text = miembro.nombre;
            if (tarjeta.textoEdad != null) tarjeta.textoEdad.text = miembro.edad + " años";
            int totalBanos = (miembro.historialBanos != null) ? miembro.historialBanos.Count : 0;
            if (tarjeta.textoDuchas != null) tarjeta.textoDuchas.text = "Duchas totales:\n" + totalBanos.ToString();
            tarjeta.AplicarTema(miembro.indiceTemaColor);
            if (!string.IsNullOrEmpty(nombreSeleccionado) && miembro.nombre == nombreSeleccionado)
                tarjeta.SeleccionarEsteMiembro();
        }
    }

    public void RemoverMiembroDeLaLista(string nombreBuscado)
    {
        listaDeMiembros.RemoveAll(m => m.nombre == nombreBuscado);

        // --- NUEVA SEGURIDAD: Deseleccionarlo de la memoria si estaba seleccionado ---
        if (nombreSeleccionado == nombreBuscado) nombreSeleccionado = "";

        GuardarEnDisco();
        RefrescarListaVisual();
        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();
        ActualizarTextoContador();

        // --- SOLUCIÓN: Obligamos al panel de Ranking a actualizarse instantáneamente ---
        ManejadorRanking ranking = UnityEngine.Object.FindFirstObjectByType<ManejadorRanking>();
        if (ranking != null)
        {
            ranking.GenerarRanking();
        }
    }

    void ActualizarTextoContador()
    {
        if (textoContadorMiembros != null)
        {
            int total = listaDeMiembros.Count;
            textoContadorMiembros.text = (total == 1) ? "1 Miembro" : total + " Miembros";
        }
    }

    public void GuardarEnDisco()
    {
        ListaWrapper wrapper = new ListaWrapper { miembros = listaDeMiembros };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("ListaUsuarios", json);
        PlayerPrefs.Save();
    }

    void CargarDatosDelTelefono()
    {
        if (PlayerPrefs.HasKey("ListaUsuarios"))
        {
            string json = PlayerPrefs.GetString("ListaUsuarios");
            ListaWrapper wrapper = JsonUtility.FromJson<ListaWrapper>(json);
            if (wrapper != null && wrapper.miembros != null)
            {
                listaDeMiembros = wrapper.miembros;
                foreach (var m in listaDeMiembros)
                {
                    if (string.IsNullOrEmpty(m.idUnico)) m.idUnico = System.Guid.NewGuid().ToString();
                    if (m.idsAsociados == null) m.idsAsociados = new List<string>();
                }
            }
        }
    }
}