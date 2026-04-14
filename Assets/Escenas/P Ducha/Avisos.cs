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
    private int contadorMiembros = 0;
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
        // Estado inicial: Niño y Botón Grande visibles. Panel pequeño oculto.
        if (botonAnadirGrande != null) botonAnadirGrande.SetActive(true);
        if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(false);
        if (imagenNino != null) imagenNino.SetActive(true);

        if (imagenLimite != null)
        {
            cgAviso = imagenLimite.GetComponent<CanvasGroup>();
            imagenLimite.SetActive(false);
        }

        if (avisoCreaMiembro != null) avisoCreaMiembro.SetActive(false);
        if (avisoSeleccionaMiembro != null) avisoSeleccionaMiembro.SetActive(false);
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
        contadorMiembros++;

        // Al guardar el primero, hacemos el intercambio de botones
        if (contadorMiembros >= 1)
        {
            if (imagenNino != null) imagenNino.SetActive(false);
            if (botonAnadirGrande != null) botonAnadirGrande.SetActive(false);
            if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(true);
        }
    }

    public void RegistrarSeleccion(SeleccionMiembros nuevoMiembro)
    {
        if (miembroSeleccionado != null && miembroSeleccionado != nuevoMiembro)
        {
            miembroSeleccionado.Deseleccionar();
        }
        miembroSeleccionado = nuevoMiembro;
    }

    public void ClickEnContinuar()
    {
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
        if (ventanaSiguiente != null) ventanaSiguiente.SetActive(true);
    }

    public void ClickEnEliminar()
    {
        if (miembroSeleccionado == null)
        {
            if (avisoSeleccionaMiembro != null) StartCoroutine(MostrarAvisoTemporal(avisoSeleccionaMiembro));
            return;
        }

        // --- NUEVA LÓGICA DE SINCRONIZACIÓN ---
        // Le pedimos al ManejadorRegistro que borre los datos internos usando el nombre del objeto
        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.RemoverMiembroDeLaLista(miembroSeleccionado.gameObject.name);
        }

        Debug.Log("Eliminando objeto: " + miembroSeleccionado.gameObject.name);
        GameObject objetoABorrar = miembroSeleccionado.gameObject;

        miembroSeleccionado = null;
        Destroy(objetoABorrar);

        contadorMiembros--;

        // Si ya no quedan miembros, volvemos al estado inicial (Botón Grande y Niño)
        if (contadorMiembros <= 0)
        {
            contadorMiembros = 0;
            if (imagenNino != null) imagenNino.SetActive(true);
            if (botonAnadirGrande != null) botonAnadirGrande.SetActive(true);
            if (panelBotonesPequenos != null) panelBotonesPequenos.SetActive(false);
        }
    }

    IEnumerator MostrarAvisoTemporal(GameObject aviso)
    {
        aviso.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        aviso.SetActive(false);
    }
}