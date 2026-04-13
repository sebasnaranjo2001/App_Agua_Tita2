using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerDrag : MonoBehaviour
{
    [System.Serializable]
    public class Fase
    {
        public string[] textos;
        public Sprite[] imagenes;
        public string[] respuestasCorrectas; // A, B, C
    }

    public Fase[] fases;

    public DropZone[] zonas;
    public DragItem[] items;

    public TMP_Text[] textosUI;
    public Image[] imagenesUI;

    public GameObject panelFinal;

    private int faseActual = 0;

    void Start()
    {
        panelFinal.SetActive(false);
        CargarFase();
    }

    void CargarFase()
    {
        Fase f = fases[faseActual];

        // 🔹 Textos
        for (int i = 0; i < textosUI.Length; i++)
        {
            textosUI[i].text = f.textos[i];
        }

        // 🔹 Imágenes
        for (int i = 0; i < imagenesUI.Length; i++)
        {
            imagenesUI[i].sprite = f.imagenes[i];
        }

        // 🔹 Asignar respuestas correctas automáticamente
        for (int i = 0; i < zonas.Length; i++)
        {
            zonas[i].idCorrecto = f.respuestasCorrectas[i].Trim().ToUpper();
            zonas[i].ResetZona();
        }

        // 🔹 Resetear posiciones
        foreach (DragItem item in items)
        {
            item.ResetPosition();
        }
    }

    public void Comprobar()
    {
        bool todoCorrecto = true;

        foreach (DropZone zona in zonas)
        {
            string actual = zona.idActual.Replace("(Clone)", "").Trim().ToUpper();
            string correcto = zona.idCorrecto.Trim().ToUpper();

            // 🔴 Si está vacío → incorrecto
            if (string.IsNullOrEmpty(actual))
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
            // 🔴 Si no coincide → incorrecto
            else if (actual != correcto)
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
            // 🟢 Correcto
            else
            {
                zona.MarcarCorrecto();
            }
        }

        if (todoCorrecto)
        {
            Invoke("SiguienteFase", 1.5f);
        }
        else
        {
            Invoke("Resetear", 1.5f);
        }
    }

    void Resetear()
    {
        foreach (DropZone zona in zonas)
        {
            zona.ResetZona();
        }

        foreach (DragItem item in items)
        {
            item.ResetPosition();
        }
    }

    void SiguienteFase()
    {
        faseActual++;

        if (faseActual >= fases.Length)
        {
            panelFinal.SetActive(true);
        }
        else
        {
            CargarFase();
        }
    }

    public void Volver()
    {
        SceneManager.LoadScene("z_juegos");
    }
}