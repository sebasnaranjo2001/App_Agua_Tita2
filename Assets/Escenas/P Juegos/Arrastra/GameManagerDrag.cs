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
        public string[] respuestasCorrectas;
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

        for (int i = 0; i < textosUI.Length; i++)
        {
            textosUI[i].text = f.textos[i];
        }

        for (int i = 0; i < imagenesUI.Length; i++)
        {
            imagenesUI[i].sprite = f.imagenes[i];
        }

        for (int i = 0; i < zonas.Length; i++)
        {
            zonas[i].idCorrecto = f.respuestasCorrectas[i];
            zonas[i].idActual = "";
        }

        foreach (DragItem item in items)
        {
            item.ResetPosition();
        }
    }

    public void Comprobar()
    {
        bool correcto = true;

        foreach (DropZone zona in zonas)
        {
            if (zona.idActual != zona.idCorrecto)
            {
                correcto = false;
                break;
            }
        }

        if (correcto)
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
        else
        {
            foreach (DragItem item in items)
                item.ResetPosition();
        }
    }

    public void Volver()
    {
        SceneManager.LoadScene("z_juegos");
    }
}