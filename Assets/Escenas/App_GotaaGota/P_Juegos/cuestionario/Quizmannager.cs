using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Pregunta
    {
        public string pregunta;
        public string[] respuestas;
        public int respuestaCorrecta;
        public Sprite imagen;
    }

    [Header("Preguntas")]
    public Pregunta[] preguntas;

    [Header("UI")]
    public TMP_Text textoPregunta;
    public TMP_Text textoAciertos;
    public TMP_Text textoProgresoPreguntas;
    public Image imagenPregunta;
    public Button[] botones;
    public RectTransform fondoPregunta;
    public ProgressBarUI barraProgreso;

    [Header("Gotas Victoria")]
    public GameObject gotaVictoria1;
    public GameObject gotaVictoria2;
    public GameObject gotaVictoria3;

    [Header("Gotas Intermedio")]
    public GameObject gotaIntermedio1;
    public GameObject gotaIntermedio2;
    public GameObject gotaIntermedio3;

    [Header("Gotas Derrota")]
    public GameObject gotaDerrota1;
    public GameObject gotaDerrota2;
    public GameObject gotaDerrota3;

    [Header("Elementos Victoria")]
    public GameObject[] elementosVictoria;

    [Header("Elementos Intermedio")]
    public GameObject[] elementosIntermedio;

    [Header("Elementos Derrota")]
    public GameObject[] elementosDerrota;

    [Header("Elementos Gameplay")]
    public GameObject[] elementosGameplay;

    [Header("Tiempo en Paneles")]
    public TMP_Text textoTiempoVictoria;
    public TMP_Text textoTiempoIntermedio;
    public TMP_Text textoTiempoDerrota;

    [Header("Musicas Paneles")]
    public AudioClip musicaVictoria;
    public AudioClip musicaIntermedio;
    public AudioClip musicaDerrota;



    [Header("Timer")]
    public float tiempoMaximo = 60f;
    float tiempoActual;
    bool tiempoActivo = true;

    public Image fillReloj;
    public TMP_Text textoTiempo;
    public RectTransform relojTransform;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip musicaGameplay;
    public AudioClip musicaTension;
    public AudioClip sonidoAcierto;
    public AudioClip sonidoError;
    public AudioClip sonidoTick;

    bool musicaTensionActiva = false;
    bool alarmaReproducida = false;
    public AudioClip alarmaTiempo;

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

    [Header("Textos de Aciertos por Panel")]
    public TMP_Text textoAciertosVictoria;
    public TMP_Text textoAciertosIntermedio;
    public TMP_Text textoAciertosDerrota;



    int indicePregunta = 0;
    int aciertos = 0;
    int respuestaCorrectaActual;

    Vector2 posFondoOriginal;
    Vector3 posRelojOriginal;

    void Start()
    {
        tiempoActual = tiempoMaximo;
        if (barraProgreso != null)
        {
            barraProgreso.ReiniciarBarra();
            barraProgreso.textoEstado.text = "Pregunta 1 de " + preguntas.Length;
        }


        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelIntermedio) panelIntermedio.SetActive(false);
        if (panelDerrota) panelDerrota.SetActive(false);


        if (musicSource != null && musicaGameplay != null)
        {
            musicSource.clip = musicaGameplay;
            musicSource.loop = true;
            musicSource.Play();
        }

        posFondoOriginal = fondoPregunta.anchoredPosition;

        // 🎬 ANIMACIÓN RELOJ ENTRADA
        posRelojOriginal = relojTransform.localPosition;
        relojTransform.localScale = Vector3.zero;

        LeanTween.scale(relojTransform.gameObject, Vector3.one, 0.5f)
            .setEaseOutBack();

        MostrarPregunta();
        ActualizarTextoAciertos();
    }

    void Update()
    {
        ActualizarCronometro();
    }

    // ================= TIMER =================
    void ActualizarCronometro()
    {
        if (!tiempoActivo) return;

        tiempoActual -= Time.deltaTime;
        if (tiempoActual < 0) tiempoActual = 0;

        if (fillReloj) fillReloj.fillAmount = 1f - (tiempoActual / tiempoMaximo);



        if (textoTiempo)
        {
            int minutos = Mathf.FloorToInt(tiempoActual / 60);
            int segundos = Mathf.FloorToInt(tiempoActual % 60);
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
            // Sonido tic-tac en los últimos 10 segundos
            if (tiempoActual <= 10f && tiempoActual > 0f)
            {
                sfxSource?.PlayOneShot(sonidoTick);
            }

        }

        // 🎵 Música de tensión
        if (tiempoActual <= 10f && !musicaTensionActiva)
        {
            musicaTensionActiva = true;
            musicSource.clip = musicaTension;
            musicSource.loop = true;
            musicSource.Play();
        }

        // 🔥 SHAKE RELOJ
        if (tiempoActual <= 5f)
        {
            float shake = Mathf.Sin(Time.time * 40f) * 3f;
            relojTransform.localPosition = posRelojOriginal + new Vector3(shake, 0, 0);
        }
        else
        {
            relojTransform.localPosition = posRelojOriginal;
        }

        // 🚨 Alarma al acabar el tiempo
        if (tiempoActual <= 0 && !alarmaReproducida)
        {
            alarmaReproducida = true;
            tiempoActivo = false;

            sfxSource?.PlayOneShot(alarmaTiempo);

            LeanTween.delayedCall(alarmaTiempo.length, () =>
            {
                MostrarResultadoFinal();
                panelDerrota.SetActive(true);
            });
        }
    }


    // ================= PREGUNTA =================
    void MostrarPregunta()
    {
        var p = preguntas[indicePregunta];

        textoPregunta.text = p.pregunta;

        // 📊 progreso
        if (textoProgresoPreguntas != null)
            textoProgresoPreguntas.text = (indicePregunta + 1) + " / " + preguntas.Length;

        if (barraProgreso != null)
        {
            barraProgreso.Actualizar(indicePregunta + 1, preguntas.Length, "Pregunta");
            barraProgreso.textoEstado.text = "Pregunta " + (indicePregunta + 1) + " de " + preguntas.Length;
        }




        if (imagenPregunta != null)
        {
            imagenPregunta.sprite = p.imagen;
            imagenPregunta.transform.localScale = Vector3.zero;
            LeanTween.scale(imagenPregunta.gameObject, Vector3.one, 0.45f).setEaseOutBack();
        }




        // 🎬 FONDO ANIMACIÓN ENTRADA
        fondoPregunta.anchoredPosition =
            new Vector2(posFondoOriginal.x - 1200, posFondoOriginal.y);

        LeanTween.move(fondoPregunta, posFondoOriginal, 0.5f)
            .setEaseOutCubic();

        // 🎬 BOTONES
        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].image.color = Color.white;
            botones[i].interactable = true;

            int idx = i;

            if (i < p.respuestas.Length)
            {
                botones[i].gameObject.SetActive(true);
                botones[i].GetComponentInChildren<TMP_Text>().text = p.respuestas[i];
            }
            else
            {
                botones[i].gameObject.SetActive(false);
                continue;
            }

            botones[i].onClick.RemoveAllListeners();
            botones[i].onClick.AddListener(() => Responder(idx));

            // 🎬 STAGGER ANIMACIÓN
            botones[i].transform.localScale = Vector3.zero;

            LeanTween.scale(botones[i].gameObject, Vector3.one, 0.3f)
                .setDelay(i * 0.1f)
                .setEaseOutBack();
        }

        respuestaCorrectaActual = p.respuestaCorrecta;
    }

    // ================= RESPUESTA =================
    void Responder(int i)
    {
        foreach (var b in botones) b.interactable = false;

        if (i == respuestaCorrectaActual)
        {
            aciertos++;
            sfxSource?.PlayOneShot(sonidoAcierto);
            botones[i].image.color = Color.green;
        }
        else
        {
            sfxSource?.PlayOneShot(sonidoError);
            botones[i].image.color = Color.red;
            botones[respuestaCorrectaActual].image.color = Color.green;
        }


        ActualizarTextoAciertos();
        Invoke(nameof(SiguientePregunta), 1.2f);
    }

    void SiguientePregunta()
    {
        indicePregunta++;

        if (indicePregunta < preguntas.Length)
        {
            MostrarPregunta();
        }
        else
        {
            MostrarResultadoFinal();
        }

    }

    void ActualizarTextoAciertos()
    {
        if (textoAciertos != null)
            textoAciertos.text = "Aciertos: " + aciertos;
    }

    void MostrarResultadoFinal()
    {
        tiempoActivo = false;
        MostrarTiempoEnPaneles();

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null) obj.SetActive(false);
        }

        string resultadoFinal = "Aciertos: " + aciertos + "/" + preguntas.Length;


        if (aciertos == preguntas.Length)
        {
            AnimarPanelVictoria();
            textoAciertosVictoria.text = resultadoFinal;

        }
        else if (aciertos <= 1)
        {
            AnimarPanelDerrota();
            textoAciertosDerrota.text = resultadoFinal;

        }
        else
        {
            AnimarPanelIntermedio();
            textoAciertosIntermedio.text = resultadoFinal;

        }
    }

    void MostrarTiempoEnPaneles()
    {
        int tiempoUsado = Mathf.RoundToInt(tiempoMaximo - tiempoActual);
        string textoFinal = tiempoUsado + " segundos";

        if (textoTiempoVictoria != null) textoTiempoVictoria.text = textoFinal;
        if (textoTiempoIntermedio != null) textoTiempoIntermedio.text = textoFinal;
        if (textoTiempoDerrota != null) textoTiempoDerrota.text = textoFinal;
    }

    void AnimarElemento(GameObject obj, float delay)
    {
        if (obj == null) return;
        obj.SetActive(true);
        obj.transform.localScale = Vector3.zero;
        LeanTween.scale(obj, Vector3.one, 0.35f).setDelay(delay).setEaseOutBack();
    }

    void AnimarElementosPanel(GameObject[] elementos)
    {
        if (elementos == null) return;
        for (int i = 0; i < elementos.Length; i++)
        {
            if (elementos[i] == null) continue;
            elementos[i].SetActive(true);
            elementos[i].transform.localScale = Vector3.zero;
            LeanTween.scale(elementos[i], Vector3.one, 0.3f).setDelay(i * 0.1f).setEaseOutBack();
        }
    }

    void AnimarPanelVictoria()
    {
        musicSource.clip = musicaVictoria;
        musicSource.loop = false;
        musicSource.Play();

        panelVictoria.SetActive(true);
        panelVictoria.transform.localScale = Vector3.zero;
        LeanTween.scale(panelVictoria, Vector3.one, 0.5f).setEaseOutBack();

        AnimarElemento(gotaVictoria1, 0.1f);
        AnimarElemento(gotaVictoria2, 0.5f);
        AnimarElemento(gotaVictoria3, 0.9f);

        LeanTween.delayedCall(1.4f, () => AnimarElementosPanel(elementosVictoria));
    }

    void AnimarPanelIntermedio()
    {
        musicSource.clip = musicaIntermedio;
        musicSource.loop = false;
        musicSource.Play();

        panelIntermedio.SetActive(true);
        panelIntermedio.transform.localScale = Vector3.zero;
        LeanTween.scale(panelIntermedio, Vector3.one, 0.5f).setEaseOutBack();

        AnimarElemento(gotaIntermedio1, 0.1f);
        AnimarElemento(gotaIntermedio2, 0.5f);
        AnimarElemento(gotaIntermedio3, 0.9f);

        LeanTween.delayedCall(1.4f, () => AnimarElementosPanel(elementosIntermedio));
    }

    void AnimarPanelDerrota()
    {
        musicSource.clip = musicaDerrota;
        musicSource.loop = false;
        musicSource.Play();

        panelDerrota.SetActive(true);
        panelDerrota.transform.localScale = Vector3.zero;
        LeanTween.scale(panelDerrota, Vector3.one, 0.5f).setEaseOutBack();

        AnimarElemento(gotaDerrota1, 0.1f);
        AnimarElemento(gotaDerrota2, 0.5f);
        AnimarElemento(gotaDerrota3, 0.9f);

        LeanTween.delayedCall(1.4f, () => AnimarElementosPanel(elementosDerrota));
    }


}