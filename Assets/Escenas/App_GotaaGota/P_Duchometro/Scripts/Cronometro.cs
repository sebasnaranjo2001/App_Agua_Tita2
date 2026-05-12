using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Cronometro : MonoBehaviour
{
    [Header("Referencias de UI - Textos")]
    public TMP_Text textoCronometro;
    public TMP_Text textoConsumo;

    [Header("Referencias de UI - Botones/Tarjetas")]
    public GameObject btnComenzar;
    public GameObject btnFinalizar;
    public GameObject btnGuardando;
    public GameObject btnGuardado;
    public GameObject btnVolverEmpezar;

    [Header("--- ANIMACIÓN BOTÓN CRONO ---")]
    public RectTransform imagenBotonCrono;
    [Space]
    public Vector2 posFuera;               // Posición X e Y original
    public Vector2 sizeFuera;              // Width y Height original
    [Space]
    public Vector2 posDentro;              // Posición X e Y presionado
    public Vector2 sizeDentro;             // Width y Height presionado
    [Space]
    public float tiempoAnimBoton = 0.2f;

    [Header("--- GESTIÓN DE LOGOS ---")]
    public GameObject logoFondoGeneral;
    public GameObject logoInternoCrono;

    [Header("--- REFERENCIAS VISUALES ---")]
    public Image fondoColor;
    public RectTransform aguaVisual;
    public RectTransform[] gotas;

    [Header("--- AJUSTES DE TIEMPO ---")]
    public float minutosParaLlenado = 10f;
    public float minutosParaAmarillo = 6f;
    public float minutosParaRojo = 10f;

    [Header("Configuración Gotas")]
    public float puntoInicialY = 300f;
    public float puntoFinalY = -350f;
    private float[] velocidadesGotas;
    private bool[] gotaEsperando;

    [Header("Configuración Llenado")]
    public float posMinY = -500f;
    public float posMaxY = 0f;
    public float litrosPorMinuto = 9.5f;

    [Header("--- COLORES ---")]
    public Color32 colorVerde = new Color32(199, 233, 176, 255);
    public Color32 colorAmarillo = new Color32(243, 229, 171, 255);
    public Color32 colorRojo = new Color32(255, 183, 178, 255);

    public bool estaContando = false;

    private float tiempoTranscurrido;
    private DateTime tiempoInicioReal;
    private float tiempoAcumuladoAnterior;

    void Start() { PrepararEscena(); }

    void OnEnable()
    {
        if (logoFondoGeneral != null) logoFondoGeneral.SetActive(false);
        if (logoInternoCrono != null) logoInternoCrono.SetActive(true);

        if (estaContando)
        {
            RecalcularTiempo();
            ConfigurarPantallaSiempreEncendida(true);
            AnimarBotonCrono(true);
        }
        else
        {
            ReiniciarTodo();
            AnimarBotonCrono(false);
        }
    }

    void OnDisable()
    {
        if (logoFondoGeneral != null) logoFondoGeneral.SetActive(true);
        if (logoInternoCrono != null) logoInternoCrono.SetActive(false);
        ConfigurarPantallaSiempreEncendida(false);
    }

    void OnApplicationPause(bool pausado)
    {
        if (estaContando)
        {
            if (pausado) ConfigurarPantallaSiempreEncendida(false);
            else { RecalcularTiempo(); ConfigurarPantallaSiempreEncendida(true); }
        }
    }

    void Update()
    {
        if (estaContando)
        {
            RecalcularTiempo();
            ActualizarInterfazUI();
            ManejarTransicionDeColor();
            ManejarLluviaUI();
        }
    }

    // --- ANIMACIÓN DE POSICIÓN Y TAMAÑO ---
    void AnimarBotonCrono(bool presionado)
    {
        if (imagenBotonCrono == null) return;

        Vector2 destinoPos = presionado ? posDentro : posFuera;
        Vector2 destinoSize = presionado ? sizeDentro : sizeFuera;

        LeanTween.cancel(imagenBotonCrono.gameObject);

        // Mueve la posición
        LeanTween.move(imagenBotonCrono, destinoPos, tiempoAnimBoton).setEase(LeanTweenType.easeOutQuad);

        // Cambia el Width y Height (SizeDelta)
        LeanTween.size(imagenBotonCrono, destinoSize, tiempoAnimBoton).setEase(LeanTweenType.easeOutQuad);
    }

    void RecalcularTiempo()
    {
        TimeSpan diferencia = DateTime.Now - tiempoInicioReal;
        tiempoTranscurrido = (float)diferencia.TotalSeconds + tiempoAcumuladoAnterior;
    }

    public void ComenzarDucha()
    {
        estaContando = true;
        tiempoInicioReal = DateTime.Now;
        tiempoAcumuladoAnterior = 0;
        ConfigurarPantallaSiempreEncendida(true);
        foreach (var gota in gotas) gota.gameObject.SetActive(true);

        AnimarBotonCrono(true); // Hundir y encoger

        SetEstadoBotones(false, true, false, false, false);
        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);
    }

    public void FinalizarDucha()
    {
        estaContando = false;
        ConfigurarPantallaSiempreEncendida(false);

        AnimarBotonCrono(false); // Sacar y agrandar

        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);
        StartCoroutine(SecuenciaGuardadoAutomatico());
    }

    public void ReiniciarTodo()
    {
        estaContando = false;
        tiempoTranscurrido = 0f;
        tiempoAcumuladoAnterior = 0f;
        ActualizarInterfazUI();
        if (fondoColor != null) fondoColor.color = colorVerde;
        if (aguaVisual != null) aguaVisual.anchoredPosition = new Vector2(0, posMinY);

        AnimarBotonCrono(false);

        SetEstadoBotones(true, false, false, false, false);
        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);
    }

    void ConfigurarPantallaSiempreEncendida(bool encendida)
    {
        if (encendida) Screen.sleepTimeout = SleepTimeout.NeverSleep;
        else Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    void ManejarLluviaUI()
    {
        if (gotas == null || gotas.Length == 0) return;
        float tMeta = minutosParaLlenado * 60f;
        bool aguaLlena = tiempoTranscurrido >= tMeta;
        for (int i = 0; i < gotas.Length; i++)
        {
            if (gotaEsperando[i]) continue;
            gotas[i].anchoredPosition += Vector2.down * velocidadesGotas[i] * Time.deltaTime;
            if (gotas[i].anchoredPosition.y <= puntoFinalY)
            {
                if (aguaLlena) gotas[i].gameObject.SetActive(false);
                else StartCoroutine(EsperarParaReiniciarGota(i));
            }
        }
    }

    IEnumerator EsperarParaReiniciarGota(int i)
    {
        gotaEsperando[i] = true;
        gotas[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 1.3f));
        if (estaContando) { ReiniciarGota(i); gotas[i].gameObject.SetActive(true); }
        gotaEsperando[i] = false;
    }

    void ReiniciarGota(int i)
    {
        velocidadesGotas[i] = UnityEngine.Random.Range(180f, 320f);
        float nuevaX = UnityEngine.Random.Range(-80f, 80f);
        gotas[i].anchoredPosition = new Vector2(nuevaX, puntoInicialY);
    }

    IEnumerator SecuenciaGuardadoAutomatico()
    {
        SetEstadoBotones(false, false, true, false, false);
        yield return new WaitForSeconds(0.7f);
        GuardarEnHistorialInterno();
        SetEstadoBotones(false, false, false, true, false);
        yield return new WaitForSeconds(0.7f);
        SetEstadoBotones(false, false, false, false, true);
    }

    void GuardarEnHistorialInterno()
    {
        string nom = ManejadorRegistro.instance != null ? ManejadorRegistro.instance.nombreSeleccionado : "Invitado";
        if (ManejadorRegistro.instance != null)
        {
            foreach (var m in ManejadorRegistro.instance.listaDeMiembros)
            {
                if (m.nombre == nom)
                {
                    if (m.mejorTiempo <= 0 || tiempoTranscurrido < m.mejorTiempo) m.mejorTiempo = tiempoTranscurrido;
                    ManejadorRegistro.RegistroBano reg = new ManejadorRegistro.RegistroBano();
                    reg.duracion = tiempoTranscurrido;
                    reg.fecha = DateTime.Now.ToString("dd/MM/yyyy");
                    reg.hora = DateTime.Now.ToString("hh:mm tt");
                    if (m.historialBanos == null) m.historialBanos = new List<ManejadorRegistro.RegistroBano>();
                    m.historialBanos.Insert(0, reg);
                    if (m.historialBanos.Count > 5) m.historialBanos.RemoveRange(5, m.historialBanos.Count - 5);
                    break;
                }
            }
            ManejadorRegistro.instance.GuardarEnDisco();
        }
    }

    void ActualizarInterfazUI()
    {
        int min = Mathf.FloorToInt(tiempoTranscurrido / 60);
        int seg = Mathf.FloorToInt(tiempoTranscurrido % 60);
        if (textoCronometro != null) textoCronometro.text = string.Format("{0:00}:{1:00}", min, seg);
        float litros = (tiempoTranscurrido / 60f) * litrosPorMinuto;
        if (textoConsumo != null) textoConsumo.text = litros.ToString("F1") + " L";
        if (aguaVisual != null)
        {
            float tMeta = minutosParaLlenado * 60f;
            float progreso = Mathf.Clamp01(tiempoTranscurrido / tMeta);
            float nuevaY = Mathf.Lerp(posMinY, posMaxY, progreso);
            float bamboleo = Mathf.Sin(Time.time * 2.5f) * 4f;
            float rotacion = Mathf.Sin(Time.time * 1.5f) * 1f;
            aguaVisual.anchoredPosition = new Vector2(bamboleo, nuevaY);
            aguaVisual.localRotation = Quaternion.Euler(0, 0, rotacion);
        }
    }

    void ManejarTransicionDeColor()
    {
        float minAct = tiempoTranscurrido / 60f;
        Color col;
        if (minAct < minutosParaAmarillo) col = Color.Lerp(colorVerde, colorAmarillo, minAct / minutosParaAmarillo);
        else if (minAct < minutosParaRojo) col = Color.Lerp(colorAmarillo, colorRojo, (minAct - minutosParaAmarillo) / (minutosParaRojo - minutosParaAmarillo));
        else col = colorRojo;
        if (fondoColor != null) fondoColor.color = col;
    }

    void SetEstadoBotones(bool com, bool fin, bool gndo, bool gdo, bool vol)
    {
        if (btnComenzar) btnComenzar.SetActive(com);
        if (btnFinalizar) btnFinalizar.SetActive(fin);
        if (btnGuardando) btnGuardando.SetActive(gndo);
        if (btnGuardado) btnGuardado.SetActive(gdo);
        if (btnVolverEmpezar) btnVolverEmpezar.SetActive(vol);
    }

    void PrepararEscena()
    {
        tiempoTranscurrido = 0f;
        if (gotas != null)
        {
            velocidadesGotas = new float[gotas.Length];
            gotaEsperando = new bool[gotas.Length];
            for (int i = 0; i < gotas.Length; i++) ReiniciarGota(i);
        }
        ActualizarInterfazUI();

        // Estado inicial del botón
        if (imagenBotonCrono != null)
        {
            imagenBotonCrono.anchoredPosition = posFuera;
            imagenBotonCrono.sizeDelta = sizeFuera;
        }

        if (fondoColor != null) fondoColor.color = colorVerde;
        SetEstadoBotones(true, false, false, false, false);
    }

    public void GuardarTiempoFinal() { if (tiempoTranscurrido > 0) GuardarEnHistorialInterno(); }
}