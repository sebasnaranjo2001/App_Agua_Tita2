using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SpotWaterManager : MonoBehaviour
{
    public Image mainImage;
    public Sprite[] levelImages;

    public TMP_Text scoreText;
    public GameObject finalPanel;
    public Image feedbackRed;

    // Errores de cada nivel
    public GameObject[] level1Errors;
    public GameObject[] level2Errors;
    public GameObject[] level3Errors;
    public GameObject[] level4Errors;
    public GameObject[] level5Errors;

    int currentLevel = 0;
    int found = 0;
    int totalErrors = 2;

    // Todos los niveles
    GameObject[][] levels;

    // Orden aleatorio
    int[] randomOrder;

    void Start()
    {
        if (finalPanel != null)
            finalPanel.SetActive(false);

        // Inicializar niveles
        levels = new GameObject[][]
        {
            level1Errors,
            level2Errors,
            level3Errors,
            level4Errors,
            level5Errors
        };

        // Crear orden aleatorio
        randomOrder = new int[levels.Length];

        for (int i = 0; i < randomOrder.Length; i++)
        {
            randomOrder[i] = i;
        }

        ShuffleLevels();

        LoadLevel(0);
    }

    void ShuffleLevels()
    {
        for (int i = 0; i < randomOrder.Length; i++)
        {
            int randomIndex = Random.Range(i, randomOrder.Length);

            int temp = randomOrder[i];
            randomOrder[i] = randomOrder[randomIndex];
            randomOrder[randomIndex] = temp;
        }
    }

    void LoadLevel(int level)
    {
        currentLevel = level;
        found = 0;

        // Nivel real aleatorio
        int realLevel = randomOrder[level];

        // Desactivar todos los errores
        foreach (GameObject[] lvl in levels)
        {
            foreach (GameObject obj in lvl)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // Activar errores del nivel actual
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

        // Cambiar imagen principal
        if (mainImage != null && realLevel < levelImages.Length)
            mainImage.sprite = levelImages[realLevel];

        UpdateCounter();
    }

    public void CorrectClick(Button btn)
    {
        btn.interactable = false;

        Image img = btn.GetComponent<Image>();

        if (img != null)
            img.color = new Color(0, 1, 0, 0.5f);

        found++;

        UpdateCounter();

        // Pasar al siguiente nivel
        if (found >= totalErrors)
        {
            Invoke(nameof(NextLevel), 1f);
        }
    }

    public void WrongClick()
    {
        if (feedbackRed != null)
            feedbackRed.gameObject.SetActive(true);

        CancelInvoke(nameof(HideRed));
        Invoke(nameof(HideRed), 0.5f);
    }

    void HideRed()
    {
        if (feedbackRed != null)
            feedbackRed.gameObject.SetActive(false);
    }

    void NextLevel()
    {
        // Si todavía hay niveles
        if (currentLevel < levels.Length - 1)
        {
            LoadLevel(currentLevel + 1);
        }
        else
        {
            // Juego terminado
            if (finalPanel != null)
                finalPanel.SetActive(true);
        }
    }

    void UpdateCounter()
    {
        if (scoreText != null)
            scoreText.text = "Errores: " + found + "/" + totalErrors;
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}