using UnityEngine;

public class AnimadorCascada : MonoBehaviour
{
    [Header("Configuración de Animación")]
    public float tiempoAnimacion = 0.4f;
    public float retrasoEntreBotones = 0.08f;

    // OnEnable se ejecuta automáticamente cada vez que el panel se enciende (SetActive true)
    void OnEnable()
    {
        float delayAcumulado = 0.1f;

        // Recorre todos los elementos que estén dentro de este contenedor
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hijo = transform.GetChild(i);

            // Los esconde al tamaño cero
            hijo.localScale = Vector3.zero;
            LeanTween.cancel(hijo.gameObject);

            // Los hace saltar con efecto PUM, sumando un pequeño retraso a cada uno
            LeanTween.scale(hijo.gameObject, Vector3.one, tiempoAnimacion)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(delayAcumulado);

            delayAcumulado += retrasoEntreBotones;
        }
    }
}