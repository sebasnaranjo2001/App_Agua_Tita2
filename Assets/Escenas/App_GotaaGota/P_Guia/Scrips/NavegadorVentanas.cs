using UnityEngine;
using System.Collections.Generic;

public class NavegadorVentanas : MonoBehaviour
{
    [Header("Pantalla Base")]
    public GameObject panelGuiaPrincipal;

    private GameObject panelActual;

    // El "Stack" funciona como un historial de navegador de internet
    private Stack<GameObject> historial = new Stack<GameObject>();

    void Start()
    {
        // Al iniciar, asumimos que estamos en el panel principal
        panelActual = panelGuiaPrincipal;
    }

    // Pon esta función en los botones que abren una nueva ventana (Submenús o Contenidos)
    public void IrHaciaAdelante(GameObject nuevoPanel)
    {
        if (panelActual == null) return;

        // 1. Guardamos el panel actual en el historial
        historial.Push(panelActual);

        // 2. Apagamos el actual
        panelActual.SetActive(false);

        // 3. Encendemos el nuevo y lo marcamos como el actual
        nuevoPanel.SetActive(true);
        panelActual = nuevoPanel;

        // 4. Animamos el nuevo panel entero para que aparezca suavemente
        nuevoPanel.transform.localScale = Vector3.zero;
        LeanTween.cancel(nuevoPanel);
        LeanTween.scale(nuevoPanel, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
    }

    // Pon esta función en TODOS tus botones de "Atrás" o "Regresar" (la X o la flecha)
    public void IrHaciaAtras()
    {
        // Si hay ventanas guardadas en el historial...
        if (historial.Count > 0)
        {
            // Apagamos la ventana donde estamos parados
            panelActual.SetActive(false);

            // Sacamos la última ventana que visitamos del historial
            GameObject panelAnterior = historial.Pop();

            // La encendemos y la marcamos como actual
            panelAnterior.SetActive(true);
            panelActual = panelAnterior;

            // Le damos una animación de entrada para que se sienta fluido
            panelAnterior.transform.localScale = Vector3.zero;
            LeanTween.cancel(panelAnterior);
            LeanTween.scale(panelAnterior, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
        }
    }
}