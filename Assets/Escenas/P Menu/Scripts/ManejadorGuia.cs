using UnityEngine;

public class ManejadorGuia : MonoBehaviour
{
    [Header("Panel Principal (Dashboard)")]
    public GameObject botonesPrincipales;

    [Header("Paneles de Zonas")]
    public GameObject panelBano;
    public GameObject panelCocina;
    public GameObject panelLavanderia;
    public GameObject panelJardin;

    void Start()
    {
        ResetearGuiaTotal();
    }

    private void ResetearGuiaTotal()
    {
        // Aseguramos que al abrir la app estemos en el menú principal
        if (botonesPrincipales)
        {
            botonesPrincipales.SetActive(true);
            botonesPrincipales.transform.localScale = Vector3.one;
        }

        // Apagamos todas las zonas
        if (panelBano) panelBano.SetActive(false);
        if (panelCocina) panelCocina.SetActive(false);
        if (panelLavanderia) panelLavanderia.SetActive(false);
        if (panelJardin) panelJardin.SetActive(false);
    }

    // --- NAVEGACIÓN DE ZONAS (FUNCIONES PARA LOS BOTONES DEL DASHBOARD) ---

    public void AbrirBano() => TransicionHacia(panelBano);
    public void AbrirCocina() => TransicionHacia(panelCocina);
    public void AbrirLavanderia() => TransicionHacia(panelLavanderia);
    public void AbrirJardin() => TransicionHacia(panelJardin);

    // --- FUNCIÓN DE REGRESO ---

    public void RegresarAlMenu()
    {
        GameObject panelACerrar = ObtenerPanelActivo();

        if (panelACerrar != null)
        {
            // Efecto hacia adentro para cerrar
            LeanTween.scale(panelACerrar, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
                panelACerrar.SetActive(false);

                // Volvemos al menú principal con el efecto PUM
                botonesPrincipales.SetActive(true);
                botonesPrincipales.transform.localScale = Vector3.zero;
                LeanTween.scale(botonesPrincipales, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
            });
        }
    }

    // --- LÓGICA DE TRANSICIÓN ---

    private void TransicionHacia(GameObject panelDestino)
    {
        // 1. Cerramos el menú principal
        LeanTween.scale(botonesPrincipales, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            botonesPrincipales.SetActive(false);

            // 2. Abrimos la zona elegida (Baño, Cocina, etc.) con efecto PUM
            panelDestino.SetActive(true);
            panelDestino.transform.localScale = Vector3.zero;
            LeanTween.scale(panelDestino, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
        });
    }

    private GameObject ObtenerPanelActivo()
    {
        if (panelBano != null && panelBano.activeSelf) return panelBano;
        if (panelCocina != null && panelCocina.activeSelf) return panelCocina;
        if (panelLavanderia != null && panelLavanderia.activeSelf) return panelLavanderia;
        if (panelJardin != null && panelJardin.activeSelf) return panelJardin;
        return null;
    }
}