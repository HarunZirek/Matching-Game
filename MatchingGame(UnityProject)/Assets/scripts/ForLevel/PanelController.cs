using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public Animator PanelAnim;
    public Animator preGame;

    private IEnumerator startLevelDelay()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
   public void showDone() //UI animasyonlarinin calismasi
    {
      if(PanelAnim!=null && preGame!=null)
        {           
            PanelAnim.SetBool("isDone", true);
            preGame.SetBool("isFading", true);
            StartCoroutine(startLevelDelay());
        }
    }
}
