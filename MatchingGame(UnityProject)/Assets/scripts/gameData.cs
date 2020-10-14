using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class gameData
{
    public bool[] levels = new bool[9];
    public int[] scores = new int[9];
  
    public gameData(saveController save)  //kayit alacagim verilerin olusturulup atanmasi
    {
        for (int i = 0; i < 9; i++)
        {
            levels[i] = save.levels[i];
        }
        for (int a = 0; a < 9; a++)
        {
            scores[a] = save.scores[a];
        }
    }
}
