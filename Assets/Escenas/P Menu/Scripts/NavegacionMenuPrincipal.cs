using UnityEngine;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    // Variable estática que permite la comunicación entre escenas
    public static string panelAbridor = "";

    [Header("Referencias de Paneles")]
    public GameObject panelInicio;
    public GameObject panelJuegos;
    public GameObject panelGuia;

    void Start()
    {
        // Al cargar la escena del Menú, revisamos si venimos de otra escena
        // queriendo abrir una sección específica.
        ConfigurarPanelInicial();
    }

    void ConfigurarPanelInicial()
    {
        if (string.IsNullOrEmpty(panelAbridor))
        {
            // Si no hay mensaje, mostramos el inicio por defecto
            MostrarInicio();
            return;
        }

        // Caso: Juegos
        if (panelAbridor == "juegos")
        {
            AbrirPanelJuegos();
        }
        // Caso: Guía
        else if (panelAbridor == "guia")
        {
            AbrirPanelGuia();
        }

        // Nota: Si el mensaje es "ranking", no hacemos nada aquí 
        // porque el ranking vive en la escena "Duchometro". 
        // El script "NavegadorEscenaDucha" se encargará allá.
    }

    // --- MÉTODOS DE NAVEGACIÓN ---

    public void AbrirPanelJuegos()
    {
        DesactivarTodosLosPaneles();
        if (panelJuegos != null) panelJuegos.SetActive(true);
        panelAbridor = ""; // Limpiamos el mensaje después de usarlo
    }

    public void AbrirPanelGuia()
    {
        DesactivarTodosLosPaneles();
        if (panelGuia != null) panelGuia.SetActive(true);
        panelAbridor = "";
    }

    public void MostrarInicio()
    {
        DesactivarTodosLosPaneles();
        if (panelInicio != null) panelInicio.SetActive(true);
        panelAbridor = "";
    }

    // Método de apoyo para limpiar la pantalla antes de mostrar un panel
    private void DesactivarTodosLosPaneles()
    {
        if (panelInicio != null) panelInicio.SetActive(false);
        if (panelJuegos != null) panelJuegos.SetActive(false);
        if (panelGuia != null) panelGuia.SetActive(false);
    }
}