using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class ControladorVideoGota : MonoBehaviour
{
    [Header("Paneles y Grupos")]
    public GameObject panelVideo;
    public CanvasGroup grupoControles;
    public RectTransform rectTransformPanel;

    [Header("Controles de Video")]
    public VideoPlayer videoPlayer;
    public Slider sliderProgreso;
    public GameObject btnPlay;
    public GameObject btnPause;

    [Header("Configuracion")]
    public float tiempoOcultarControles = 2.5f;

    private bool controlesVisibles = true;
    private bool arrastrandoSlider = false;

    void Start()
    {
        rectTransformPanel.localScale = Vector3.zero;
        grupoControles.alpha = 0;

        // --- NUEVO: Empezamos con los controles desactivados físicamente ---
        grupoControles.interactable = false;
        grupoControles.blocksRaycasts = false;

        videoPlayer.loopPointReached += AlTerminarVideo;
    }

    void Update()
    {
        // Hace que la barra se mueva sola
        if (videoPlayer.isPlaying && !arrastrandoSlider && videoPlayer.frameCount > 0)
        {
            sliderProgreso.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        CerrarVideo();
    }

    public void AbrirVideo()
    {
        // --- NUEVO: Evita que la pantalla se apague mientras ve el video ---
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        panelVideo.SetActive(true);
        LeanTween.scale(rectTransformPanel.gameObject, Vector3.one, 0.5f).setEaseOutBack();

        StartCoroutine(RutinaPlaySeguro());
    }

    IEnumerator RutinaPlaySeguro()
    {
        // Espera una fracción de segundo para que el reproductor no tire error
        yield return new WaitForSeconds(0.1f);

        videoPlayer.Play();
        btnPlay.SetActive(false);
        btnPause.SetActive(true);
        MostrarControles();
    }

    public void CerrarVideo()
    {
        // --- NUEVO: Devuelve el control de la pantalla a la configuración normal del celular ---
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        videoPlayer.Stop();
        LeanTween.scale(rectTransformPanel.gameObject, Vector3.zero, 0.4f).setEaseInBack().setOnComplete(() => {
            panelVideo.SetActive(false);
        });
    }

    public void ClickPlay()
    {
        videoPlayer.Play();
        btnPlay.SetActive(false);
        btnPause.SetActive(true);
        MostrarControles();
    }

    public void ClickPause()
    {
        videoPlayer.Pause();
        btnPlay.SetActive(true);
        btnPause.SetActive(false);
        MostrarControles();
    }

    public void AlternarControles()
    {
        if (controlesVisibles) OcultarControles();
        else MostrarControles();
    }

    public void MostrarControles()
    {
        controlesVisibles = true;

        // --- NUEVO: Activamos la interacción en cuanto aparecen ---
        grupoControles.interactable = true;
        grupoControles.blocksRaycasts = true;

        LeanTween.cancel(grupoControles.gameObject);
        LeanTween.alphaCanvas(grupoControles, 1f, 0.3f);
        CancelInvoke("OcultarControles");
        Invoke("OcultarControles", tiempoOcultarControles);
    }

    void OcultarControles()
    {
        controlesVisibles = false;

        // --- NUEVO: Desactivamos la interacción para que los clics pasen de largo ---
        grupoControles.interactable = false;
        grupoControles.blocksRaycasts = false;

        LeanTween.alphaCanvas(grupoControles, 0f, 0.5f);
    }

    public void OnSliderDown() { arrastrandoSlider = true; }

    public void OnSliderUp()
    {
        float frame = (float)sliderProgreso.value * videoPlayer.frameCount;
        videoPlayer.frame = (long)frame;
        arrastrandoSlider = false;
    }
}