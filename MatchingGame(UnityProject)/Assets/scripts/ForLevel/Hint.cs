using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    private Board board;
    public float delay;
    private float delaySeconds;
    public GameObject hintParticle;
    public GameObject hint;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        delaySeconds = delay;
    }
    private void Update() //hint vermeden onceki geri sayim (sahnede gecen her framede calisiyor)
    {
       if(board.currentState==GameState.move)
        {
            delaySeconds -= Time.deltaTime;
            if (delaySeconds <= 0 && hint == null)
            {
                Showhint();
                delaySeconds = delay;
            }
        }
       else
        {
            delaySeconds = delay;
        }
    }

    private List<GameObject> currentMatches() //oyun sahasindaki olasi eslestirmeleri bulup listeliyor
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.switchAndMatchForLock(i, j, Vector2.right) == true)
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.switchAndMatchForLock(i, j, Vector2.up) == true)
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    private GameObject pickMove() //olasi eslestirmelerden birinin secimi
    {
        List<GameObject> moves = new List<GameObject>();
        moves = currentMatches();
        if(moves.Count>0)
        {
            int hintToUse = Random.Range(0, moves.Count);
            return moves[hintToUse];
        }
        return null;
    }
    private void Showhint() //hintin olusturulmasi
    {
        GameObject TempHint = pickMove();
        if (TempHint != null)
        {
            hint = Instantiate(hintParticle, TempHint.transform.position, Quaternion.identity);
        }
    }
    public void destroyHint()
    {
        if(hint!=null)
        {
            Destroy(hint);
            hint = null;
            delaySeconds = delay;
        }
    }
}
