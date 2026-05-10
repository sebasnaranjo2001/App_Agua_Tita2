using UnityEngine;

public class ManejadorGuia : MonoBehaviour
{
    [Header("Panel Principal")]
    public GameObject botonesPrincipales;

    [Header("Paneles de Zonas")]
    public GameObject panelBano;
    public GameObject panelCocina;
    public GameObject panelLavanderia; // O ducha, según cómo lo nombraste
    public GameObject panelJardin;

    // --- FUNCIONES PARA LOS BOTONES ---

    public void AbrirBano() => TransicionA(panelBano);
    public void AbrirCocina() => TransicionA(panelCocina);
    public void AbrirLavanderia() => TransicionA(panelLavanderia);
    public void AbrirJardin() => TransicionA(panelJardin);

    // Función para regresar al menú de la guía
    public void VolverADashboard()
    {
        // Buscamos cuál panel está activo para cerrarlo
        GameObject panelActivo = ObtenerPanelActivo();

        if (panelActivo != null)
        {
            LeanTween.scale(panelActivo, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                panelActivo.SetActive(false);
                botonesPrincipales.SetActive(true);
                botonesPrincipales.transform.localScale = Vector3.zero;
                LeanTween.scale(botonesPrincipales, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
            });
        }
    }

    // Lógica interna de la animación
    private void TransicionA(GameObject panelDestino)
    {
        // 1. Achicamos y apagamos el menú principal
        LeanTween.scale(botonesPrincipales, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            botonesPrincipales.SetActive(false);

            // 2. Encendemos y agrandamos el panel de la zona
            panelDestino.SetActive(true);
            panelDestino.transform.localScale = Vector3.zero;
            LeanTween.scale(panelDestino, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
        });
    }

    private GameObject ObtenerPanelActivo()
    {
        if (panelBano.activeSelf) return panelBano;
        if (panelCocina.activeSelf) return panelCocina;
        if (panelLavanderia.activeSelf) return panelLavanderia;
        if (panelJardin.activeSelf) return panelJardin;
        return null;
    }
}