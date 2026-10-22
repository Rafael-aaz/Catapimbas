
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    [Header("Músicas da Gameplay")]
    [SerializeField] private AudioSource introSource;
    [SerializeField] private AudioSource loopSource;

    private bool musicStarted = false;

    // Momento em que o Loop começa no relógio de áudio.
    private double loopStartDspTime;

    // Informa quando o Loop realmente deve estar tocando.
    public bool IsLoopActive =>
        musicStarted &&
        AudioSettings.dspTime >= loopStartDspTime;

    
    public bool TryAccelerateIntro(float pitch, float duration)
    {
        // Não permite modificar uma transição que já aconteceu.
        if (!musicStarted || IsLoopActive)
            return false;

        if (pitch <= 1f || duration <= 0f)
            return false;

        // Evita alterar o agendamento muito perto
        // do momento em que o Loop deve começar.
        const double safetyMargin = 2d;

        if (AudioSettings.dspTime + duration + safetyMargin
            >= loopStartDspTime)
        {
            return false;
        }

        // Calcula quanto tempo a Intro ganhará
        // ao ser reproduzida mais rapidamente.
        double timeGained = (pitch - 1f) * duration;

        // Antecipa o Loop para compensar a aceleração.
        double newLoopStart = loopStartDspTime - timeGained;

        loopSource.SetScheduledStartTime(newLoopStart);
        loopStartDspTime = newLoopStart;

        // Acelera a introdução.
        introSource.pitch = pitch;

        return true;
    }
    

    public void StartMusic()
    {
        // Impede que as músicas sejam iniciadas novamente.
        if (musicStarted)
            return;

        if (introSource == null ||
            loopSource == null ||
            introSource.clip == null ||
            loopSource.clip == null)
        {
            Debug.LogError(
                "GameMusicController: configure os dois AudioSources e seus clips.",
                this
            );
            return;
        }

        introSource.Stop();
        loopSource.Stop();

        introSource.loop = false;
        loopSource.loop = true;

        // Agenda o início da primeira música.
        // Uma pequena antecedência permite preparar o áudio.
        double startTime = AudioSettings.dspTime + 0.15d;

        // Calcula a duração exata da introdução.
        double introDuration =
            (double)introSource.clip.samples /
            introSource.clip.frequency;

        // A introdução toca apenas uma vez.
        introSource.PlayScheduled(startTime);

        // Guarda o instante exato da transição.
        loopStartDspTime = startTime + introDuration;

        loopSource.PlayScheduled(loopStartDspTime);

        Debug.Log(
            "Duração da Intro: " +
            introDuration.ToString("F1") +
            " segundos. A aceleração será liberada após a Intro."
        );
        musicStarted = true;
    }

    public void StopMusic()
    {
        // Interrompe tanto a introdução quanto o loop,
        // inclusive quando uma reprodução está agendada.
        if (introSource != null)
            introSource.Stop();

        if (loopSource != null)
            loopSource.Stop();

        musicStarted = false;
        loopStartDspTime = 0d;
    }
}
