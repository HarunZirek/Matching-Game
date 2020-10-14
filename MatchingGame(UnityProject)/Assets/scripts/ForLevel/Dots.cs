using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dots : MonoBehaviour
{
    public int row;
    public int column;
    public int targetX;
    public int targetY;
    public int preColumn;
    public int preRow;
    private Board board;
    public GameObject otherDot;
    private FindMatches find;
    private EndGameManager end;
    private Hint hint;
    private Vector2 firstTouch;
    private Vector2 finalTouch;
    private Vector2 tempPos;
    public float angle = 0;
    public float swipeResistOnClick = 1f;
    public bool isMatched = false;


    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    public bool isBigBomb;
    public GameObject colorBomb;
    public GameObject columnBomb;
    public GameObject rowBomb;
    public GameObject Bigbomb;
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isBigBomb = false;
        board = FindObjectOfType<Board>();
        find = FindObjectOfType<FindMatches>();
        hint = FindObjectOfType<Hint>();
        end = FindObjectOfType<EndGameManager>();
    }

     private void OnMouseOver() //test levelinde kullanilacak bombalarin olusturulması ve tus atamasi (hile)
     {
        if(end.numberOfLevel==10)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                MakeColorBomb();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MakeBigBomb();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MakeColumnBomb();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MakeRowBomb();
            }
        }
    }
    void Update() //(sahnede gecen her framede calisiyor)
    {
        targetX = column;
        targetY = row;       

        movePieces();     
    }
    private void OnMouseDown() //mouse ilk tiklandiginda koordinatini tutuyor
    {
       if(hint!=null)
        {
            hint.destroyHint();
        }
        if(board.currentState==GameState.move)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }     
    }
    private void OnMouseUp() //mouse kaldirildiginda koordinati aliyor
    {
       if(board.currentState==GameState.move)
        {
            finalTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }    //1
    public void CalculateAngle() //mouse hareketi bittiginde koordinatlar uzerinde kotanjant uygulayarak cizilen aciyi hesaplama
    {
        if(Mathf.Abs(finalTouch.y-firstTouch.y)>swipeResistOnClick || Mathf.Abs(finalTouch.x-firstTouch.x)>swipeResistOnClick) //sadece tiklamayla kotanjant 0 saydigi icin sinirlama 
        {
            angle = Mathf.Atan2(finalTouch.y - firstTouch.y, finalTouch.x - firstTouch.x) * 180 / Mathf.PI;
            moveCoordinates();
            if(otherDot!=null)
            {
                board.currentState = GameState.wait;
            }
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    } //2
    public IEnumerator CheckMatchOnMove() //hareket halinde ne olacaginin hesaplanmasi
    {
        if(isColorBomb==true)
        {
            find.FindSameColor(otherDot.tag);
            isMatched = true;
            otherDot.GetComponent<Dots>().isMatched = true;
        }
        else if( otherDot!=null && otherDot.GetComponent<Dots>().isColorBomb==true)
        {
            find.FindSameColor(this.gameObject.tag);
            otherDot.GetComponent<Dots>().isMatched = true;
            isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(otherDot!=null)
        {
            if(isMatched==false && otherDot.GetComponent<Dots>().isMatched==false) //eslesme yoksa eski yerine donmesi
            {
                otherDot.GetComponent<Dots>().row = row;
                otherDot.GetComponent<Dots>().column = column;
                column = preColumn;
                row = preRow;
                yield return new WaitForSeconds(.3f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if(end!=null)
                {
                    if(end.requirements.gameType==variables.move)
                    {
                        end.decreaseMoveCount();
                    }
                }
                board.DestroyMatches();
            }
             
        }       
    }   //4
    public void moveCoordinates() //cizilen aciya gore tiklanan noktanin gidecegi yonu belirleyeme
    {
        if(angle>-45 && angle<=45 &&column<board.width-1 ) //Right swipe
        {          
            swipeControl(Vector2.right);
        }
        else if (angle > 45 && angle <= 135 && row<board.height-1) //Up swipe
        {
            swipeControl(Vector2.up);
        }
        else if (angle > 135 && angle <= 225  && column>0) //Left swipe
        {
            swipeControl(Vector2.left);
        }
        else if (angle < -45 && angle >= -135 && row>0) //Down swipe
        {
            swipeControl(Vector2.down);
        }
        StartCoroutine(CheckMatchOnMove());
    }  //3
    public void swipeControl(Vector2 direction) //tiklanan nokta ve yone gore yer degistirecegi noktanin bulunmasi
    {
        otherDot = board.allDots[column + (int)direction.x, row+(int)direction.y];
        preColumn = column;
        preRow = row;
       if(otherDot!=null)
        {
            otherDot.GetComponent<Dots>().column += (-1 * (int)direction.x);
            otherDot.GetComponent<Dots>().row += (-1 * (int)direction.y);
            column += (int)direction.x;
            row += (int)direction.y;
        }
       else
        {
            board.currentState = GameState.move;
        }
    } //3.1
    public void movePieces() //parcalarin yer degistirmesi ve eslestirme kontrolu
    {
        if (Mathf.Abs(targetX - transform.position.x) > .1)  //x ekseni
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if(board.allDots[column,row]!=this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            find.findAllMatches();
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;         
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)  //y ekseni
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            find.findAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }


    //bomba tiplerinin olusturulmasi
    public void MakeRowBomb()
    {
       if(isColorBomb==false && isColumnBomb==false && isBigBomb==false)
        {
            isRowBomb = true;
            GameObject bomb = Instantiate(rowBomb, transform.position, Quaternion.identity);
            bomb.transform.parent = this.transform;
        }
    }
    public void MakeColumnBomb()
    {
        if (isColorBomb == false && isRowBomb == false && isBigBomb == false)
        {
            isColumnBomb = true;
            GameObject bomb = Instantiate(columnBomb, transform.position, Quaternion.identity);
            bomb.transform.parent = this.transform;
        }          
    }
    public void MakeColorBomb()
    {
        if (isColumnBomb == false && isRowBomb == false && isBigBomb == false)
        {
            isColorBomb = true;
            GameObject bomb = Instantiate(colorBomb, transform.position, Quaternion.identity);
            bomb.transform.parent = this.transform;
            this.gameObject.tag = "ColorBomb";
        }
    }
    public void MakeBigBomb()
    {
        if (isColumnBomb == false && isRowBomb == false && isColorBomb == false)
        {
            isBigBomb = true;
            GameObject bomb = Instantiate(Bigbomb, transform.position, Quaternion.identity);
            bomb.transform.parent = this.transform;
        }
    }






































    /* public void findMatches()                
     {
         if(column>0 &&column<board.width-1)
         {
             GameObject leftDot = board.allDots[column - 1, row];
             GameObject rightDot = board.allDots[column + 1, row];
            if(leftDot!=null && rightDot!=null)
             {
                 if (leftDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag)
                 {
                     leftDot.GetComponent<Dots>().isMatched = true;
                     rightDot.GetComponent<Dots>().isMatched = true;
                     isMatched = true;
                 }
             }
         }
         if (row > 0 && row < board.height - 1)
         {
             GameObject upDot = board.allDots[column, row + 1];
             GameObject downDot = board.allDots[column, row - 1];
             if(upDot!=null && downDot!=null)
             {
                 if (upDot.tag == this.gameObject.tag && downDot.tag == this.gameObject.tag)
                 {
                     upDot.GetComponent<Dots>().isMatched = true;
                     downDot.GetComponent<Dots>().isMatched = true;
                     isMatched = true;
                 }
             }
         }
     }*/
}
