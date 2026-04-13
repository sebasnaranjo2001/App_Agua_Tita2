using UnityEngine;
using TMPro; // Importante para que reconozca el texto
using UnityEngine.UI; // Por si quieres sincronizar la barra aquí también

public class ContadorCarga : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI textoPorcentaje;
    public Image barraDeCarga; // Arrastra aquí la imagen de tu barra

    [Header("Configuración")]
    public float tiempoCarga = 2.0f;
    private float tiempoPasado = 0f;

    void Update()
    {
        if (tiempoPasado < tiempoCarga)
        {
            // Aumentamos el tiempo poco a poco
            tiempoPasado += Time.deltaTime;

            // Calculamos el progreso de 0 a 1 (0.5 es la mitad, 1 es el final)
            float progreso = Mathf.Clamp01(tiempoPasado / tiempoCarga);

            // Actualizamos el texto de 0 a 100
            int porcentaje = Mathf.FloorToInt(progreso * 100);
            textoPorcentaje.text = porcentaje + "%";

            // Sincronizamos la barra (asegúrate que sea tipo 'Filled' en Unity)
            if (barraDeCarga != null)
            {
                barraDeCarga.fillAmount = progreso;
            }
        }
        else
        {
            // Cuando llega al final, nos aseguramos que marque 100%
            textoPorcentaje.text = "100%";
            if (barraDeCarga != null) barraDeCarga.fillAmount = 1f;

            // AQUÍ puedes poner el código para cambiar de escena cuando termine
        }
    }
}