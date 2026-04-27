using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Necesario para TextMesh Pro

public class ControladorCargaGota : MonoBehaviour
{
    [Header("Referencias de la Gota")]
    public Image gotaLiquido;
    public RectTransform tapaOndas;

    [Header("Referencia de Texto")]
    public TextMeshProUGUI textoCarga; // Solo para el objeto "CARGANDO"

    [Header("Configuración")]
    public string nombreEscenaMenu = "MenuPrincipal";
    public float tiempoMinimoCarga = 3f;

    void Start()
    {
        // Inicialización de seguridad
        if (gotaLiquido != null) gotaLiquido.fillAmount = 0;
        if (textoCarga != null) textoCarga.text = "CARGANDO";

        StartCoroutine(RutinaDeCarga());
    }

    IEnumerator RutinaDeCarga()
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscenaMenu);
        operacion.allowSceneActivation = false;

        float tiempoTranscurrido = 0f;

        // Variables para la animación de los puntos (...)
        float tiempoPuntos = 0f;
        int contadorPuntos = 0;

        while (tiempoTranscurrido < tiempoMinimoCarga)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoMinimoCarga;

            // 1. Animación de los puntos (0 a 3 puntos)
            tiempoPuntos += Time.deltaTime;
            if (tiempoPuntos >= 0.5f)
            {
                contadorPuntos++;
                if (contadorPuntos > 3) contadorPuntos = 0;

                string puntos = new string('.', contadorPuntos);
                if (textoCarga != null) textoCarga.text = "CARGANDO" + puntos;

                tiempoPuntos = 0f;
            }

            // 2. Llenado y Bamboleo de la gota
            if (gotaLiquido != null)
            {
                gotaLiquido.fillAmount = progreso;
                // Pequeño vaivén para dar sensación de agua
                float bamboleo = Mathf.Sin(Time.time * 4f) * 1.5f;
                gotaLiquido.transform.localRotation = Quaternion.Euler(0, 0, bamboleo);
            }

            // 3. Movimiento de la tapa (si existe)
            if (tapaOndas != null)
            {
                float alturaGota = gotaLiquido.rectTransform.rect.height;
                float nuevaY = (progreso * alturaGota) - (alturaGota / 2);
                tapaOndas.anchoredPosition = new Vector2(Mathf.Sin(Time.time * 5f) * 10f, nuevaY);
            }

            yield return null;
        }

        // --- Finalización ---
        if (gotaLiquido != null)
        {
            gotaLiquido.fillAmount = 1f;
            gotaLiquido.transform.localRotation = Quaternion.identity;
        }

        if (textoCarga != null) textoCarga.text = "¡LISTO!";

        yield return new WaitForSeconds(0.8f);
        operacion.allowSceneActivation = true;
    }
}