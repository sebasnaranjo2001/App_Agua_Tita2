using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ManejadorRegistro : MonoBehaviour
{
    [Header("Configuración de Entrada")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputEdad;

    [Header("Configuración de Lista")]
    public GameObject prefabMiembro; // Aquí arrastrarás el cubito azul
    public Transform contenedorLista; // Aquí arrastrarás el panel "ContenedorLista"

    // Clase para guardar los datos (Base de datos interna)
    [System.Serializable]
    public class DatosMiembro
    {
        public string nombre;
        public string edad;
    }

    public List<DatosMiembro> listaDeMiembros = new List<DatosMiembro>();

    public void GuardarDatos()
    {
        // 1. Validar que no falten datos
        if (string.IsNullOrEmpty(inputNombre.text) || string.IsNullOrEmpty(inputEdad.text))
        {
            Debug.LogWarning("¡Sebas, llena ambos campos!");
            return;
        }

        // 2. Crear y guardar en la lista de datos
        DatosMiembro nuevoMiembro = new DatosMiembro();
        nuevoMiembro.nombre = inputNombre.text;
        nuevoMiembro.edad = inputEdad.text;
        listaDeMiembros.Add(nuevoMiembro);

        // 3. Crear visualmente el objeto en la lista
        CrearItemEnLista(nuevoMiembro.nombre, nuevoMiembro.edad);

        // 4. Limpiar los campos para el siguiente
        inputNombre.text = "";
        inputEdad.text = "";
    }

    void CrearItemEnLista(string nombre, string edad)
    {
        // Instanciamos el prefab dentro del contenedor
        GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);

        // Buscamos todos los componentes de texto que hay dentro del prefab
        // Importante: El código detectará los textos en el orden en que estén en la jerarquía
        TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();

        if (textos.Length >= 2)
        {
            textos[0].text = nombre;        // El primer texto será el nombre
            textos[1].text = edad + " años"; // El segundo texto será la edad
        }
    }
}