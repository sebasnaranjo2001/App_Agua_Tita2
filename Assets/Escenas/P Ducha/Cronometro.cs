using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Cronometro : MonoBehaviour
{
    [Header("Referencias de UI - Textos")]
    public TMP_Text textoCronometro;
    public TMP_Text textoConsumo;

    [Header("Referencias de UI - Botones/Tarjetas")]
    public GameObject btnComenzar;
    public GameObject btnFinalizar;
    public GameObject btnGuardando;
    public GameObject btnGuardado;
    public GameObject btnVolverEmpezar;

    [Header("Configuración")]
    public float litrosPorMinuto = 9.5f;

    private float tiempoTranscurrido;
    private bool estaContando = false;

    private Color colorVerde = new Color32(52, 201, 70, 255);
    private Color colorAmarillo = Color.yellow;
    private Color colorRojo = Color.red;

    void Start() { PrepararEscena(); }

    void Update()
    {
        if (estaContando)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarInterfazUI();
            ManejarTransicionDeColor();
        }
    }

    public void GuardarTiempoFinal()
    {
        if (tiempoTranscurrido > 0)
        {
            Debug.Log("Guardando tiempo final de la ducha...");
            GuardarEnHistorialInterno();
        }
    }

    public void ComenzarDucha()
    {
        estaContando = true;
        SetEstadoBotones(false, true, false, false, false);
    }

    public void FinalizarDucha()
    {
        estaContando = false;
        StartCoroutine(SecuenciaGuardadoAutomatico());
    }

    IEnumerator SecuenciaGuardadoAutomatico()
    {
        SetEstadoBotones(false, false, true, false, false);
        yield return new WaitForSeconds(1.5f);
        GuardarEnHistorialInterno();
        SetEstadoBotones(false, false, false, true, false);
        yield return new WaitForSeconds(1.5f);
        SetEstadoBotones(false, false, false, false, true);
    }

    public void ReiniciarTodo()
    {
        tiempoTranscurrido = 0f;
        ActualizarInterfazUI();
        textoCronometro.color = colorVerde;
        SetEstadoBotones(true, false, false, false, false);
    }

    void GuardarEnHistorialInterno()
    {
        string nombreUsuario = ManejadorRegistro.instance != null ? ManejadorRegistro.instance.nombreSeleccionado : "Invitado";

        if (ManejadorRegistro.instance != null)
        {
            foreach (var miembro in ManejadorRegistro.instance.listaDeMiembros)
            {
                if (miembro.nombre == nombreUsuario)
                {
                    if (miembro.mejorTiempo <= 0 || tiempoTranscurrido < miembro.mejorTiempo)
                    {
                        miembro.mejorTiempo = tiempoTranscurrido;
                    }

                    ManejadorRegistro.RegistroBano nuevoRegistro = new ManejadorRegistro.RegistroBano();
                    nuevoRegistro.duracion = tiempoTranscurrido;
                    nuevoRegistro.fecha = System.DateTime.Now.ToString("dd/MM/yyyy");
                    nuevoRegistro.hora = System.DateTime.Now.ToString("hh:mm tt");

                    miembro.historialBanos.Insert(0, nuevoRegistro);

                    if (miembro.historialBanos.Count > 5)
                    {
                        miembro.historialBanos.RemoveRange(5, miembro.historialBanos.Count - 5);
                    }
                    break;
                }
            }
            ManejadorRegistro.instance.GuardarEnDisco();
        }
    }

    void SetEstadoBotones(bool com, bool fin, bool gndo, bool gdo, bool vol)
    {
        if (btnComenzar) btnComenzar.SetActive(com);
        if (btnFinalizar) btnFinalizar.SetActive(fin);
        if (btnGuardando) btnGuardando.SetActive(gndo);
        if (btnGuardado) btnGuardado.SetActive(gdo);
        if (btnVolverEmpezar) btnVolverEmpezar.SetActive(vol);
    }

    void ActualizarInterfazUI()
    {
        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);
        textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        float litrosTotales = (tiempoTranscurrido / 60f) * litrosPorMinuto;
        textoConsumo.text = litrosTotales.ToString("F1") + " L";
    }

    void ManejarTransicionDeColor()
    {
        float min = tiempoTranscurrido / 60f;
        if (min < 5f) textoCronometro.color = colorVerde;
        else if (min < 8f) textoCronometro.color = Color.Lerp(colorVerde, colorAmarillo, (min - 5f) / 3f);
        else textoCronometro.color = Color.Lerp(colorAmarillo, colorRojo, (min - 8f) / 2f);
    }

    void PrepararEscena()
    {
        tiempoTranscurrido = 0f;
        ActualizarInterfazUI();
        textoCronometro.color = colorVerde;
        SetEstadoBotones(true, false, false, false, false);
    }
}