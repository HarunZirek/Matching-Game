using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuStuff : MonoBehaviour
{
    private Board board;
    public GameObject pauseMenuUI;

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void resume()
    {
        pauseMenuUI.SetActive(false);
        board.currentState = GameState.move;
    }
    public void pause()
    {
        pauseMenuUI.SetActive(true);
        board.currentState = GameState.pause;
    }
    public void mainmenu()
    {
        StartCoroutine(LoadLevel(0));
    }
    public void restart()
    {
        StartCoroutine(LoadLevel((SceneManager.GetActiveScene().buildIndex)));
    }
    public void levelSelectMenu()
    {
        StartCoroutine(LoadLevel(1));
    }
    public void nextLevel()
    {
        StartCoroutine(LoadLevel((SceneManager.GetActiveScene().buildIndex+1)));
    }
    public void quit()
    {
        Application.Quit();
    }
    
    IEnumerator LoadLevel(int index)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(index);
    }
}
