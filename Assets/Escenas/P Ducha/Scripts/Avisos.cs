using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    private CanvasGroup cgAviso;
    private Coroutine rutinaDesvanecer;

    [Header("Gestión de Selección")]
    public SeleccionMiembros miembroSeleccionado;

    private ManejadorNavegacion navegador;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        navegador = Object.FindFirstObjectByType<ManejadorNavegacion>();

        if (imagenLimite != null)
        {
            cgAviso = imagenLimite.GetComponent<CanvasGroup>();
            imagenLimite.SetActive(false);
        }

        if (avisoCreaMiembro != null) avisoCreaMiembro.SetActive(false);
        if (avisoSeleccionaMiembro != null) avisoSeleccionaMiembro.SetActive(false);

        // Al arrancar, verificamos el estado de la lista
        ActualizarInterfazSegunContador();
    }

    // --- ESTA FUNCIÓN AHORA ES INFALIBLE ---
    public void ActualizarInterfazSegunContador()
    {
        if (ManejadorRegistro.instance == null) return;

        // Le preguntamos la verdad absoluta al Manejador de Registro
        int totalMiembros = ManejadorRegistro.instance.listaDeMiembros.Count;
        bool hayMiembros = (totalMiembros > 0);

        // Controlamos los objetos según si hay o no gente
        if (imagenNino != null) imagenNino.SetActive(!hayMiembros);
        if (botonAnadirGrande != null) botonAnadirGrande.SetActive(!hayMiembros);
        if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(hayMiembros);

        Debug.Log("Sincronizando interfaz: ¿Hay miembros? " + hayMiembros);
    }

    public void PuenteGuardar()
    {
        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.GuardarDatos();
            // No hace falta llamar a actualizar aquí porque 
            // ManejadorRegistro llamará a NotificarMiembroGuardado()
        }
    }

    public void IntentarAbrirRegistro()
    {
        int total = ManejadorRegistro.instance.listaDeMiembros.Count;

        if (total < 7)
        {
            if (ventanaRegistro != null) ventanaRegistro.SetActive(true);
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
        if (cgAviso == null) yield break;
        cgAviso.alpha = 1f;
        imagenLimite.SetActive(true);
        yield return new WaitForSeconds(3f);
        float tiempo = 0; float duracionFade = 1f;
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
        // Forzamos el refresco total de la pantalla
        ActualizarInterfazSegunContador();
    }

    public void RegistrarSeleccion(SeleccionMiembros nuevoMiembro)
    {
        if (miembroSeleccionado != null && miembroSeleccionado != nuevoMiembro)
            miembroSeleccionado.Deseleccionar();

        miembroSeleccionado = nuevoMiembro;

        if (ManejadorRegistro.instance != null && nuevoMiembro != null)
            ManejadorRegistro.instance.nombreSeleccionado = nuevoMiembro.gameObject.name;
    }

    public void ClickEnContinuar()
    {
        int total = ManejadorRegistro.instance.listaDeMiembros.Count;

        if (total == 0)
        {
            if (avisoCreaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoCreaMiembro));
            return;
        }
        if (miembroSeleccionado == null)
        {
            if (avisoSeleccionaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoSeleccionaMiembro));
            return;
        }
        if (navegador != null) navegador.IrACronometro();
    }

    public void ClickEnEliminar()
    {
        if (miembroSeleccionado == null) return;

        string nombreABorrar = miembroSeleccionado.gameObject.name;

        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.RemoverMiembroDeLaLista(nombreABorrar);
            ManejadorRegistro.instance.nombreSeleccionado = "";
        }

        Destroy(miembroSeleccionado.gameObject);
        miembroSeleccionado = null;

        // Esperamos un milisegundo a que Unity termine de borrar el objeto y actualizamos
        Invoke("ActualizarInterfazSegunContador", 0.05f);
    }

    IEnumerator MostrarAvisoTemporal(GameObject aviso)
    {
        aviso.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        aviso.SetActive(false);
    }
}