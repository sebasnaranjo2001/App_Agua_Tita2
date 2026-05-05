using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ManejadorSwiper2 : MonoBehaviour
{
    [Header("Textos del Líder (Ranking)")]
    public TextMeshProUGUI txtNombreLider;
    public TextMeshProUGUI txtTiempoLider;

    [Header("Configuración de Escena")]
    public GameObject tarjetaRanking;
    public string nombreEscenaDuchometro = "Duchometro";

    void Start()
    {
        // 1. Verificamos si hay datos guardados para mostrar la tarjeta
        // Esta señal la manda el script ManejadorRegistro al guardar
        if (PlayerPrefs.GetInt("HayDatosDucha", 0) == 1)
        {
            CargarLiderMenu();
        }
        else
        {
            // Si no hay nadie con tiempos, ocultamos la tarjeta de ranking
            if (tarjetaRanking != null) tarjetaRanking.SetActive(false);
        }
    }

    void CargarLiderMenu()
    {
        string json = PlayerPrefs.GetString("ListaUsuarios", "");
        if (string.IsNullOrEmpty(json)) return;

        try
        {
            ManejadorRegistro.ListaWrapper wrapper = JsonUtility.FromJson<ManejadorRegistro.ListaWrapper>(json);

            if (wrapper != null && wrapper.miembros != null && wrapper.miembros.Count > 0)
            {
                ManejadorRegistro.DatosMiembro mejor = null;
                float record = float.MaxValue;

                // Buscamos al miembro con el tiempo más bajo (el más ahorrador)
                foreach (var m in wrapper.miembros)
                {
                    if (m.mejorTiempo > 0 && m.mejorTiempo < record)
                    {
                        record = m.mejorTiempo;
                        mejor = m;
                    }
                }

                if (mejor != null)
                {
                    // Asignamos nombre sin etiquetas de negrita
                    txtNombreLider.text = mejor.nombre;

                    // Formateamos tiempo m:ss (Ej: 5:08)
                    int min = Mathf.FloorToInt(mejor.mejorTiempo / 60);
                    int seg = Mathf.FloorToInt(mejor.mejorTiempo % 60);
                    txtTiempoLider.text = string.Format("{0}:{1:00} min", min, seg);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al cargar el líder: " + e.Message);
        }
    }

    // FUNCIÓN PARA EL BOTÓN DE LA TARJETA EN EL SWIPER
    public void IrAlDuchometroRanking()
    {
        // 1. Dejamos el recado en el buzón estático
        NavegacionMenuPrincipal.panelAbridor = "ranking";

        // 2. Saltamos a la escena del Duchómetro
        // Asegúrate de que el nombre coincida con tu Build Settings
        SceneManager.LoadScene(nombreEscenaDuchometro);
    }
}