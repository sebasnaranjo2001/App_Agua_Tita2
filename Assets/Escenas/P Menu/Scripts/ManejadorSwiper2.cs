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
    public LayoutElement[] puntos; // Arrastra los 3 puntos aquí
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

    void Start()
    {
        // 1. Cargamos los datos del líder (Tu función original)
        if (PlayerPrefs.GetInt("HayDatosDucha", 0) == 1)
        {
            CargarLiderMenu();
        }
        else
        {
            if (tarjetaRanking != null) tarjetaRanking.SetActive(false);
            // Si falta la tarjeta, podrías desactivar el primer punto si quisieras
        }

        // 2. Guardamos posición inicial
        if (contenedor != null)
            posInicialContenedor = contenedor.anchoredPosition;

        ActualizarIndicadores(true);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Movimiento Horizontal (Derecha a Izquierda)
        float diferenciaX = eventData.pressPosition.x - eventData.position.x;

        if (diferenciaX > sensibilidadSwipe && indexActual < puntos.Length - 1)
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
        // Calculamos la posición en X (restando para avanzar a la derecha)
        float nuevaX = posInicialContenedor.x - (indexActual * (anchoTarjeta + espacioEntreTarjetas));

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(nuevaX, contenedor.anchoredPosition.y), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null) continue;

            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = puntos[i].GetComponent<Image>();

            LeanTween.cancel(el.gameObject);

            // Animación de tamaño (igual que el script 1)
            LeanTween.value(el.gameObject, el.preferredHeight, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredHeight = val;
                    el.preferredWidth = val;
                });

            // Animación de color
            if (img != null)
            {
                LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
            }
        }
    }

    // --- FUNCIONES DE LÓGICA DE RANKING (SIN CAMBIOS) ---

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