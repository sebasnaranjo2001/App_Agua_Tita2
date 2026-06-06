using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class ManejadorRetos : MonoBehaviour
{
    [Header("Referencias UI Generales")]
    public GameObject panelRetos;
    public TMP_Text textoDelReto;
    public Button btnOtroReto;

    [Header("Referencias del Peor Dato")]
    public Transform contenedorPeorDato;
    public GameObject prefabItemHistorial;

    [Header("--- LÍMITES Y COLORES DEL PREFAB ---")]
    public float limiteTiempoVerde = 300f;
    public float limiteTiempoAmarillo = 480f;
    public Color colorFondoVerde = new Color32(84, 179, 138, 255);
    public Color colorFondoAmarillo = new Color32(221, 103, 37, 255);
    public Color colorFondoRojo = new Color32(122, 34, 40, 255);
    public Color colorAzulCorporativo = new Color32(26, 58, 95, 255);

    [Header("--- LISTAS DE RETOS POR EDAD ---")]
    [TextArea]
    public string[] retosNinos = new string[] {
        "Dile a un adulto que revise las llaves de agua.",
        "Usa un vaso de agua para lavarte los dientes hoy.",
        "Dibuja un cartel sobre ahorrar agua y pegalo en el bano."
    };

    [TextArea]
    public string[] retosJovenesAdultos = new string[] {
        "¡Duchate en menos de 5 minutos hoy!",
        "Paga el recibo del agua de este mes.",
        "Lava los platos de hoy cerrando el grifo mientras enjabonas."
    };

    private int ultimoIndice = -1;
    private int contadorRefrescos = 0;
    private bool estaAnimando = false;

    private int edadDelPeor = 0;

    void Start()
    {
        if (panelRetos != null) panelRetos.SetActive(false);
    }

    public void AbrirRetos()
    {
        if (panelRetos == null) return;

        contadorRefrescos = 0;
        if (btnOtroReto != null) btnOtroReto.interactable = true;

        ConfigurarPeorDato();

        if (textoDelReto != null)
        {
            textoDelReto.transform.localScale = Vector3.one;
            textoDelReto.transform.localRotation = Quaternion.identity;
        }

        MostrarNuevoReto();

        LeanTween.cancel(panelRetos);
        panelRetos.transform.localScale = Vector3.zero;
        panelRetos.SetActive(true);
        LeanTween.scale(panelRetos, Vector3.one, 0.4f).setEaseOutBack();
    }

    private void ConfigurarPeorDato()
    {
        int childCount = contenedorPeorDato.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform hijo = contenedorPeorDato.GetChild(i);
            hijo.SetParent(null);
            Destroy(hijo.gameObject);
        }

        if (ManejadorRegistro.instance == null || ManejadorRegistro.instance.listaDeMiembros.Count == 0) return;

        float peorTiempo = -1f;
        string peorNombre = "";
        string peorFecha = "";
        int peorEdad = 0;

        foreach (var miembro in ManejadorRegistro.instance.listaDeMiembros)
        {
            foreach (var bano in miembro.historialBanos)
            {
                if (bano.duracion > peorTiempo)
                {
                    peorTiempo = bano.duracion;
                    peorNombre = miembro.nombre;
                    peorFecha = bano.fecha;

                    int.TryParse(miembro.edad, out peorEdad);
                }
            }
        }

        edadDelPeor = peorEdad;

        if (peorTiempo > 0)
        {
            GameObject itemPeor = Instantiate(prefabItemHistorial, contenedorPeorDato);

            itemPeor.SetActive(true);
            itemPeor.transform.localScale = Vector3.one;
            itemPeor.transform.localPosition = Vector3.zero;

            TMP_Text[] textos = itemPeor.GetComponentsInChildren<TMP_Text>();
            if (textos.Length >= 3)
            {
                textos[0].text = FormatearFechaHistorial(peorFecha);
                textos[1].text = peorNombre;
                textos[2].text = FormatearTiempoSimple(peorTiempo);
            }

            foreach (var txt in textos) { if (txt != null) txt.color = colorAzulCorporativo; }

            Image barritaColor = itemPeor.GetComponent<Image>();
            if (barritaColor != null) barritaColor.color = ObtenerColorFondoPorTiempo(peorTiempo);

            LayoutRebuilder.ForceRebuildLayoutImmediate(contenedorPeorDato.GetComponent<RectTransform>());
        }
    }

    public void RefrescarReto()
    {
        if (contadorRefrescos >= 3 || estaAnimando || textoDelReto == null) return;

        contadorRefrescos++;
        estaAnimando = true;

        GameObject txtObj = textoDelReto.gameObject;
        LeanTween.cancel(txtObj);

        LeanTween.scale(txtObj, Vector3.zero, 0.25f).setEaseInQuad();
        LeanTween.rotateAroundLocal(txtObj, Vector3.forward, -180f, 0.25f).setEaseInQuad().setOnComplete(() => {

            MuestraTextoInterno();
            txtObj.transform.localRotation = Quaternion.Euler(0, 0, 180f);

            LeanTween.scale(txtObj, Vector3.one, 0.25f).setEaseOutQuad();
            LeanTween.rotateAroundLocal(txtObj, Vector3.forward, -180f, 0.25f).setEaseOutQuad().setOnComplete(() => {

                txtObj.transform.localRotation = Quaternion.identity;
                estaAnimando = false;

                if (contadorRefrescos >= 3 && btnOtroReto != null)
                {
                    btnOtroReto.interactable = false;
                }
            });
        });
    }

    private void MostrarNuevoReto()
    {
        MuestraTextoInterno();
    }

    private void MuestraTextoInterno()
    {
        string[] listaSeleccionada;

        // --- LÓGICA SIMPLIFICADA A DOS GRUPOS ---
        if (edadDelPeor <= 18)
        {
            listaSeleccionada = retosNinos;
        }
        else
        {
            listaSeleccionada = retosJovenesAdultos; // 19 en adelante
        }

        if (listaSeleccionada == null || listaSeleccionada.Length == 0) return;

        int nuevoIndice;
        if (listaSeleccionada.Length > 1)
        {
            do { nuevoIndice = UnityEngine.Random.Range(0, listaSeleccionada.Length); } while (nuevoIndice == ultimoIndice);
        }
        else { nuevoIndice = 0; }

        ultimoIndice = nuevoIndice;
        textoDelReto.text = listaSeleccionada[nuevoIndice];
    }

    public void CerrarRetos()
    {
        if (panelRetos == null) return;

        LeanTween.cancel(panelRetos);
        if (textoDelReto != null) LeanTween.cancel(textoDelReto.gameObject);
        estaAnimando = false;

        LeanTween.scale(panelRetos, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => {
            panelRetos.SetActive(false);
        });
    }

    string FormatearTiempoSimple(float t) => string.Format("{0}:{1:00}", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60));
    Color ObtenerColorFondoPorTiempo(float t) => (t <= limiteTiempoVerde) ? colorFondoVerde : (t < limiteTiempoAmarillo) ? colorFondoAmarillo : colorFondoRojo;

    string FormatearFechaHistorial(string f)
    {
        try
        {
            DateTime fecha = DateTime.ParseExact(f, "dd/MM/yyyy", null);
            string fechaHoy = DateTime.Now.ToString("dd/MM/yyyy");
            return (f == fechaHoy)
                ? "HOY, " + fecha.ToString("d 'DE' MMMM 'DEL' yyyy", new System.Globalization.CultureInfo("es-ES")).ToUpper()
                : fecha.ToString("d 'DE' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES")).ToUpper();
        }
        catch { return f.ToUpper(); }
    }
}