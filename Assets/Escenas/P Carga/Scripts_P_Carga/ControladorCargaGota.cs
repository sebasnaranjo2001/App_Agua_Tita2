using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ControladorCargaGota : MonoBehaviour
{
    [Header("Referencias de la Gota")]
    public Image gotaLiquido;
    public RectTransform tapaOndas;
    public RectTransform logoApp;

    [Header("Referencia de Texto")]
    public TextMeshProUGUI textoCarga;

    [Header("Animación de Salida (Pum y Transición)")]
    public GameObject[] objetosDesaparecer;

    // VARIABLES ABIERTAS PARA LOS DOS FONDOS
    public Image fondoCarga;  // El fondo oscuro de carga (se va a desvanecer)
    public Image fondoInicio; // El fondo del menú de inicio (se va a quedar)

    [Header("Configuración")]
    public string nombreEscenaMenu = "MenuPrincipal";
    public float tiempoMinimoCarga = 3f;

    [Header("Ajustes del Latido")]
    public float velocidadLatido = 4f;
    public float fuerzaLatido = 0.015f;

    void Start()
    {
        if (gotaLiquido != null) gotaLiquido.fillAmount = 0;
        if (textoCarga != null) textoCarga.text = "CARGANDO";

        // Aseguramos que ambos fondos estén encendidos al iniciar
        if (fondoCarga != null) fondoCarga.gameObject.SetActive(true);
        if (fondoInicio != null) fondoInicio.gameObject.SetActive(true);

        StartCoroutine(RutinaDeCarga());
    }

    IEnumerator RutinaDeCarga()
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscenaMenu);
        operacion.allowSceneActivation = false;

        float tiempoTranscurrido = 0f;
        float tiempoPuntos = 0f;
        int contadorPuntos = 0;

        while (tiempoTranscurrido < tiempoMinimoCarga)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / tiempoMinimoCarga;

            // 1. Animación de los puntos
            tiempoPuntos += Time.deltaTime;
            if (tiempoPuntos >= 0.5f)
            {
                contadorPuntos++;
                if (contadorPuntos > 3) contadorPuntos = 0;

                string puntos = new string('.', contadorPuntos);
                if (textoCarga != null) textoCarga.text = "CARGANDO" + puntos;

                tiempoPuntos = 0f;
            }

            // 2. Llenado y Bamboleo
            if (gotaLiquido != null)
            {
                gotaLiquido.fillAmount = progreso;
                float bamboleo = Mathf.Sin(Time.time * 4f) * 1.5f;
                gotaLiquido.transform.localRotation = Quaternion.Euler(0, 0, bamboleo);
            }

            // 3. Movimiento de la tapa
            if (tapaOndas != null)
            {
                float alturaGota = gotaLiquido.rectTransform.rect.height;
                float nuevaY = (progreso * alturaGota) - (alturaGota / 2);
                tapaOndas.anchoredPosition = new Vector2(Mathf.Sin(Time.time * 5f) * 10f, nuevaY);
            }

            // 4. Palpitar del Logo
            if (logoApp != null)
            {
                float escalaLatido = 1f + (Mathf.Sin(Time.time * velocidadLatido) * fuerzaLatido);
                logoApp.localScale = new Vector3(escalaLatido, escalaLatido, 1f);
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
        if (logoApp != null) logoApp.localScale = Vector3.one;

        // --- Efecto de Salida: Los elementos hacen "Pum" hacia adentro ---
        for (int i = 0; i < objetosDesaparecer.Length; i++)
        {
            if (objetosDesaparecer[i] != null)
            {
                LeanTween.scale(objetosDesaparecer[i], Vector3.zero, 0.4f)
                         .setEase(LeanTweenType.easeInBack)
                         .setDelay(i * 0.05f);
            }
        }

        // --- NUEVA TRANSICIÓN FLUIDA ENTRE FONDOS ---
        // Desvanecemos el fondo de carga a Opacidad 0 para que se revele el fondo de inicio que está abajo
        if (fondoCarga != null)
        {
            LeanTween.alpha(fondoCarga.rectTransform, 0f, 0.6f).setEase(LeanTweenType.easeInOutQuad);
        }

        yield return new WaitForSeconds(0.8f);
        operacion.allowSceneActivation = true;
    }
}