using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Avisos : MonoBehaviour
{
    public static Avisos instance;

    [Header("--- PANELES ---")]
    public GameObject panelDeslizable;
    public GameObject ventanaRegistro;

    [Header("--- FONDO VACÍO (PANTALLA INICIO) ---")]
    public GameObject avisoCreaMiembro;

    [Header("--- NUEVO SISTEMA DE AVISOS (POPUP) ---")]
    public GameObject contenedorAvisos;
    public GameObject blurAvisos;
    public GameObject panelBlancoAvisos;

    [Header("Textos Intercambiables")]
    public GameObject popCreaMiembro;
    public GameObject popSeleccionaPrimero;
    public GameObject popLimiteMiembros;
    public GameObject popMiembroDuplicado;

    [Header("--- BOTONES NAV ---")]
    public UnityEngine.UI.Button btnRegistro;
    public UnityEngine.UI.Button btnRanking;
    public UnityEngine.UI.Button btnCronometro;
    public UnityEngine.UI.Button btnEmpezar;

    [Header("--- REFERENCIAS EXTERNAS ---")]
    public ManejadorNavegacion navegador;

    [Header("--- ESTADO ---")]
    public SeleccionMiembros miembroSeleccionado;

    void Awake()
    {
        if (instance == null) instance = this;
        OcultarAvisosAlInicio();
    }

    void Start()
    {
        if (navegador == null) navegador = Object.FindFirstObjectByType<ManejadorNavegacion>();
        ActualizarInterfazSegunContador(true);
    }

    public void ActualizarInterfazSegunContador(bool conPum)
    {
        if (ManejadorRegistro.instance == null) return;

        int total = ManejadorRegistro.instance.listaDeMiembros.Count;
        bool hayGente = total > 0;

        if (avisoCreaMiembro) avisoCreaMiembro.SetActive(!hayGente);
        if (panelDeslizable) panelDeslizable.SetActive(hayGente);

        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        bool cronoCorriendo = (crono != null && crono.estaContando);

        if (cronoCorriendo)
        {
            SetAlpha(btnRegistro, 0.4f);
            SetAlpha(btnRanking, 0.4f);
            SetAlpha(btnCronometro, 1f);
            SetAlpha(btnEmpezar, 1f);
        }
        else
        {
            SetAlpha(btnRegistro, 1f);
            SetAlpha(btnRanking, 1f);
            float alphaSel = (miembroSeleccionado != null) ? 1f : 0.4f;
            SetAlpha(btnCronometro, alphaSel);
            SetAlpha(btnEmpezar, alphaSel);
        }

        if (navegador != null) navegador.ActualizarElementosRegistro(hayGente, conPum);
    }

    void SetAlpha(UnityEngine.UI.Button btn, float a)
    {
        if (btn == null) return;
        CanvasGroup cg = btn.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = a;
    }

    public void IntentarIrARegistro()
    {
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;
        if (navegador != null) navegador.IrARegistro();
    }

    public void IntentarIrACronometro()
    {
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;
        if (navegador != null) navegador.IrACronometro();
    }

    public void IntentarIrARanking()
    {
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;
        if (navegador != null) navegador.IrARanking();
    }

    public void IntentarAbrirRegistro()
    {
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;

        if (ManejadorRegistro.instance.listaDeMiembros.Count < 7)
        {
            if (navegador != null) navegador.AbrirTarjetaRegistro();
            ActualizarInterfazSegunContador(false);
        }
        else
        {
            MostrarAvisoPopUp(popLimiteMiembros);
        }
    }

    public bool VerificarEstadoRegistro()
    {
        if (ManejadorRegistro.instance.listaDeMiembros.Count == 0)
        {
            if (navegador != null) navegador.IrARegistro();
            MostrarAvisoPopUp(popCreaMiembro);
            return false;
        }
        if (miembroSeleccionado == null)
        {
            if (navegador != null) navegador.IrARegistro();
            MostrarAvisoPopUp(popSeleccionaPrimero);
            return false;
        }
        return true;
    }

    // --- CORRECCIÓN: FUNCIÓN EXCLUSIVA DE CONTROL PARA EL BOTÓN EMPEZAR DE LA UI ---
    public void PresionarBotonEmpezar()
    {
        if (VerificarEstadoRegistro())
        {
            if (navegador != null) navegador.IrACronometro();
        }
    }

    public void MostrarAvisoPopUp(GameObject contenidoActivar)
    {
        if (popCreaMiembro) popCreaMiembro.SetActive(false);
        if (popSeleccionaPrimero) popSeleccionaPrimero.SetActive(false);
        if (popLimiteMiembros) popLimiteMiembros.SetActive(false);
        if (popMiembroDuplicado) popMiembroDuplicado.SetActive(false);

        if (contenidoActivar) contenidoActivar.SetActive(true);
        if (contenedorAvisos) contenedorAvisos.SetActive(true);

        if (blurAvisos != null)
        {
            CanvasGroup cg = blurAvisos.GetComponent<CanvasGroup>() ?? blurAvisos.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            LeanTween.alphaCanvas(cg, 1f, 0.3f);
        }

        if (panelBlancoAvisos != null)
        {
            panelBlancoAvisos.transform.localScale = Vector3.zero;
            LeanTween.scale(panelBlancoAvisos, Vector3.one, 0.4f).setEaseOutBack();
        }
    }

    public void CerrarAvisoPopUp()
    {
        if (blurAvisos != null)
        {
            CanvasGroup cg = blurAvisos.GetComponent<CanvasGroup>();
            if (cg != null) LeanTween.alphaCanvas(cg, 0f, 0.3f);
        }

        if (panelBlancoAvisos != null)
        {
            LeanTween.scale(panelBlancoAvisos, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => {
                if (contenedorAvisos) contenedorAvisos.SetActive(false);
            });
        }
        else if (contenedorAvisos) contenedorAvisos.SetActive(false);
    }

    public void RegistrarSeleccion(SeleccionMiembros nuevo)
    {
        if (nuevo != null && miembroSeleccionado == nuevo) return;
        if (miembroSeleccionado != null && miembroSeleccionado != nuevo) miembroSeleccionado.Deseleccionar();
        miembroSeleccionado = nuevo;

        if (nuevo != null)
        {
            LeanTween.cancel(nuevo.gameObject);
            nuevo.transform.localScale = Vector3.one;
            LeanTween.scale(nuevo.gameObject, Vector3.one * 0.9f, 0.05f).setEaseOutQuad().setOnComplete(() => {
                LeanTween.scale(nuevo.gameObject, Vector3.one, 0.1f).setEaseOutQuad();
            });
            if (ManejadorRegistro.instance != null) ManejadorRegistro.instance.nombreSeleccionado = nuevo.gameObject.name;
        }
        ActualizarInterfazSegunContador(false);
    }

    public void ClickEnEliminar()
    {
        if (miembroSeleccionado == null) return;
        ManejadorRegistro.instance.RemoverMiembroDeLaLista(miembroSeleccionado.gameObject.name);
        Destroy(miembroSeleccionado.gameObject);
        miembroSeleccionado = null;

        if (ManejadorRegistro.instance != null) ManejadorRegistro.instance.nombreSeleccionado = "";

        Invoke("RefrescarConPum", 0.1f);
    }

    public void ForzarOcultarAvisos()
    {
        if (contenedorAvisos) { LeanTween.cancel(panelBlancoAvisos); contenedorAvisos.SetActive(false); }
    }

    void RefrescarConPum() { ActualizarInterfazSegunContador(true); }
    public void NotificarMiembroGuardado() { ActualizarInterfazSegunContador(true); }
    void OcultarAvisosAlInicio() { if (avisoCreaMiembro) avisoCreaMiembro.SetActive(false); if (contenedorAvisos) contenedorAvisos.SetActive(false); }
}