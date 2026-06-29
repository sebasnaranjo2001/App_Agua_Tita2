using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel Tutorial")]
    public GameObject panelTutorial;

    [Header("UI Tutorial")]
    public TMP_Text textoTutorial;
    public RawImage videoPreview;

    [Header("Gameplay")]
    public GameObject[] elementosGameplay;

    [Header("Evento al iniciar")]
    public UnityEvent alIniciarJuego;

    [Header("Animaciones")]
    public float animTime = 0.5f;

    [Header("Configuración")]
    public bool iniciarConTutorial = true;

    void Start()
    {
        if (iniciarConTutorial)
        {
            SetGameplay(false);
            MostrarTutorial();
        }
        else
        {
            panelTutorial.SetActive(false);


            alIniciarJuego?.Invoke();
        }
    }

    // -------------------------
    // TUTORIAL
    // -------------------------
    public void MostrarTutorial()
    {
        panelTutorial.SetActive(true);

        panelTutorial.transform.localScale = Vector3.zero;

        LeanTween.scale(panelTutorial, Vector3.one, animTime)
                 .setEaseOutBack();
    }

    public void CerrarTutorial()
    {
        LeanTween.scale(panelTutorial, Vector3.zero, animTime)
                 .setEaseInBack()
                 .setOnComplete(() =>
                 {
                     panelTutorial.SetActive(false);

                     SetGameplay(true);
                     alIniciarJuego?.Invoke();
                 });
    }

    

    // -------------------------
    // GAMEPLAY CONTROL
    // -------------------------
    void SetGameplay(bool estado)
    {
        if (elementosGameplay == null) return;

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(estado);
        }

    }

    // -------------------------
    // BOTÓN UI
    // -------------------------
    public void BotonEntendido()
    {
        CerrarTutorial();
    }
}