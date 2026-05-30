using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class ManejadorRanking : MonoBehaviour
{
    [Header("Configuración Ranking")]
    public GameObject itemPrefab;
    public Transform contenedor;

    [Header("Configuración Historial (Panel Detalles)")]
    public GameObject panelDetalles;
    public TMP_Text txtNombreTitulo;
    public Transform contenedorHistorial;
    public GameObject prefabItemHistorial;

    [Header("Sprites de Colores")]
    public Sprite barraVerde;
    public Sprite barraAmarilla;
    public Sprite barraRoja;

    // --- CORRECCIÓN: Usamos OnEnable en lugar de Start ---
    // OnEnable se ejecuta cada vez que el objeto (el panel) se activa.
    void OnEnable()
    {
        if (panelDetalles != null) panelDetalles.SetActive(false);
        GenerarRanking();
    }

    public void GenerarRanking()
    {
        if (contenedor == null || ManejadorRegistro.instance == null) return;
        foreach (Transform hijo in contenedor) { Destroy(hijo.gameObject); }

        var listaOrdenada = ManejadorRegistro.instance.listaDeMiembros
            .Where(m => m.mejorTiempo > 0)
            .OrderBy(m => m.mejorTiempo)
            .ToList();

        for (int i = 0; i < listaOrdenada.Count; i++)
        {
            GameObject nuevoItem = Instantiate(itemPrefab, contenedor);
            var datos = listaOrdenada[i];

            DetalleBotonRanking scriptBoton = nuevoItem.GetComponent<DetalleBotonRanking>();
            if (scriptBoton != null) scriptBoton.nombreMiembro = datos.nombre;

            TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 3)
            {
                textos[0].text = "#" + (i + 1);
                textos[1].text = datos.nombre;
                textos[2].text = FormatearTiempo(datos.mejorTiempo);
            }

            UnityEngine.UI.Image fondo = nuevoItem.GetComponent<UnityEngine.UI.Image>();
            fondo.sprite = ObtenerSpritePorTiempo(datos.mejorTiempo);
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

            itemH.GetComponent<UnityEngine.UI.Image>().sprite = ObtenerSpritePorTiempo(bano.duracion);
            contadorBano++;
        }
    }

    string FormatearFechaEspecial(string fechaOriginal)
    {
        try
        {
            DateTime fecha = DateTime.ParseExact(fechaOriginal, "dd/MM/yyyy", null);
            string diaNombre = fecha.ToString("ddd", new System.Globalization.CultureInfo("es-ES"));
            diaNombre = char.ToUpper(diaNombre[0]) + diaNombre.Substring(1).Replace(".", "");
            return diaNombre + "-" + fecha.ToString("dd-yyyy");
        }
        catch { return fechaOriginal; }
    }

    Sprite ObtenerSpritePorTiempo(float t)
    {
        if (t <= 300f) return barraVerde;
        if (t < 480f) return barraAmarilla;
        return barraRoja;
    }

    string FormatearTiempo(float t)
    {
        int min = Mathf.FloorToInt(t / 60);
        int seg = Mathf.FloorToInt(t % 60);
        return string.Format("{0}:{1:00} min", min, seg);
    }
}