
using UnityEngine;

public class MusicGlitchController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource introSource;
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private GameMusicController gameMusicController;

    [Header("Primeiro Glitch")]
    [SerializeField] private float firstDelayMin = 8f;
    [SerializeField] private float firstDelayMax = 15f;

    [Header("Intervalo Entre Glitches")]
    [SerializeField] private float intervalMin = 12f;
    [SerializeField] private float intervalMax = 20f;

    [Header("Duração da Distorção e Falha")]
    [SerializeField] private float durationMin = 0.3f;
    [SerializeField] private float durationMax = 0.7f;

    [Header("Glitch de Aceleração")]
    [SerializeField, Range(1f, 2f)]
    private float accelerationPitch = 1.2f;

    [SerializeField, Min(0.05f)]
    private float accelerationDuration = 0.4f;

    private AudioDistortionFilter introDistortion;
    private AudioDistortionFilter loopDistortion;

    private AudioLowPassFilter introLowPass;
    private AudioLowPassFilter loopLowPass;

    private float nextGlitchTimer;
    private float glitchDurationTimer;

    private bool glitchActive = false;

    private void Awake()
    {
        if (gameManager == null ||
            gameMusicController == null ||
            introSource == null ||
            loopSource == null)
        {
            Debug.LogError(
                "MusicGlitchController: referências não configuradas.",
                this
            );

            enabled = false;
            return;
        }

        // Recupera os filtros dos dois AudioSources.
        introDistortion =
            introSource.GetComponent<AudioDistortionFilter>();

        loopDistortion =
            loopSource.GetComponent<AudioDistortionFilter>();

        introLowPass =
            introSource.GetComponent<AudioLowPassFilter>();

        loopLowPass =
            loopSource.GetComponent<AudioLowPassFilter>();

        if (introDistortion == null ||
            loopDistortion == null ||
            introLowPass == null ||
            loopLowPass == null)
        {
            Debug.LogError(
                "MusicGlitchController: filtros não encontrados.",
                this
            );

            enabled = false;
            return;
        }

        RestoreNormalAudio();

        // Sorteia quando ocorrerá o primeiro glitch.
        nextGlitchTimer = Random.Range(
            firstDelayMin,
            firstDelayMax
        );
    }

    private void Update()
    {
        // Encerra qualquer interferência ao morrer.
        if (gameManager.IsGameOver)
        {
            if (glitchActive)
                EndGlitch();

            return;
        }

        // Não avança temporizadores antes do início
        // da partida ou durante o Pause.
        if (!gameManager.IsPlaying)
            return;

        if (glitchActive)
        {
            glitchDurationTimer -= Time.deltaTime;

            if (glitchDurationTimer <= 0f)
                EndGlitch();

            return;
        }

        nextGlitchTimer -= Time.deltaTime;

        if (nextGlitchTimer <= 0f)
            StartGlitch();
    }

    private void StartGlitch()
    {
        glitchActive = true;

        // Sorteia um dos três tipos de glitch.
        // 0 = Distorção digital
        // 1 = Falha de transmissão
        // 2 = Aceleração da música
        int glitchType = Random.Range(0, 3);

        switch (glitchType)
        {
            case 0:
                glitchDurationTimer = Random.Range(
                    durationMin,
                    durationMax
                );

                introDistortion.enabled = true;
                loopDistortion.enabled = true;

                Debug.Log("GLITCH: Distorção digital");
                break;

            case 1:
                glitchDurationTimer = Random.Range(
                    durationMin,
                    durationMax
                );

                introLowPass.enabled = true;
                loopLowPass.enabled = true;

                Debug.Log("GLITCH: Falha de transmissão");
                break;

            
            case 2:
                glitchDurationTimer = accelerationDuration;

                if (gameMusicController.IsLoopActive)
                {
                    // Durante o Loop, basta acelerar diretamente.
                    loopSource.pitch = accelerationPitch;
                }
                else
                {
                    // Durante a Intro, ajusta também o momento
                    // programado para a entrada do Loop.
                    bool accelerated = gameMusicController.TryAccelerateIntro(
                        accelerationPitch,
                        accelerationDuration
                    );

                    if (!accelerated)
                    {
                        // Próximo da transição, usa distorção
                        // para evitar problemas no agendamento.
                        glitchDurationTimer = Random.Range(
                            durationMin,
                            durationMax
                        );

                        introDistortion.enabled = true;
                        loopDistortion.enabled = true;

                        Debug.Log("GLITCH: Distorção (proteção da transição)");
                        break;
                    }
                }

                Debug.Log("GLITCH: Aceleração da música");
                break;

        }
    }

    private void EndGlitch()
    {
        // Remove todos os efeitos, incluindo
        // a aceleração, e restaura a música normal.
        RestoreNormalAudio();

        glitchActive = false;

        nextGlitchTimer = Random.Range(
            intervalMin,
            intervalMax
        );
    }

    private void RestoreNormalAudio()
    {
        if (introDistortion != null)
            introDistortion.enabled = false;

        if (loopDistortion != null)
            loopDistortion.enabled = false;

        if (introLowPass != null)
            introLowPass.enabled = false;

        if (loopLowPass != null)
            loopLowPass.enabled = false;

        // Restaura a velocidade original da música.
        if (loopSource != null)
            loopSource.pitch = 1f;

            // Restaura a velocidade normal das duas músicas.
        if (introSource != null)
            introSource.pitch = 1f;
    }

    private void OnDisable()
    {
        // Evita deixar algum efeito ativo ao
        // desativar o controlador ou trocar de cena.
        RestoreNormalAudio();
    }
}
