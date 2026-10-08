using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;

    private AudioSource soundSource;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        // Play sounds only when an action happens, without looping.
        soundSource.playOnAwake = false;
        soundSource.loop = false;
        soundSource.spatialBlend = 0f;
        soundSource.volume = 1f;
        soundSource.Stop();
    }

    // Each action picks its own sound.
    public void PlayJump() { Play(jumpSound); }
    public void PlayAttack() { Play(attackSound); }
    public void PlayHurt() { Play(hurtSound); }
    public void PlayDeath() { Play(deathSound); }

    private void Play(AudioClip clip)
    {
        if (isActiveAndEnabled && soundSource != null && clip != null)
        {
            // Play this clip once at the chosen volume.
            soundSource.PlayOneShot(clip, volume);
        }
    }
}
