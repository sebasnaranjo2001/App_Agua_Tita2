using UnityEngine;
using TMPro;
using UnityEngine.UI; // <-- ESTO ES LO QUE FALTABA
using System.Collections.Generic;
using System.Linq;
using System;

public class ManejadorRanking : MonoBehaviour
{
    [Header("Configuración Ranking")]
    public GameObject itemPrefab;
    public Transform contenedor;
    public GameObject panelSinDatos;
    public GameObject panelConDatos;

    [Header("Configuración Historial")]
    public GameObject panelDetalles;
    public TMP_Text txtNombreTitulo;
    public Transform contenedorHistorial;
    public GameObject prefabItemHistorial;

    [Header("Colores Dinámicos")]
    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

    void OnEnable()
    {
        if (panelDetalles != null) panelDetalles.SetActive(false);
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

            // Conexión automática del botón
            Button btn = nuevoItem.GetComponent<Button>();
            if (btn == null) btn = nuevoItem.AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => AbrirHistorial(datos.nombre));

            TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 3)
            {
                textos[0].text = "#" + (i + 1);
                textos[1].text = datos.nombre;
                textos[2].text = FormatearTiempo(datos.mejorTiempo);
            }

            Image fondo = nuevoItem.GetComponent<Image>();
            if (fondo != null) fondo.color = ObtenerColorPorTiempo(datos.mejorTiempo);
        }
    }

    public void AbrirHistorial(string nombre)
    {
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
                textos[3].text = FormatearTiempo(bano.duracion);
            }
            itemH.GetComponent<Image>().color = ObtenerColorPorTiempo(bano.duracion);
            contadorBano++;
        }
    }

    string FormatearFechaEspecial(string f) { try { DateTime fecha = DateTime.ParseExact(f, "dd/MM/yyyy", null); string dia = fecha.ToString("ddd", new System.Globalization.CultureInfo("es-ES")); return char.ToUpper(dia[0]) + dia.Substring(1).Replace(".", "") + "-" + fecha.ToString("dd-yyyy"); } catch { return f; } }
    string FormatearTiempo(float t) { return string.Format("{0}:{1:00} min", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60)); }
    Color ObtenerColorPorTiempo(float t) { if (t <= 300f) return colorVerde; if (t < 480f) return colorAmarillo; return colorRojo; }
}