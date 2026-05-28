using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [Header("Configuración del Prefab")]
    public GameObject prefabMiembro;

    [Header("Datos en Memoria")]
    public string nombreSeleccionado;
    public List<DatosMiembro> listaDeMiembros = new List<DatosMiembro>();

    [System.Serializable]
    public class RegistroBano
    {
        public float duracion;
        public string fecha;
        public string hora;
    }

    [System.Serializable]
    public class DatosMiembro
    {
        public string nombre;
        public string edad;
        public float mejorTiempo;
        public List<RegistroBano> historialBanos = new List<RegistroBano>();
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

    public void GuardarDatos()
    {
        // Si está vacío, el código se detiene aquí y no hace nada más.
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text)) return;

        DatosMiembro nuevoMiembro = new DatosMiembro
        {
            nombre = inputNombre.text,
            edad = inputEdad.text,
            mejorTiempo = 0,
            historialBanos = new List<RegistroBano>()
        };

        listaDeMiembros.Add(nuevoMiembro);

        CrearItemEnLista(nuevoMiembro);
        GuardarEnDisco();

        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();

        inputNombre.text = "";
        inputEdad.text = "";
        ActualizarTextoContador();

        ValidarCamposLlenos();

        // --- NUEVO: Cerramos el panel deslizable SOLO si se guardó con éxito ---
        if (Avisos.instance != null && Avisos.instance.navegador != null)
        {
            Avisos.instance.navegador.CerrarTarjetaRegistro();
        }
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
        nuevoItem.transform.SetAsFirstSibling();
        nuevoItem.name = miembro.nombre;

        SeleccionMiembros tarjeta = nuevoItem.GetComponent<SeleccionMiembros>();

        if (tarjeta != null)
        {
            if (tarjeta.textoNombre != null) tarjeta.textoNombre.text = miembro.nombre;
            if (tarjeta.textoEdad != null) tarjeta.textoEdad.text = miembro.edad + " años";

            int totalBanos = (miembro.historialBanos != null) ? miembro.historialBanos.Count : 0;
            if (tarjeta.textoDuchas != null) tarjeta.textoDuchas.text = "Duchas totales:\n" + totalBanos.ToString();
        }
    }

    public void RemoverMiembroDeLaLista(string nombreBuscado)
    {
        listaDeMiembros.RemoveAll(m => m.nombre == nombreBuscado);
        GuardarEnDisco();
        RefrescarListaVisual();

        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();
        ActualizarTextoContador();
    }

    void ActualizarTextoContador()
    {
        if (textoContadorMiembros != null)
        {
            int total = listaDeMiembros.Count;
            if (total == 1) textoContadorMiembros.text = "1 Miembro";
            else textoContadorMiembros.text = total + " Miembros";
        }
    }

    public void GuardarEnDisco()
    {
        ListaWrapper wrapper = new ListaWrapper { miembros = listaDeMiembros };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("ListaUsuarios", json);

        bool hayAlMenosUnBano = false;
        foreach (DatosMiembro m in listaDeMiembros)
        {
            if (m.historialBanos != null && m.historialBanos.Count > 0)
            {
                hayAlMenosUnBano = true;
                break;
            }
        }

        PlayerPrefs.SetInt("HayDatosDucha", hayAlMenosUnBano ? 1 : 0);
        PlayerPrefs.Save();
    }

    void CargarDatosDelTelefono()
    {
        if (PlayerPrefs.HasKey("ListaUsuarios"))
        {
            string json = PlayerPrefs.GetString("ListaUsuarios");
            ListaWrapper wrapper = JsonUtility.FromJson<ListaWrapper>(json);
            if (wrapper != null && wrapper.miembros != null) listaDeMiembros = wrapper.miembros;
        }
    }
}