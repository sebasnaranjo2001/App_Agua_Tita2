using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ManejadorRegistro : MonoBehaviour
{
    public static ManejadorRegistro instance;

    [Header("Referencias de UI (Búsqueda Automática)")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;
    public Transform contenedorLista;

    [Header("Importante: Arrastra aquí el bloque azul")]
    public GameObject prefabMiembro;

    [Header("Datos Guardados")]
    public string nombreSeleccionado;
    public List<DatosMiembro> listaDeMiembros = new List<DatosMiembro>();

    // --- NUEVA ESTRUCTURA PARA CADA BAÑO ---
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
        // Aquí se guardarán los últimos 5 baños de este miembro
        public List<RegistroBano> historialBanos = new List<RegistroBano>();
    }

    [System.Serializable]
    public class ListaWrapper { public List<DatosMiembro> miembros = new List<DatosMiembro>(); }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CargarDatosDelTelefono();
            SceneManager.sceneLoaded += AlCargarEscena;
        }
        else { Destroy(gameObject); }
    }

    void OnDestroy() { SceneManager.sceneLoaded -= AlCargarEscena; }

    void Start() { RefrescarListaVisual(); }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        inputNombre = null;
        inputEdad = null;
        contenedorLista = null;

        inputNombre = BuscarEnTodaLaEscena<TMP_InputField>("InputNombre");
        inputEdad = BuscarEnTodaLaEscena<TMP_InputField>("InputEdad");

        GameObject objCont = BuscarObjetoAunApagado("ContenedorLista");
        if (objCont) contenedorLista = objCont.transform;

        if (contenedorLista != null)
        {
            RefrescarListaVisual();
        }
    }

    public void GuardarDatos()
    {
        if (inputNombre == null) inputNombre = BuscarEnTodaLaEscena<TMP_InputField>("InputNombre");
        if (inputEdad == null) inputEdad = BuscarEnTodaLaEscena<TMP_InputField>("InputEdad");

        if (inputNombre == null || inputEdad == null) return;
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text)) return;

        // Al crear uno nuevo, la lista de historial empieza vacía
        DatosMiembro nuevoMiembro = new DatosMiembro
        {
            nombre = inputNombre.text,
            edad = inputEdad.text,
            mejorTiempo = 0,
            historialBanos = new List<RegistroBano>()
        };

        listaDeMiembros.Add(nuevoMiembro);

        CrearItemEnLista(nuevoMiembro.nombre, nuevoMiembro.edad);
        if (Avisos.instance != null) Avisos.instance.NotificarMiembroGuardado();

        GuardarEnDisco();
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
            }
        }
    }

    // --- HERRAMIENTAS DE BÚSQUEDA ---
    T BuscarEnTodaLaEscena<T>(string nombre) where T : Component
    {
        GameObject obj = BuscarObjetoAunApagado(nombre);
        if (obj) return obj.GetComponent<T>();
        return null;
    }

    GameObject BuscarObjetoAunApagado(string nombre)
    {
        GameObject[] raices = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject r in raices)
        {
            Transform t = BuscarRecursivo(r.transform, nombre);
            if (t != null) return t.gameObject;
        }
        return null;
    }

    Transform BuscarRecursivo(Transform padre, string nombre)
    {
        if (padre.name == nombre) return padre;
        foreach (Transform hijo in padre)
        {
            Transform resultado = BuscarRecursivo(hijo, nombre);
            if (resultado != null) return resultado;
        }
        return null;
    }
}