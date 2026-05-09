using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManejadorSwiper2 : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Referencias de la UI")]
    public RectTransform contenedor;
    public TextMeshProUGUI txtNombreLider;
    public TextMeshProUGUI txtTiempoLider;

    [Header("Indicadores de Progreso")]
    public LayoutElement[] puntos;
    public GameObject contenedorPuntos; // Arrastra el objeto PADRE de los puntos aquí
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f);

    [Header("Configuración de Tarjeta Ranking")]
    public GameObject tarjetaRanking;

    [Header("Ajustes de Movimiento (Horizontal)")]
    public float anchoTarjeta = 739f;
    public float espacioEntreTarjetas = 50f;
    public float sensibilidadSwipe = 20f;

    [Header("Ajustes Visuales Puntos")]
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    private int indexActual = 0;
    private Vector2 posInicialContenedor;
    private int totalTarjetasActivas = 1;

    void Start()
    {
        // 1. Lógica de activación por datos
        if (PlayerPrefs.GetInt("HayDatosDucha", 0) == 1)
        {
            if (tarjetaRanking != null) tarjetaRanking.SetActive(true);
            CargarLiderMenu();
            totalTarjetasActivas = 2; // Ranking + Video
        }
        else
        {
            if (tarjetaRanking != null) tarjetaRanking.SetActive(false);
            totalTarjetasActivas = 1; // Solo Video
            indexActual = 0;
        }

        // 2. Guardamos posición inicial
        if (contenedor != null)
        {
            posInicialContenedor = contenedor.anchoredPosition;
            if (totalTarjetasActivas == 1)
            {
                contenedor.anchoredPosition = posInicialContenedor;
            }
        }

        // 3. Ejecutamos la actualización de indicadores
        ActualizarIndicadores(true);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Bloqueo de movimiento si no hay nada que navegar
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

        // --- EL ARREGLO CLAVE ESTÁ AQUÍ ---
        if (totalTarjetasActivas <= 1)
        {
            // Si solo hay una tarjeta (Video), apagamos TODO el objeto de puntos
            contenedorPuntos.SetActive(false);
            return; // Salimos de la función para no procesar nada más
        }
        else
        {
            // Si hay más de una tarjeta, nos aseguramos de que esté encendido
            contenedorPuntos.SetActive(true);
        }

        // Si llegamos aquí, es porque hay más de una tarjeta y debemos animar los puntos
        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null) continue;

            // Apagamos los puntos individuales que no se usan (por si el array tiene 3 pero solo hay 2 tarjetas)
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

    // --- LÓGICA DE RANKING (SIN CAMBIOS) ---
    public void IrAlDuchometroRanking()
    {
        NavegacionMenuPrincipal.panelAbridor = "ranking";
        SceneManager.LoadScene("Duchometro");
    }

    void CargarLiderMenu()
    {
        string json = PlayerPrefs.GetString("ListaUsuarios", "");
        if (string.IsNullOrEmpty(json)) return;

        ManejadorRegistro.ListaWrapper wrapper = JsonUtility.FromJson<ManejadorRegistro.ListaWrapper>(json);
        if (wrapper != null && wrapper.miembros.Count > 0)
        {
            ManejadorRegistro.DatosMiembro mejor = null;
            float record = float.MaxValue;
            foreach (var m in wrapper.miembros)
            {
                if (m.mejorTiempo > 0 && m.mejorTiempo < record) { record = m.mejorTiempo; mejor = m; }
            }
            if (mejor != null)
            {
                txtNombreLider.text = mejor.nombre;
                int min = Mathf.FloorToInt(mejor.mejorTiempo / 60);
                int seg = Mathf.FloorToInt(mejor.mejorTiempo % 60);
                txtTiempoLider.text = string.Format("{0}:{1:00} min", min, seg);
            }
        }
    }
}