using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    [Header("--- ESTADO ---")]
    public SeleccionMiembros miembroSeleccionado;

    private ManejadorNavegacion navegador;

    void Awake()
    {
        if (instance == null) instance = this;
        OcultarAvisosAlInicio();
    }

    void Start()
    {
        navegador = Object.FindFirstObjectByType<ManejadorNavegacion>();
        ActualizarInterfazSegunContador(true);
    }

    public void ForzarOcultarAvisos()
    {
        StopAllCoroutines();
        if (avisoLimiteMiembros) { LeanTween.cancel(avisoLimiteMiembros); avisoLimiteMiembros.SetActive(false); }
        if (avisoSeleccionaAlguien) { LeanTween.cancel(avisoSeleccionaAlguien); avisoSeleccionaAlguien.SetActive(false); }
    }

    public void ActualizarInterfazSegunContador(bool conPum)
    {
        if (ManejadorRegistro.instance == null) return;

        int total = ManejadorRegistro.instance.listaDeMiembros.Count;
        bool hayGente = total > 0;
        bool registroAbierto = (ventanaRegistro != null && ventanaRegistro.activeInHierarchy);

        if (avisoCreaMiembro) avisoCreaMiembro.SetActive(!hayGente && !registroAbierto);
        if (panelDeslizable) panelDeslizable.SetActive(hayGente && !registroAbierto);

        SetAlpha(btnRegistro, 1f);
        SetAlpha(btnRanking, hayGente ? 1f : 0.4f);
        float alphaSel = (miembroSeleccionado != null) ? 1f : 0.4f;
        SetAlpha(btnCronometro, alphaSel);
        SetAlpha(btnEmpezar, alphaSel);

        if (navegador != null) navegador.ActualizarElementosRegistro(hayGente, conPum);
    }

    void SetAlpha(UnityEngine.UI.Button btn, float a)
    {
        if (btn == null) return;
        CanvasGroup cg = btn.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = a;
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

    public void IntentarIrACronometro()
    {
        if (ManejadorRegistro.instance.listaDeMiembros.Count == 0)
        {
            AnimarAtencionAviso();
            return;
        }
        if (miembroSeleccionado == null) { StartCoroutine(FlashAviso(avisoSeleccionaAlguien)); return; }
        if (navegador != null) navegador.IrACronometro();
    }

    public void IntentarIrARanking()
    {
        if (ManejadorRegistro.instance.listaDeMiembros.Count == 0)
        {
            AnimarAtencionAviso();
            return;
        }
        if (navegador != null) navegador.IrARanking();
    }

    public void RegistrarSeleccion(SeleccionMiembros nuevo)
    {
        if (miembroSeleccionado != null && miembroSeleccionado != nuevo) miembroSeleccionado.Deseleccionar();
        miembroSeleccionado = nuevo;
        if (nuevo != null)
        {
            LeanTween.cancel(nuevo.gameObject);
            nuevo.transform.localScale = Vector3.one * 0.8f;
            LeanTween.scale(nuevo.gameObject, Vector3.one, 0.3f).setEaseOutBack();

            // --- LÍNEA RESTAURADA: Guarda el nombre para el Cronómetro ---
            if (ManejadorRegistro.instance != null)
                ManejadorRegistro.instance.nombreSeleccionado = nuevo.gameObject.name;
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

    void RefrescarConPum() { ActualizarInterfazSegunContador(true); }
    public void NotificarMiembroGuardado() { ActualizarInterfazSegunContador(true); }

    public void IntentarAbrirRegistro()
    {
        if (ManejadorRegistro.instance.listaDeMiembros.Count < 7)
        {
            if (navegador != null) navegador.AbrirTarjetaRegistro();
            ActualizarInterfazSegunContador(false);
        }
        else { StartCoroutine(FlashAviso(avisoLimiteMiembros)); }
    }

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