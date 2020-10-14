using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class saveController : MonoBehaviour
{
    public bool[] levels = new bool[9];
    public int[] scores = new int[9];

    private void Awake() //sahne gecisinde objenin yok olmamasi icin gerekli kod
    {
        DontDestroyOnLoad(gameObject);
    }
   
    public void SaveGame()
    {
        gameSavingSystem.SaveGame(this);
    }

    public void LoadGame() //save dosyasindan verilerin alinip oyundakilerle degistirilmesi
    {
        gameData data = gameSavingSystem.LoadGame();

        for (int i = 0; i < 9; i++)
        {
            levels[i] = data.levels[i];
        }
        for (int a = 0; a < 9; a++)
        {
            scores[a] = data.scores[a];
        }
        
    }
}
