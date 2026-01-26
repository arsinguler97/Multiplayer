using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource windSource;

    [Header("Audio Library")]
    public List<Sound> sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
        PlayWindWoosh();
    }

    public void PlaySFX(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    public void PlayMusic(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Music: {name} not found!");
            return;
        }
        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayWindWoosh()
    {
        string name = "WindWoosh";
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        windSource.clip = s.clip;
        windSource.volume = s.volume;
        windSource.loop = true;
        windSource.Play();
    }

    public void PlayShooting() => PlaySFX("Shooting");
    public void PlayPlayerDamaged() => PlaySFX("PlayerDamaged");
    public void PlayEnemyDamaged() => PlaySFX("EnemyDamaged");
    public void PlayPlayerDeath() => PlaySFX("PlayerDeath");
    public void PlayEnemyDeath() => PlaySFX("EnemyDeath");
    public void PlayWinning() => PlaySFX("Winning");
    public void PlayBackgroundMusic() => PlayMusic("BackgroundMusic");

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }
}