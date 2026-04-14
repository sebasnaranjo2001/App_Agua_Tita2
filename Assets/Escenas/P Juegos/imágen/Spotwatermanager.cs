using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpotWaterManager : MonoBehaviour
{
    public Image mainImage;
    public Sprite[] levelImages;

    public TMP_Text scoreText;
    public GameObject finalPanel;
    public Image feedbackRed;

    public GameObject[] level1Errors;
    public GameObject[] level2Errors;
    public GameObject[] level3Errors;

    int currentLevel = 0;
    int found = 0;
    int totalErrors = 2;

    void Start()
    {
        finalPanel.SetActive(false);
        LoadLevel(0);
    }

    void LoadLevel(int level)
    {
        currentLevel = level;
        found = 0;

        GameObject[][] levels =
        {
            level1Errors,
            level2Errors,
            level3Errors
        };

        foreach (GameObject[] lvl in levels)
        {
            foreach (GameObject obj in lvl)
                obj.SetActive(false);
        }

        foreach (GameObject obj in levels[level])
        {
            obj.SetActive(true);

            obj.GetComponent<Button>().interactable = true;

            obj.GetComponent<Image>().color =
                new Color(1, 1, 1, 0);
        }

        mainImage.sprite = levelImages[level];

        UpdateCounter();
    }

    public void CorrectClick(Button btn)
    {
        btn.interactable = false;

        btn.GetComponent<Image>().color =
            new Color(0, 1, 0, 0.5f);

        found++;

        UpdateCounter();

        if (found >= totalErrors)
        {
            Invoke("NextLevel", 1f);
        }
    }

    public void WrongClick()
    {
        feedbackRed.gameObject.SetActive(true);
        CancelInvoke("HideRed");
        Invoke("HideRed", 0.5f);
    }

    void HideRed()
    {
        feedbackRed.gameObject.SetActive(false);
    }

    void NextLevel()
    {
        if (currentLevel == 0)
            LoadLevel(1);

        else if (currentLevel == 1)
            LoadLevel(2);

        else
            finalPanel.SetActive(true);
    }

    void UpdateCounter()
    {
        scoreText.text = "Errores: " + found + "/2";
    }
}