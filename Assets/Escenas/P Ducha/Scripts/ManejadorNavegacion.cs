using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManejadorNavegacion : MonoBehaviour
{
    [Header("Paneles")]
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
    public Cronometro scriptCronometro; // <-- ASIGNA ESTO EN EL INSPECTOR

    [Header("Mensajes de Aviso")]
    public GameObject avisoCreaMiembro;
    public GameObject avisoSeleccionaMiembro;

    private void Start()
    {
        panelRegistro.anchoredPosition = Vector2.zero;
        panelCronometro.anchoredPosition = new Vector2(1500, 0);
        panelRanking.anchoredPosition = new Vector2(1500, 0);

        ActualizarEstadoBotones("registro");
    }

    public void IrAlMenuPrincipal() => SceneManager.LoadScene("Menu");

    public void IrARegistro()
    {
        MoverPanel(panelRegistro);
        ActualizarEstadoBotones("registro");
    }

    public void IrACronometro()
    {
        if (ManejadorRegistro.instance != null && !string.IsNullOrEmpty(ManejadorRegistro.instance.nombreSeleccionado))
        {
            // 1. Refrescar nombre en el cronómetro
            if (scriptBuscador != null) scriptBuscador.ActualizarInterfaz();

            // 2. REINICIO VISUAL: Limpiamos el cronómetro antes de mostrarlo
            if (scriptCronometro != null) scriptCronometro.ReiniciarTodo();

            MoverPanel(panelCronometro);
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

            MoverPanel(panelRanking);
            ActualizarEstadoBotones("ranking");
        }
        else
        {
            MostrarAvisoTemporal(avisoCreaMiembro);
        }
    }

    private void MoverPanel(RectTransform destino)
    {
        LeanTween.moveX(panelRegistro, -1500, 0.4f).setEaseInExpo();
        LeanTween.moveX(panelCronometro, -1500, 0.4f).setEaseInExpo();
        LeanTween.moveX(panelRanking, -1500, 0.4f).setEaseInExpo();

        destino.anchoredPosition = new Vector2(1500, 0);
        LeanTween.moveX(destino, 0, 0.6f).setEaseOutBack();
    }

    private void ActualizarEstadoBotones(string panelActivo)
    {
        if (btnIrRegistro) btnIrRegistro.interactable = (panelActivo != "registro");
        if (btnIrRanking) btnIrRanking.interactable = (panelActivo != "ranking");
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