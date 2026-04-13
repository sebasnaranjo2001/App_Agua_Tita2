using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public partial class LoadingScreen : MonoBehaviour
{
    [Header("Configuración de UI")]
    public Image fillImage; // Aquí va tu imagen roja "Fill"

    [Header("Configuración de Escena")]
    public string nombreEscenaACargar;
    public float tiempoMinimoCarga = 2f; // El tiempo que quieres que dure (2 segundos)

    void Start()
    {
        // Iniciamos la carga
        StartCoroutine(CargarEscenaAsync());
    }

    IEnumerator CargarEscenaAsync()
    {
        // 1. Empezamos a cargar la escena en segundo plano
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscenaACargar);

        // Evitamos que la escena se active sola apenas termine
        operacion.allowSceneActivation = false;

        float tiempoTranscurrido = 0f;

        // 2. Mientras el tiempo sea menor a 2 segundos o la carga no termine...
        while (tiempoTranscurrido < tiempoMinimoCarga || operacion.progress < 0.9f)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Calculamos el progreso visual (de 0 a 1) basado en el tiempo
            float progresoVisual = tiempoTranscurrido / tiempoMinimoCarga;

            // Actualizamos la barra (limitada a 1)
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(progresoVisual);
            }

            yield return null;
        }

        // 3. Cuando pasen los 2 segundos, terminamos de llenar y activamos la escena
        if (fillImage != null) fillImage.fillAmount = 1f;
        yield return new WaitForSeconds(0.2f); // Un respiro visual al final

        operacion.allowSceneActivation = true;
    }
}