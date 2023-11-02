using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioTracks;
    private int currentTrackIndex;

    void Start()
    {
        // Ensure there's an AudioSource component attached to the GameObject
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Check if there are audio tracks to play
        if (audioTracks.Length == 0)
        {
            Debug.LogError("No audio tracks provided. Add audio clips to the 'audioTracks' array.");
            return;
        }

        // Play the first track
        PlayNextTrack();
    }

    void Update()
    {
        // Check if the current track has finished playing
        if (!audioSource.isPlaying)
        {
            // Play the next track in the loop
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        // Choose a random track from the list
        int randomIndex = Random.Range(0, audioTracks.Length);
        currentTrackIndex = randomIndex;

        // Assign and play the chosen track
        audioSource.clip = audioTracks[randomIndex];
        audioSource.Play();
    }
}
