using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> matches = new List<GameObject>();
    
    void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void findAllMatches()
    {
        StartCoroutine(findMatches());
    }
    private void addToList(GameObject dot)
    {
        if (!matches.Contains(dot))
        {
            matches.Add(dot);          
        }
        dot.GetComponent<Dots>().isMatched = true;
    } //1.1
    private void fillTHeList(GameObject dot1,GameObject dot2,GameObject dot3)
    {
        addToList(dot1);
        addToList(dot2);
        addToList(dot3);        
    }   //1.2
    private IEnumerator findMatches() //butun matriste gezerek her bir noktanin (alt ve ustunun) ve (sag ve solunun) eslesme kontrolu 
    {
        yield return new WaitForSeconds(.2f);
        for(int i=0;i<board.width;i++)
        {
            for(int j=0;j<board.height;j++)
            {
                GameObject dot = board.allDots[i, j];
                
                if(dot!=null)
                {
                    Dots dotDot = dot.GetComponent<Dots>();
                    if (i>0 && i<board.width-1)   //x ekseninde sag ve sol
                    {
                        GameObject rightDot = board.allDots[i + 1, j];                     
                        GameObject leftDot = board.allDots[i - 1, j];
                        if(leftDot!=null & rightDot!=null)
                        {
                            if (leftDot.tag == dot.tag && rightDot.tag == dot.tag)
                            {
                               if(rightDot!=null && leftDot!=null)
                                {
                                    Dots rightDotDot = rightDot.GetComponent<Dots>();  //oyun objesi olarak atayinca null reference hatasi aliyordum. ikinci bir dot nesnesi atamasi gerekti.
                                    Dots leftDotDot = leftDot.GetComponent<Dots>();
                                    matches.Union(isRowBomb(leftDotDot, rightDotDot, dotDot));
                                    matches.Union(isColumnBomb(leftDotDot, rightDotDot, dotDot));
                                    matches.Union(isBigBomb(leftDotDot, rightDotDot, dotDot));
                                    fillTHeList(leftDot, rightDot, dot);
                                }
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {                       
                        GameObject DownDot = board.allDots[i, j-1];                      
                        GameObject upDot = board.allDots[i, j+1];
                        if (upDot != null & DownDot != null) //y ekseninde
                        {
                            if (upDot.tag == dot.tag && DownDot.tag == dot.tag)
                            {
                                if(upDot!=null && DownDot!=null)
                                {
                                    Dots upDotDot = upDot.GetComponent<Dots>();
                                    Dots downDotDot = DownDot.GetComponent<Dots>();
                                    matches.Union(isColumnBomb(upDotDot, downDotDot, dotDot));
                                    matches.Union(isRowBomb(upDotDot, downDotDot, dotDot));
                                    matches.Union(isBigBomb(upDotDot, downDotDot, dotDot));
                                    fillTHeList(upDot, DownDot, dot);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void FindSameColor(string color) //colorBomb un eslestigi renkleri oyun alaninda bulan kod parcasi
    {
        for(int i=0;i<board.width;i++)
        {
            for(int j=0;j<board.height;j++)
            {
                if(board.allDots[i,j]!=null)
                {
                    if(board.allDots[i,j].tag==color)
                    {
                        Dots dot = board.allDots[i, j].GetComponent<Dots>();    
                        if(dot.isRowBomb==true)
                        {
                            for (int x = 0; x < board.width; x++)
                            {
                                if (board.allDots[x, j] != null)
                                {                                  
                                    board.allDots[x,j].GetComponent<Dots>().isMatched = true;
                                }
                            }
                        }
                        else if (dot.isColumnBomb == true)
                        {
                            for (int y = 0; y < board.height; y++)
                            {
                                if (board.allDots[i, y] != null)
                                {
                                    board.allDots[i, y].GetComponent<Dots>().isMatched = true;
                                }
                            }
                        }
                        else if (dot.isBigBomb == true)
                        {
                            for (int x = i-1; x <= i+1; x++)
                            {
                                for (int y = j - 1; y <= j + 1; y++)
                                {
                                    board.allDots[x, y].GetComponent<Dots>().isMatched = true;
                                }
                            }
                        }
                        dot.isMatched = true;
                    }
                }
            }
        }
    }


    //eslestirme kontrolu aninda powerUp larin kontrolu ve listeye eklenmesi
    private List<GameObject> isBigBomb(Dots dot1, Dots dot2, Dots dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isBigBomb)
        {
            matches.Union(bigBomb(dot1.column,dot1.row));
        }
        if (dot2.isBigBomb)
        {
            matches.Union(bigBomb(dot2.column,dot2.row));
        }
        if (dot3.isBigBomb)
        {
            matches.Union(bigBomb(dot3.column,dot3.row));
        }
        return currentDots;
    }
    private List<GameObject> isColumnBomb(Dots dot1, Dots dot2, Dots dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            matches.Union(columnBomb(dot1.column));
        }
        if (dot2.isColumnBomb)
        {
            matches.Union(columnBomb(dot2.column));
        }
        if (dot3.isColumnBomb)
        {
            matches.Union(columnBomb(dot3.column));
        }
        return currentDots;
    }
    private List<GameObject> isRowBomb(Dots dot1, Dots dot2, Dots dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            matches.Union(RowBomb(dot1.row));
        }
        if (dot2.isRowBomb)
        {
            matches.Union(RowBomb(dot2.row));
        }
        if (dot3.isRowBomb)
        {
            matches.Union(RowBomb(dot3.row));
        }
        return currentDots;
    }



    //bombalarin islevleri ve diger bombalari tetikleme fonksiyonlari
    private List<GameObject> bigBomb(int column,int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i=column-1;i<=column+1;i++)
        {
            for (int j = row - 1; j <= row+ 1; j++)
            {
                if(i>=0 && i< board.width && j>=0 && j<board.height)
                {
                    if(board.allDots[i,j]!=null)
                    {
                        Dots dot = board.allDots[i, j].GetComponent<Dots>();
                        if (dot.isRowBomb == true)
                        {
                            for (int x = 0; x < board.width; x++)
                            {
                                if (board.allDots[x, j] != null)
                                {
                                    Dots forRowBomb = board.allDots[x, j].GetComponent<Dots>();                                    
                                    dots.Add(board.allDots[x, j]);
                                    forRowBomb.isMatched = true;
                                }
                            }
                        }
                        else if (dot.isColumnBomb == true)
                        {
                            for (int y = 0; y < board.height; y++)
                            {
                                if (board.allDots[i, y] != null)
                                {
                                    Dots forColumnBomb = board.allDots[i, y].GetComponent<Dots>();
                                    dots.Add(board.allDots[i, y]);
                                    forColumnBomb.isMatched = true;
                                }
                            }
                        }
                        else if(dot.isColorBomb==true)
                        {
                            FindSameColor(board.allDots[column, row].tag);
                        }
                        dots.Add(board.allDots[i, j]);
                        dot.isMatched = true;
                    }
                }
            }
        }

        return dots;
    }
    private List<GameObject> columnBomb(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i=0;i<board.height;i++)
        {
            if(board.allDots[column,i]!=null)
            {
                Dots dot = board.allDots[column, i].GetComponent<Dots>();
                if (dot.isRowBomb == true)
                {
                    dots.Union(RowBomb(i)).ToList();
                }
                else if (dot.isBigBomb == true)
                {
                    dots.Union(bigBomb(column, i)).ToList();
                }
                else if (dot.isColorBomb == true)
                {
                    FindSameColor(board.allDots[column, 0].tag);
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }
    private List<GameObject> RowBomb(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dots dot = board.allDots[i, row].GetComponent<Dots>();

                if (dot.isColumnBomb == true)
                {
                    dots.Union(columnBomb(i)).ToList();
                }
                else if (dot.isBigBomb == true)
                {
                    dots.Union(bigBomb(i,row)).ToList();
                }
                else if (dot.isColorBomb == true)
                {
                    FindSameColor(board.allDots[0, row].tag);
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }


    public void CheckBombs() //row ve column bombun mouse acisina gore olusturulmasi
    {
        if(board.currentDot!=null)
        {
            if(board.currentDot.isMatched==true)
            {
                board.currentDot.isMatched = false;
              
                if((board.currentDot.angle > -45 && board.currentDot.angle <= 45) || (board.currentDot.angle > 135 && board.currentDot.angle <= 225) )
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            else if(board.currentDot.otherDot!=null)
            {
                Dots otherDot = board.currentDot.otherDot.GetComponent<Dots>();
                if(otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                   
                    if ((board.currentDot.angle > -45 && board.currentDot.angle <= 45) || (board.currentDot.angle > 135 && board.currentDot.angle <= 225))
                    {
                       otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
