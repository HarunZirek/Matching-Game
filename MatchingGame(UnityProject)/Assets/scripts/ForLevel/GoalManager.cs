using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GoalNeeded  //gereksinimlerin verileri
{
    public int numberNeed;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchName;
}
public class GoalManager : MonoBehaviour
{
    
    public GoalNeeded[] levelGoals;
    public List<GoalPanels> currentGoals = new List<GoalPanels>();
    private EndGameManager end;
    public GameObject prefab;
    public GameObject goalParent;
    public GameObject introParent;
    void Start()
    {
        end = FindObjectOfType<EndGameManager>();
        setUpGoals();
    }

    private void setUpGoals() // bolum bitirme gereksinimlerinin UI da kullanıcıya gosterilmesi 
    {
        for(int i=0;i<levelGoals.Length;i++)
        {
            GameObject goal = Instantiate(prefab, introParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(introParent.transform);

            GoalPanels panel = goal.GetComponent<GoalPanels>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeed;


            GameObject ınGamegoal = Instantiate(prefab, goalParent.transform.position, Quaternion.identity);
            ınGamegoal.transform.SetParent(goalParent.transform);
            panel = ınGamegoal.GetComponent<GoalPanels>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeed;
        }
    }
   
   public void updateGoals() //toplanan parcalarin bolumun bitirme gereksinimlerini karsilayip karsilamadiginin kontrolu
    {
        int goalsCompleted = 0;
        for(int i=0; i<levelGoals.Length;i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeed;
            if(levelGoals[i].numberCollected>=levelGoals[i].numberNeed)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeed + "/" + levelGoals[i].numberNeed;
            }
        }
        if(goalsCompleted>=levelGoals.Length)
        {
           if(end!=null)
            {
                end.Lwin = true;
                StartCoroutine(end.LevelComplete());
            }
        }
    }

    public void addGoal(string dotType) //eslestirmelerden bolum bitirme gereksinimlerine uyanlarin eklenmesi
    {
        for(int i=0;i<levelGoals.Length;i++)
        {
            if(dotType==levelGoals[i].matchName)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
