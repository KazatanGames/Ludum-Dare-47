using UnityEngine;
using System.Collections;

public class MiniMusicManager : SingletonMonoBehaviour<MiniMusicManager>
{
    [SerializeField]
    protected AudioClip startMusic;
    [SerializeField]
    protected AudioClip loopMusic;
    [SerializeField]
    protected AudioSource musicSource;
    [SerializeField]
    protected float fadeTime;
    [SerializeField]
    protected float initialVol;
    [SerializeField]
    protected float startVol;
    [SerializeField]
    protected float loopVol;


    protected bool loop = false;
    protected bool started = false;
    protected float fadeTimeRem;

    protected override void Initialise()
    {
        base.Initialise();

        fadeTimeRem = fadeTime;
        musicSource.clip = startMusic;
    }

    public void Go()
    {
        musicSource.Play();
        started = true;
    }

    private void Update()
    {
        if (!loop && fadeTimeRem > 0)
        {
            fadeTimeRem -= Time.deltaTime;

            musicSource.volume = Mathf.Lerp(initialVol, startVol, Mathf.InverseLerp(fadeTime, 0, fadeTimeRem));
        }

        if (started && !musicSource.isPlaying && !loop)
        {
            musicSource.clip = loopMusic;
            musicSource.loop = true;
            musicSource.volume = loopVol;
            musicSource.Play();
        }
    }

}
