using UnityEngine;
using UnityEngine.UI;

// Garante que o objeto tenha um componente Image.
[RequireComponent(typeof(Image))]
public class GlitchFlashController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;

    [Header("Intervalo entre Sequências")]
    [Min(0f)]
    [SerializeField] private float minInterval = 25f;

    [Min(0f)]
    [SerializeField] private float maxInterval = 40f;

    [Header("Configurações da Piscada")]
    [Range(0f, 1f)]
    [SerializeField] private float flashOpacity = 0.12f;

    [Min(0.01f)]
    [SerializeField] private float flashDuration = 0.08f;

    [Header("Sequência de Glitch")]
    [Min(1)]
    [SerializeField] private int minFlashesPerSequence = 2;

    [Min(1)]
    [SerializeField] private int maxFlashesPerSequence = 3;

    [Min(0f)]
    [SerializeField] private float timeBetweenFlashes = 0.05f;

    // Imagem usada para a piscada.
    private Image flashImage;

    // Tempo até a próxima sequência completa de glitch.
    private float timeUntilNextSequence;

    // Indica se uma sequência de glitch está em andamento.
    private bool isSequenceRunning;

    // Indica se a imagem está visível neste momento.
    private bool isFlashVisible;

    // Tempo restante do estado atual (visível ou invisível).
    private float stateTimer;

    // Quantas piscadas ainda faltam terminar nesta sequência.
    private int flashesRemaining;

    private void Awake()
    {
        // Obtém a Image do próprio objeto.
        flashImage = GetComponent<Image>();

        // Garante que a imagem comece invisível.
        SetFlashAlpha(0f);

        // Impede que a imagem bloqueie cliques na UI.
        flashImage.raycastTarget = false;

        // Valida a referência do GameManager.
        if (gameManager == null)
        {
            Debug.LogError("GlitchFlashController: GameManager não configurado!");
            enabled = false;
            return;
        }

        // Sorteia o tempo até a primeira sequência.
        ScheduleNextSequence();
    }

    private void Update()
    {
        // O efeito só deve rodar durante a gameplay.
        if (!gameManager.IsPlaying)
        {
            // Se o jogo sair do estado Playing durante uma sequência,
            // interrompe imediatamente e limpa o visual.
            if (isSequenceRunning || isFlashVisible)
            {
                StopAllFlashEffects();
                ScheduleNextSequence();
            }

            return;
        }

        // Se uma sequência estiver em andamento, controla seus estados.
        if (isSequenceRunning)
        {
            UpdateFlashSequence();
            return;
        }

        // Conta o tempo até a próxima sequência.
        timeUntilNextSequence -= Time.deltaTime;

        // Quando o tempo acabar, inicia uma nova sequência.
        if (timeUntilNextSequence <= 0f)
        {
            StartFlashSequence();
        }
    }

    private void StartFlashSequence()
    {
        // Garante que o valor máximo nunca seja menor que o mínimo.
        int safeMaxFlashes = Mathf.Max(minFlashesPerSequence, maxFlashesPerSequence);

        // Sorteia quantas piscadas esta sequência terá.
        flashesRemaining = Random.Range(minFlashesPerSequence, safeMaxFlashes + 1);

        // Marca que a sequência começou.
        isSequenceRunning = true;

        // A primeira piscada já começa visível.
        ShowFlash();
    }

    private void UpdateFlashSequence()
    {
        // Atualiza o temporizador do estado atual.
        stateTimer -= Time.deltaTime;

        // Enquanto o tempo do estado não acabar, não faz transição.
        if (stateTimer > 0f)
        {
            return;
        }

        // Se a imagem está visível, agora ela deve apagar.
        if (isFlashVisible)
        {
            HideFlash();

            // Uma piscada completa terminou.
            flashesRemaining--;

            // Se ainda faltam piscadas, espera um curto intervalo
            // antes da próxima piscada da mesma sequência.
            if (flashesRemaining > 0)
            {
                stateTimer = timeBetweenFlashes;
            }
            else
            {
                // Se não faltam mais piscadas, encerra a sequência
                // e agenda a próxima.
                isSequenceRunning = false;
                ScheduleNextSequence();
            }

            return;
        }

        // Se a imagem está invisível e ainda restam piscadas,
        // começa a próxima piscada da sequência.
        if (flashesRemaining > 0)
        {
            ShowFlash();
        }
    }

    private void ShowFlash()
    {
        // Marca a imagem como visível.
        isFlashVisible = true;

        // Define a duração da piscada.
        stateTimer = flashDuration;

        // Aplica a opacidade configurada.
        SetFlashAlpha(flashOpacity);
    }

    private void HideFlash()
    {
        // Marca a imagem como invisível.
        isFlashVisible = false;

        // Esconde completamente a imagem.
        SetFlashAlpha(0f);
    }

    private void ScheduleNextSequence()
    {
        // Garante que o máximo não seja menor que o mínimo.
        float safeMaxInterval = Mathf.Max(minInterval, maxInterval);

        // Sorteia o tempo até a próxima sequência.
        timeUntilNextSequence = Random.Range(minInterval, safeMaxInterval);
    }

    private void StopAllFlashEffects()
    {
        // Interrompe a sequência atual e limpa todos os estados.
        isSequenceRunning = false;
        isFlashVisible = false;
        flashesRemaining = 0;
        stateTimer = 0f;

        // Garante que a imagem fique invisível.
        SetFlashAlpha(0f);
    }

    private void SetFlashAlpha(float alpha)
    {
        // Mantém a cor da imagem definida no Inspector
        // e altera apenas a transparência.
        Color currentColor = flashImage.color;
        currentColor.a = alpha;
        flashImage.color = currentColor;
    }

    private void OnDisable()
    {
        // Garante que o efeito não fique preso na tela
        // se o componente for desativado.
        if (flashImage != null)
        {
            StopAllFlashEffects();
        }
    }
}