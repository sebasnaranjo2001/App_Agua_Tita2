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

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

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

        // =========================
        // SI ENCONTRÓ LOS 2
        // =========================

        if (found >= totalErrors)
        {
            // SUMAR ACIERTO
            aciertos++;

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
        // Si todavía hay niveles
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
        // =========================
        // OCULTAR GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

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
        // TEXTO FINAL
        // =========================

        string resultadoFinal =
            "Aciertos: " +
            aciertos +
            "/" +
            levels.Length;

        // =========================
        // VICTORIA
        // 5/5
        // =========================

        if (aciertos == levels.Length)
        {
            if (panelVictoria != null)
                panelVictoria.SetActive(true);

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text =
                    resultadoFinal;
        }

        // =========================
        // DERROTA
        // 0 o 1
        // =========================

        else if (aciertos <= 1)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text =
                    resultadoFinal;
        }

        // =========================
        // INTERMEDIO
        // 2,3,4
        // =========================

        else
        {
            if (panelIntermedio != null)
                panelIntermedio.SetActive(true);

            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text =
                    resultadoFinal;
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
}