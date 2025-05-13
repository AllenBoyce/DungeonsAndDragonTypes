using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Instance = this;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.name == "TitleScreen") {
            PlayMusic(Resources.Load<AudioClip>("Audio/Music/MenuMusic/Top Menu Theme"));
        }
        else if(scene.name == "Game") {
            PlayMusic(Resources.Load<AudioClip>("Audio/Music/CombatMusic/Battle Theme"));
        }
    }

    void OnAwake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }
    public void PlaySFX(AudioClip sound) {
        _sfxSource.PlayOneShot(sound, PlayerPrefs.GetFloat("SFXVolume"));
    }
    public void PlayMusic(AudioClip sound) {
        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        _musicSource.clip = sound;
        _musicSource.Play();
    }
    public void StopMusic() {
        _musicSource.Stop();
    }

    public void SetMusicSource(AudioSource source) {
        _musicSource = source;
    }
    
    
}
