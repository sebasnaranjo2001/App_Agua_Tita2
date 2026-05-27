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
        public string[] respuestas;
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
    // AQUÍ PON TODO MENOS EL FONDO Y LOS PANELES
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

        // Mostrar elementos gameplay
        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Actualizar contador inicial
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

        // Botones
        for (int i = 0; i < botones.Length; i++)
        {
            botones[i].GetComponentInChildren<TMP_Text>().text = p.respuestas[i];

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
        Pregunta p = preguntas[indicePregunta];

        // Correcta
        if (index == p.respuestaCorrecta)
        {
            botones[index].image.color = Color.green;

            aciertos++;

            ActualizarTextoAciertos();
        }
        // Incorrecta
        else
        {
            botones[index].image.color = Color.red;
            botones[p.respuestaCorrecta].image.color = Color.green;
        }

        // Desactivar botones
        foreach (Button b in botones)
        {
            b.interactable = false;
        }

        // Esperar antes de pasar
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
            textoAciertos.text = "Aciertos: " + aciertos;
        }
    }

    // =========================
    // MOSTRAR RESULTADO FINAL
    // =========================
    void MostrarResultado()
    {
        // =========================
        // OCULTAR SOLO GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // =========================
        // DESACTIVAR TODOS LOS PANELES
        // =========================

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // Texto final
        string resultadoFinal = "Aciertos: " + aciertos + "/" + preguntas.Length;

        // =========================
        // VICTORIA
        // =========================

        if (aciertos == preguntas.Length)
        {
            if (panelVictoria != null)
                panelVictoria.SetActive(true);

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;
        }

        // =========================
        // DERROTA
        // =========================

        else if (aciertos == 0)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;
        }

        // =========================
        // INTERMEDIO
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}