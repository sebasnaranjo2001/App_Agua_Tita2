using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManejadorNavegacion : MonoBehaviour
{
    [Header("Paneles (RectTransform)")]
    public RectTransform panelRegistro;
    public RectTransform panelCronometro;
    public RectTransform panelRanking;

    [Header("Botones de Barra Inferior")]
    public Button btnIrRegistro;
    public Button btnIrRanking;
    public Button btnIrInicio;

    [Header("Scripts de Actualización")]
    public BuscadorDeMiembros scriptBuscador;
    public ManejadorRanking scriptRanking;
    public Cronometro scriptCronometro;

    [Header("Mensajes de Aviso")]
    public GameObject avisoCreaMiembro;
    public GameObject avisoSeleccionaMiembro;

    [Header("Configuración de Animación")]
    public float duracionCapa = 0.8f;
    public LeanTweenType tipoSuavizado = LeanTweenType.easeOutCubic;

    private float distanciaX = 1100f;
    private float distanciaY = 2000f;
    private string panelActual = "registro";

    private void Start()
    {
        panelRegistro.anchoredPosition = Vector2.zero;
        panelRanking.anchoredPosition = new Vector2(distanciaX, 0);
        panelCronometro.anchoredPosition = new Vector2(0, distanciaY);

        panelActual = "registro";
        ActualizarEstadoBotones("registro");
    }

    public void IrAlMenuPrincipal() => SceneManager.LoadScene("Menu");

    public void IrARegistro()
    {
        if (panelActual == "cronometro")
        {
            // Salida vertical
            MoverPanel(panelCronometro, new Vector2(0, distanciaY));
            // Teletransportamos el registro ABAJO antes de que suba
            panelRegistro.anchoredPosition = new Vector2(0, -distanciaY);
            MoverPanel(panelRegistro, Vector2.zero);
        }
        else
        {
            // Salida horizontal (desde Ranking)
            // Teletransportamos el registro a la IZQUIERDA antes de que entre
            panelRegistro.anchoredPosition = new Vector2(-distanciaX, 0);
            MoverPanel(panelRegistro, Vector2.zero);
            MoverPanel(panelRanking, new Vector2(distanciaX, 0));
            // Aseguramos que el cronómetro esté arriba y no estorbe
            panelCronometro.anchoredPosition = new Vector2(0, distanciaY);
        }

        panelActual = "registro";
        ActualizarEstadoBotones("registro");
    }

    public void IrACronometro()
    {
        if (ManejadorRegistro.instance != null && !string.IsNullOrEmpty(ManejadorRegistro.instance.nombreSeleccionado))
        {
            if (scriptBuscador != null) scriptBuscador.ActualizarInterfaz();
            if (scriptCronometro != null) scriptCronometro.ReiniciarTodo();

            // Al ir al cronómetro, mandamos los otros hacia ABAJO
            MoverPanel(panelRegistro, new Vector2(0, -distanciaY));
            MoverPanel(panelRanking, new Vector2(0, -distanciaY));

            // Teletransportamos cronómetro ARRIBA antes de que caiga
            panelCronometro.anchoredPosition = new Vector2(0, distanciaY);
            MoverPanel(panelCronometro, Vector2.zero);

            panelActual = "cronometro";
            ActualizarEstadoBotones("cronometro");
        }
        else
        {
            MostrarAvisoTemporal(avisoSeleccionaMiembro);
        }
    }

    public void IrARanking()
    {
        if (ManejadorRegistro.instance != null && ManejadorRegistro.instance.listaDeMiembros.Count > 0)
        {
            if (scriptRanking != null) scriptRanking.GenerarRanking();

            if (panelActual == "cronometro")
            {
                // Salida vertical
                MoverPanel(panelCronometro, new Vector2(0, -distanciaY));
                // Teletransportamos ranking ARRIBA antes de que caiga
                panelRanking.anchoredPosition = new Vector2(0, distanciaY);
                MoverPanel(panelRanking, Vector2.zero);
            }
            else
            {
                // Salida horizontal (desde Registro)
                // Teletransportamos ranking a la DERECHA antes de que entre
                panelRanking.anchoredPosition = new Vector2(distanciaX, 0);
                MoverPanel(panelRanking, Vector2.zero);
                MoverPanel(panelRegistro, new Vector2(-distanciaX, 0));
                // Aseguramos que el cronómetro esté arriba
                panelCronometro.anchoredPosition = new Vector2(0, distanciaY);
            }

            panelActual = "ranking";
            ActualizarEstadoBotones("ranking");
        }
        else
        {
            MostrarAvisoTemporal(avisoCreaMiembro);
        }
    }

    private void MoverPanel(RectTransform panel, Vector2 destino)
    {
        // Cancelamos cualquier movimiento previo para evitar conflictos
        LeanTween.cancel(panel.gameObject);
        LeanTween.move(panel, destino, duracionCapa).setEase(tipoSuavizado);
    }

    private void ActualizarEstadoBotones(string panelActivo)
    {
        if (btnIrRegistro) btnIrRegistro.interactable = (panelActivo != "registro");
        if (btnIrRanking) btnIrRanking.interactable = (panelActivo != "ranking");
        if (btnIrInicio) btnIrInicio.interactable = (panelActivo != "cronometro");
    }

    void MostrarAvisoTemporal(GameObject aviso)
    {
        if (aviso == null) return;
        aviso.SetActive(true);
        aviso.transform.localScale = Vector3.zero;
        LeanTween.scale(aviso, Vector3.one, 0.5f).setEaseOutBack();
        LeanTween.delayedCall(2.5f, () => {
            LeanTween.scale(aviso, Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() => aviso.SetActive(false));
        });
    }
}