 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Pregunta
    {
        public string pregunta;

        // Respuestas originales
        public string[] respuestas;

        // Índice correcto ORIGINAL
        public int respuestaCorrecta;

        public Sprite imagen;
    }

    [Header("Preguntas")]
    public Pregunta[] preguntas;

    [Header("UI Pregunta")]
    public TMP_Text textoPregunta;
    public TMP_Text textoAciertos;
    public Image imagenPregunta;
    public Button[] botones;
    public TMP_Text tituloPregunta;
    public RectTransform fondoPregunta;

    [Header("ELEMENTOS A OCULTAR AL FINAL")]
    public GameObject[] elementosGameplay;

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

    [Header("Textos de Aciertos por Panel")]
    public TMP_Text textoAciertosVictoria;
    public TMP_Text textoAciertosIntermedio;
    public TMP_Text textoAciertosDerrota;
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

    [Header("Elementos Panel Victoria")]
    public GameObject[] elementosVictoria;

    [Header("Elementos Panel Intermedio")]
    public GameObject[] elementosIntermedio;

    [Header("Elementos Panel Derrota")]
    public GameObject[] elementosDerrota;

    private int indicePregunta = 0;
    private int aciertos = 0;

    private Vector2 posicionOriginalTitulo;
    private Vector2 posicionOriginalFondo;

    // RESPUESTA CORRECTA ACTUAL
    private int respuestaCorrectaActual;

    void Start()
    {
        // Mezclar preguntas aleatoriamente
        MezclarPreguntas();

        // Desactivar panels finales
        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // Mostrar gameplay
        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // =====================================
        // ANIMACIÓN DEL TÍTULO (SOLO UNA VEZ)
        // =====================================

        posicionOriginalTitulo =
            tituloPregunta.rectTransform.anchoredPosition;

        // Lo colocamos más arriba temporalmente
        tituloPregunta.rectTransform.anchoredPosition =
            new Vector2(
                posicionOriginalTitulo.x,
                posicionOriginalTitulo.y + 300f
            );

        // Lo animamos hacia su posición real
        LeanTween.move(
            tituloPregunta.rectTransform,
            posicionOriginalTitulo,
            0.6f
        ).setEaseOutBack();

        // Guardar posición original del fondo
        posicionOriginalFondo =
            fondoPregunta.anchoredPosition;

        // Actualizar contador
        ActualizarTextoAciertos();

        // Mostrar primera pregunta
        MostrarPregunta();

    }

    // =========================
    // MEZCLAR PREGUNTAS
    // =========================
    void MezclarPreguntas()
    {
        for (int i = 0; i < preguntas.Length; i++)
        {
            int randomIndex = Random.Range(i, preguntas.Length);

            Pregunta temp = preguntas[i];
            preguntas[i] = preguntas[randomIndex];
            preguntas[randomIndex] = temp;
        }
    }

    // =========================
    // MOSTRAR PREGUNTA
    // =========================
    void MostrarPregunta()
    {
        Pregunta p = preguntas[indicePregunta];

        // Texto pregunta
        textoPregunta.text = p.pregunta;

        // Animación fondo + texto
        fondoPregunta.anchoredPosition =
            new Vector2(
                posicionOriginalFondo.x - 1200f,
                posicionOriginalFondo.y
            );

        LeanTween.move(
            fondoPregunta,
            posicionOriginalFondo,
            0.5f
        ).setEaseOutBack();

        // Imagen
        if (p.imagen != null)
        {
            imagenPregunta.sprite = p.imagen;
            imagenPregunta.gameObject.SetActive(true);

            imagenPregunta.transform.localScale = Vector3.zero;

            LeanTween.scale(
                imagenPregunta.gameObject,
                Vector3.one,
                0.4f
            ).setEaseOutBack();
        }
        else
        {
            imagenPregunta.gameObject.SetActive(false);
        }

        // =========================
        // MEZCLAR RESPUESTAS
        // =========================

        string[] respuestasMezcladas = (string[])p.respuestas.Clone();

        // Guardar índices
        int[] indices = new int[respuestasMezcladas.Length];

        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        // Fisher-Yates Shuffle
        for (int i = 0; i < respuestasMezcladas.Length; i++)
        {
            int randomIndex = Random.Range(i, respuestasMezcladas.Length);

            // Intercambiar respuestas
            string tempRespuesta = respuestasMezcladas[i];
            respuestasMezcladas[i] = respuestasMezcladas[randomIndex];
            respuestasMezcladas[randomIndex] = tempRespuesta;

            // Intercambiar índices
            int tempIndex = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = tempIndex;
        }

        // Encontrar nueva posición correcta
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] == p.respuestaCorrecta)
            {
                respuestaCorrectaActual = i;
                break;
            }
        }

        // =========================
        // CONFIGURAR BOTONES
        // =========================

        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].GetComponentInChildren<TMP_Text>().text =
                respuestasMezcladas[i];

            int index = i;

            botones[i].onClick.RemoveAllListeners();
            botones[i].onClick.AddListener(() => Responder(index));

            botones[i].image.color = Color.white;
            botones[i].interactable = true;
            // Animación botón
            botones[i].transform.localScale = Vector3.zero;

            LeanTween.scale(
                botones[i].gameObject,
                Vector3.one,
                0.3f
            )
            .setDelay(i * 0.1f)
            .setEaseOutBack();
        }
    }

    // =========================
    // RESPONDER
    // =========================
    void Responder(int index)
    {
        // Correcta
        if (index == respuestaCorrectaActual)
        {
            botones[index].image.color = Color.green;
            LeanTween.scale(
    botones[index].gameObject,
    Vector3.one * 1.2f,
    0.15f
).setLoopPingPong(1);

            aciertos++;

            ActualizarTextoAciertos();
        }
        // Incorrecta
        else
        {
            botones[index].image.color = Color.red;
            Vector3 posOriginal =
    botones[index].transform.localPosition;

            LeanTween.moveLocalX(
                botones[index].gameObject,
                posOriginal.x + 20f,
                0.05f
            ).setLoopPingPong(4);
            botones[respuestaCorrectaActual].image.color = Color.green;
        }

        // Desactivar botones
        foreach (Button b in botones)
        {
            b.interactable = false;
        }

        // Esperar
        Invoke("SiguientePregunta", 1.5f);
    }

    // =========================
    // SIGUIENTE PREGUNTA
    // =========================
    void SiguientePregunta()
    {
        indicePregunta++;

        if (indicePregunta < preguntas.Length)
        {
            MostrarPregunta();
        }
        else
        {
            MostrarResultado();
        }
    }

    // =========================
    // ACTUALIZAR TEXTO ACIERTOS
    // =========================
    void ActualizarTextoAciertos()
    {
        if (textoAciertos != null)
        {
            textoAciertos.text =
                "Aciertos: " + aciertos;
        }
    }

    // =========================
    // MOSTRAR RESULTADO FINAL
    // =========================
    void MostrarResultado()
    {
        // =========================
        // OCULTAR GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // =========================
        // DESACTIVAR PANELES
        // =========================

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // Resultado final
        string resultadoFinal =
            "Aciertos: " + aciertos + "/" + preguntas.Length;

        // =========================
        // VICTORIA → 5 ACIERTOS
        // =========================

        if (aciertos == 5)
        {
            if (panelVictoria != null)
                panelVictoria.SetActive(true);

            panelVictoria.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                panelVictoria,
                Vector3.one,
                0.5f
            ).setEaseOutBack();

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;
            AnimarPanelVictoria();
        }

        // =========================
        // DERROTA → 0 o 1 ACIERTO
        // =========================

        else if (aciertos <= 1)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);

            panelDerrota.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                panelDerrota,
                Vector3.one,
                0.5f
            ).setEaseOutBack();

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;
            AnimarPanelDerrota();

        }

        // =========================
        // INTERMEDIO → 2,3,4
        // =========================

        else
        {
            if (panelIntermedio != null)
                panelIntermedio.SetActive(true);

            panelIntermedio.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                panelIntermedio,
                Vector3.one,
                0.5f
            ).setEaseOutBack();

            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text = resultadoFinal;
            AnimarPanelIntermedio();
        }
    }

    // =========================
    // BOTONES
    // =========================
    void AnimarElemento(GameObject obj, float delay)
    {
        if (obj == null) return;

        obj.transform.localScale = Vector3.zero;

        LeanTween.scale(
            obj,
            Vector3.one,
            0.35f
        )
        .setDelay(delay)
        .setEaseOutBack();
    }

    void AnimarElementosPanel(GameObject[] elementos)
    {
        for (int i = 0; i < elementos.Length; i++)
        {
            if (elementos[i] == null)
                continue;

            elementos[i].SetActive(true);

            elementos[i].transform.localScale = Vector3.zero;

            LeanTween.scale(
                elementos[i],
                Vector3.one,
                0.3f
            )
            .setDelay(i * 0.1f)
            .setEaseOutBack();
        }
    }

    void AnimarPanelDerrota()
    {
        foreach (GameObject obj in elementosDerrota)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        AnimarElemento(gotaDerrota1, 0.1f);

        LeanTween.delayedCall(
            0.6f,
            () =>
            {
                AnimarElementosPanel(elementosDerrota);
            });
    }

    void AnimarPanelIntermedio()
    {
        foreach (GameObject obj in elementosIntermedio)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Ocultar las gotas al iniciar
        if (gotaIntermedio1 != null) gotaIntermedio1.SetActive(false);
        if (gotaIntermedio2 != null) gotaIntermedio2.SetActive(false);
        if (gotaIntermedio3 != null) gotaIntermedio3.SetActive(false);

        // Primera gota
        if (gotaIntermedio1 != null)
        {
            gotaIntermedio1.SetActive(true);
            AnimarElemento(gotaIntermedio1, 0f);
        }

        // Segunda gota
        if (gotaIntermedio2 != null)
        {
            gotaIntermedio2.SetActive(true);
            AnimarElemento(gotaIntermedio2, 0.4f);
        }

        // Tercera gota (apagada)
        if (gotaIntermedio3 != null)
        {
            gotaIntermedio3.SetActive(true);
            AnimarElemento(gotaIntermedio3, 0.8f);
        }

        // Mostrar resto de elementos después
        LeanTween.delayedCall(
            1.3f,
            () =>
            {
                AnimarElementosPanel(elementosIntermedio);
            });
    }

    void AnimarPanelVictoria()
    {
        foreach (GameObject obj in elementosVictoria)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        AnimarElemento(gotaVictoria1, 0.1f);
        AnimarElemento(gotaVictoria2, 0.5f);
        AnimarElemento(gotaVictoria3, 0.9f);

        LeanTween.delayedCall(
            1.4f,
            () =>
            {
                AnimarElementosPanel(elementosVictoria);
            });
    }
    public void ReiniciarQuiz()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}