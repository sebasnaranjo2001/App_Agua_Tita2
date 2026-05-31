using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SpotWaterManager : MonoBehaviour
{
    [Header("Imagen Principal")]
    public Image mainImage;
    public Sprite[] levelImages;

    [Header("Texto Gameplay")]
    public TMP_Text scoreText;

    [Header("Animaciones UI")]
    public TMP_Text tituloJuego;
    public TMP_Text categoriaJuego;
    public GameObject fondoCategoria;
    public GameObject fondoContador;

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

    [Header("Gotas Victoria")]
    public GameObject gotaVictoria1;
    public GameObject gotaVictoria2;
    public GameObject gotaVictoria3;

    [Header("Gotas Intermedio")]
    public GameObject gotaIntermedio1;
    public GameObject gotaIntermedio2;
    public GameObject gotaIntermedio3;

    [Header("Gotas Derrota")]
    public GameObject gotaDerrota1;
    public GameObject gotaDerrota2;
    public GameObject gotaDerrota3;

    [Header("Elementos Victoria")]
    public GameObject[] elementosVictoria;

    [Header("Elementos Intermedio")]
    public GameObject[] elementosIntermedio;

    [Header("Elementos Derrota")]
    public GameObject[] elementosDerrota;

    [Header("Textos de Aciertos por Panel")]
    public TMP_Text textoAciertosVictoria;
    public TMP_Text textoAciertosIntermedio;
    public TMP_Text textoAciertosDerrota;

    [Header("Elementos Gameplay")]
    // PON TODO EL GAMEPLAY MENOS:
    // fondo y panels finales
    public GameObject[] elementosGameplay;

    [Header("Feedback")]
    public Image feedbackRed;

    // =========================
    // ERRORES POR NIVEL
    // =========================

    public GameObject[] level1Errors;
    public GameObject[] level2Errors;
    public GameObject[] level3Errors;
    public GameObject[] level4Errors;
    public GameObject[] level5Errors;

    // =========================
    // VARIABLES
    // =========================

    int currentLevel = 0;

    // encontrados en el nivel actual
    int found = 0;

    // errores por imagen
    int totalErrors = 2;

    // aciertos reales del juego
    // SOLO suma si encontró los 2 errores
    int aciertos = 0;
    bool resultadoMostrado = false;

    private Vector2 posicionOriginalTitulo;
    

    // Todos los niveles
    GameObject[][] levels;

    // Orden aleatorio ya implementado
    int[] randomOrder;

    void Start()
    {
        // =========================
        // DESACTIVAR PANELES
        // =========================

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // =========================
        // MOSTRAR GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // =========================
        // ANIMACIONES INICIALES
        // =========================

        if (tituloJuego != null)
        {
            posicionOriginalTitulo =
                tituloJuego.rectTransform.anchoredPosition;

            tituloJuego.rectTransform.anchoredPosition =
                new Vector2(
                    posicionOriginalTitulo.x,
                    posicionOriginalTitulo.y + 250f);

            LeanTween.move(
                tituloJuego.rectTransform,
                posicionOriginalTitulo,
                0.6f).setEaseOutBack();
        }

        if (fondoCategoria != null)
        {
            fondoCategoria.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                fondoCategoria,
                Vector3.one,
                0.6f)
                .setDelay(0.15f)
                .setEaseOutBack();
        }

        if (fondoContador != null)
        {
            fondoContador.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                fondoContador,
                Vector3.one,
                0.6f)
                .setDelay(0.3f)
                .setEaseOutBack();
        }

        // =========================
        // INICIALIZAR NIVELES
        // =========================

        levels = new GameObject[][]
        {
            level1Errors,
            level2Errors,
            level3Errors,
            level4Errors,
            level5Errors
        };

        // =========================
        // ORDEN ALEATORIO
        // =========================

        randomOrder = new int[levels.Length];

        for (int i = 0; i < randomOrder.Length; i++)
        {
            randomOrder[i] = i;
        }

        ShuffleLevels();

        LoadLevel(0);
    }

    // =========================
    // MEZCLAR NIVELES
    // =========================
    void ShuffleLevels()
    {
        for (int i = 0; i < randomOrder.Length; i++)
        {
            int randomIndex =
                Random.Range(i, randomOrder.Length);

            int temp = randomOrder[i];
            randomOrder[i] = randomOrder[randomIndex];
            randomOrder[randomIndex] = temp;
        }
    }

    // =========================
    // CARGAR NIVEL
    // =========================
    void LoadLevel(int level)
    {
        currentLevel = level;

        // Reiniciar encontrados
        found = 0;

        // Nivel real aleatorio
        int realLevel = randomOrder[level];

        // =========================
        // DESACTIVAR TODOS
        // =========================

        foreach (GameObject[] lvl in levels)
        {
            foreach (GameObject obj in lvl)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // =========================
        // ACTIVAR NIVEL ACTUAL
        // =========================

        foreach (GameObject obj in levels[realLevel])
        {
            if (obj != null)
            {
                obj.SetActive(true);

                Button btn = obj.GetComponent<Button>();
                Image img = obj.GetComponent<Image>();

                if (btn != null)
                    btn.interactable = true;

                if (img != null)
                    img.color = new Color(1, 1, 1, 0);
            }
        }

        // =========================
        // CAMBIAR IMAGEN
        // =========================

        if (mainImage != null &&
            realLevel < levelImages.Length)
        {
            mainImage.sprite =
    levelImages[realLevel];

            mainImage.transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                mainImage.gameObject,
                Vector3.one,
                0.45f)
                .setEaseOutBack();
        }

        UpdateCounter();
    }

    // =========================
    // CLICK CORRECTO
    // =========================
    public void CorrectClick(Button btn)
    {
        btn.interactable = false;

        Image img = btn.GetComponent<Image>();

        if (img != null)
            img.color = new Color(0, 1, 0, 0.5f);

        found++;

        UpdateCounter();

        if (found >= totalErrors)
        {
            aciertos++;

            CancelInvoke(nameof(NextLevel));
            Invoke(nameof(NextLevel), 1f);
        }
    }

    // =========================
    // CLICK INCORRECTO
    // =========================
    public void WrongClick()
    {
        if (feedbackRed != null)
            feedbackRed.gameObject.SetActive(true);

        CancelInvoke(nameof(HideRed));
        Invoke(nameof(HideRed), 0.5f);
    }

    // =========================
    // OCULTAR FEEDBACK
    // =========================
    void HideRed()
    {
        if (feedbackRed != null)
            feedbackRed.gameObject.SetActive(false);
    }

    // =========================
    // SIGUIENTE NIVEL
    // =========================
    void NextLevel()
    {
        if (currentLevel < levels.Length - 1)
        {
            LoadLevel(currentLevel + 1);
        }
        else
        {
            MostrarResultadoFinal();
        }
    }

    // =========================
    // RESULTADO FINAL
    // =========================

    void MostrarResultadoFinal()
    {
        Debug.Log("MOSTRAR RESULTADO FINAL");

        if (resultadoMostrado)
            return;

        resultadoMostrado = true;

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        string resultadoFinal =
            "Aciertos: " +
            aciertos +
            "/" +
            levels.Length;

        if (aciertos == levels.Length)
        {
            AnimarPanelVictoria();
            Debug.Log("VICTORIA");

            foreach (GameObject obj in elementosVictoria)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;
        }
        else if (aciertos <= 1)
        {
            AnimarPanelDerrota();

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;
        }
        else
        {
            AnimarPanelIntermedio();

            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text = resultadoFinal;
        }
    }
    // =========================
    // CONTADOR GAMEPLAY
    // =========================
    void UpdateCounter()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Errores de ahorro: " +
                found +
                "/" +
                totalErrors;
        }
    }

    // =========================
    // VOLVER
    // =========================
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // =========================
    // REINICIAR
    // =========================
    public void Reiniciar()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void AnimarElemento(GameObject obj, float delay)
    {
        if (obj == null) return;

        obj.SetActive(true);

        obj.transform.localScale = Vector3.zero;

        LeanTween.scale(
            obj,
            Vector3.one,
            0.35f)
            .setDelay(delay)
            .setEaseOutBack();
    }

    void AnimarElementosPanel(GameObject[] elementos)
    {
        if (elementos == null) return;

        for (int i = 0; i < elementos.Length; i++)
        {
            if (elementos[i] == null)
                continue;

            elementos[i].SetActive(true);

            elementos[i].transform.localScale =
                Vector3.zero;

            LeanTween.scale(
                elementos[i],
                Vector3.one,
                0.3f)
                .setDelay(i * 0.1f)
                .setEaseOutBack();
        }
    }

    void AnimarPanelVictoria()
    {
        panelVictoria.SetActive(true);

        foreach (GameObject obj in elementosVictoria)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        gotaVictoria1.SetActive(false);
        gotaVictoria2.SetActive(false);
        gotaVictoria3.SetActive(false);

        panelVictoria.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelVictoria,
            Vector3.one,
            0.5f)
            .setEaseOutBack();

        AnimarElemento(gotaVictoria1, 0.1f);
        AnimarElemento(gotaVictoria2, 0.5f);
        AnimarElemento(gotaVictoria3, 0.9f);

        LeanTween.delayedCall(
            1.4f,
            () =>
            {
                AnimarElementosPanel(elementosVictoria);
            });
    }

    void AnimarPanelIntermedio()
    {
        panelIntermedio.SetActive(true);

        foreach (GameObject obj in elementosIntermedio)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        gotaIntermedio1.SetActive(false);
        gotaIntermedio2.SetActive(false);
        gotaIntermedio3.SetActive(false);

        panelIntermedio.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelIntermedio,
            Vector3.one,
            0.5f)
            .setEaseOutBack();

        AnimarElemento(gotaIntermedio1, 0.1f);
        AnimarElemento(gotaIntermedio2, 0.5f);
        AnimarElemento(gotaIntermedio3, 0.9f);

        LeanTween.delayedCall(
            1.4f,
            () =>
            {
                AnimarElementosPanel(elementosIntermedio);
            });
    }

    void AnimarPanelDerrota()
    {
        panelDerrota.SetActive(true);

        foreach (GameObject obj in elementosDerrota)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        gotaDerrota1.SetActive(false);
        gotaDerrota2.SetActive(false);
        gotaDerrota3.SetActive(false);

        panelDerrota.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelDerrota,
            Vector3.one,
            0.5f)
            .setEaseOutBack();

        AnimarElemento(gotaDerrota1, 0.1f);
        AnimarElemento(gotaDerrota2, 0.5f);
        AnimarElemento(gotaDerrota3, 0.9f);

        LeanTween.delayedCall(
            1.4f,
            () =>
            {
                AnimarElementosPanel(elementosDerrota);
            });
    }
}