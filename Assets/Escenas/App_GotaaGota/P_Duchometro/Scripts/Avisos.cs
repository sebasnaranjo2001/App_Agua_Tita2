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

    [Header("--- TARJETAS DE AVISOS ---")]
    public GameObject avisoCreaMiembro;
    public GameObject avisoLimiteMiembros;
    public GameObject avisoSeleccionaAlguien;

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
        bool hayDatos = AlgunaPersonaTieneDatos();
        bool registroAbierto = (ventanaRegistro != null && ventanaRegistro.activeInHierarchy);

        bool cronoCorriendo = false;
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null) cronoCorriendo = crono.estaContando;

        if (avisoCreaMiembro) avisoCreaMiembro.SetActive(!hayGente && !registroAbierto && !cronoCorriendo);
        if (panelDeslizable) panelDeslizable.SetActive(hayGente && !registroAbierto && !cronoCorriendo);

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
            SetAlpha(btnRanking, (hayGente && hayDatos) ? 1f : 0.4f);
            float alphaSel = (miembroSeleccionado != null) ? 1f : 0.4f;
            SetAlpha(btnCronometro, alphaSel);
            SetAlpha(btnEmpezar, alphaSel);
        }

        if (navegador != null) navegador.ActualizarElementosRegistro(hayGente, conPum);
    }

    bool AlgunaPersonaTieneDatos()
    {
        if (ManejadorRegistro.instance == null) return false;
        foreach (var m in ManejadorRegistro.instance.listaDeMiembros)
        {
            if (m.historialBanos != null && m.historialBanos.Count > 0) return true;
        }
        return false;
    }

    void SetAlpha(UnityEngine.UI.Button btn, float a)
    {
        if (btn == null) return;
        CanvasGroup cg = btn.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = a;
    }

    // --- NAVEGACION ---
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
        if (ManejadorRegistro.instance.listaDeMiembros.Count == 0) { AnimarAtencionAviso(); return; }
        if (miembroSeleccionado == null) { StartCoroutine(FlashAviso(avisoSeleccionaAlguien)); return; }
        if (navegador != null) navegador.IrACronometro();
    }

    public void IntentarIrARanking()
    {
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;
        if (ManejadorRegistro.instance.listaDeMiembros.Count == 0) { AnimarAtencionAviso(); return; }
        if (!AlgunaPersonaTieneDatos()) return;
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
        else { StartCoroutine(FlashAviso(avisoLimiteMiembros)); }
    }

    // --- MIEMBROS Y LEANTWEEN ---
    public void RegistrarSeleccion(SeleccionMiembros nuevo)
    {
        if (nuevo != null && miembroSeleccionado == nuevo) return;

        if (miembroSeleccionado != null && miembroSeleccionado != nuevo) miembroSeleccionado.Deseleccionar();
        miembroSeleccionado = nuevo;

        if (nuevo != null)
        {
            LeanTween.cancel(nuevo.gameObject);

            // 1. Aseguramos que inicie en su tamaño normal (100%)
            nuevo.transform.localScale = Vector3.one;

            // 2. EL VERDADERO PUM: Se encoge rápido al 90% (0.05 segundos)
            LeanTween.scale(nuevo.gameObject, Vector3.one * 0.9f, 0.05f).setEaseOutQuad().setOnComplete(() => {

                // 3. Vuelve a subir exactamente al 100% y se clava ahí sin rebotar.
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
        Invoke("RefrescarConPum", 0.1f);
    }

    public void ForzarOcultarAvisos()
    {
        StopAllCoroutines();
        if (avisoLimiteMiembros) { LeanTween.cancel(avisoLimiteMiembros); avisoLimiteMiembros.SetActive(false); }
        if (avisoSeleccionaAlguien) { LeanTween.cancel(avisoSeleccionaAlguien); avisoSeleccionaAlguien.SetActive(false); }
    }

    private void AnimarAtencionAviso()
    {
        if (avisoCreaMiembro != null && avisoCreaMiembro.activeSelf)
        {
            LeanTween.cancel(avisoCreaMiembro);
            LeanTween.scale(avisoCreaMiembro, Vector3.one * 1.1f, 0.15f).setEaseOutQuad().setOnComplete(() => {
                LeanTween.scale(avisoCreaMiembro, Vector3.one, 0.15f).setEaseInQuad();
            });
        }
    }

    void RefrescarConPum() { ActualizarInterfazSegunContador(true); }
    public void NotificarMiembroGuardado() { ActualizarInterfazSegunContador(true); }
    public void PuenteGuardar() { ManejadorRegistro.instance.GuardarDatos(); }

    IEnumerator FlashAviso(GameObject obj)
    {
        if (!obj) yield break;
        obj.SetActive(true);
        obj.transform.localScale = Vector3.zero;
        LeanTween.scale(obj, Vector3.one, 0.4f).setEaseOutBack();
        yield return new WaitForSeconds(2.5f);
        LeanTween.scale(obj, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => obj.SetActive(false));
    }

    void OcultarAvisosAlInicio()
    {
        if (avisoCreaMiembro) avisoCreaMiembro.SetActive(false);
        if (avisoLimiteMiembros) avisoLimiteMiembros.SetActive(false);
        if (avisoSeleccionaAlguien) avisoSeleccionaAlguien.SetActive(false);
    }
}