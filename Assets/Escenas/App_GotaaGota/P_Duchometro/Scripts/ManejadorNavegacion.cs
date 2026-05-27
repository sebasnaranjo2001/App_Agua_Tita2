using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManejadorNavegacion : MonoBehaviour
{
    [Header("--- PANELES PRINCIPALES ---")]
    public GameObject panelRegistro;
    public GameObject panelCronometro;
    public GameObject panelRanking;

    [Header("--- BARRA DE SELECCIÓN ---")]
    public RectTransform indicadorSeleccion;
    public float tiempoAnimacion = 0.35f;
    public LeanTweenType tipoCurva = LeanTweenType.easeOutBack;

    [Header("Configuración Barra")]
    public float posRegX; public float anchoReg;
    public float posCronX; public float anchoCron;
    public float posRankX; public float anchoRank;

    [Header("--- ELEMENTOS UI REGISTRO ---")]
    public GameObject textoNumeroMiembros; // <-- Nuevo elemento en reemplazo del tablero
    public GameObject logoApp;
    public GameObject avisos;
    public GameObject botonEmpezar;
    public GameObject botonAnadirGrande;
    public GameObject panelBotonesPequenos;
    public GameObject panelDeslizable;
    public GameObject panelTarjetaRegistro;

    [Header("--- ELEMENTOS UI CRONOMETRO ---")]
    public GameObject panelPrincipalCrono;
    public GameObject panelSecundarioCrono;
    public GameObject panelBotonesCrono;

    private Dictionary<GameObject, Vector3> escalasOriginales = new Dictionary<GameObject, Vector3>();
    private Vector2 posDisenoReg;
    private string panelActualNombre = "";

    void Awake()
    {
        RegistrarEscalas();
        if (panelRegistro) posDisenoReg = panelRegistro.GetComponent<RectTransform>().anchoredPosition;
    }

    void OnEnable() { ConfiguracionInicial(); }

    void RegistrarEscalas()
    {
        GameObject[] todos = { textoNumeroMiembros, logoApp, avisos, botonEmpezar, botonAnadirGrande,
                              panelBotonesPequenos, panelDeslizable, panelTarjetaRegistro,
                              panelPrincipalCrono, panelSecundarioCrono, panelBotonesCrono };
        foreach (GameObject obj in todos)
        {
            if (obj != null && !escalasOriginales.ContainsKey(obj))
                escalasOriginales.Add(obj, obj.transform.localScale);
        }
    }

    public void ApagarTodo()
    {
        if (panelRegistro) panelRegistro.SetActive(false);
        if (panelCronometro) panelCronometro.SetActive(false);
        if (panelRanking) panelRanking.SetActive(false);
        if (panelTarjetaRegistro) panelTarjetaRegistro.SetActive(false);
        if (Avisos.instance != null) Avisos.instance.ForzarOcultarAvisos();
    }

    public void ConfiguracionInicial()
    {
        panelActualNombre = "";
        // --- LOGICA DE PRIORIDAD ---
        Cronometro crono = Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando)
        {
            IrACronometro();
        }
        else
        {
            IrARegistro();
        }
    }

    public void IrARegistro()
    {
        if (panelActualNombre == "registro") return;
        ApagarTodo();
        panelRegistro.SetActive(true);
        panelRegistro.GetComponent<RectTransform>().anchoredPosition = posDisenoReg;
        AnimarPum();
        ActualizarBarraVisual(posRegX, anchoReg);
        panelActualNombre = "registro";
    }

    public void IrACronometro()
    {
        if (panelActualNombre == "cronometro") return;
        ApagarTodo();
        panelCronometro.SetActive(true);
        AnimarPumCrono();
        ActualizarBarraVisual(posCronX, anchoCron);
        panelActualNombre = "cronometro";
    }

    public void IrARanking()
    {
        if (panelActualNombre == "ranking") return;
        ApagarTodo();
        panelRanking.SetActive(true);
        ActualizarBarraVisual(posRankX, anchoRank);
        panelActualNombre = "ranking";
    }

    public void ActualizarBarraVisual(float x, float w)
    {
        if (indicadorSeleccion == null) return;
        LeanTween.cancel(indicadorSeleccion.gameObject);
        LeanTween.move(indicadorSeleccion, new Vector2(x, indicadorSeleccion.anchoredPosition.y), tiempoAnimacion).setEase(tipoCurva);
        LeanTween.size(indicadorSeleccion, new Vector2(w, indicadorSeleccion.sizeDelta.y), tiempoAnimacion).setEase(tipoCurva);
    }

    public void AnimarPum()
    {
        List<GameObject> aAnimar = new List<GameObject>();
        // textoNumeroMiembros agregado a la lista de posibles para la animación
        GameObject[] posibles = { textoNumeroMiembros, avisos, botonEmpezar, botonAnadirGrande, panelBotonesPequenos, panelDeslizable };
        foreach (GameObject obj in posibles)
        {
            if (obj != null && obj.activeSelf) { aAnimar.Add(obj); obj.transform.localScale = Vector3.zero; }
        }
        for (int i = 0; i < aAnimar.Count; i++)
        {
            Vector3 ef = escalasOriginales.ContainsKey(aAnimar[i]) ? escalasOriginales[aAnimar[i]] : Vector3.one;
            LeanTween.scale(aAnimar[i], ef, 0.45f).setEase(LeanTweenType.easeOutBack).setDelay(0.05f * i);
        }
    }

    public void AnimarPumCrono()
    {
        List<GameObject> aAnimar = new List<GameObject>();
        GameObject[] posibles = { panelPrincipalCrono, panelSecundarioCrono, panelBotonesCrono };
        foreach (GameObject obj in posibles)
        {
            if (obj != null) { aAnimar.Add(obj); obj.transform.localScale = Vector3.zero; }
        }
        for (int i = 0; i < aAnimar.Count; i++)
        {
            Vector3 ef = escalasOriginales.ContainsKey(aAnimar[i]) ? escalasOriginales[aAnimar[i]] : Vector3.one;
            LeanTween.scale(aAnimar[i], ef, 0.45f).setEase(LeanTweenType.easeOutBack).setDelay(0.08f * i);
        }
    }

    public void ActualizarElementosRegistro(bool hay, bool conPum)
    {
        if (botonAnadirGrande) botonAnadirGrande.SetActive(!hay);
        if (panelBotonesPequenos) panelBotonesPequenos.SetActive(hay);
        if (botonEmpezar) botonEmpezar.SetActive(hay);
        if (conPum) AnimarPum();
    }

    public void AbrirTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null) return;
        panelTarjetaRegistro.SetActive(true);
        panelTarjetaRegistro.transform.localScale = Vector3.zero;
        LeanTween.scale(panelTarjetaRegistro, escalasOriginales[panelTarjetaRegistro], 0.4f).setEase(LeanTweenType.easeOutBack);
    }

    public void CerrarTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null) return;
        LeanTween.scale(panelTarjetaRegistro, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => {
            panelTarjetaRegistro.SetActive(false);
            if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);
        });
    }
}