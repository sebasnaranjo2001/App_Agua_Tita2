using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Cronometro : MonoBehaviour
{
    [Header("--- TEXTOS DEL CRONÓMETRO ---")]
    public TMP_Text textoNombreMiembro;
    public TMP_Text textoCronometro;
    public TMP_Text textoAguaConsumida;
    public GameObject textoActivo;
    public GameObject textoPausa;

    [Header("--- BOTONES ---")]
    public GameObject btnComenzar;
    public GameObject btnPausa;
    public GameObject btnPlay;
    public GameObject btnFinalizar;

    [Header("--- VISUALES CRÓNO ---")]
    public RectTransform fondoColorRect;
    public Image fondoColor;

    [Header("--- PANEL GUARDADO (POP-UP) ---")]
    public GameObject panelGuardado;
    public GameObject blurGuardado;
    public GameObject tarjetaGuardado;
    public TMP_Text textoPuntos;
    public TMP_Text textoNombreGuardado;
    public TMP_Text textoTiempoFinal;
    public TMP_Text textoGastoFinal;

    [Header("--- AJUSTES DE CIERRE ---")]
    public float tiempoCierrePanel = 4.0f;

    [Header("--- AJUSTES DE TIEMPO Y COLOR ---")]
    public float litrosPorMinuto = 9.5f;
    public float minutosAlertaVerde = 5f;
    public float minutosParaAmarillo = 7f;
    public float minutosParaRojo = 10f;

    public Color32 colorVerde = new Color32(199, 233, 176, 255);
    public Color32 colorAmarillo = new Color32(243, 229, 171, 255);
    public Color32 colorRojo = new Color32(255, 183, 178, 255);

    [Header("--- ALERTAS DE AUDIO Y VOZ ---")]
    public AudioSource altavozCronometro;
    public AudioClip audioVerde;
    public AudioClip audioAmarillo;
    public AudioClip audioRojo;

    private bool yaSonoVerde = false;
    private bool yaSonoAmarillo = false;
    private bool yaSonoRojo = false;

    public bool estaContando = false;
    private bool estaPausado = false;
    private float tiempoTranscurrido;
    private DateTime tiempoInicioReal;
    private float tiempoAcumuladoAnterior;

    void Start() { PrepararEscenaInicial(); IniciarEfectoAguaFondo(); }

    void OnEnable() { ActualizarNombreMiembro(); }

    void Update()
    {
        if (estaContando && !estaPausado)
        {
            RecalcularTiempo();
            ActualizarInterfazUI();
            ManejarTransicionDeColor();
            VerificarAlertasDeAudio();
        }
    }

    public void PrepararEscenaInicial()
    {
        estaContando = false;
        estaPausado = false;
        tiempoTranscurrido = 0f;
        tiempoAcumuladoAnterior = 0f;
        ActualizarInterfazUI();
        if (fondoColor != null) fondoColor.color = colorVerde;
        ConfigurarElementosUI(true, false, false, false, false, false);

        yaSonoVerde = false;
        yaSonoAmarillo = false;
        yaSonoRojo = false;
        if (altavozCronometro != null) altavozCronometro.Stop();

        // Si se cierra el panel, le avisamos al manejador que reanude la música
        if (ManejadorMusica.instance != null) ManejadorMusica.instance.ReanudarMusica();
    }

    public void BtnComenzar_Click()
    {
        if (Avisos.instance != null && !Avisos.instance.VerificarEstadoRegistro()) return;

        estaContando = true;
        estaPausado = false;
        tiempoInicioReal = DateTime.Now;
        tiempoAcumuladoAnterior = 0f;
        ConfigurarPantalla(true);
        ConfigurarElementosUI(false, true, false, true, true, false);
        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);

        // --- LE AVISAMOS AL MANEJADOR QUE PAUSE LA MÚSICA ---
        if (ManejadorMusica.instance != null) ManejadorMusica.instance.PausarMusica();
    }

    public void BtnPausa_Click()
    {
        estaPausado = true;
        tiempoAcumuladoAnterior = tiempoTranscurrido;
        ConfigurarPantalla(false);
        ConfigurarElementosUI(false, false, true, true, false, true);

        // --- REANUDAMOS LA MÚSICA EN LA PAUSA ---
        if (ManejadorMusica.instance != null) ManejadorMusica.instance.ReanudarMusica();
    }

    public void BtnPlay_Click()
    {
        estaPausado = false;
        tiempoInicioReal = DateTime.Now;
        ConfigurarPantalla(true);
        ConfigurarElementosUI(false, true, false, true, true, false);

        // --- VOLVEMOS A PAUSAR LA MÚSICA AL SEGUIR LA DUCHA ---
        if (ManejadorMusica.instance != null) ManejadorMusica.instance.PausarMusica();
    }

    public void BtnFinalizar_Click()
    {
        if (altavozCronometro != null) altavozCronometro.Stop();

        // --- REANUDAMOS LA MÚSICA DE FONDO AL TERMINAR ---
        if (ManejadorMusica.instance != null) ManejadorMusica.instance.ReanudarMusica();

        estaContando = false;
        estaPausado = false;
        ConfigurarPantalla(false);

        GuardarEnHistorialInterno();

        string nombre = ManejadorRegistro.instance != null ? ManejadorRegistro.instance.nombreSeleccionado : "Invitado";
        textoNombreGuardado.text = nombre;
        textoTiempoFinal.text = textoCronometro.text;
        textoGastoFinal.text = textoAguaConsumida.text;

        StartCoroutine(SecuenciaGuardadoEmergente());
    }

    IEnumerator SecuenciaGuardadoEmergente()
    {
        panelGuardado.SetActive(true);
        CanvasGroup cg = blurGuardado.GetComponent<CanvasGroup>();
        if (cg == null) cg = blurGuardado.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        LeanTween.alphaCanvas(cg, 1f, 0.3f);
        tarjetaGuardado.transform.localScale = Vector3.zero;
        LeanTween.scale(tarjetaGuardado, Vector3.one, 0.4f).setEaseOutBack();

        float tiempoInicio = Time.time;
        while (Time.time < tiempoInicio + tiempoCierrePanel)
        {
            textoPuntos.text = "."; yield return new WaitForSeconds(0.4f);
            textoPuntos.text = ".."; yield return new WaitForSeconds(0.4f);
            textoPuntos.text = "..."; yield return new WaitForSeconds(0.4f);
        }

        LeanTween.alphaCanvas(cg, 0f, 0.3f);
        LeanTween.scale(tarjetaGuardado, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => {
            panelGuardado.SetActive(false);

            if (ManejadorRegistro.instance != null) ManejadorRegistro.instance.nombreSeleccionado = "";
            if (Avisos.instance != null)
            {
                if (Avisos.instance.miembroSeleccionado != null) Avisos.instance.miembroSeleccionado.Deseleccionar();
                Avisos.instance.miembroSeleccionado = null;
            }

            PrepararEscenaInicial();

            if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);

            ManejadorNavegacion nav = UnityEngine.Object.FindFirstObjectByType<ManejadorNavegacion>();
            if (nav != null) nav.IrARanking();
        });
    }

    private void ConfigurarElementosUI(bool comenzar, bool pausa, bool play, bool finalizar, bool txtActivo, bool txtPausa)
    {
        if (btnComenzar) btnComenzar.SetActive(comenzar);
        if (btnPausa) btnPausa.SetActive(pausa);
        if (btnPlay) btnPlay.SetActive(play);
        if (btnFinalizar) btnFinalizar.SetActive(finalizar);
        if (textoActivo) textoActivo.SetActive(txtActivo);
        if (textoPausa) textoPausa.SetActive(txtPausa);
    }

    void ActualizarNombreMiembro()
    {
        if (textoNombreMiembro != null && ManejadorRegistro.instance != null)
            textoNombreMiembro.text = string.IsNullOrEmpty(ManejadorRegistro.instance.nombreSeleccionado) ? "---" : ManejadorRegistro.instance.nombreSeleccionado;
    }

    void RecalcularTiempo() { tiempoTranscurrido = (float)(DateTime.Now - tiempoInicioReal).TotalSeconds + tiempoAcumuladoAnterior; }

    void ActualizarInterfazUI()
    {
        if (textoCronometro != null) textoCronometro.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(tiempoTranscurrido / 60), Mathf.FloorToInt(tiempoTranscurrido % 60));
        if (textoAguaConsumida != null) textoAguaConsumida.text = ((tiempoTranscurrido / 60f) * litrosPorMinuto).ToString("F1") + " L";
    }

    void GuardarEnHistorialInterno()
    {
        string nom = ManejadorRegistro.instance != null ? ManejadorRegistro.instance.nombreSeleccionado : "Invitado";
        if (ManejadorRegistro.instance != null && tiempoTranscurrido > 0)
        {
            foreach (var m in ManejadorRegistro.instance.listaDeMiembros)
            {
                if (m.nombre == nom)
                {
                    if (m.mejorTiempo <= 0 || tiempoTranscurrido < m.mejorTiempo) m.mejorTiempo = tiempoTranscurrido;
                    if (m.historialBanos == null) m.historialBanos = new List<ManejadorRegistro.RegistroBano>();
                    m.historialBanos.Insert(0, new ManejadorRegistro.RegistroBano { duracion = tiempoTranscurrido, fecha = DateTime.Now.ToString("dd/MM/yyyy"), hora = DateTime.Now.ToString("hh:mm tt") });
                    if (m.historialBanos.Count > 5) m.historialBanos.RemoveRange(5, m.historialBanos.Count - 5);
                    break;
                }
            }
            ManejadorRegistro.instance.GuardarEnDisco();
        }
    }

    void ManejarTransicionDeColor()
    {
        float minAct = tiempoTranscurrido / 60f;
        Color col = (minAct < minutosParaAmarillo) ? Color.Lerp(colorVerde, colorAmarillo, minAct / minutosParaAmarillo) : (minAct < minutosParaRojo) ? Color.Lerp(colorAmarillo, colorRojo, (minAct - minutosParaAmarillo) / (minutosParaRojo - minutosParaAmarillo)) : colorRojo;
        if (fondoColor != null) fondoColor.color = col;
    }

    void VerificarAlertasDeAudio()
    {
        float minutosActuales = tiempoTranscurrido / 60f;

        if (minutosActuales >= minutosAlertaVerde && !yaSonoVerde)
        {
            yaSonoVerde = true;
            ReproducirAlerta(audioVerde);
        }

        if (minutosActuales >= minutosParaAmarillo && !yaSonoAmarillo)
        {
            yaSonoAmarillo = true;
            ReproducirAlerta(audioAmarillo);
        }

        if (minutosActuales >= minutosParaRojo && !yaSonoRojo)
        {
            yaSonoRojo = true;
            ReproducirAlerta(audioRojo);
        }
    }

    void ReproducirAlerta(AudioClip clip)
    {
        if (altavozCronometro != null && clip != null)
        {
            altavozCronometro.Stop();
            altavozCronometro.clip = clip;
            altavozCronometro.Play();
        }
    }

    void IniciarEfectoAguaFondo()
    {
        if (fondoColorRect == null) return;
        LeanTween.scale(fondoColorRect, Vector3.one * 1.05f, 2.5f).setEaseInOutSine().setLoopPingPong();
        LeanTween.rotateZ(fondoColorRect.gameObject, 1.5f, 3f).setEaseInOutSine().setLoopPingPong();
    }

    void ConfigurarPantalla(bool encendida) { Screen.sleepTimeout = encendida ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting; }

    public void GuardarTiempoFinal() { GuardarEnHistorialInterno(); }
}