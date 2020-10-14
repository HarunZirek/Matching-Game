using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreSystem : MonoBehaviour
{
    public Text scoreText;
    public int score;
    void Start()
    {
        
    }
    void Update() //kullanicinin o anki skorunu UI da gosteren kod (her framede calisiyor)
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int addAmount)
    {
        score += addAmount;
    }

   
}
