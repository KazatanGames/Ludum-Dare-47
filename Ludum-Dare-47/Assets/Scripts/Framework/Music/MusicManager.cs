using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/**
 * Music Manager
 * 
 * Kazatan Games Framework - should not require customization per game.
 * 
 * The Music Manager allows App wide control of the current playing music track based on the
 * concept of catalogs (categories).
 */
public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    protected string catalog = "";
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected List<MusicData> musicDatas;

    protected Dictionary<string, List<MusicData>> filteredMusicDatas;
    protected List<MusicData> currentMusicDatas;
    protected int catalogIndex;
    protected bool stopped = false;

    public bool MusicEnabled
    {
        get { return AppManager.INSTANCE.AppModel.audioPreferences.Data.musicEnabled; }
    }

    public void MusicSetCatalog(string catalog)
    {
        if (this.catalog == catalog) return;

        MusicStop();

        this.catalog = catalog;
        if (filteredMusicDatas.ContainsKey(catalog))
        {
            currentMusicDatas = filteredMusicDatas[catalog];
        } else
        {
            currentMusicDatas = null;
        }

        if (MusicEnabled)
        {
            PlayCatalog();
        }
    }

    public void MusicToggleEnabled()
    {
        AppManager.INSTANCE.AppModel.audioPreferences.SetMusicEnabled(!MusicEnabled);

        if (MusicEnabled)
        {
            PlayCatalog();
        } else
        {
            MusicStop();
        }
    }

    public void MusicPause()
    {
        stopped = true;
        MusicStop();
    }

    public void MusicResume()
    {
        stopped = false;
        if (MusicEnabled)
        {
            PlayCatalog();
        }
    }

    public void MusicNext()
    {
        if (currentMusicDatas != null && currentMusicDatas.Count > 0)
        {
            catalogIndex++;
            if (catalogIndex >= currentMusicDatas.Count)
            {
                catalogIndex = 0;
            }
            StartMusic(currentMusicDatas[catalogIndex].clip);
        }
    }

    protected override void Initialise()
    {
        catalogIndex = 0;
        FilterMusic();
    }

    protected void PlayCatalog()
    {
        MusicStop();
        if (currentMusicDatas != null && currentMusicDatas.Count > 0)
        {
            catalogIndex = Random.Range(0, currentMusicDatas.Count);
            StartMusic(currentMusicDatas[catalogIndex].clip);
        }
    }

    protected void FilterMusic()
    {
        filteredMusicDatas = new Dictionary<string, List<MusicData>>();

        foreach (MusicData md in musicDatas)
        {
            if (md.catalog != null && md.catalog != "")
            {
                if (!filteredMusicDatas.ContainsKey(md.catalog))
                {
                    filteredMusicDatas.Add(md.catalog, new List<MusicData>());
                }
                filteredMusicDatas[md.catalog].Add(md);
            }
        }
    }

    protected void StartMusic(AudioClip clip)
    {
        if (stopped) return;
        source.clip = clip;
        source.Play();
    }

    protected void MusicStop()
    {
        source.Stop();
    }

    protected void Update()
    {
        if (MusicEnabled && !source.isPlaying)
        {
            MusicNext();
        }
    }
}
