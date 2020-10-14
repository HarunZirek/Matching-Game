using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    breakable,
    normal,
    blank
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tile;
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSetForSlide;
    public GameObject tilePrefab;
    public GameObject[] dots;

    private bool[,] blankTiles;
    private Tiles[,] breakables;
    public GameObject breakableTile;
    public GameObject[,] allDots;
    public GameObject destroyParticle;
    public Dots currentDot;
    private FindMatches find;
    private GoalManager goals;
    public TileType[] tiles;

    private ScoreSystem score;
    public int HighScore;
    private int scoreForPiece = 10;
    public int comboBonus = 1;
    void Start()
    {
        goals = FindObjectOfType<GoalManager>();
        find = FindObjectOfType<FindMatches>();
        score = FindObjectOfType<ScoreSystem>();
        blankTiles = new bool[width, height];
        breakables = new Tiles[width, height]; 
        allDots = new  GameObject[width, height];
        SetUp();
        currentState = GameState.pause;
    }

    public void DoBlanks()  //matriste bos alanlar bırakarak oyuna gorsel cesitlilik
    {
        for(int i=0;i<tiles.Length;i++)
        {
            if(tiles[i].tile==TileKind.blank)
            {
                blankTiles[tiles[i].x, tiles[i].y] = true;
                allDots[tiles[i].x, tiles[i].y] = null;
            }
        }
    } //5.3
    public void DoBreakables() //matristeki noktaların altına breakable objesini ekliyor ve oyuna yeni bir mekanik
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].tile == TileKind.breakable)
            {
                Vector2 tempPos = new Vector2(tiles[i].x, tiles[i].y);
                GameObject breakableObject = Instantiate(breakableTile, tempPos, Quaternion.identity);
                breakables[tiles[i].x, tiles[i].y] = breakableObject.GetComponent<Tiles>();
            }
        }
    } //5.2
    private void SetUp()
    {
        DoBlanks();
        DoBreakables();
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
              if(blankTiles[i,j]==false)
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    GameObject backGroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backGroundTile.transform.parent = this.transform;
                    backGroundTile.name = "(" + i + "," + j + ")";
                    int dotToUse = Random.Range(0, dots.Length);
                    while (CheckMatchesOnStart(i, j, dots[dotToUse]))
                    {
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dots>().column = i;
                    dot.GetComponent<Dots>().row = j;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    } //5
    private bool CheckMatchesOnStart(int column,int row, GameObject piece)  //bolumun ekranda eslesme ile baslamamasi icin bir noktanin kontrolu
    {
        if(column > 1 && row > 1)
        {
           if(allDots[column-1,row]!=null && allDots[column-2,row]!=null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
           if(allDots[column,row-1]!=null && allDots[column,row-2]!=null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column,row-1]!=null && allDots[column,row-2]!=null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if(allDots[column-1,row]!=null && allDots[column-2,row]!=null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    } //5.1


    private int BombType() //eslemenin sayisina ve sekline gore olusacak bomba cesidini belirliyor
    {
        List<GameObject> matchCopy = find.matches as List<GameObject>;  
        for(int i=0;i<matchCopy.Count;i++)
        {
            Dots thisDot = matchCopy[i].GetComponent<Dots>();
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for(int j=0;j<matchCopy.Count;j++)
            {
                Dots nextDot = matchCopy[j].GetComponent<Dots>();
                if(nextDot==thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.CompareTag(thisDot.tag))
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.CompareTag(thisDot.tag))
                {
                    rowMatch++;
                }
            }

            if(columnMatch==4 || rowMatch==4)
            {
                return 1;   //colorBomb
            }
            if ((columnMatch == 2 || rowMatch == 2) && (matchCopy.Count==5 ||matchCopy.Count==8))
            {
                return 2; //bigbomb
            }
            if((columnMatch == 3 || rowMatch == 3) &&(matchCopy.Count==4 || matchCopy.Count==7))
            {
                return 3;  //row or column bomb
            } 
        }
        return 0;
       
    } //3.1
    private void MakingBombs() //belirlenen bomba cesidine gore bombayi olusturuyor
    {
        if(find.matches.Count>3)
        {
            int theBomb = BombType();

            if(theBomb==1)
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched == true)
                    {
                        if (currentDot.isColorBomb == false)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dots otherDot = currentDot.otherDot.GetComponent<Dots>();
                            if (otherDot.isMatched == true)
                            {
                                if (otherDot.isColorBomb == false)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }  //makeColorBomb
            else if(theBomb==2)
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched == true)
                    {
                        if (currentDot.isBigBomb == false)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeBigBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dots otherDot = currentDot.otherDot.GetComponent<Dots>();
                            if (otherDot.isMatched == true)
                            {
                                if (otherDot.isBigBomb == false)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeBigBomb();
                                }
                            }
                        }
                    }
                }
            } //makeBigBomb
            else if(theBomb==3)
            {
                find.CheckBombs();
            }//make row or colum bomb
        }
       
    } //3.2


    private void DestroyDot(int column, int row)  //eslestirmedeki her bir noktanın uzerinde yapilacak islemin kontrolu
    {
        if(allDots[column,row].GetComponent<Dots>().isMatched==true)
        {
           if(find.matches.Count>=4)  
            {
                MakingBombs();
            }          
           if(breakables[column,row]!=null) //eger eslesen noktanın altinda breakable varsa onu yok ediyor
            {
                breakables[column, row].getHit(1);
                if(breakables[column,row].health<=0)
                {
                    breakables[column, row] = null;
                }
            }

           if(goals!=null)  //yok olan nokta bitirme kosulunu sagliyorsa guncelliyor
            {
                goals.addGoal(allDots[column,row].tag.ToString());
                goals.updateGoals();
            }
            GameObject particle = Instantiate(destroyParticle, allDots[column, row].transform.position, Quaternion.identity); //yok olma efektinin cagirilmasi
            Destroy(particle, 0.5f);
            if(comboBonus==1)
            {
                FindObjectOfType<audioManager>().Play("ping1");
            }
            else if(comboBonus==2)
            {
                FindObjectOfType<audioManager>().Play("ping2");
            }
            else if(comboBonus>=3)
            {
                FindObjectOfType<audioManager>().Play("ping3");
            }
            Destroy(allDots[column, row]);
            score.AddScore(scoreForPiece * comboBonus);
            HighScore += (scoreForPiece * comboBonus);
            allDots[column, row] = null;  //nokto yok edilip koordinatları null olarak degistiriliyor
        }
    } //1.1
    public void DestroyMatches()  //oyun alanindaki parcalarin herbiri icin yoketme fonksiyonunu cagiriyor
    {
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]!=null)
                {
                    DestroyDot(i, j);
                }
            }
        }
        find.matches.Clear();
        StartCoroutine(collapseThePiecesV2());
    } //1.2
    public bool matchesOnBoard() // oyun alanında eslesme olup olmadiginin kontrolu
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dots>().isMatched == true)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    } //2.2


    private IEnumerator collapseThePiecesV2() //eslesmenin ustunde kalan noktalarin bos alanlari doldurmasi icin koordinat duzenlemesi
    {
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(blankTiles[i,j]==false && allDots[i,j]==null)
                {
                    for(int k=j+1;k<height;k++)
                    {
                        if(allDots[i,k]!=null)
                        {
                            allDots[i, k].GetComponent<Dots>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(fillBoard());
    }//2.4
    private void refillEmpty()   //eslesme sonrasinda en ustte kalan bos alanlara yeni noktaların eklenmesi 
    {
        currentState = GameState.wait;
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]==null && blankTiles[i,j]==false)
                {
                    Vector2 tempPos = new Vector2(i, j + offSetForSlide);
                    int newDot = Random.Range(0, dots.Length);
                    GameObject Dotpiece = Instantiate(dots[newDot], tempPos, Quaternion.identity);
                    allDots[i, j] = Dotpiece;
                    Dotpiece.GetComponent<Dots>().row = j;
                    Dotpiece.GetComponent<Dots>().column = i;
                }
            } 
        }
    } //2.1
    private IEnumerator fillBoard() //oyun sahasi tekrar doldurulmasi ve ardindan eslesme ve kilit kontrolleri
    {
        refillEmpty();
        yield return new WaitForSeconds(0.4f);
        while(matchesOnBoard()==true)
        {
            comboBonus+=1;
            currentState = GameState.wait;
            yield return new WaitForSeconds(0.2f);
            DestroyMatches();
        }
        find.matches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(0.4f);
        if(isLocked()==true)
        {
            shuffleBoard();
        }
        StartCoroutine(ComboAndSoundDelay());
    }  //2.3   //4.5
    private IEnumerator ComboAndSoundDelay() //kombo bonusuna gore olusacak sesin hep ayni olmamasi icin delay
    {
        yield return new WaitForSeconds(0.5f);
        if (matchesOnBoard() == false)
        {
            currentState = GameState.move;
            comboBonus = 1;
        }
    }

    private void switchForLock(int column,int row,Vector2 direction) //verilen parcanin istenilen yondeki parca ile koordinat degisimi
    {
        GameObject tempDot = allDots[column+(int)direction.x, row+(int)direction.y] as GameObject;
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = tempDot;
    }  //4.1
    private bool controlMatchesForLock() //ekranda eslesme olup olmadiginin kontrolu
    {
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]!=null)
                {
                   if(i<width-2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                  if(j<height-2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }  //4.2
    public bool switchAndMatchForLock(int column,int row, Vector2 direction) //bir parcanin hareket halinde eslesmesi olup olmadiginin kontrolu
    {
        switchForLock(column, row, direction);
        if(controlMatchesForLock()==true)
        {
            switchForLock(column, row, direction);
            return true;
        }
        switchForLock(column, row, direction);
        return false;
    }  //4.3
    private bool isLocked() // ekranda yapilacak hamle olup olmadiginin kontrolu 
    {
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]!=null)
                {
                    if(i<width-1)
                    {
                        if(switchAndMatchForLock(i,j,Vector2.right)==true)
                        {
                            return false;
                        }
                    }
                    if(j<height-1)
                    {
                        if(switchAndMatchForLock(i,j,Vector2.up)==true)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    } //4.4
    private void shuffleBoard() //eger yapilacak hamle yoksa ekrandaki noktalarin karistirilmasi
    {      
        List<GameObject> shuffleList = new List<GameObject>();
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]!=null)
                {
                    shuffleList.Add(allDots[i, j]); 
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (blankTiles[i,j]==false)
                {
                    int pieceToPut = Random.Range(0, shuffleList.Count);                  
                    while (CheckMatchesOnStart(i, j, shuffleList[pieceToPut])) //karistirdiktan sonra eslesme olmamasi icin kontrol
                    {
                        pieceToPut = Random.Range(0, shuffleList.Count);
                    }
                    Dots piece = shuffleList[pieceToPut].GetComponent<Dots>();
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = shuffleList[pieceToPut];
                    shuffleList.Remove(shuffleList[pieceToPut]);
                }
            }
        }
        if(isLocked()==true)
        {
            shuffleBoard();
        }
    } //4.5   //5.1
   

}



/*  private IEnumerator collapseThePieces()     
    {
        int emptyCount = 0;
        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                if(allDots[i,j]==null)
                {
                    emptyCount++;
                }
                else if(emptyCount>0)
                {
                    allDots[i, j].GetComponent<Dots>().row -= emptyCount;
                    allDots[i, j] = null;
                }
            }
            emptyCount = 0;
        }
        yield return new WaitForSeconds(0.4f);  
        StartCoroutine(fillBoard());
    } */
