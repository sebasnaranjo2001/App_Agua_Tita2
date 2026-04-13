using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ManejadorRegistro : MonoBehaviour
{
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

    // --- NUEVO: Esto se ejecuta al iniciar el juego ---
    void Start()
    {
        // Esto bloquea el teclado para que solo acepte números en la edad
        if (inputEdad != null)
        {
            inputEdad.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
    }

    public void GuardarDatos()
    {
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text))
        {
            Debug.LogWarning("¡Sebas, llena ambos campos!");
            return;
        }

        DatosMiembro nuevoMiembro = new DatosMiembro();
        nuevoMiembro.nombre = inputNombre.text;
        nuevoMiembro.edad = inputEdad.text;
        listaDeMiembros.Add(nuevoMiembro);

        CrearItemEnLista(nuevoMiembro.nombre, nuevoMiembro.edad);

        inputNombre.text = "";
        inputEdad.text = "";
    }

    void CrearItemEnLista(string nombre, string edad)
    {
        GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);

        // --- NUEVO: Mueve el nuevo miembro al principio de la lista (ARRIBA) ---
        nuevoItem.transform.SetAsFirstSibling();

        TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            textos[0].text = nombre;
            textos[1].text = edad + " años";
        }
    }
}