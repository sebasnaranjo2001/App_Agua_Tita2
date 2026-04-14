using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Avisos : MonoBehaviour
{
    public static Avisos instance;

    [Header("Configuración de UI")]
    public GameObject imagenNino;
    public GameObject imagenLimite;
    public GameObject ventanaRegistro;

    [Header("Nuevos Avisos de Validación")]
    public GameObject avisoCreaMiembro;
    public GameObject avisoSeleccionaMiembro;

    [Header("Control de Botones de Registro")]
    public GameObject botonAnadirGrande;
    public GameObject panelBotonesPequenos;

    [Header("Configuración de Escena")]
    public string nombreEscenaCronometro = "Cronometro";

    private CanvasGroup cgAviso;
    public int contadorMiembros = 0; // Lo puse public para que lo veas en el inspector
    private Coroutine rutinaDesvanecer;

    [Header("Gestión de Selección")]
    public SeleccionMiembros miembroSeleccionado;
    public GameObject ventanaSiguiente;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // --- NUEVO: Sincronizar con el Manejador al iniciar la escena ---
        if (ManejadorRegistro.instance != null)
        {
            // Le pedimos al manejador cuántos miembros hay en su lista
            contadorMiembros = ManejadorRegistro.instance.listaDeMiembros.Count;

            // Si ya hay gente, actualizamos la interfaz de inmediato
            ActualizarInterfazSegunContador();
        }

        if (imagenLimite != null)
        {
            cgAviso = imagenLimite.GetComponent<CanvasGroup>();
            imagenLimite.SetActive(false);
        }

        if (avisoCreaMiembro != null) avisoCreaMiembro.SetActive(false);
        if (avisoSeleccionaMiembro != null) avisoSeleccionaMiembro.SetActive(false);
    }

    // --- NUEVO: Función para poner la UI en orden según cuántos miembros hay ---
    void ActualizarInterfazSegunContador()
    {
        if (contadorMiembros > 0)
        {
            if (imagenNino != null) imagenNino.SetActive(false);
            if (botonAnadirGrande != null) botonAnadirGrande.SetActive(false);
            if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(true);
        }
        else
        {
            if (imagenNino != null) imagenNino.SetActive(true);
            if (botonAnadirGrande != null) botonAnadirGrande.SetActive(true);
            if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(false);
        }
    }

    public void PuenteGuardar()
    {
        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.GuardarDatos();
        }
    }

    public void IntentarAbrirRegistro()
    {
        if (contadorMiembros < 7)
        {
            ventanaRegistro.SetActive(true);
            if (imagenLimite != null) imagenLimite.SetActive(false);
        }
        else
        {
            if (imagenLimite != null)
            {
                if (rutinaDesvanecer != null) StopCoroutine(rutinaDesvanecer);
                rutinaDesvanecer = StartCoroutine(MostrarYDesvanecer());
            }
        }
    }

    IEnumerator MostrarYDesvanecer()
    {
        cgAviso.alpha = 1f;
        imagenLimite.SetActive(true);
        yield return new WaitForSeconds(3f);
        float tiempo = 0;
        float duracionFade = 1f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            cgAviso.alpha = Mathf.Lerp(1f, 0f, tiempo / duracionFade);
            yield return null;
        }
        imagenLimite.SetActive(false);
    }

    public void NotificarMiembroGuardado()
    {
        // Recalculamos directamente desde la lista real para evitar errores
        if (ManejadorRegistro.instance != null)
        {
            contadorMiembros = ManejadorRegistro.instance.listaDeMiembros.Count;
        }
        else
        {
            contadorMiembros++;
        }

        ActualizarInterfazSegunContador();
    }

    public void RegistrarSeleccion(SeleccionMiembros nuevoMiembro)
    {
        if (miembroSeleccionado != null && miembroSeleccionado != nuevoMiembro)
        {
            miembroSeleccionado.Deseleccionar();
        }

        miembroSeleccionado = nuevoMiembro;

        if (ManejadorRegistro.instance != null && nuevoMiembro != null)
        {
            ManejadorRegistro.instance.nombreSeleccionado = nuevoMiembro.gameObject.name;
        }
    }

    public void ClickEnContinuar()
    {
        // Ahora contadorMiembros estará sincronizado
        if (contadorMiembros == 0)
        {
            if (avisoCreaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoCreaMiembro));
            return;
        }

        if (miembroSeleccionado == null)
        {
            if (avisoSeleccionaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoSeleccionaMiembro));
            return;
        }

        SceneManager.LoadScene(nombreEscenaCronometro);
    }

    public void ClickEnEliminar()
    {
        if (miembroSeleccionado == null)
        {
            if (avisoSeleccionaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoSeleccionaMiembro));
            return;
        }

        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.RemoverMiembroDeLaLista(miembroSeleccionado.gameObject.name);
            ManejadorRegistro.instance.nombreSeleccionado = "";
        }

        GameObject objetoABorrar = miembroSeleccionado.gameObject;
        miembroSeleccionado = null;
        Destroy(objetoABorrar);

        // Actualizamos el contador después de eliminar
        if (ManejadorRegistro.instance != null)
        {
            contadorMiembros = ManejadorRegistro.instance.listaDeMiembros.Count;
        }
        else
        {
            contadorMiembros--;
        }

        ActualizarInterfazSegunContador();
    }

    IEnumerator MostrarAvisoTemporal(GameObject aviso)
    {
        aviso.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        aviso.SetActive(false);
    }
}