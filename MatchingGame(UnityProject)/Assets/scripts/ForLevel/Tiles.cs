using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public int health;
    private SpriteRenderer sprite;
    private GoalManager goals;
    private void Start()
    {
        goals = FindObjectOfType<GoalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update() //breakable objesinin can degerinin kontrolu 
    {     
        if(health<=0)
        {
            if (goals != null)
            {
                goals.addGoal(this.gameObject.tag);
                goals.updateGoals();
            }
            Destroy(this.gameObject);
        }
    }
    public void getHit(int damage)
    {
        health -= damage;      
    }
}
