using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ManejadorAvisos : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Configuración de Textos")]
    public TextMeshProUGUI textElement; // Aquí va el objeto 'Text'
    [TextArea] public string[] listaSabiasQue;

    [Header("Navegación de Paneles")]
    public RectTransform contenedor; // Aquí va 'Contenedor_Paneles'
    private int panelActual = 0;
    private float alturaSalto = 1200f; // Ajusta según el alto de tu diseño
    private Vector3 posicionObjetivo;

    void Start()
    {
        // 1. Elegir Sabías que aleatorio al iniciar
        if (listaSabiasQue.Length > 0 && textElement != null)
        {
            int index = Random.Range(0, listaSabiasQue.Length);
            textElement.text = listaSabiasQue[index];
        }

        if (contenedor != null)
            posicionObjetivo = contenedor.anchoredPosition;
    }

    void Update()
    {
        // 2. Movimiento suave del contenedor
        if (contenedor != null)
        {
            contenedor.anchoredPosition = Vector3.Lerp(contenedor.anchoredPosition, posicionObjetivo, Time.deltaTime * 10f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Necesario para que funcione el EndDrag, no borrar
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Detectar deslizamiento hacia arriba
        if (eventData.delta.y > 50f && panelActual < 2)
        {
            panelActual++;
            posicionObjetivo += new Vector3(0, alturaSalto, 0);
        }
        // Detectar deslizamiento hacia abajo
        else if (eventData.delta.y < -50f && panelActual > 0)
        {
            panelActual--;
            posicionObjetivo -= new Vector3(0, alturaSalto, 0);
        }
    }
}