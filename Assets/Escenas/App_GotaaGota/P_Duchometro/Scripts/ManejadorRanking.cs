using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
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

    [Header("--- LÍMITES DE TIEMPO (MINUTOS) ---")]
    public float limiteMinutosVerde = 5f;
    public float limiteMinutosAmarillo = 7f;

    [Header("Frases Motivacionales")]
    public string[] frasesVerdes = { "Eficiencia Total", "¡Eres un pro!", "Velocidad increíble" };
    public string[] frasesAmarillas = { "Sigue mejorando", "Casi perfecto", "Buen ritmo" };
    public string[] frasesRojas = { "¡Ahorra más!", "Inténtalo de nuevo", "Menos tiempo, más agua" };

    [Header("--- COLORES DE FONDO ---")]
    public Color colorFondoVerde = new Color32(84, 179, 138, 255);
    public Color colorFondoAmarillo = new Color32(221, 103, 37, 255);
    public Color colorFondoRojo = new Color32(122, 34, 40, 255);

    [Header("--- COLORES DE TEXTO RANKING ---")]
    public Color colorTextoVerde = new Color32(6, 26, 17, 255);
    public Color colorTextoAmarillo = new Color32(58, 26, 5, 255);
    public Color colorTextoRojo = new Color32(46, 10, 13, 255);

    [Header("--- COLOR FIJO HISTORIAL ---")]
    public Color colorAzulCorporativo = new Color32(26, 58, 95, 255);

    [Header("Configuración Historial")]
    public GameObject panelDetalles;
    public TMP_Text txtNombreTitulo;
    public Transform contenedorHistorial;
    public GameObject prefabItemHistorial;

    [Header("--- NUEVO: TEXTOS DEL RETO ---")]
    public TMP_Text txtRetoFijoHistorial; // Arrastra aquí el texto del panel chiquito
    public string textoSinRetoDefault = "Aún no tienes un reto tomado registrado."; // Puedes cambiar esto desde el Inspector

    void OnEnable()
    {
        GenerarRanking();
        StartCoroutine(ResetearScrollAlInicio());
    }

    IEnumerator ResetearScrollAlInicio()
    {
        yield return new WaitForEndOfFrame();
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
        panelDetalles.transform.localScale = Vector3.zero;
        LeanTween.cancel(panelDetalles);
        LeanTween.scale(panelDetalles, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);

        txtNombreTitulo.text = "Historial de " + nombre;
        txtNombreTitulo.color = colorAzulCorporativo;

        // --- NUEVO: Mostrar el reto guardado o el texto por defecto ---
        if (txtRetoFijoHistorial != null)
        {
            if (string.IsNullOrEmpty(miembro.ultimoRetoAceptado))
            {
                txtRetoFijoHistorial.text = textoSinRetoDefault;
            }
            else
            {
                txtRetoFijoHistorial.text = miembro.ultimoRetoAceptado;
            }
        }

        foreach (Transform hijo in contenedorHistorial) { Destroy(hijo.gameObject); }

        foreach (var bano in miembro.historialBanos)
        {
            GameObject itemH = Instantiate(prefabItemHistorial, contenedorHistorial);
            Color fondoActual = ObtenerColorFondoPorTiempo(bano.duracion);

            TMP_Text[] textos = itemH.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 3)
            {
                textos[0].text = FormatearFechaHistorial(bano.fecha);
                textos[1].text = nombre;
                textos[2].text = FormatearTiempoSimple(bano.duracion);
            }

            foreach (var txt in textos)
            {
                if (txt != null) txt.color = colorAzulCorporativo;
            }

            Image barritaColor = itemH.GetComponent<Image>();
            if (barritaColor != null) barritaColor.color = fondoActual;
        }
    }

    public void CerrarHistorial()
    {
        if (panelDetalles == null) return;

        LeanTween.cancel(panelDetalles);
        LeanTween.scale(panelDetalles, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            panelDetalles.SetActive(false);
        });
    }

    string FormatearTiempoSimple(float t) => string.Format("{0}:{1:00}", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60));

    // --- CONVERSIÓN AUTOMÁTICA A MINUTOS PARA EVALUAR ---
    string ObtenerFrase(float t)
    {
        float minutos = t / 60f;
        if (minutos <= limiteMinutosVerde) return frasesVerdes[UnityEngine.Random.Range(0, frasesVerdes.Length)];
        if (minutos < limiteMinutosAmarillo) return frasesAmarillas[UnityEngine.Random.Range(0, frasesAmarillas.Length)];
        return frasesRojas[UnityEngine.Random.Range(0, frasesRojas.Length)];
    }

    Color ObtenerColorFondoPorTiempo(float t)
    {
        float minutos = t / 60f;
        return (minutos <= limiteMinutosVerde) ? colorFondoVerde : (minutos < limiteMinutosAmarillo) ? colorFondoAmarillo : colorFondoRojo;
    }

    Color ObtenerColorTextoPorTiempo(float t)
    {
        float minutos = t / 60f;
        return (minutos <= limiteMinutosVerde) ? colorTextoVerde : (minutos < limiteMinutosAmarillo) ? colorTextoAmarillo : colorTextoRojo;
    }

    string FormatearFechaHistorial(string f)
    {
        try
        {
            DateTime fecha = DateTime.ParseExact(f, "dd/MM/yyyy", null);
            string fechaHoySistema = DateTime.Now.ToString("dd/MM/yyyy");

            if (f == fechaHoySistema)
            {
                return "HOY, " + fecha.ToString("d 'DE' MMMM 'DEL' yyyy", new System.Globalization.CultureInfo("es-ES")).ToUpper();
            }
            else
            {
                return fecha.ToString("d 'DE' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES")).ToUpper();
            }
        }
        catch
        {
            return f.ToUpper();
        }
    }
}