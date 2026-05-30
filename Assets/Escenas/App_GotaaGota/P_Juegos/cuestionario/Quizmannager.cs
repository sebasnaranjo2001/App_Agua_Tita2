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

    private int indicePregunta = 0;
    private int aciertos = 0;

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

        // Imagen
        if (p.imagen != null)
        {
            imagenPregunta.sprite = p.imagen;
            imagenPregunta.gameObject.SetActive(true);
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

            aciertos++;

            ActualizarTextoAciertos();
        }
        // Incorrecta
        else
        {
            botones[index].image.color = Color.red;
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

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;
        }

        // =========================
        // DERROTA → 0 o 1 ACIERTO
        // =========================

        else if (aciertos <= 1)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;
        }

        // =========================
        // INTERMEDIO → 2,3,4
        // =========================

        else
        {
            if (panelIntermedio != null)
                panelIntermedio.SetActive(true);

            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text = resultadoFinal;
        }
    }

    // =========================
    // BOTONES
    // =========================

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