using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using ZXing;
using ZXing.QrCode;

public class ControladorSincronizacionQR : MonoBehaviour
{
    [Header("--- PANELES Y TARJETAS ---")]
    public GameObject panel1SelectorAccion;
    public GameObject tarjeta1Informativa;
    public GameObject tarjeta2Opciones;

    public GameObject panel2ResolucionConflicto;
    public GameObject tarjeta1Miembros;
    public GameObject tarjeta2Similitud;

    [Header("--- VISUALES DE QR Y CÁMARA ---")]
    public RawImage imagenMiQR;
    public RawImage visorCamara;

    [Header("--- TARJETA 1: SELECCIÓN ---")]
    public TMP_Text textoNumeroContador; // Solo el número dinámico
    public Transform contenedorLista; // El contenedor que dejamos limpio
    public GameObject prefabMiembro; // El molde original de tus Assets

    [Header("--- TARJETA 2: SIMILITUD ---")]
    public SeleccionMiembros miembroLocalReferencia;
    public SeleccionMiembros miembroQRNuevo;

    // Variables internas para el control de la cámara y datos
    private WebCamTexture texturaCamara;
    private bool esEscanerActivo = false;
    private string datosDecodificadosRaw;

    // Listas para manejar la lógica de "iluminación" que diseñaste
    private List<ManejadorRegistro.DatosMiembro> listaRecibidaQR = new List<ManejadorRegistro.DatosMiembro>();
    private Dictionary<GameObject, bool> itemSeleccionadoMap = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, ManejadorRegistro.DatosMiembro> itemDatosMap = new Dictionary<GameObject, ManejadorRegistro.DatosMiembro>();

    private int indiceConflictoActual = 0;
    private List<Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>> conflictosDetectados = new List<Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>>();

    void Start()
    {
        ConfigurarEstadoInicial();
    }

    void Update()
    {
        // Si el visor de la cámara está activo, buscamos códigos QR activamente en cada frame
        if (esEscanerActivo && texturaCamara != null && texturaCamara.isPlaying)
        {
            AnalizarFrameCamara();
        }
    }

    public void ConfigurarEstadoInicial()
    {
        panel1SelectorAccion.SetActive(true);
        tarjeta1Informativa.SetActive(true);
        tarjeta2Opciones.SetActive(false);
        panel2ResolucionConflicto.SetActive(false);

        if (texturaCamara != null && texturaCamara.isPlaying) texturaCamara.Stop();
        esEscanerActivo = false;
    }

    // --- ENTRAR A LA TARJETA 2 Y GENERAR MI QR ---
    public void PresionarAceptarInformativo()
    {
        tarjeta1Informativa.SetActive(false);
        tarjeta2Opciones.SetActive(true);
        visorCamara.gameObject.SetActive(false);
        imagenMiQR.gameObject.SetActive(true);

        GenerarMiCodigoQR();
    }

    private void GenerarMiCodigoQR()
    {
        if (ManejadorRegistro.instance == null) return;

        // Convertimos la lista de miembros actual del teléfono a texto JSON
        ManejadorRegistro.ListaWrapper wrapper = new ManejadorRegistro.ListaWrapper { miembros = ManejadorRegistro.instance.listaDeMiembros };
        string jsonDatos = JsonUtility.ToJson(wrapper);

        // Crear la textura del código QR usando el motor ZXing
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions { Width = 512, Height = 512, Margin = 1 }
        };

        Color32[] pixeles = writer.Write(jsonDatos);
        Texture2D texturaQR = new Texture2D(512, 512);
        texturaQR.SetPixels32(pixeles);
        texturaQR.Apply();

        imagenMiQR.texture = texturaQR;
    }

    // --- CONTROL DE LA CÁMARA (ESTADO B) ---
    public void PresionarEscanearCodigo()
    {
        imagenMiQR.gameObject.SetActive(false);
        visorCamara.gameObject.SetActive(true);

        // Inicializamos la cámara física del celular
        if (texturaCamara == null)
        {
            texturaCamara = new WebCamTexture(Screen.width, Screen.height);
        }

        if (!texturaCamara.isPlaying)
        {
            texturaCamara.Play();
        }

        visorCamara.texture = texturaCamara;
        esEscanerActivo = true;
    }

    private void AnalizarFrameCamara()
    {
        try
        {
            IBarcodeReader reader = new BarcodeReader();
            // Le pasamos los píxeles de la cámara al lector ZXing
            var resultado = reader.Decode(texturaCamara.GetPixels32(), texturaCamara.width, texturaCamara.height);

            if (resultado != null)
            {
                datosDecodificadosRaw = resultado.Text;
                texturaCamara.Stop();
                esEscanerActivo = false;

                // Efecto de sonido o respuesta háptica aquí si lo deseas
                ProcesarDatosEscaneados(datosDecodificadosRaw);
            }
        }
        catch (Exception)
        {
            // Buscando código... (Silenciamos errores menores de la cámara)
        }
    }

    // --- PROCESAMIENTO DE DATOS ENTRANTES ---
    private void ProcesarDatosEscaneados(string json)
    {
        try
        {
            ManejadorRegistro.ListaWrapper datosAmigo = JsonUtility.FromJson<ManejadorRegistro.ListaWrapper>(json);
            if (datosAmigo == null || datosAmigo.miembros == null) return;

            listaRecibidaQR = datosAmigo.miembros;
            panel1SelectorAccion.SetActive(false);
            panel2ResolucionConflicto.SetActive(true);

            CalcularConflictosYSimilitudes();
        }
        catch (Exception e)
        {
            Debug.LogError("El código QR escaneado no pertenece al formato del juego: " + e.Message);
            ConfigurarEstadoInicial();
        }
    }

    private void CalcularConflictosYSimilitudes()
    {
        conflictosDetectados.Clear();
        List<ManejadorRegistro.DatosMiembro> limpiosDeSimilitud = new List<ManejadorRegistro.DatosMiembro>();

        foreach (var entrante in listaRecibidaQR)
        {
            bool coincidenciaEncontrada = false;

            foreach (var local in ManejadorRegistro.instance.listaDeMiembros)
            {
                // Si tienen el mismo nombre (Ignorando mayúsculas/minúsculas) o son altamente similares
                if (entrante.nombre.ToLower().Trim() == local.nombre.ToLower().Trim())
                {
                    conflictosDetectados.Add(new Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>(local, entrante));
                    coincidenciaEncontrada = true;
                    break;
                }
            }

            if (!coincidenciaEncontrada)
            {
                limpiosDeSimilitud.Add(entrante);
            }
        }

        // Si hay conflictos de nombres, resolvemos la Tarjeta 2 primero
        if (conflictosDetectados.Count > 0)
        {
            indiceConflictoActual = 0;
            ActivarTarjetaSimilitud();
        }
        else
        {
            // Si no hay nombres idénticos/similares, vamos directo a la lista de selección de miembros
            ActivarTarjetaSeleccionMiembros(limpiosDeSimilitud);
        }
    }

    // --- LOGICA TARJETA 2: RESOLUCIÓN INDIVIDUAL ---
    private void ActivarTarjetaSimilitud()
    {
        tarjeta1Miembros.SetActive(false);
        tarjeta2Similitud.SetActive(true);

        var parActual = conflictosDetectados[indiceConflictoActual];

        // Llenamos visualmente las dos tarjetas de comparación fijas que creaste
        miembroLocalReferencia.textoNombre.text = parActual.Item1.nombre;
        miembroLocalReferencia.textoEdad.text = parActual.Item1.edad + " años";
        miembroLocalReferencia.textoDuchas.text = "Duchas totales:\n" + (parActual.Item1.historialBanos?.Count ?? 0);
        miembroLocalReferencia.AplicarTema(parActual.Item1.indiceTemaColor);

        miembroQRNuevo.textoNombre.text = parActual.Item2.nombre;
        miembroQRNuevo.textoEdad.text = parActual.Item2.edad + " años";
        miembroQRNuevo.textoDuchas.text = "Duchas totales:\n" + (parActual.Item2.historialBanos?.Count ?? 0);
        miembroQRNuevo.AplicarTema(parActual.Item2.indiceTemaColor);
    }

    // Respuesta Botón: SI, FUSIONAR DATOS
    public void Action_UnificarPerfiles()
    {
        var par = conflictosDetectados[indiceConflictoActual];

        // Fusionamos historiales sin duplicar registros exactos
        if (par.Item2.historialBanos != null)
        {
            if (par.Item1.historialBanos == null) par.Item1.historialBanos = new List<ManejadorRegistro.RegistroBano>();

            foreach (var banoQR in par.Item2.historialBanos)
            {
                // Un filtro simple para evitar meter la misma ducha a la misma hora
                bool yaExiste = par.Item1.historialBanos.Exists(b => b.fecha == banoQR.fecha && b.hora == banoQR.hora);
                if (!yaExiste)
                {
                    par.Item1.historialBanos.Add(banoQR);
                }
            }
            // Recalculamos el mejor tiempo histórico tras la fusión
            float mejor = 0;
            foreach (var b in par.Item1.historialBanos) { if (mejor == 0 || b.duracion < mejor) mejor = b.duracion; }
            par.Item1.mejorTiempo = mejor;
        }

        AvanzarEnConflictos();
    }

    // Respuesta Botón: NO, CREAR COMO NUEVO SEPARADO
    public void Action_CrearComoNuevoSeparado()
    {
        var par = conflictosDetectados[indiceConflictoActual];
        // Le agregamos un distintivo al nombre para que no colisionen en la base de datos
        par.Item2.nombre = par.Item2.nombre + " (QR)";

        // Lo añadimos temporalmente a la lista de "por procesar" en la siguiente tarjeta
        listaRecibidaQR.Add(par.Item2);

        AvanzarEnConflictos();
    }

    private void AvanzarEnConflictos()
    {
        indiceConflictoActual++;
        if (indiceConflictoActual < conflictosDetectados.Count)
        {
            ActivarTarjetaSimilitud();
        }
        else
        {
            // Ya terminamos los conflictos, ahora filtramos los que sobrevivieron para mostrarlos en la lista
            List<ManejadorRegistro.DatosMiembro> listosParaMostrar = new List<ManejadorRegistro.DatosMiembro>();
            foreach (var m in listaRecibidaQR)
            {
                // Evitamos volver a listar los que ya se fusionaron directamente en la base local
                bool esLocal = ManejadorRegistro.instance.listaDeMiembros.Exists(l => l.nombre.ToLower() == m.nombre.ToLower());
                if (!esLocal) listosParaMostrar.Add(m);
            }
            ActivarTarjetaSeleccionMiembros(listosParaMostrar);
        }
    }

    // --- LOGICA TARJETA 1: LISTA SELECCIÓN POR ILUMINACIÓN ---
    private void ActivarTarjetaSeleccionMiembros(List<ManejadorRegistro.DatosMiembro> miembrosAEnlistar)
    {
        tarjeta2Similitud.SetActive(false);
        tarjeta1Miembros.SetActive(true);

        // Limpieza absoluta del contenedor deslizable
        foreach (Transform hijo in contenedorLista) { Destroy(hijo.gameObject); }
        itemSeleccionadoMap.Clear();
        itemDatosMap.Clear();

        textoNumeroContador.text = miembrosAEnlistar.Count.ToString();

        // Si no hay miembros para enlistar, cerramos automáticamente
        if (miembrosAEnlistar.Count == 0)
        {
            FinalizarSincronizacionYRefrescar();
            return;
        }

        foreach (var miembro in miembrosAEnlistar)
        {
            GameObject nuevoItem = Instantiate(prefabMiembro, contenedorLista);
            nuevoItem.name = miembro.nombre;

            SeleccionMiembros uiTarjeta = nuevoItem.GetComponent<SeleccionMiembros>();
            if (uiTarjeta != null)
            {
                uiTarjeta.textoNombre.text = miembro.nombre;
                uiTarjeta.textoEdad.text = miembro.edad + " años";
                uiTarjeta.textoDuchas.text = "Duchas totales:\n" + (miembro.historialBanos?.Count ?? 0);
                uiTarjeta.AplicarTema(miembro.indiceTemaColor);

                // LOGICA DE TU NUEVA UX: Inician deseleccionados por defecto (opacos)
                uiTarjeta.imagenFondo.color = uiTarjeta.imagenFondo.color * 0.4f; // Súper oscurecido
                itemSeleccionadoMap.Add(nuevoItem, false); // false = no incluido
                itemDatosMap.Add(nuevoItem, miembro);

                // Reemplazamos el botón nativo para que responda a nuestra lógica de iluminación
                Button btn = nuevoItem.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => AlHacerClicEnMiembroDeLista(nuevoItem, uiTarjeta));
                }
            }
        }
    }

    private void AlHacerClicEnMiembroDeLista(GameObject goItem, SeleccionMiembros uiRef)
    {
        // Alternamos el estado
        bool estaIncluido = !itemSeleccionadoMap[goItem];
        itemSeleccionadoMap[goItem] = estaIncluido;

        if (estaIncluido)
        {
            //UX Iluminado: Color Brillante Normal de fábrica
            uiRef.imagenFondo.color = uiRef.imagenFondo.color * (1f / 0.4f);
            // Efecto de escala sutil con LeanTween para feedback táctil
            LeanTween.scale(goItem, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
        }
        else
        {
            //UX Apagado: Se vuelve opaco/oscuro de nuevo
            uiRef.imagenFondo.color = uiRef.imagenFondo.color * 0.4f;
        }
    }

    // --- ACCIONES FINALES DE BOTONES DE CONTROL ---
    public void Action_GuardarMiembrosSeleccionados()
    {
        if (ManejadorRegistro.instance == null) return;

        foreach (var item in itemSeleccionadoMap)
        {
            // Si el objeto se quedó iluminado (true), lo guardamos oficialmente en el disco
            if (item.Value == true)
            {
                ManejadorRegistro.DatosMiembro datosAGuardar = itemDatosMap[item.Key];
                ManejadorRegistro.instance.listaDeMiembros.Add(datosAGuardar);
            }
        }

        FinalizarSincronizacionYRefrescar();
    }

    private void FinalizarSincronizacionYRefrescar()
    {
        if (ManejadorRegistro.instance != null)
        {
            ManejadorRegistro.instance.GuardarEnDisco();
            ManejadorRegistro.instance.RefrescarListaVisual();
        }

        // Buscamos el ranking en la escena para que se actualice de inmediato con los datos nuevos
        ManejadorRanking ranking = UnityEngine.Object.FindFirstObjectByType<ManejadorRanking>();
        if (ranking != null) ranking.GenerarRanking();

        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(true);

        ConfigurarEstadoInicial();
        this.gameObject.transform.parent.gameObject.SetActive(false); // Cierra el Panel_QR_Principal por completo
    }

    public void Action_CancelarYSalir()
    {
        ConfigurarEstadoInicial();
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}