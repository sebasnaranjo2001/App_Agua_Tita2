using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class ManejadorRanking : MonoBehaviour
{
    [Header("Configuración Ranking")]
    public GameObject itemPrefab;
    public Transform contenedor;
    public ScrollRect scrollRect; // <-- NUEVA REFERENCIA
    public GameObject panelSinDatos;
    public GameObject panelConDatos;

    [Header("Frases Motivacionales")]
    public string[] frasesVerdes = { "Eficiencia Total", "¡Eres un pro!", "Velocidad increíble" };
    public string[] frasesAmarillas = { "Sigue mejorando", "Casi perfecto", "Buen ritmo" };
    public string[] frasesRojas = { "¡Ahorra más!", "Inténtalo de nuevo", "Menos tiempo, más agua" };

    [Header("Colores")]
    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

    [Header("Configuración Historial")]
    public GameObject panelDetalles;
    public TMP_Text txtNombreTitulo;
    public Transform contenedorHistorial;
    public GameObject prefabItemHistorial;

    void OnEnable()
    {
        // Resetear scroll al tope
        if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;

        GenerarRanking();
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

            DetalleBotonRanking refs = nuevoItem.GetComponent<DetalleBotonRanking>();
            if (refs != null)
            {
                refs.nombreMiembro = datos.nombre;
                if (refs.txtPuesto) refs.txtPuesto.text = (i + 1).ToString();
                if (refs.txtNombre) refs.txtNombre.text = datos.nombre;
                if (refs.txtTiempo) refs.txtTiempo.text = FormatearTiempoSimple(datos.mejorTiempo);
                if (refs.txtMotivacion) refs.txtMotivacion.text = ObtenerFrase(datos.mejorTiempo);
            }

            Image fondo = nuevoItem.GetComponent<Image>();
            if (fondo != null) fondo.color = ObtenerColorPorTiempo(datos.mejorTiempo);
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
            TMP_Text[] textos = itemH.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 4)
            {
                textos[0].text = "#" + contadorBano;
                textos[1].text = FormatearFechaEspecial(bano.fecha);
                textos[2].text = bano.hora;
                textos[3].text = FormatearTiempoSimple(bano.duracion);
            }
            itemH.GetComponent<Image>().color = ObtenerColorPorTiempo(bano.duracion);
            contadorBano++;
        }
    }

    string FormatearTiempoSimple(float t) => string.Format("{0}:{1:00}", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60));
    string ObtenerFrase(float t)
    {
        if (t <= 300f) return frasesVerdes[UnityEngine.Random.Range(0, frasesVerdes.Length)];
        if (t < 480f) return frasesAmarillas[UnityEngine.Random.Range(0, frasesAmarillas.Length)];
        return frasesRojas[UnityEngine.Random.Range(0, frasesRojas.Length)];
    }
    Color ObtenerColorPorTiempo(float t) => (t <= 300f) ? colorVerde : (t < 480f) ? colorAmarillo : colorRojo;
    string FormatearFechaEspecial(string f) { try { DateTime fecha = DateTime.ParseExact(f, "dd/MM/yyyy", null); string dia = fecha.ToString("ddd", new System.Globalization.CultureInfo("es-ES")); return char.ToUpper(dia[0]) + dia.Substring(1).Replace(".", "") + "-" + fecha.ToString("dd-yyyy"); } catch { return f; } }
}