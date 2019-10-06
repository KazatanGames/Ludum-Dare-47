using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField]
    protected AudioClip music;

    [SerializeField]
    protected AudioClip finishMusic;

    protected AudioSource audioPlayer;


    // Start is called before the first frame update
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        PlayMusic();
    }

    private void Update()
    {
        if (!audioPlayer.isPlaying)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        audioPlayer.Stop();
        audioPlayer.clip = music;
        audioPlayer.loop = true;
        audioPlayer.volume = 0.33f;
        audioPlayer.Play();
    }

    public void Finish()
    {
        audioPlayer.Stop();
        audioPlayer.clip = finishMusic;
        audioPlayer.loop = false;
        audioPlayer.volume = 0.42f;
        audioPlayer.Play();
    }
}
