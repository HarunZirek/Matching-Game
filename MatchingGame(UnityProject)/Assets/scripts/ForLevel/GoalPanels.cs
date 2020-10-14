using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoalPanels : MonoBehaviour
{
    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;

    void Start()
    {
        setUp();
    }
    void setUp() //UI in yazisi ve gorselinin atamasi
    {
        thisImage.sprite = thisSprite;
        thisText.text = thisString;
    }
  
}
