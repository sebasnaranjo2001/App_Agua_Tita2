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
            MoverPanel(panelCronometro, new Vector2(0, distanciaY));
            panelRegistro.anchoredPosition = new Vector2(0, -distanciaY);
            MoverPanel(panelRegistro, Vector2.zero);
        }
        else
        {
            panelRegistro.anchoredPosition = new Vector2(-distanciaX, 0);
            MoverPanel(panelRegistro, Vector2.zero);
            MoverPanel(panelRanking, new Vector2(distanciaX, 0));
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

            MoverPanel(panelRegistro, new Vector2(0, -distanciaY));
            MoverPanel(panelRanking, new Vector2(0, -distanciaY));

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
                MoverPanel(panelCronometro, new Vector2(0, -distanciaY));
                panelRanking.anchoredPosition = new Vector2(0, distanciaY);
                MoverPanel(panelRanking, Vector2.zero);
            }
            else
            {
                panelRanking.anchoredPosition = new Vector2(distanciaX, 0);
                MoverPanel(panelRanking, Vector2.zero);
                MoverPanel(panelRegistro, new Vector2(-distanciaX, 0));
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
        if (panel == null) return;
        LeanTween.cancel(panel.gameObject);
        LeanTween.move(panel, destino, duracionCapa).setEase(tipoSuavizado);
    }

    private void ActualizarEstadoBotones(string panelActivo)
    {
        if (btnIrRegistro) btnIrRegistro.interactable = (panelActivo != "registro");
        if (btnIrRanking) btnIrRanking.interactable = (panelActivo != "ranking");
        if (btnIrInicio) btnIrInicio.interactable = (panelActivo != "cronometro");
    }

    // --- FUNCIÓN CORREGIDA PARA EVITAR EL ERROR ROJO ---
    void MostrarAvisoTemporal(GameObject aviso)
    {
        // 1. Verificamos que el objeto exista en el Inspector
        if (aviso == null) return;

        // 2. Cancelamos cualquier animación que estuviera haciendo antes 
        // para que no se "vuelva loco" si el usuario da clics repetidos
        LeanTween.cancel(aviso);

        aviso.SetActive(true);
        aviso.transform.localScale = Vector3.zero;

        // Animación de entrada
        LeanTween.scale(aviso, Vector3.one, 0.5f).setEaseOutBack();

        // 3. Usamos la versión de delayedCall que se ata al GameObject.
        // Si el objeto se destruye o el script para, esta llamada se cancela sola.
        LeanTween.delayedCall(aviso, 2.5f, () => {

            // 4. Verificación final antes de ejecutar el cierre
            if (aviso != null)
            {
                LeanTween.scale(aviso, Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() => {
                    if (aviso != null) aviso.SetActive(false);
                });
            }
        });
    }
}