using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveLoader : MonoBehaviour
{
    private GameObject load;
    void Start()  //oyun acildiginda kullanicinin kaydini acmak icin gerekli
    {
        load= GameObject.Find("SaveController");

        load.GetComponent<saveController>().LoadGame();
    }

 
}
