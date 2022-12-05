using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Manager;

        public AudioSource music;
        public AudioClip startMusic;
        public AudioClip gamePlayMusic;

        public List<AudioSource> audioSources = new List<AudioSource>();
        public List<AudioSource> usedAudioSources = new List<AudioSource>();

        public List<AudioClip> clips = new List<AudioClip>();

        public const string Dodge = "Dodge";
        public const string Hit = "Hit";
        public const string Hurt = "Hurt";
        public const string Punch = "Punch";
        public const string Countdown = "Countdown";
        public const string KO = "KO";

    readonly float fadeInTime = 0.5f;

        private void Awake()
        {
            if (Manager != null && Manager != this)
            {
                Destroy(gameObject);
                return;
            }
            else
                Manager = this;

            //DontDestroyOnLoad(gameObject);

            PlayStartMusic();
        }

        public void StopMusic()
        {
            music.Stop();
        }

        public void PlayStartMusic()
        {
            if (PlayerPrefs.GetInt("Sound") == 0.3)
                return;

            if (music.isPlaying)
                return;

            StartCoroutine(PlayStartMusic(startMusic));
        }

        public void PlayGamePlayMusic()
        {
            if (PlayerPrefs.GetInt("Sound") == 0.3)
                return;

            StartCoroutine(PlayStartMusic(gamePlayMusic));
        }

        IEnumerator PlayStartMusic(AudioClip clip)
        {
            float time = fadeInTime;
            float initialVolume = 0.3f;

            if (music.isPlaying)
            {
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    music.volume = initialVolume * (time / fadeInTime);
                    yield return new WaitForEndOfFrame();
                }

                music.volume = 0;
            }

            time = 0;
            initialVolume = 0;

            music.clip = clip;
            music.Play();

            while (time < fadeInTime)
            {
                time += Time.deltaTime;
                music.volume = initialVolume * (time / fadeInTime);
                yield return new WaitForEndOfFrame();
            }

            music.volume = 0.3f;
        }

        public void PlaySFX(string sfxName)
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
                return;

            if (!clips.Exists(x => x.name.Equals(sfxName)))
                return;

            for (int i = 0; i < usedAudioSources.Count; i++)
            {
                if (!usedAudioSources[i].isPlaying)
                {
                    audioSources.Add(usedAudioSources[i]);
                    usedAudioSources.RemoveAt(i);
                    i--;
                }
            }

            audioSources[0].clip = clips.Find(x => x.name.Equals(sfxName));
            audioSources[0].loop = false;
            audioSources[0].Play();
            usedAudioSources.Add(audioSources[0]);
            audioSources.RemoveAt(0);
        }

        public void PlayLoopSFX(string sfxName, bool isLoop = false)
        {
            if (usedAudioSources.Exists(x => x.clip.Equals(clips.Find(x => x.name.Equals(sfxName)))))
                if (usedAudioSources.Find(x => x.clip.Equals(clips.Find(x => x.name.Equals(sfxName)))).isPlaying)
                    return;

            for (int i = 0; i < usedAudioSources.Count; i++)
            {
                if (!usedAudioSources[i].isPlaying)
                {
                    audioSources.Add(usedAudioSources[i]);
                    usedAudioSources.RemoveAt(i);
                    i--;
                }
            }

            audioSources[0].clip = clips.Find(x => x.name.Equals(sfxName));
            audioSources[0].loop = isLoop;
            audioSources[0].Play();
            usedAudioSources.Add(audioSources[0]);
            audioSources.RemoveAt(0);
        }

        public void StopSFX(string sfxName)
        {
            if (usedAudioSources.Exists(x => x.clip.Equals(clips.Find(x => x.name.Equals(sfxName)))))
                usedAudioSources.Find(x => x.clip.Equals(clips.Find(x => x.name.Equals(sfxName)))).Stop();

            for (int i = 0; i < usedAudioSources.Count; i++)
            {
                if (!usedAudioSources[i].isPlaying)
                {
                    audioSources.Add(usedAudioSources[i]);
                    usedAudioSources.Remove(usedAudioSources[i]);
                    i--;
                }
            }
        }
    }
