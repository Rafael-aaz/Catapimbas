
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSFXController : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip deathSFX;

    private AudioSource audioSource;

    // Informa a duração do som de morte em segundos.
    public float DeathSoundDuration =>
    deathSFX != null ? deathSFX.length : 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        if (jumpSFX == null)
            return;

        // Reproduz o efeito a cada impulso do jogador.
        audioSource.PlayOneShot(jumpSFX);
    }

    public void PlayDeath()
    {
        if (deathSFX == null)
            return;

        // Interrompe possíveis efeitos de pulo
        // para dar prioridade ao som de morte.
        audioSource.Stop();

        audioSource.PlayOneShot(deathSFX);
    }
}
