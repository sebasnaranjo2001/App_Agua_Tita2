using UnityEngine;

public class ManejadorMusica : MonoBehaviour
{
    // Instancia pública para que el cronómetro pueda encontrar la música fácilmente
    public static ManejadorMusica instance;

    private AudioSource miAudioSource;

    void Awake()
    {
        // Creamos el puente de comunicación (Singleton)
        if (instance == null) instance = this;
        else Destroy(gameObject);

        miAudioSource = GetComponent<AudioSource>();
    }

    // Funciones públicas que el cronómetro llamará desde su panel oculto
    public void PausarMusica()
    {
        if (miAudioSource != null && miAudioSource.isPlaying)
        {
            miAudioSource.Pause();
        }
    }

    public void ReanudarMusica()
    {
        if (miAudioSource != null)
        {
            miAudioSource.UnPause();
        }
    }
}