using UnityEngine;

public class ManejadorGuia : MonoBehaviour
{
    [Header("--- DASHBOARD PRINCIPAL ---")]
    public GameObject panelDashboard;
    public GameObject[] itemsDashboard;

    [Header("--- ZONAS PRINCIPALES ---")]
    public GameObject panelBano;
    public GameObject[] itemsBano;
    public GameObject panelCocina;
    public GameObject[] itemsCocina;
    public GameObject panelLavanderia;
    public GameObject[] itemsLavanderia;
    public GameObject panelJardin;
    public GameObject[] itemsJardin;

    [Header("--- PANELES DE INFORMACIÓN (NUEVO) ---")]
    [Tooltip("Arrastra aquí TODOS tus paneles finales (Info Ducha, Info Manos, etc) para que se limpien al volver al menú")]
    public GameObject[] panelesDeInformacion;

    [Header("Configuración de Animación")]
    public float tiempoAnimacion = 0.4f;
    public float retrasoEntreItems = 0.08f;

    void OnEnable()
    {
        Invoke("ArrancarDashboard", 0.05f);
    }

    public void ArrancarDashboard()
    {
        ApagarTodo();
        if (panelDashboard != null)
        {
            panelDashboard.SetActive(true);
            panelDashboard.transform.localScale = Vector3.one;
            AnimarLista(itemsDashboard);
        }
    }

    public void AbrirBano()
    {
        ApagarTodo();
        panelBano.SetActive(true);
        panelBano.transform.localScale = Vector3.one;
        AnimarLista(itemsBano);
    }

    public void AbrirCocina()
    {
        ApagarTodo();
        panelCocina.SetActive(true);
        panelCocina.transform.localScale = Vector3.one;
        AnimarLista(itemsCocina);
    }

    public void AbrirLavanderia()
    {
        ApagarTodo();
        panelLavanderia.SetActive(true);
        panelLavanderia.transform.localScale = Vector3.one;
        AnimarLista(itemsLavanderia);
    }

    public void AbrirJardin()
    {
        ApagarTodo();
        panelJardin.SetActive(true);
        panelJardin.transform.localScale = Vector3.one;
        AnimarLista(itemsJardin);
    }

    public void RegresarAlDashboard()
    {
        ArrancarDashboard();
    }

    private void ApagarTodo()
    {
        if (panelDashboard) panelDashboard.SetActive(false);
        if (panelBano) panelBano.SetActive(false);
        if (panelCocina) panelCocina.SetActive(false);
        if (panelLavanderia) panelLavanderia.SetActive(false);
        if (panelJardin) panelJardin.SetActive(false);

        // ESTO EVITA QUE SE QUEDEN MONTADOS
        if (panelesDeInformacion != null)
        {
            foreach (GameObject panelInfo in panelesDeInformacion)
            {
                if (panelInfo != null) panelInfo.SetActive(false);
            }
        }
    }

    private void AnimarLista(GameObject[] listaDeItems)
    {
        if (listaDeItems == null || listaDeItems.Length == 0) return;

        float delayAcumulado = 0.1f;

        foreach (GameObject item in listaDeItems)
        {
            if (item != null)
            {
                item.SetActive(true);
                LeanTween.cancel(item);
                item.transform.localScale = Vector3.zero;

                LeanTween.scale(item, Vector3.one, tiempoAnimacion)
                    .setEase(LeanTweenType.easeOutBack)
                    .setDelay(delayAcumulado);

                delayAcumulado += retrasoEntreItems;
            }
        }
    }
}