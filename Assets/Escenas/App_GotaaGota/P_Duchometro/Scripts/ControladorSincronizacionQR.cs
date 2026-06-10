using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;
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
    public GameObject botonEscanearUI;

    [Header("--- TARJETA 1: SELECCIÓN ---")]
    public TMP_Text textoNumeroContador;
    public TMP_Text textoNumeroSeleccionados;
    public Transform contenedorLista;
    public GameObject prefabMiembro;

    [Header("--- TARJETA 2: SIMILITUD ---")]
    public SeleccionMiembros miembroLocalReferencia;
    public SeleccionMiembros miembroQRNuevo;

    private WebCamTexture texturaCamara;
    private bool esEscanerActivo = false;
    private string datosDecodificadosRaw;

    private List<ManejadorRegistro.DatosMiembro> listaRecibidaQR = new List<ManejadorRegistro.DatosMiembro>();
    private Dictionary<GameObject, bool> itemSeleccionadoMap = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, ManejadorRegistro.DatosMiembro> itemDatosMap = new Dictionary<GameObject, ManejadorRegistro.DatosMiembro>();

    private int indiceConflictoActual = 0;
    private List<Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>> conflictosDetectados = new List<Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>>();

    private bool huboFusionesSilenciosas = false;

    void Start() { ConfigurarEstadoInicial(); }

    void Update()
    {
        if (esEscanerActivo && texturaCamara != null && texturaCamara.isPlaying)
        {
            AjustarRotacionCamaraMovil();
            AnalizarFrameCamara();
        }
    }

    public void ConfigurarEstadoInicial()
    {
        if (panel2ResolucionConflicto != null) panel2ResolucionConflicto.SetActive(false);
        huboFusionesSilenciosas = false;

        if (texturaCamara != null && texturaCamara.isPlaying) texturaCamara.Stop();
        esEscanerActivo = false;
        if (botonEscanearUI != null) botonEscanearUI.SetActive(true);

        if (PlayerPrefs.GetInt("YaVioInfoQR", 0) == 0)
        {
            if (panel1SelectorAccion != null) panel1SelectorAccion.SetActive(true);
            if (tarjeta1Informativa != null) tarjeta1Informativa.SetActive(true);
            if (tarjeta2Opciones != null) tarjeta2Opciones.SetActive(false);
        }
        else
        {
            if (panel1SelectorAccion != null) panel1SelectorAccion.SetActive(true);
            if (tarjeta1Informativa != null) tarjeta1Informativa.SetActive(false);
            if (tarjeta2Opciones != null) tarjeta2Opciones.SetActive(true);
            if (visorCamara != null) visorCamara.gameObject.SetActive(false);
            if (imagenMiQR != null) imagenMiQR.gameObject.SetActive(true);
            GenerarMiCodigoQR();
        }
    }

    public void PresionarAceptarInformativo()
    {
        PlayerPrefs.SetInt("YaVioInfoQR", 1);
        PlayerPrefs.Save();

        tarjeta1Informativa.SetActive(false);
        tarjeta2Opciones.SetActive(true);
        visorCamara.gameObject.SetActive(false);
        imagenMiQR.gameObject.SetActive(true);

        GenerarMiCodigoQR();
    }

    private void GenerarMiCodigoQR()
    {
        if (ManejadorRegistro.instance == null) return;
        ManejadorRegistro.ListaWrapper wrapper = new ManejadorRegistro.ListaWrapper { miembros = ManejadorRegistro.instance.listaDeMiembros };
        string jsonDatos = JsonUtility.ToJson(wrapper);

        BarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = new QrCodeEncodingOptions { Width = 512, Height = 512, Margin = 1 } };
        Color32[] pixeles = writer.Write(jsonDatos);
        Texture2D texturaQR = new Texture2D(512, 512);
        texturaQR.SetPixels32(pixeles);
        texturaQR.Apply();
        if (imagenMiQR != null) imagenMiQR.texture = texturaQR;
    }

    public void PresionarEscanearCodigo()
    {
        imagenMiQR.gameObject.SetActive(false);
        visorCamara.gameObject.SetActive(true);
        if (botonEscanearUI != null) botonEscanearUI.SetActive(false);
        StartCoroutine(IniciarCamaraSeguraAndroid());
    }

    private IEnumerator IniciarCamaraSeguraAndroid()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
        }
#endif

        yield return null;

        string nombreCamaraTrasera = "";
        if (WebCamTexture.devices.Length == 0) yield break;

        foreach (var dispositivo in WebCamTexture.devices)
        {
            if (!dispositivo.isFrontFacing) { nombreCamaraTrasera = dispositivo.name; break; }
        }
        if (nombreCamaraTrasera == "") nombreCamaraTrasera = WebCamTexture.devices[0].name;

        if (texturaCamara != null && texturaCamara.isPlaying) texturaCamara.Stop();

        texturaCamara = new WebCamTexture(nombreCamaraTrasera, Screen.width, Screen.height);
        visorCamara.texture = texturaCamara;
        texturaCamara.Play();
        esEscanerActivo = true;
    }

    private void AjustarRotacionCamaraMovil()
    {
        int rotacion = texturaCamara.videoRotationAngle;
        visorCamara.rectTransform.localEulerAngles = new Vector3(0, 0, -rotacion);
    }

    private void AnalizarFrameCamara()
    {
        try
        {
            if (texturaCamara.width < 100) return;
            IBarcodeReader reader = new BarcodeReader();
            var resultado = reader.Decode(texturaCamara.GetPixels32(), texturaCamara.width, texturaCamara.height);

            if (resultado != null)
            {
                datosDecodificadosRaw = resultado.Text;
                texturaCamara.Stop();
                esEscanerActivo = false;
                ProcesarDatosEscaneados(datosDecodificadosRaw);
            }
        }
        catch (Exception) { }
    }

    private void ProcesarDatosEscaneados(string json)
    {
        try
        {
            ManejadorRegistro.ListaWrapper datosAmigo = JsonUtility.FromJson<ManejadorRegistro.ListaWrapper>(json);

            if (datosAmigo == null || datosAmigo.miembros == null)
            {
                Debug.LogWarning("Formato QR inválido.");
                return;
            }

            listaRecibidaQR = datosAmigo.miembros;

            if (panel1SelectorAccion != null) panel1SelectorAccion.SetActive(false);
            if (panel2ResolucionConflicto != null) panel2ResolucionConflicto.SetActive(true);

            CalcularConflictosYSimilitudes();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
    }

    private void CalcularConflictosYSimilitudes()
    {
        conflictosDetectados.Clear();
        List<ManejadorRegistro.DatosMiembro> limpiosDeSimilitud = new List<ManejadorRegistro.DatosMiembro>();

        foreach (var entrante in listaRecibidaQR)
        {
            bool esFusionSilenciosa = false;
            bool coincidenciaDeNombre = false;
            ManejadorRegistro.DatosMiembro localMatch = null;

            foreach (var local in ManejadorRegistro.instance.listaDeMiembros)
            {
                if (entrante.idUnico == local.idUnico || (local.idsAsociados != null && local.idsAsociados.Contains(entrante.idUnico)))
                {
                    esFusionSilenciosa = true;
                    localMatch = local;
                    break;
                }

                if (entrante.nombre.ToLower().Trim() == local.nombre.ToLower().Trim())
                {
                    coincidenciaDeNombre = true;
                    localMatch = local;
                }
            }

            if (esFusionSilenciosa)
            {
                FusionarHistoriales(localMatch, entrante);
                huboFusionesSilenciosas = true;
            }
            else if (coincidenciaDeNombre)
            {
                conflictosDetectados.Add(new Tuple<ManejadorRegistro.DatosMiembro, ManejadorRegistro.DatosMiembro>(localMatch, entrante));
            }
            else
            {
                limpiosDeSimilitud.Add(entrante);
            }
        }

        if (conflictosDetectados.Count > 0)
        {
            indiceConflictoActual = 0;
            ActivarTarjetaSimilitud();
        }
        else
        {
            ActivarTarjetaSeleccionMiembros(limpiosDeSimilitud);
        }
    }

    private void FusionarHistoriales(ManejadorRegistro.DatosMiembro local, ManejadorRegistro.DatosMiembro entrante)
    {
        if (entrante.historialBanos != null)
        {
            if (local.historialBanos == null) local.historialBanos = new List<ManejadorRegistro.RegistroBano>();

            foreach (var banoQR in entrante.historialBanos)
            {
                bool yaExiste = local.historialBanos.Exists(b => b.fecha == banoQR.fecha && b.hora == banoQR.hora);
                if (!yaExiste) local.historialBanos.Add(banoQR);
            }
            float mejor = 0;
            foreach (var b in local.historialBanos) { if (mejor == 0 || b.duracion < mejor) mejor = b.duracion; }
            local.mejorTiempo = mejor;
        }
    }

    private void ActivarTarjetaSimilitud()
    {
        if (tarjeta1Miembros != null) tarjeta1Miembros.SetActive(false);
        if (tarjeta2Similitud != null) tarjeta2Similitud.SetActive(true);

        var parActual = conflictosDetectados[indiceConflictoActual];

        if (miembroLocalReferencia != null && miembroQRNuevo != null)
        {
            miembroLocalReferencia.textoNombre.text = parActual.Item1.nombre;
            miembroLocalReferencia.textoEdad.text = parActual.Item1.edad + " años";
            miembroLocalReferencia.textoDuchas.text = "Duchas:\n" + (parActual.Item1.historialBanos?.Count ?? 0);
            miembroLocalReferencia.AplicarTema(parActual.Item1.indiceTemaColor);

            miembroQRNuevo.textoNombre.text = parActual.Item2.nombre;
            miembroQRNuevo.textoEdad.text = parActual.Item2.edad + " años";
            miembroQRNuevo.textoDuchas.text = "Duchas:\n" + (parActual.Item2.historialBanos?.Count ?? 0);
            miembroQRNuevo.AplicarTema(parActual.Item2.indiceTemaColor);
        }
    }

    public void Action_UnificarPerfiles()
    {
        var par = conflictosDetectados[indiceConflictoActual];

        FusionarHistoriales(par.Item1, par.Item2);
        huboFusionesSilenciosas = true;

        if (par.Item1.idsAsociados == null) par.Item1.idsAsociados = new List<string>();
        if (!par.Item1.idsAsociados.Contains(par.Item2.idUnico))
        {
            par.Item1.idsAsociados.Add(par.Item2.idUnico);
        }

        AvanzarEnConflictos();
    }

    public void Action_CrearComoNuevoSeparado()
    {
        var par = conflictosDetectados[indiceConflictoActual];
        string nombreBase = par.Item2.nombre;
        string nuevoNombre = nombreBase + " 2";
        int contador = 2;

        while (ManejadorRegistro.instance.listaDeMiembros.Exists(m => m.nombre.ToLower() == nuevoNombre.ToLower()) ||
               listaRecibidaQR.Exists(m => m != par.Item2 && m.nombre.ToLower() == nuevoNombre.ToLower()))
        {
            contador++;
            nuevoNombre = nombreBase + " " + contador;
        }

        par.Item2.nombre = nuevoNombre;
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
            List<ManejadorRegistro.DatosMiembro> listosParaMostrar = new List<ManejadorRegistro.DatosMiembro>();
            foreach (var m in listaRecibidaQR)
            {
                bool yaFusionado = ManejadorRegistro.instance.listaDeMiembros.Exists(l => l.idUnico == m.idUnico || (l.idsAsociados != null && l.idsAsociados.Contains(m.idUnico)));
                if (!yaFusionado) listosParaMostrar.Add(m);
            }
            ActivarTarjetaSeleccionMiembros(listosParaMostrar);
        }
    }

    private void ActivarTarjetaSeleccionMiembros(List<ManejadorRegistro.DatosMiembro> miembrosAEnlistar)
    {
        if (tarjeta2Similitud != null) tarjeta2Similitud.SetActive(false);
        if (tarjeta1Miembros != null) tarjeta1Miembros.SetActive(true);

        foreach (Transform hijo in contenedorLista) { Destroy(hijo.gameObject); }
        itemSeleccionadoMap.Clear();
        itemDatosMap.Clear();

        if (textoNumeroContador != null) textoNumeroContador.text = miembrosAEnlistar.Count.ToString();
        ActualizarTextoSeleccionados();

        if (miembrosAEnlistar.Count == 0)
        {
            if (huboFusionesSilenciosas) FinalizarSincronizacionYRefrescar();
            else Action_CancelarYSalir();
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
                uiTarjeta.textoDuchas.text = "Duchas:\n" + (miembro.historialBanos?.Count ?? 0);

                uiTarjeta.AplicarTema(miembro.indiceTemaColor);
                Color col = uiTarjeta.imagenFondo.color;
                uiTarjeta.imagenFondo.color = new Color(col.r * 0.4f, col.g * 0.4f, col.b * 0.4f, col.a);

                itemSeleccionadoMap.Add(nuevoItem, false);
                itemDatosMap.Add(nuevoItem, miembro);

                // --- SOLUCIÓN APLICADA AQUÍ ---
                Button btn = nuevoItem.GetComponent<Button>();
                if (btn == null) btn = nuevoItem.AddComponent<Button>();

                btn.onClick.RemoveAllListeners(); // Limpiamos cualquier función vieja

                // Congelamos las variables para evitar bugs en el foreach
                GameObject itemGuardado = nuevoItem;
                SeleccionMiembros uiGuardada = uiTarjeta;
                ManejadorRegistro.DatosMiembro miembroGuardado = miembro;

                btn.onClick.AddListener(() => AlHacerClicEnMiembroDeLista(itemGuardado, uiGuardada, miembroGuardado));
            }
        }
    }

    private void AlHacerClicEnMiembroDeLista(GameObject goItem, SeleccionMiembros uiRef, ManejadorRegistro.DatosMiembro datos)
    {
        bool estaIncluido = !itemSeleccionadoMap[goItem];
        itemSeleccionadoMap[goItem] = estaIncluido;

        if (estaIncluido)
        {
            uiRef.AplicarTema(datos.indiceTemaColor);
            LeanTween.scale(goItem, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
        }
        else
        {
            uiRef.AplicarTema(datos.indiceTemaColor);
            Color col = uiRef.imagenFondo.color;
            uiRef.imagenFondo.color = new Color(col.r * 0.4f, col.g * 0.4f, col.b * 0.4f, col.a);
        }

        ActualizarTextoSeleccionados();
    }

    private void ActualizarTextoSeleccionados()
    {
        if (textoNumeroSeleccionados == null) return;
        int contador = 0;
        foreach (var estado in itemSeleccionadoMap.Values) { if (estado) contador++; }
        textoNumeroSeleccionados.text = contador.ToString();
    }

    public void Action_GuardarMiembrosSeleccionados()
    {
        if (ManejadorRegistro.instance == null) return;

        foreach (var item in itemSeleccionadoMap)
        {
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

        ManejadorRanking ranking = UnityEngine.Object.FindFirstObjectByType<ManejadorRanking>();
        if (ranking != null) ranking.GenerarRanking();

        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(true);

        ConfigurarEstadoInicial();
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void Action_CancelarYSalir()
    {
        ConfigurarEstadoInicial();
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}