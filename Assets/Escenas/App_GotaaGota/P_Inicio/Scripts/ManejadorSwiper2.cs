using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManejadorSwiper2 : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Referencias de la UI")]
    public RectTransform contenedor;
    public TextMeshProUGUI txtNombreLider;
    public TextMeshProUGUI txtTiempoLider;

    [Header("Indicadores de Progreso")]
    public LayoutElement[] puntos;
    public GameObject contenedorPuntos;
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f);

    [Header("Configuración de Tarjeta Ranking")]
    public GameObject tarjetaRanking;

    [Header("Ajustes de Movimiento (Horizontal)")]
    public float anchoTarjeta = 740f;
    public float espacioEntreTarjetas = 83.06f;
    public float sensibilidadSwipe = 25f;

    [Header("Ajustes Visuales Puntos")]
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    [Header("Navegación Single Scene")]
    public GameObject panelPadreDuchometro;

    private int indexActual = 0;
    private Vector2 posInicialContenedor;
    private int totalTarjetasActivas = 1;

    void Awake()
    {
        if (contenedor != null)
        {
            posInicialContenedor = contenedor.anchoredPosition;
        }
    }

    void OnEnable()
    {
        RefrescarPanel();
    }

    public void RefrescarPanel()
    {
        // CORRECCIÓN AQUÍ: Siempre reiniciar el índice lógico a 0 al refrescar,
        // ya que el contenedor visual siempre se regresa a la posición inicial más abajo.
        indexActual = 0;

        // En lugar de confiar en una variable externa, revisamos la fuente real de datos
        bool tenemosUnLiderValido = CargarLiderMenu();

        if (tenemosUnLiderValido)
        {
            if (tarjetaRanking != null) tarjetaRanking.SetActive(true);
            totalTarjetasActivas = 2;
        }
        else
        {
            if (tarjetaRanking != null) tarjetaRanking.SetActive(false);
            totalTarjetasActivas = 1;
        }

        if (contenedor != null)
        {
            contenedor.anchoredPosition = posInicialContenedor;
        }

        ActualizarIndicadores(true);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (totalTarjetasActivas <= 1) return;

        float diferenciaX = eventData.pressPosition.x - eventData.position.x;

        if (diferenciaX > sensibilidadSwipe && indexActual < totalTarjetasActivas - 1)
        {
            indexActual++;
            MoverContenedor();
        }
        else if (diferenciaX < -sensibilidadSwipe && indexActual > 0)
        {
            indexActual--;
            MoverContenedor();
        }
    }

    void MoverContenedor()
    {
        float nuevaX = posInicialContenedor.x - (indexActual * (anchoTarjeta + espacioEntreTarjetas));

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(nuevaX, contenedor.anchoredPosition.y), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        if (contenedorPuntos == null) return;

        if (totalTarjetasActivas <= 1)
        {
            contenedorPuntos.SetActive(false);
            return;
        }
        else
        {
            contenedorPuntos.SetActive(true);
        }

        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null) continue;

            if (i >= totalTarjetasActivas)
            {
                puntos[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                puntos[i].gameObject.SetActive(true);
            }

            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = puntos[i].GetComponent<Image>();

            LeanTween.cancel(el.gameObject);

            LeanTween.value(el.gameObject, el.preferredHeight, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredHeight = val;
                    el.preferredWidth = val;
                });

            if (img != null)
            {
                LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
            }
        }
    }

    public void IrAlDuchometroRanking()
    {
        NavegacionMenuPrincipal navPrincipal = Object.FindFirstObjectByType<NavegacionMenuPrincipal>();
        if (navPrincipal != null)
        {
            navPrincipal.AbrirPanelDuchometro();
        }

        if (panelPadreDuchometro != null)
        {
            ManejadorNavegacion manejadorDuchometro = panelPadreDuchometro.GetComponentInChildren<ManejadorNavegacion>();

            if (manejadorDuchometro != null)
            {
                manejadorDuchometro.IrARanking();
            }
        }
    }

    // Transformamos esta función en un booleano para que actúe como un escáner de la verdad
    bool CargarLiderMenu()
    {
        string json = PlayerPrefs.GetString("ListaUsuarios", "");

        // Si el JSON está vacío, definitivamente no hay datos
        if (string.IsNullOrEmpty(json)) return false;

        ManejadorRegistro.ListaWrapper wrapper = JsonUtility.FromJson<ManejadorRegistro.ListaWrapper>(json);

        // Si la lista existe pero está vacía (0 miembros), tampoco hay datos
        if (wrapper == null || wrapper.miembros.Count == 0) return false;

        ManejadorRegistro.DatosMiembro mejor = null;
        float record = float.MaxValue;

        foreach (var m in wrapper.miembros)
        {
            if (m.mejorTiempo > 0 && m.mejorTiempo < record)
            {
                record = m.mejorTiempo;
                mejor = m;
            }
        }

        // Si encontramos a alguien con un tiempo válido, actualizamos la UI y devolvemos true
        if (mejor != null)
        {
            if (txtNombreLider != null) txtNombreLider.text = mejor.nombre;
            int min = Mathf.FloorToInt(mejor.mejorTiempo / 60);
            int seg = Mathf.FloorToInt(mejor.mejorTiempo % 60);
            if (txtTiempoLider != null) txtTiempoLider.text = string.Format("{0}:{1:00} min", min, seg);

            return true;
        }

        // Si hay miembros creados pero NADIE tiene un tiempo guardado todavía, ocultamos la tarjeta
        return false;
    }
}