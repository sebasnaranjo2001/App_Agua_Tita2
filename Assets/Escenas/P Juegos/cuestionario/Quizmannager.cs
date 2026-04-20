using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Pregunta
    {
        public string pregunta;
        public string[] respuestas; // 3 respuestas
        public int respuestaCorrecta; // índice (0-2)
        public Sprite imagen;
    }

    public Pregunta[] preguntas;

    public TMP_Text textoPregunta;
    public Image imagenPregunta;
    public Button[] botones;

    public GameObject panelFinal;
    public TMP_Text textoResultado;

    [Header("Panel de otra escena")]
    public string nombreEscenaDestino;   // Escena donde está el panel
    public string nombrePanelDestino;    // Nombre exacto del panel
    public float tiempoEspera = 3f;

    private int indicePregunta = 0;
    private int aciertos = 0;

    void Start()
    {
        panelFinal.SetActive(false);
        MostrarPregunta();
    }

    void MostrarPregunta()
    {
        Pregunta p = preguntas[indicePregunta];

        textoPregunta.text = p.pregunta;

        if (p.imagen != null)
        {
            imagenPregunta.sprite = p.imagen;
            imagenPregunta.gameObject.SetActive(true);
        }
        else
        {
            imagenPregunta.gameObject.SetActive(false);
        }

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

    void Responder(int index)
    {
        Pregunta p = preguntas[indicePregunta];

        if (index == p.respuestaCorrecta)
        {
            botones[index].image.color = Color.green;
            aciertos++;
        }
        else
        {
            botones[index].image.color = Color.red;
            botones[p.respuestaCorrecta].image.color = Color.green;
        }

        foreach (Button b in botones)
            b.interactable = false;

        Invoke("SiguientePregunta", 1.5f);
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
            MostrarResultado();
        }
    }

    void MostrarResultado()
    {
        panelFinal.SetActive(true);
        textoResultado.text = "Aciertos: " + aciertos + "/" + preguntas.Length;

        StartCoroutine(CargarPanelDespues());
    }

    IEnumerator CargarPanelDespues()
    {
        yield return new WaitForSeconds(tiempoEspera);

        SceneManager.LoadScene(nombreEscenaDestino);

        yield return null; // Espera un frame para que cargue la escena

        GameObject panel = GameObject.Find(nombrePanelDestino);

        if (panel != null)
            panel.SetActive(true);
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}