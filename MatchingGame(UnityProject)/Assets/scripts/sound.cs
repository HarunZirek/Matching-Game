using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class sound  //eklenen her bir ses iç isim ton ve yükseklik ayarlarının verileri
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;


    [HideInInspector]
    public AudioSource source;
}
