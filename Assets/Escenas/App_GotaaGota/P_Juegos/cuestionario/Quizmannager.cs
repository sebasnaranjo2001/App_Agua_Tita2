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

    int indicePregunta = 0;
    int aciertos = 0;
    int respuestaCorrectaActual;

    Vector2 posFondoOriginal;
    Vector3 posRelojOriginal;

    void Start()
    {
        tiempoActual = tiempoMaximo;

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

        float p = tiempoActual / tiempoMaximo;

        if (fillReloj) fillReloj.fillAmount = p;
        if (textoTiempo) textoTiempo.text = Mathf.CeilToInt(tiempoActual).ToString();

        if (tiempoActual <= 10f && musicSource.clip != musicaTension)
        {
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
            barraProgreso.Actualizar(
    indicePregunta + 1,
    preguntas.Length,
    "Pregunta"
);

        if (imagenPregunta != null)
            imagenPregunta.sprite = p.imagen;

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
            if (sfxSource) sfxSource.PlayOneShot(sonidoAcierto);

            // 💚 animación acierto
            LeanTween.scale(botones[i].gameObject, Vector3.one * 1.2f, 0.15f)
                .setLoopPingPong(1);
        }
        else
        {
            if (sfxSource) sfxSource.PlayOneShot(sonidoError);

            // ❤️ shake error
            Vector3 pos = botones[i].transform.localPosition;

            LeanTween.moveLocalX(botones[i].gameObject, pos.x + 20f, 0.05f)
                .setLoopPingPong(4);

            botones[respuestaCorrectaActual].image.color = Color.green;
        }

        ActualizarTextoAciertos();
        Invoke(nameof(SiguientePregunta), 1.2f);
    }

    void SiguientePregunta()
    {
        indicePregunta++;

        if (indicePregunta < preguntas.Length)
            MostrarPregunta();
        else
            Debug.Log("FIN QUIZ");
    }

    void ActualizarTextoAciertos()
    {
        if (textoAciertos != null)
            textoAciertos.text = "Aciertos: " + aciertos;
    }
}