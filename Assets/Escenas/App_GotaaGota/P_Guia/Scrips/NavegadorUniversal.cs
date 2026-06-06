using UnityEngine;

public class NavegadorUniversal : MonoBehaviour
{
    [Header("--- NAVEGACIÓN ---")]
    [Tooltip("El panel donde estás parado ahora (Ej: Panel Baño)")]
    public GameObject panelOrigen;

    [Tooltip("El panel al que quieres ir (Ej: Panel Info Ducha)")]
    public GameObject panelDestino;

    [Header("--- ANIMACIONES AL ENTRAR (IR) ---")]
    [Tooltip("Arrastra aquí los textos, imágenes o botones que quieres animar en el Destino")]
    public GameObject[] itemsDestino;

    [Header("--- ANIMACIONES AL SALIR (REGRESAR) ---")]
    [Tooltip("Arrastra aquí los botones del Padre para que se vuelvan a animar al regresar")]
    public GameObject[] itemsOrigen;

    [Header("Configuración")]
    public float tiempoAnimacion = 0.4f;
    public float retrasoEntreItems = 0.08f;

    // --- IDA ---
    public void IrAlDestino()
    {
        if (panelDestino)
        {
            panelDestino.SetActive(true);
            panelDestino.transform.localScale = Vector3.one;
        }

        // LeanTween se encarga del retraso de forma global, sin Invokes
        AnimarLista(itemsDestino);

        if (panelOrigen) panelOrigen.SetActive(false);
    }

    // --- REGRESO ---
    public void RegresarAlOrigen()
    {
        if (panelOrigen)
        {
            panelOrigen.SetActive(true);
            panelOrigen.transform.localScale = Vector3.one;
        }

        AnimarLista(itemsOrigen);

        if (panelDestino) panelDestino.SetActive(false);
    }

    // --- MOTOR DE ANIMACIÓN ---
    private void AnimarLista(GameObject[] listaDeItems)
    {
        if (listaDeItems == null || listaDeItems.Length == 0) return;

        float delayAcumulado = 0.05f; // Micro-retraso inicial directo en LeanTween

        foreach (GameObject item in listaDeItems)
        {
            if (item != null)
            {
                item.SetActive(true);

                LeanTween.cancel(item);
                item.transform.localScale = Vector3.zero;

                LeanTween.scale(item, Vector3.one, tiempoAnimacion)
                    .setEase(LeanTweenType.easeOutBack)
                    .setDelay(delayAcumulado);

                delayAcumulado += retrasoEntreItems;
            }
        }
    }
}