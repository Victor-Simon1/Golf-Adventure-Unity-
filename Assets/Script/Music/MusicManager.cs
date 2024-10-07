using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Services;
public class MusicManager : MonoRegistrable
{

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClip;
    private int nextSong;

    [Header("Volume")]
    private float minVolume = 0f;
    private float maxVolume = 0.02f;
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Register<MusicManager>(this,false) ;
        audioSource = GetComponent<AudioSource>();
        nextSong = -1;
        ShuffleSong();
        if (!audioSource.isPlaying)
            ChangeSong();

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
            ChangeSong();
    }

    private void ShuffleSong()
    {
        Debug.Log("Change song ");
        List<AudioClip> tempAudioClip = new List<AudioClip>(audioClip);
        for (int i = 0;i < tempAudioClip.Count; i++) 
        {
           var temp = tempAudioClip[i];
            int rand = Random.Range(i, tempAudioClip.Count);
            tempAudioClip[i] = tempAudioClip[rand];
            tempAudioClip[rand] = temp;
        }
        audioClip = tempAudioClip.ToArray();
    }

    private void ChangeSong()
    {
        Debug.Log("Change song ");
        if (nextSong+1 < audioClip.Length)
            nextSong++;
        else
            nextSong = 0;
        audioSource.clip = audioClip[nextSong];
        audioSource.Play();
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
    public float GetMaxVolume()
    {
        return maxVolume;
    }
    public float GetMinVolume()
    {
        return minVolume;
    }
}
