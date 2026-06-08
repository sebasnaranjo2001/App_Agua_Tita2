using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ManejadorSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Referencias de la UI")]
    public RectTransform contenedor;
    public GameObject[] tarjetas; // Arrastra tus 3 tarjetas (T1, T2, T3)

    [Header("Textos Dinámicos (Sabías Que)")]
    [TextArea(3, 10)] public string[] frasesSabiasQue; // Tus frases de ahorro
    public TextMeshProUGUI textoSabiasQue; // Arrastra el texto de la T1 aquí

    [Header("Indicadores (3 puntos)")]
    public LayoutElement[] puntos;
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f);
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    [Header("Contenido (Nombres exactos de los hijos)")]
    // [0] = SabiasQue, [1-4] = Video, Grafica, Nino, Noticia
    public string[] nombresDeContenidos = { "SabiasQue", "Video", "Grafica", "Nino", "Noticia" };

    [Header("Configuración de Deslizamiento")]
    public float tiempoAutoDesliz = 6.0f;
    public float sensibilidadSwipe = 20f;

    private int indexActual = 0;
    private float tiempoUltimoMovimiento = 0;
    private int direccionAuto = 1;

    private Vector2 posInicialContenedor;
    private float pasoTotal = 0;
    private bool inicializado = false;

    // NUEVO: Arreglo invisible para recordar las probabilidades de cada panel
    private int[] pesosContenidos;

    void Awake()
    {
        if (contenedor != null) posInicialContenedor = contenedor.anchoredPosition;
    }

    void OnEnable()
    {
        InicializarContenidoAleatorio();
        if (inicializado) ActualizarIndicadores(true);
    }

    void Start()
    {
        ActualizarIndicadores(true);
        inicializado = true;
    }

    void Update()
    {
        if (Time.time - tiempoUltimoMovimiento > tiempoAutoDesliz)
        {
            DeslizarAutomatico();
        }
    }

    public void InicializarContenidoAleatorio()
    {
        // 1. Actualizar SOLO la frase de la Tarjeta 1
        if (frasesSabiasQue.Length > 0 && textoSabiasQue != null)
        {
            textoSabiasQue.text = frasesSabiasQue[Random.Range(0, frasesSabiasQue.Length)];
        }

        // 2. Tarjeta 1: Mantener siempre activo su panel "SabiasQue" (índice 0)
        if (tarjetas.Length > 0 && tarjetas[0] != null && nombresDeContenidos.Length > 0)
        {
            Transform hijoT1 = tarjetas[0].transform.Find(nombresDeContenidos[0]);
            if (hijoT1 != null) ActivarSoloUno(tarjetas[0], hijoT1.gameObject);
        }

        // 3. Sistema de Probabilidad Ponderada para Tarjetas 2 y 3

        // Si es la primera vez o cambiamos la cantidad de paneles, inicializamos los "boletos"
        if (pesosContenidos == null || pesosContenidos.Length != nombresDeContenidos.Length)
        {
            pesosContenidos = new int[nombresDeContenidos.Length];
            for (int p = 1; p < pesosContenidos.Length; p++) pesosContenidos[p] = 100; // Todos empiezan con 100 boletos
        }

        List<int> disponibles = new List<int>();
        for (int p = 1; p < nombresDeContenidos.Length; p++) disponibles.Add(p);

        List<int> elegidosEstaVez = new List<int>();

        for (int i = 1; i < tarjetas.Length; i++)
        {
            if (tarjetas[i] == null || disponibles.Count == 0) continue;

            // Sumar el total de boletos disponibles
            int pesoTotal = 0;
            foreach (int d in disponibles) pesoTotal += pesosContenidos[d];

            // Elegir un boleto al azar
            int valorRandom = Random.Range(0, pesoTotal);
            int pesoAcumulado = 0;
            int elegido = disponibles[0];

            // Buscar de quién es el boleto ganador
            foreach (int d in disponibles)
            {
                pesoAcumulado += pesosContenidos[d];
                if (valorRandom < pesoAcumulado)
                {
                    elegido = d;
                    break;
                }
            }

            // Activar el panel elegido
            Transform hijo = tarjetas[i].transform.Find(nombresDeContenidos[elegido]);
            if (hijo != null) ActivarSoloUno(tarjetas[i], hijo.gameObject);

            // Guardarlo en la lista de los que salieron hoy para bajarles el peso después
            elegidosEstaVez.Add(elegido);
            disponibles.Remove(elegido); // Que no se repita en la otra tarjeta al mismo tiempo
        }

        // 4. Ajustar probabilidades para la próxima vez
        for (int p = 1; p < pesosContenidos.Length; p++)
        {
            if (elegidosEstaVez.Contains(p))
            {
                pesosContenidos[p] = 10; // Si salió, su probabilidad cae drásticamente
            }
            else
            {
                pesosContenidos[p] += 40; // Si NO salió, gana 40 boletos extra para la próxima
            }
        }

        tiempoUltimoMovimiento = Time.time;
    }

    void ActivarSoloUno(GameObject tarjeta, GameObject activar)
    {
        foreach (Transform child in tarjeta.transform)
        {
            child.gameObject.SetActive(false);
        }
        activar.SetActive(true);
    }

    void DeslizarAutomatico()
    {
        if (indexActual >= tarjetas.Length - 1) direccionAuto = -1;
        else if (indexActual <= 0) direccionAuto = 1;

        indexActual += direccionAuto;
        MoverContenedor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        tiempoUltimoMovimiento = Time.time;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dif = eventData.pressPosition.x - eventData.position.x;

        if (dif > sensibilidadSwipe && indexActual < tarjetas.Length - 1)
            indexActual++;
        else if (dif < -sensibilidadSwipe && indexActual > 0)
            indexActual--;

        MoverContenedor();
    }

    void CalcularPaso()
    {
        HorizontalLayoutGroup layout = contenedor.GetComponent<HorizontalLayoutGroup>();
        if (layout != null && contenedor.childCount > 0)
        {
            float anchoTarjeta = contenedor.GetChild(0).GetComponent<RectTransform>().rect.width;
            pasoTotal = anchoTarjeta + layout.spacing;
        }
        else
        {
            pasoTotal = 800f;
        }
    }

    void MoverContenedor()
    {
        CalcularPaso();

        tiempoUltimoMovimiento = Time.time;
        float nuevaX = posInicialContenedor.x - (indexActual * pasoTotal);

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(nuevaX, contenedor.anchoredPosition.y), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        if (puntos == null || puntos.Length == 0) return;

        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null) continue;

            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = el.GetComponent<Image>();

            LeanTween.cancel(el.gameObject);
            LeanTween.value(el.gameObject, el.preferredHeight, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredHeight = val;
                    el.preferredWidth = val;
                });

            if (img != null) LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
        }
    }
}