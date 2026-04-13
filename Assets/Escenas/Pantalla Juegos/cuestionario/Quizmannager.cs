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
        public string[] respuestas; // 3 respuestas
        public int respuestaCorrecta; // índice (0-2)
        public Sprite imagen;
    }

    public Pregunta[] preguntas;

    public TMP_Text textoPregunta;        // ✅ TMP
    public Image imagenPregunta;
    public Button[] botones;              // tamaño 3

    public GameObject panelFinal;
    public TMP_Text textoResultado;       // ✅ TMP

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

        // 🖼️ Imagen
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
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Pantalla de juegos");
    }
}