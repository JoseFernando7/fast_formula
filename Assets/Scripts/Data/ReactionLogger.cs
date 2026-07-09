using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reacciona al resultado de la mezcla del Cauldron.
/// - type == "Success" -> mezcla correcta (no game over).
/// - cualquier otro tipo (Fail, Explosion, Hazard) -> reproduce el sonido
///   de explosión y muestra la pantalla de Game Over ("Panel perdiste").
///
/// Engánchalo al evento OnMixResult del Cauldron desde el Inspector,
/// igual que ya lo tenías con HandleReaction.
/// </summary>
public class ReactionLogger : MonoBehaviour
{
    /// <summary>
    /// True apenas se dispara el Game Over. Otros scripts (ElementSpawner,
    /// DraggableElement) lo consultan para dejar de responder a clicks/drags
    /// mientras está la pantalla de "Perdiste".
    /// </summary>
    public static bool IsGameOver { get; private set; }

    [Header("Game Over")]
    [Tooltip("Arrastra aquí el GameObject 'Panel perdiste'.")]
    public GameObject gameOverPanel;
    [Tooltip("Segundos de espera antes de mostrar el panel (para que se alcance a ver/oír la explosión).")]
    public float delayBeforeGameOver = 1f;

    [Header("Feedback de explosión")]
    [Tooltip("Si se deja vacío, se busca un AudioSource en este mismo GameObject.")]
    public AudioSource audioSource;
    [Tooltip("Arrastra aquí Assets/UI/Sounds/Explosion.mp3")]
    public AudioClip explosionClip;

    void Awake()
    {
        // Se resetea siempre al cargar la escena (por si el static quedó en
        // true de una partida anterior al reiniciar sin recargar el dominio).
        IsGameOver = false;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void HandleReaction(ReactionData data)
    {
        bool isSuccess = data != null && data.type == "Success";

        if (data == null)
        {
            Debug.Log("Resultado: NULL (no se encontró ninguna reacción, ni siquiera la de fallo)");
        }
        else if (isSuccess)
        {
            Debug.Log($"✅ Reacción exitosa: {data.resultName} (tipo: {data.type})");
        }
        else
        {
            Debug.Log($"💥 Mezcla peligrosa/fallida: {data.resultName} (tipo: {data.type})");
        }

        if (!isSuccess)
        {
            TriggerExplosionAndGameOver();
        }
    }

    void TriggerExplosionAndGameOver()
    {
        IsGameOver = true;

        if (audioSource != null && explosionClip != null)
        {
            audioSource.PlayOneShot(explosionClip);
        }

        // Limpia cualquier elemento suelto (los que quedaron en la mesa sin
        // llegar al Perol) para que no queden encima de la pantalla de Perdiste.
        ClearAllLooseElements();

        // Oculta también el estante de elementos (los botones para generar
        // más), ya no tiene sentido que se sigan viendo con el juego terminado.
        ElementSpawner.SetAllVisible(false);

        // Y el Perol también, para que la pantalla de Perdiste quede limpia.
        Cauldron.SetAllVisible(false);

        if (gameOverPanel != null)
        {
            Invoke(nameof(ShowGameOverPanel), delayBeforeGameOver);
        }
    }

    /// <summary>
    /// Destruye todos los elementos arrastrables que sigan sueltos en escena
    /// (spawnados pero no consumidos por el Perol).
    /// </summary>
    void ClearAllLooseElements()
    {
        DraggableElement[] loose = FindObjectsOfType<DraggableElement>();
        foreach (DraggableElement element in loose)
        {
            if (element != null)
                Destroy(element.gameObject);
        }
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Conectado al OnClick() del "Button restart" dentro del Panel perdiste.
    /// Recarga la escena actual desde cero (reinicia el nivel).
    /// </summary>
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}