using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ManejadorNavegacion : MonoBehaviour
{
    [Header("--- PANELES PRINCIPALES ---")]
    public GameObject panelRegistro;
    public GameObject panelCronometro;
    public GameObject panelRanking;

    [Header("--- BARRA DE SELECCIÓN (LA OSCURA) ---")]
    public RectTransform indicadorSeleccion;
    public float tiempoAnimacion = 0.4f;
    public LeanTweenType tipoCurva = LeanTweenType.easeOutBack;

    [Header("Configuración Barra (Pos y Ancho)")]
    public float posRegX; public float anchoReg;
    public float posCronX; public float anchoCron;
    public float posRankX; public float anchoRank;

    [Header("--- ELEMENTOS PUM (REGISTRO) ---")]
    public GameObject tablero;
    public GameObject avisos;
    public GameObject botonEmpezar;
    public GameObject botonAnadir;
    public GameObject panelDeslizable;
    public GameObject botonAnadirBorrar;

    [Header("--- CONTROL DEL SCROLL ---")]
    public ScrollRect scrollListaMiembros;

    [Header("--- SUB-PANEL REGISTRO ---")]
    public GameObject panelTarjetaRegistro;

    [Header("--- SCRIPTS EXTERNOS ---")]
    public ManejadorRanking scriptRanking;
    public Cronometro scriptCronometro;

    private Dictionary<GameObject, Vector3> escalasOriginales = new Dictionary<GameObject, Vector3>();
    private Vector2 posDisenoReg, posDisenoCron, posDisenoRank;
    private float distanciaX = 1100f;
    private float distanciaY = 2000f;
    private GameObject panelActualObj;
    private string panelActualNombre = "registro";

    private void Start()
    {
        RegistrarEscalas();
        GuardarPosicionesDeDiseno();
        ConfiguracionInicial();
    }

    void RegistrarEscalas()
    {
        GameObject[] objetosPum = { tablero, avisos, botonEmpezar, botonAnadir, panelDeslizable, botonAnadirBorrar, panelTarjetaRegistro };
        foreach (GameObject obj in objetosPum)
        {
            if (obj != null && !escalasOriginales.ContainsKey(obj))
                escalasOriginales.Add(obj, obj.transform.localScale);
        }
    }

    void GuardarPosicionesDeDiseno()
    {
        if (panelRegistro) posDisenoReg = panelRegistro.GetComponent<RectTransform>().anchoredPosition;
        if (panelCronometro) posDisenoCron = panelCronometro.GetComponent<RectTransform>().anchoredPosition;
        if (panelRanking) posDisenoRank = panelRanking.GetComponent<RectTransform>().anchoredPosition;
    }

    public void ConfiguracionInicial()
    {
        if (panelCronometro) panelCronometro.SetActive(false);
        if (panelRanking) panelRanking.SetActive(false);
        if (panelTarjetaRegistro) panelTarjetaRegistro.SetActive(false);

        if (panelRegistro)
        {
            panelRegistro.SetActive(true);
            panelActualObj = panelRegistro;
            panelRegistro.GetComponent<RectTransform>().anchoredPosition = posDisenoReg;
            AnimarEntradaRegistro();
            ResetearScrollAlInicio();
        }

        panelActualNombre = "registro";
        ActualizarBarraInmediato(posRegX, anchoReg);
    }

    public void IrARegistro()
    {
        if (panelActualNombre == "registro") return;
        panelRegistro.SetActive(true);
        if (panelTarjetaRegistro) panelTarjetaRegistro.SetActive(false);
        RectTransform rt = panelRegistro.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(posDisenoReg.x - distanciaX, posDisenoReg.y);
        MoverYApagar(panelActualObj, ObtenerPosicionSalida(panelActualNombre, "derecha"));
        MoverPanel(rt, posDisenoReg);
        AnimarEntradaRegistro();
        ResetearScrollAlInicio();
        AnimarBarraSeleccion(posRegX, anchoReg);
        panelActualObj = panelRegistro;
        panelActualNombre = "registro";
    }

    public void IrACronometro()
    {
        if (panelActualNombre == "cronometro") return;
        panelCronometro.SetActive(true);
        RectTransform rt = panelCronometro.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(posDisenoCron.x, posDisenoCron.y + distanciaY);
        MoverYApagar(panelActualObj, ObtenerPosicionSalida(panelActualNombre, "abajo"));
        MoverPanel(rt, posDisenoCron);
        AnimarBarraSeleccion(posCronX, anchoCron);
        panelActualObj = panelCronometro;
        panelActualNombre = "cronometro";
    }

    public void IrARanking()
    {
        if (panelActualNombre == "ranking") return;
        if (scriptRanking != null) scriptRanking.GenerarRanking();
        panelRanking.SetActive(true);
        RectTransform rt = panelRanking.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(posDisenoRank.x + distanciaX, posDisenoRank.y);
        MoverYApagar(panelActualObj, ObtenerPosicionSalida(panelActualNombre, "izquierda"));
        MoverPanel(rt, posDisenoRank);
        AnimarBarraSeleccion(posRankX, anchoRank);
        panelActualObj = panelRanking;
        panelActualNombre = "ranking";
    }

    public void AbrirTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null) return;
        tablero.SetActive(false);
        avisos.SetActive(false);
        botonAnadir.SetActive(false);
        panelDeslizable.SetActive(false);
        botonAnadirBorrar.SetActive(false);
        panelTarjetaRegistro.SetActive(true);
        panelTarjetaRegistro.transform.localScale = Vector3.zero;
        Pop(panelTarjetaRegistro, 0.4f, 0.1f);
    }

    public void CerrarTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null) return;
        LeanTween.scale(panelTarjetaRegistro, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            panelTarjetaRegistro.SetActive(false);
            tablero.SetActive(true);
            avisos.SetActive(true);
            botonAnadir.SetActive(true);
            panelDeslizable.SetActive(true);
            botonAnadirBorrar.SetActive(true);
            AnimarEntradaRegistro();
            ResetearScrollAlInicio();
        });
    }

    public void ResetearScrollAlInicio()
    {
        if (gameObject.activeInHierarchy) StartCoroutine(ForzarScrollArriba());
    }

    IEnumerator ForzarScrollArriba()
    {
        yield return new WaitForEndOfFrame();
        if (scrollListaMiembros != null)
        {
            scrollListaMiembros.verticalNormalizedPosition = 1f;
            Canvas.ForceUpdateCanvases();
            scrollListaMiembros.verticalNormalizedPosition = 1f;
        }
    }

    private void AnimarEntradaRegistro()
    {
        SetScaleZero(tablero, avisos, botonEmpezar, botonAnadir, panelDeslizable, botonAnadirBorrar);
        Pop(tablero, 0.5f, 0.1f);
        Pop(avisos, 0.4f, 0.15f);
        Pop(botonEmpezar, 0.4f, 0.2f);
        Pop(botonAnadir, 0.4f, 0.25f);
        Pop(panelDeslizable, 0.5f, 0.3f);
        Pop(botonAnadirBorrar, 0.4f, 0.35f);
    }

    private void Pop(GameObject obj, float tiempo, float delay)
    {
        if (obj == null) return;
        Vector3 escalaFinal = escalasOriginales.ContainsKey(obj) ? escalasOriginales[obj] : Vector3.one;
        LeanTween.cancel(obj);
        LeanTween.scale(obj, escalaFinal, tiempo).setEase(LeanTweenType.easeOutBack).setDelay(delay);
    }

    private void SetScaleZero(params GameObject[] objetos)
    {
        foreach (GameObject obj in objetos) if (obj != null) obj.transform.localScale = Vector3.zero;
    }

    private void MoverPanel(RectTransform rt, Vector2 destino)
    {
        if (rt == null) return;
        LeanTween.cancel(rt.gameObject);
        LeanTween.move(rt, destino, tiempoAnimacion).setEase(LeanTweenType.easeOutCubic);
    }

    private void MoverYApagar(GameObject objViejo, Vector2 destino)
    {
        if (objViejo == null) return;
        RectTransform rtViejo = objViejo.GetComponent<RectTransform>();
        LeanTween.cancel(objViejo);
        LeanTween.move(rtViejo, destino, tiempoAnimacion).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => {
            objViejo.SetActive(false);
        });
    }

    private void AnimarBarraSeleccion(float destinoX, float nuevoAncho)
    {
        if (indicadorSeleccion == null) return;
        LeanTween.cancel(indicadorSeleccion.gameObject);
        LeanTween.moveLocalX(indicadorSeleccion.gameObject, destinoX, tiempoAnimacion).setEase(tipoCurva);
        LeanTween.size(indicadorSeleccion, new Vector2(nuevoAncho, indicadorSeleccion.sizeDelta.y), tiempoAnimacion).setEase(tipoCurva);
    }

    private void ActualizarBarraInmediato(float x, float w)
    {
        if (indicadorSeleccion == null) return;
        indicadorSeleccion.anchoredPosition = new Vector2(x, indicadorSeleccion.anchoredPosition.y);
        indicadorSeleccion.sizeDelta = new Vector2(w, indicadorSeleccion.sizeDelta.y);
    }

    private Vector2 ObtenerPosicionSalida(string nombrePanel, string direccion)
    {
        Vector2 basePos = Vector2.zero;
        if (nombrePanel == "registro") basePos = posDisenoReg;
        else if (nombrePanel == "cronometro") basePos = posDisenoCron;
        else if (nombrePanel == "ranking") basePos = posDisenoRank;

        if (direccion == "derecha") return new Vector2(basePos.x + distanciaX, basePos.y);
        if (direccion == "izquierda") return new Vector2(basePos.x - distanciaX, basePos.y);
        if (direccion == "abajo") return new Vector2(basePos.x, basePos.y - distanciaY);
        return basePos;
    }
}