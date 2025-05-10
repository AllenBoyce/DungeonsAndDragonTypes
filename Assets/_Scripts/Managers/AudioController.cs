using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }
    public void PlaySound(AudioClip sound) {
        AudioSource.PlayClipAtPoint(sound, transform.position);
    }
    
    
}
