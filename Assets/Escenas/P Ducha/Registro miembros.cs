using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ManejadorRegistro : MonoBehaviour
{
    // Permitimos que otros scripts accedan a este manejador
    public static ManejadorRegistro instance;

    [Header("Configuración de Entrada")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;

    [Header("Configuración de Lista")]
    public GameObject prefabMiembro;
    public Transform contenedorLista;

    [System.Serializable]
    public class DatosMiembro
    {
        public string nombre;
        public string edad;
    }

    public List<DatosMiembro> listaDeMiembros = new List<DatosMiembro>();

    void Awake()
    {
        // Configuramos la instancia para el equipo
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (inputEdad != null)
        {
            inputEdad.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
    }

    public void GuardarDatos()
    {
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text))
        {
            Debug.LogWarning("¡Sebas, hay que llenar ambos campos!");
            return;
        }

        DatosMiembro nuevoMiembro = new DatosMiembro();
        nuevoMiembro.nombre = inputNombre.text;
        nuevoMiembro.edad = inputEdad.text;
        listaDeMiembros.Add(nuevoMiembro);

        CrearItemEnLista(nuevoMiembro.nombre, nuevoMiembro.edad);

        // Notificamos al sistema de Avisos que el grupo tiene un nuevo integrante
        if (Avisos.instance != null)
        {
            Avisos.instance.NotificarMiembroGuardado();
        }

        inputNombre.text = "";
        inputEdad.text = "";
    }

    void CrearItemEnLista(string nombre, string edad)
    {
        GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);
        nuevoItem.transform.SetAsFirstSibling();

        // IMPORTANTE: Ponemos el nombre del miembro al objeto para poder borrarlo luego
        nuevoItem.name = nombre;

        TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            textos[0].text = nombre;
            textos[1].text = edad + " años";
        }
    }

    // --- NUEVA FUNCIÓN PARA ELIMINAR DATOS ---
    public void RemoverMiembroDeLaLista(string nombreBuscado)
    {
        // Buscamos en nuestra lista y eliminamos los datos que coincidan con el nombre
        listaDeMiembros.RemoveAll(m => m.nombre == nombreBuscado);
        Debug.Log("Datos eliminados de la lista interna para: " + nombreBuscado);
    }
}