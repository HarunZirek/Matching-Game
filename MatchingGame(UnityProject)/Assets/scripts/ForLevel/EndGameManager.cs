using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum variables
{
    move,
    //time
}

[System.Serializable]
public class endingReq
{
    public variables gameType;
    public int ValueCount;
}
public class EndGameManager : MonoBehaviour
{
    public int numberOfLevel;
    public endingReq requirements;
    public Text moveCountText;
    private int moveCount;
    private Board board;
    private saveController save;
    public GameObject winPanel;
    public GameObject losePanel;
    public bool Lwin;
    public bool Llose;
    void Start()
    {
        save = FindObjectOfType<saveController>();
        board = FindObjectOfType<Board>();
        setUp();
    }

    private void Update()  //kazanma ve kayıp durumunun kontrolu (sahnede gecen her framede calisiyor)
    {
        if(numberOfLevel!=10)
        {
            if (Lwin == true && Llose == false)
            {
                board.currentState = GameState.win;
            }
            if (Lwin == false && Llose == true)
            {
                board.currentState = GameState.lose;
            }
        }
      
    }
    void setUp() //kalan hamle sayisinin UI atamasi
    {
        moveCount = requirements.ValueCount;
        moveCountText.text = "" + moveCount; 
    }
    public void decreaseMoveCount() 
    {      
        moveCount--;
        moveCountText.text = "" + moveCount;
        if(moveCount<=0)
        {
            StartCoroutine(endGameDelay());
        }
    }
    public IEnumerator endGameDelay() //son hamlede oyun kazanmasina engel olan hata icin delay
    {
        yield return new WaitForSeconds(2f);
        if (Lwin==false)
        {
            StartCoroutine(LevelFailed());
        }
       
    }
    public IEnumerator LevelComplete() //kazanma UI ve sesinin tetiklenmesi
    {
        if(numberOfLevel!=10)
        {
            board.currentState = GameState.win;
            yield return new WaitForSeconds(2f);
            FindObjectOfType<audioManager>().Play("win");
            winPanel.SetActive(true);
            save.GetComponent<saveController>().levels[numberOfLevel - 1] = true;
            if (board.HighScore >= save.GetComponent<saveController>().scores[numberOfLevel - 1])
            {
                save.GetComponent<saveController>().scores[numberOfLevel - 1] = board.HighScore;
            }
            save.GetComponent<saveController>().SaveGame();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            board.currentState = GameState.move;
        }
    }
    public IEnumerator LevelFailed() //kaybetme UI ve sesinin tetiklenmesi
    {        
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<audioManager>().Play("lose");
        losePanel.SetActive(true);
        Llose = true;
        board.currentState = GameState.lose;
    }
   
}
