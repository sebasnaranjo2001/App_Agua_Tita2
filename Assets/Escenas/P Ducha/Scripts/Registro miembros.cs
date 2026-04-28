using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ManejadorRegistro : MonoBehaviour
{
    public static ManejadorRegistro instance;

    [Header("Referencias de UI")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;
    public Transform contenedorLista;

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
    }

    void ConfigurarInputs()
    {
        if (inputEdad != null)
        {
            inputEdad.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputEdad.characterLimit = 3;
        }

        if (inputNombre != null)
        {
            inputNombre.characterLimit = 10;
            inputNombre.onValueChanged.AddListener(delegate { ValidarNombre(); });
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

    public void GuardarDatos()
    {
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text)) return;

        DatosMiembro nuevoMiembro = new DatosMiembro
        {
            nombre = inputNombre.text,
            edad = inputEdad.text,
            mejorTiempo = 0,
            historialBanos = new List<RegistroBano>()
        };

        listaDeMiembros.Add(nuevoMiembro);
        CrearItemEnLista(nuevoMiembro.nombre, nuevoMiembro.edad);
        GuardarEnDisco();

        if (Avisos.instance != null)
        {
            Avisos.instance.NotificarMiembroGuardado();
        }

        inputNombre.text = "";
        inputEdad.text = "";
    }

    public void RefrescarListaVisual()
    {
        if (contenedorLista == null) return;
        foreach (Transform hijo in contenedorLista) { Destroy(hijo.gameObject); }
        foreach (DatosMiembro m in listaDeMiembros) { CrearItemEnLista(m.nombre, m.edad); }
    }

    void CrearItemEnLista(string nombre, string edad)
    {
        if (contenedorLista == null || prefabMiembro == null) return;
        GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);
        nuevoItem.transform.SetAsFirstSibling();
        nuevoItem.name = nombre;

        TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();
        if (textos.Length >= 2)
        {
            textos[0].text = nombre;
            textos[1].text = edad + " años";
        }
    }

    public void RemoverMiembroDeLaLista(string nombreBuscado)
    {
        listaDeMiembros.RemoveAll(m => m.nombre == nombreBuscado);
        GuardarEnDisco();
        RefrescarListaVisual();

        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();
    }

    public void GuardarEnDisco()
    {
        ListaWrapper wrapper = new ListaWrapper { miembros = listaDeMiembros };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("ListaUsuarios", json);

        // --- NUEVA LÓGICA PARA EL SWIPER (Duchómetro) ---
        // Revisamos si existe al menos un baño registrado en toda la lista de miembros
        bool hayAlMenosUnBano = false;
        foreach (DatosMiembro m in listaDeMiembros)
        {
            if (m.historialBanos != null && m.historialBanos.Count > 0)
            {
                hayAlMenosUnBano = true;
                break; // Con uno que encontremos es suficiente
            }
        }

        // Guardamos la señal para el script ManejadorSwiperHorizontal
        PlayerPrefs.SetInt("HayDatosDucha", hayAlMenosUnBano ? 1 : 0);
        // ------------------------------------------------

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