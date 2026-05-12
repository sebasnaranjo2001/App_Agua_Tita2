using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

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
        panelVideo.SetActive(false);
        rectTransformPanel.localScale = Vector3.zero;
        grupoControles.alpha = 0;

        // Suscribirse a eventos
        videoPlayer.loopPointReached += AlTerminarVideo;

        // IMPORTANTE: Empezar a preparar el video apenas abra la escena
        videoPlayer.Prepare();
    }

    void AlTerminarVideo(VideoPlayer vp) { CerrarVideo(); }

    public void AbrirVideo()
    {
        panelVideo.SetActive(true);
        panelVideo.transform.SetAsLastSibling();

        Screen.orientation = ScreenOrientation.AutoRotation;
        LeanTween.scale(rectTransformPanel.gameObject, Vector3.one, 0.5f).setEaseOutBack();

        // Lógica inteligente: Si ya está preparado, Play. Si no, esperamos a que termine de preparar.
        if (videoPlayer.isPrepared)
        {
            EjecutarPlayYControles();
        }
        else
        {
            videoPlayer.prepareCompleted += AlEstarListoParaSonar;
            videoPlayer.Prepare();
        }
    }

    // Esta función se activa sola cuando el video por fin se carga
    void AlEstarListoParaSonar(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= AlEstarListoParaSonar; // Limpiamos el evento
        EjecutarPlayYControles();
    }

    void EjecutarPlayYControles()
    {
        videoPlayer.Play();
        btnPlay.SetActive(false);
        btnPause.SetActive(true);
        MostrarControles();
    }

    public void CerrarVideo()
    {
        videoPlayer.Stop();
        Screen.orientation = ScreenOrientation.Portrait;
        LeanTween.scale(rectTransformPanel.gameObject, Vector3.zero, 0.4f).setEaseInBack().setOnComplete(() => {
            panelVideo.SetActive(false);
        });
    }

    public void ClickPlay()
    {
        if (videoPlayer.isPrepared) videoPlayer.Play();
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
        LeanTween.cancel(grupoControles.gameObject);
        LeanTween.alphaCanvas(grupoControles, 1f, 0.3f);
        CancelInvoke("OcultarControles");
        Invoke("OcultarControles", tiempoOcultarControles);
    }

    void OcultarControles()
    {
        controlesVisibles = false;
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