using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelSelectButton : MonoBehaviour
{
    private GameObject saveManager;

    public int SceneToOpen;
    public int levelNumber;
    private Button isOpen;
    public Button buttonsToHide;
    public GameObject preLevelMenu;
    public Text scoreText;
    public int score;
      
    private void Start()
    {
        saveManager = GameObject.Find("SaveController");
        isOpen = GetComponent<Button>();
        isOpen.interactable = false;
    }

    private void Update() //bölümlerin acik olup olmadigini kontrol ediyor (sahnede gecen her framede calisiyor)
    {
        if (levelNumber == 1 || levelNumber==10)
        {
            isOpen.interactable = true;
            return;
        }
        else
        {
            if (saveManager.GetComponent<saveController>().levels[levelNumber - 2] == true)
            {
                isOpen.interactable = true;
            }
        }
       
       
    }
    public void preMenuControlOpen() //secilen levelin UI'ini aciyor
    {
        if(levelNumber!=10)
        {
            score = saveManager.GetComponent<saveController>().scores[levelNumber - 1];
        }
        else
        {
            score = 0;
        }
        buttonsToHide.interactable = false;
        preLevelMenu.SetActive(true);
        scoreText.text = score.ToString();
    }
    public void preMenuControlClose() //secilen levelin UI'ini aciyor
    {
        preLevelMenu.SetActive(false);
        buttonsToHide.interactable = true;
    }
    public void openLevel()
    {     
        StartCoroutine(LoadLevel(SceneToOpen));
    }

    public void goBackMainMenu()
    {
        StartCoroutine(LoadLevel(0));
    }
    IEnumerator LoadLevel(int index)
    {      
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(index);
    }
}
