using UnityEngine;

public class SpawnSoundManager : MonoBehaviour
{
    public static SpawnSoundManager Instance { get; private set; }

    private AudioSource audioSource;
    private bool isSpawnSoundPlaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySpawnSound(AudioClip clip)
    {
        if (!isSpawnSoundPlaying && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            isSpawnSoundPlaying = true;
            Invoke(nameof(ResetSpawnSoundFlag), clip.length);
        }
    }

    private void ResetSpawnSoundFlag()
    {
        isSpawnSoundPlaying = false;
    }
}
