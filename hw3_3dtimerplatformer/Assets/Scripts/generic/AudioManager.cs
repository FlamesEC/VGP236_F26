using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource clipSource;
    public List<AudioClip> clipList;

    public AudioSource musicSource;
    public List<AudioClip> songList;

    string musicBufferName = "NULL";
    float secondsForFadeout;
    float defaultVolumeMusic;
    bool fadingOut = false;
    float fadeoutTimestamp = 0;

    private void Awake()
    {
        if (instance != null)
        {
            print($"INSTANCE OF {this.GetType()} ALREADY EXISTS.");
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private void Start()
    {
        defaultVolumeMusic = musicSource.volume;
    }

    private void Update()
    {
        if (musicBufferName == "NULL")
            return;

        if (fadingOut)
        {
            musicSource.volume = Mathf.Lerp(defaultVolumeMusic, 0, (Time.time - fadeoutTimestamp) / secondsForFadeout);

            if (musicSource.volume <= 0)
            {
                fadeoutTimestamp = Time.time;
                fadingOut = false;

                if (musicBufferName != "X_INST_STOP")
                {
                    PlayMusic(musicBufferName);
                }
                else
                {
                    StopMusic();
                    musicBufferName = "NULL";
                    musicSource.volume = defaultVolumeMusic;
                }
            }
        }
        else
        {
            musicSource.volume = Mathf.Lerp(0, defaultVolumeMusic, (Time.time - fadeoutTimestamp) / secondsForFadeout);

            if (musicSource.volume >= defaultVolumeMusic)
            {
                musicBufferName = "NULL";
            }
        }
    }

    public void PlaySFX(string audioName)
    {
        PlaySFX(audioName, 1);
    }

    public void PlaySFX(string audioName, float volume)
    {
        for (int i = 0; i < clipList.Count; i++)
        {
            if (clipList[i].name != audioName)
                continue;

            clipSource.PlayOneShot(clipList[i], volume);
            break;
        }
    }

    /// <summary>
    /// Pitch usage is more expensive; Must instantiate source and set pitch individually.
    /// </summary>
    /// <param name="audioName"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public void PlaySFX(string audioName, float volume, float pitch)
    {
        for (int i = 0; i < clipList.Count; i++)
        {
            if (clipList[i].name != audioName)
                continue;

            AudioSource s = Instantiate(clipSource.gameObject, transform).gameObject.GetComponent<AudioSource>();
            
            s.pitch = pitch;
            s.PlayOneShot(clipList[i], volume);

            Destroy(s.gameObject, clipList[i].length);

            break;
        }
    }

    public void PlayMusic(string songName, float secondsFadeout)
    {
        if (musicSource.clip != null && musicSource.clip.name == songName)
        {
            musicSource.UnPause();
            return;
        }

        musicBufferName = songName;
        secondsForFadeout = secondsFadeout;
        fadingOut = true;
        fadeoutTimestamp = Time.time;

        if (musicSource == null || !musicSource.isPlaying)
        {
            musicSource.volume = 0;
        }
    }

    public void PlayMusic(string songName)
    {
        if (musicSource.clip != null && musicSource.clip.name == songName)
        {
            musicSource.UnPause();
            return;
        }

        musicSource.Stop();
        musicSource.clip = null;

        for (int i = 0; i < songList.Count; i++)
        {
            if (songList[i].name != songName)
                continue;

            musicSource.clip = songList[i];
            musicSource.Play();
            break;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopMusic(float secondsForFadeout)
    {
        PlayMusic("X_INST_STOP", secondsForFadeout);
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void SetMusicPitch(float pitch)
    {
        musicSource.pitch = pitch;
    }

    public AudioClip GetCurrentSong()
    {
        return musicSource.clip;
    }
}
