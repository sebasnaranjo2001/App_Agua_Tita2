using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // <-- IMPORTANTE PARA QUE FUNCIONE LA CORRUTINA
using System.Collections.Generic;
using System.Linq;
using System;

public class ManejadorRanking : MonoBehaviour
{
    [Header("Configuración Ranking")]
    public GameObject itemPrefab;
    public Transform contenedor;
    public ScrollRect scrollRect;
    public GameObject panelSinDatos;
    public GameObject panelConDatos;

    [Header("--- LÍMITES DE TIEMPO (SEGUNDOS) ---")]
    [Tooltip("Tiempo máximo para estar en VERDE. (Ej: 300 = 5 minutos)")]
    public float limiteTiempoVerde = 300f;
    [Tooltip("Tiempo máximo para estar en AMARILLO. (Ej: 480 = 8 minutos)")]
    public float limiteTiempoAmarillo = 480f;

    [Header("Frases Motivacionales")]
    public string[] frasesVerdes = { "Eficiencia Total", "¡Eres un pro!", "Velocidad increíble" };
    public string[] frasesAmarillas = { "Sigue mejorando", "Casi perfecto", "Buen ritmo" };
    public string[] frasesRojas = { "¡Ahorra más!", "Inténtalo de nuevo", "Menos tiempo, más agua" };

    [Header("--- COLORES DE FONDO ---")]
    public Color colorFondoVerde = new Color32(84, 179, 138, 255);     // #54B38A
    public Color colorFondoAmarillo = new Color32(221, 103, 37, 255);  // #DD6725
    public Color colorFondoRojo = new Color32(122, 34, 40, 255);       // #7A2228

    [Header("--- COLORES DE TEXTO ---")]
    public Color colorTextoVerde = new Color32(6, 26, 17, 255);        // #061A11
    public Color colorTextoAmarillo = new Color32(58, 26, 5, 255);     // #3A1A05
    public Color colorTextoRojo = new Color32(46, 10, 13, 255);        // #2E0A0D

    [Header("Configuración Historial")]
    public GameObject panelDetalles;
    public TMP_Text txtNombreTitulo;
    public Transform contenedorHistorial;
    public GameObject prefabItemHistorial;

    void OnEnable()
    {
        GenerarRanking();

        // --- CORRECCIÓN: Usamos una corrutina para no bloquear el Scroll ---
        StartCoroutine(ResetearScrollAlInicio());
    }

    IEnumerator ResetearScrollAlInicio()
    {
        // Esperamos a que Unity termine de dibujar las tarjetas nuevas
        yield return new WaitForEndOfFrame();

        // Ahora sí, lo mandamos al tope de forma segura
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
    }

    public void GenerarRanking()
    {
        if (contenedor == null || ManejadorRegistro.instance == null) return;

        foreach (Transform hijo in contenedor) { Destroy(hijo.gameObject); }

        var lista = ManejadorRegistro.instance.listaDeMiembros.Where(m => m.mejorTiempo > 0).OrderBy(m => m.mejorTiempo).ToList();

        if (panelSinDatos) panelSinDatos.SetActive(lista.Count == 0);
        if (panelConDatos) panelConDatos.SetActive(lista.Count > 0);

        for (int i = 0; i < lista.Count; i++)
        {
            GameObject nuevoItem = Instantiate(itemPrefab, contenedor);
            var datos = lista[i];

            Color fondoActual = ObtenerColorFondoPorTiempo(datos.mejorTiempo);
            Color textoActual = ObtenerColorTextoPorTiempo(datos.mejorTiempo);

            DetalleBotonRanking refs = nuevoItem.GetComponent<DetalleBotonRanking>();
            if (refs != null)
            {
                refs.nombreMiembro = datos.nombre;

                if (refs.txtPuesto) { refs.txtPuesto.text = (i + 1).ToString(); refs.txtPuesto.color = textoActual; }
                if (refs.txtNombre) { refs.txtNombre.text = datos.nombre; refs.txtNombre.color = textoActual; }
                if (refs.txtTiempo) { refs.txtTiempo.text = FormatearTiempoSimple(datos.mejorTiempo); refs.txtTiempo.color = textoActual; }
                if (refs.txtMotivacion) { refs.txtMotivacion.text = ObtenerFrase(datos.mejorTiempo); refs.txtMotivacion.color = textoActual; }
            }

            Image fondo = nuevoItem.GetComponent<Image>();
            if (fondo != null) fondo.color = fondoActual;
        }
    }

    public void AbrirHistorial(string nombre)
    {
        if (prefabItemHistorial == null) { Debug.LogError("prefabItemHistorial está vacío!"); return; }

        var miembro = ManejadorRegistro.instance.listaDeMiembros.Find(m => m.nombre == nombre);
        if (miembro == null) return;

        panelDetalles.SetActive(true);
        txtNombreTitulo.text = "Historial de " + nombre;

        foreach (Transform hijo in contenedorHistorial) { Destroy(hijo.gameObject); }

        int contadorBano = 1;
        foreach (var bano in miembro.historialBanos)
        {
            GameObject itemH = Instantiate(prefabItemHistorial, contenedorHistorial);

            Color fondoActual = ObtenerColorFondoPorTiempo(bano.duracion);
            Color textoActual = ObtenerColorTextoPorTiempo(bano.duracion);

            TMP_Text[] textos = itemH.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 4)
            {
                textos[0].text = "#" + contadorBano;
                textos[1].text = FormatearFechaEspecial(bano.fecha);
                textos[2].text = bano.hora;
                textos[3].text = FormatearTiempoSimple(bano.duracion);

                foreach (var txt in textos) { txt.color = textoActual; }
            }
            itemH.GetComponent<Image>().color = fondoActual;
            contadorBano++;
        }
    }

    string FormatearTiempoSimple(float t) => string.Format("{0}:{1:00}", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60));

    string ObtenerFrase(float t)
    {
        if (t <= limiteTiempoVerde) return frasesVerdes[UnityEngine.Random.Range(0, frasesVerdes.Length)];
        if (t < limiteTiempoAmarillo) return frasesAmarillas[UnityEngine.Random.Range(0, frasesAmarillas.Length)];
        return frasesRojas[UnityEngine.Random.Range(0, frasesRojas.Length)];
    }

    Color ObtenerColorFondoPorTiempo(float t) => (t <= limiteTiempoVerde) ? colorFondoVerde : (t < limiteTiempoAmarillo) ? colorFondoAmarillo : colorFondoRojo;
    Color ObtenerColorTextoPorTiempo(float t) => (t <= limiteTiempoVerde) ? colorTextoVerde : (t < limiteTiempoAmarillo) ? colorTextoAmarillo : colorTextoRojo;

    string FormatearFechaEspecial(string f) { try { DateTime fecha = DateTime.ParseExact(f, "dd/MM/yyyy", null); string dia = fecha.ToString("ddd", new System.Globalization.CultureInfo("es-ES")); return char.ToUpper(dia[0]) + dia.Substring(1).Replace(".", "") + "-" + fecha.ToString("dd-yyyy"); } catch { return f; } }
}