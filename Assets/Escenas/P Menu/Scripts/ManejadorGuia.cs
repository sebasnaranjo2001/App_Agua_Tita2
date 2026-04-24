using UnityEngine;
using UnityEngine.UI;

public class ManejadorGuia : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject[] avisos; // Arrastra tus 3 avisos aquí
    public GameObject botonSiguiente;
    public GameObject botonAnterior;

    private int indiceActual;

    void OnEnable() // Se ejecuta cada vez que el panel de Guía se activa
    {
        // 1. Elegimos un aviso al azar para empezar
        indiceActual = Random.Range(0, avisos.Length);
        ActualizarPantalla();
    }

    public void IrSiguiente()
    {
        if (indiceActual < avisos.Length - 1)
        {
            indiceActual++;
            ActualizarPantalla();
        }
    }

    public void IrAnterior()
    {
        if (indiceActual > 0)
        {
            indiceActual--;
            ActualizarPantalla();
        }
    }

    void ActualizarPantalla()
    {
        // Apagamos todos los avisos y encendemos solo el actual
        for (int i = 0; i < avisos.Length; i++)
        {
            avisos[i].SetActive(i == indiceActual);
        }

        // Lógica de botones que pediste:
        // Si es el primero (0), solo siguiente.
        // Si es el último, solo atrás.
        // Si es el del medio, ambos.

        if (indiceActual == 0) // Primer aviso
        {
            botonAnterior.SetActive(false);
            botonSiguiente.SetActive(true);
        }
        else if (indiceActual == avisos.Length - 1) // Último aviso
        {
            botonAnterior.SetActive(true);
            botonSiguiente.SetActive(false);
        }
        else // Avisos intermedios
        {
            botonAnterior.SetActive(true);
            botonSiguiente.SetActive(true);
        }
    }
}