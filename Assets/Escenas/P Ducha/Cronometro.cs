using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ControladorDucha : MonoBehaviour
{
    [Header("Referencias de UI - Textos")]
    public TMP_Text textoCronometro;
    public TMP_Text textoConsumo;

    [Header("Referencias de UI - Botones/Tarjetas")]
    public GameObject btnComenzar;
    public GameObject btnFinalizar;
    public GameObject btnGuardando;      // Aparece 1.5s
    public GameObject btnGuardado;       // Aparece 1.5s
    public GameObject btnVolverEmpezar;  // Botón final de reinicio

    [Header("Configuración")]
    public float litrosPorMinuto = 9.5f;

    private float tiempoTranscurrido;
    private bool estaContando = false;

    // Colores Estéticos
    private Color colorVerde = new Color32(52, 201, 70, 255);
    private Color colorAmarillo = Color.yellow;
    private Color colorRojo = Color.red;

    void Start()
    {
        // Estado inicial: solo botón Comenzar activo
        PrepararEscena();
    }

    void Update()
    {
        if (estaContando)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarInterfazUI();
            ManejarTransicionDeColor();
        }
    }

    // --- FLUJO AUTOMATIZADO ---

    public void ComenzarDucha()
    {
        estaContando = true;
        SetEstadoBotones(false, true, false, false, false);
    }

    public void FinalizarDucha()
    {
        estaContando = false;
        // Iniciamos la secuencia automática de guardado
        StartCoroutine(SecuenciaGuardadoAutomatico());
    }

    IEnumerator SecuenciaGuardadoAutomatico()
    {
        // 1. Mostrar "Guardando..." por 1.5 segundos
        SetEstadoBotones(false, false, true, false, false);
        yield return new WaitForSeconds(1.5f);

        // 2. Ejecutar la lógica de guardado en memoria (PlayerPrefs)
        GuardarEnHistorialInterno();

        // 3. Mostrar "Guardado" por 1.5 segundos
        SetEstadoBotones(false, false, false, true, false);
        yield return new WaitForSeconds(1.5f);

        // 4. Mostrar botón de "Volver a empezar"
        SetEstadoBotones(false, false, false, false, true);
    }

    public void ReiniciarTodo()
    {
        // Reset de valores y regreso al botón "Comenzar"
        tiempoTranscurrido = 0f;
        ActualizarInterfazUI();
        textoCronometro.color = colorVerde;
        SetEstadoBotones(true, false, false, false, false);
    }

    // --- LÓGICA DE MEMORIA (5 BAÑOS) ---

    void GuardarEnHistorialInterno()
    {
        // Obtenemos el nombre del usuario actual desde tu ManejadorRegistro
        string nombreUsuario = ManejadorRegistro.instance != null ? ManejadorRegistro.instance.nombreSeleccionado : "Invitado";
        string claveUser = "Historial_" + nombreUsuario;

        // Registro actual
        string nuevoRegistro = $"{textoCronometro.text} | {textoConsumo.text}";

        // Cargar, Gestionar lista y Guardar
        string datosViejos = PlayerPrefs.GetString(claveUser, "");
        List<string> listaDuchas = new List<string>();

        if (!string.IsNullOrEmpty(datosViejos))
        {
            listaDuchas.AddRange(datosViejos.Split(';'));
        }

        listaDuchas.Insert(0, nuevoRegistro); // Insertar al inicio

        // Mantener solo los 5 más recientes
        if (listaDuchas.Count > 5)
        {
            listaDuchas.RemoveRange(5, listaDuchas.Count - 5);
        }

        PlayerPrefs.SetString(claveUser, string.Join(";", listaDuchas));
        PlayerPrefs.Save();

        Debug.Log("Auto-Guardado exitoso para: " + nombreUsuario);
    }

    // --- UTILIDADES ---

    void SetEstadoBotones(bool com, bool fin, bool gndo, bool gdo, bool vol)
    {
        if (btnComenzar) btnComenzar.SetActive(com);
        if (btnFinalizar) btnFinalizar.SetActive(fin);
        if (btnGuardando) btnGuardando.SetActive(gndo);
        if (btnGuardado) btnGuardado.SetActive(gdo);
        if (btnVolverEmpezar) btnVolverEmpezar.SetActive(vol);
    }

    void ActualizarInterfazUI()
    {
        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);
        textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        float litrosTotales = (tiempoTranscurrido / 60f) * litrosPorMinuto;
        textoConsumo.text = litrosTotales.ToString("F1") + " L";
    }

    void ManejarTransicionDeColor()
    {
        float min = tiempoTranscurrido / 60f;
        if (min < 5f) textoCronometro.color = colorVerde;
        else if (min < 8f) textoCronometro.color = Color.Lerp(colorVerde, colorAmarillo, (min - 5f) / 3f);
        else textoCronometro.color = Color.Lerp(colorAmarillo, colorRojo, (min - 8f) / 2f);
    }

    void PrepararEscena()
    {
        tiempoTranscurrido = 0f;
        ActualizarInterfazUI();
        textoCronometro.color = colorVerde;
        SetEstadoBotones(true, false, false, false, false);
    }
}