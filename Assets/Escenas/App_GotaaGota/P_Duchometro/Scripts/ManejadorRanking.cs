using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System; // Necesario para el manejo de fechas

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

    void Start()
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
            // El Ranking principal sigue usando 3 textos (Puesto, Nombre, Tiempo)
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

            // Historial ahora usa 4 textos (Puesto, Fecha, Hora, Tiempo)
            if (textos.Length >= 4)
            {
                // 1. PUESTO (#1)
                textos[0].text = "#" + contadorBano;

                // 2. FECHA (Formato: Mier-12-2026)
                textos[1].text = FormatearFechaEspecial(bano.fecha);

                // 3. HORA (Formato: 5:10 PM)
                textos[2].text = bano.hora;

                // 4. TIEMPO (Formato: 5:55 min)
                textos[3].text = FormatearTiempo(bano.duracion);
            }

            itemH.GetComponent<UnityEngine.UI.Image>().sprite = ObtenerSpritePorTiempo(bano.duracion);
            contadorBano++;
        }
    }

    // Función para transformar "dd/MM/yyyy" a "Día-dd-yyyy"
    string FormatearFechaEspecial(string fechaOriginal)
    {
        try
        {
            // Convertimos el texto guardado a una fecha real
            DateTime fecha = DateTime.ParseExact(fechaOriginal, "dd/MM/yyyy", null);

            // Obtenemos el nombre del día en español (Lun, Mar, Mie...)
            string diaNombre = fecha.ToString("ddd", new System.Globalization.CultureInfo("es-ES"));

            // Limpiamos el punto y ponemos la primera en mayúscula
            diaNombre = char.ToUpper(diaNombre[0]) + diaNombre.Substring(1).Replace(".", "");

            // Armamos el formato: Mier-14-2026
            return diaNombre + "-" + fecha.ToString("dd-yyyy");
        }
        catch
        {
            return fechaOriginal; // Si algo falla, devuelve la fecha normal
        }
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