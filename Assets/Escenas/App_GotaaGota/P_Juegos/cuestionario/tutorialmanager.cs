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

        CanvasGroup canvasGroup = panelTutorial.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = panelTutorial.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        LeanTween.alphaCanvas(canvasGroup, 1f, animTime)
                 .setEaseOutQuad();
    }

    public void CerrarTutorial()
    {
        CanvasGroup canvasGroup = panelTutorial.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = panelTutorial.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        LeanTween.alphaCanvas(canvasGroup, 0f, animTime)
                 .setEaseOutQuad()
                 .setOnComplete(() =>
                 {
                     panelTutorial.SetActive(false);

                     // Restaurar para la próxima vez
                     canvasGroup.alpha = 1f;
                     canvasGroup.interactable = true;
                     canvasGroup.blocksRaycasts = true;

                     // Activar gameplay
                     SetGameplay(true);

                     // Ejecutar el método configurado en el Inspector
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