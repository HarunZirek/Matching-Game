using UnityEngine.Audio;
using System;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public sound[] sounds;

    public static audioManager instance;
    void Awake()   //normalde sahne degistikce o sahnedeki objeler kaybolur. soundManager'ın kaybolmamasi icin kod parcasi
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);  //eger sahnede 2. soundManager bulursa onu sahneden siliyor
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (sound s in sounds)  //eklenen seslerin verilerini editorde kullanıcı ayarlamasına sunmak icin gerekli kod
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
        }
    }

    public void Play(string name)  //istenilen sesin sadece ismi yazılarak oynatılmasını sagliyor. 
    {
        sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}
