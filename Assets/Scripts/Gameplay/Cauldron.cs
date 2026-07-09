using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JUEGO 100% 2D. Va en el GameObject del Perol / Matraz / Botella.
/// Requiere un Collider2D marcado como "Is Trigger" (o sin trigger, ya que
/// aquí se usa OverlapPoint desde el elemento arrastrado, así que también
/// funciona como collider normal).
///
/// Responsabilidades cubiertas:
/// - Detectar cuándo un elemento entra al recipiente.
/// - Almacenar el dato del elemento que se añadió (array de ids).
/// - Consultar al ReactionManager (de tu compañero) si la mezcla es válida.
/// - Limpiar el recipiente después de verificar la mezcla.
/// </summary>
public class Cauldron : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si se deja vacío, se busca automáticamente en la escena.")]
    public ReactionManager reactionManager;

    [Header("Configuración")]
    public int maxElements = 2;
    [Tooltip("Posiciones visuales (vacíos/Transforms) donde se acomoda cada elemento dentro del recipiente.")]
    public Transform[] slotPositions;
    public float delayBeforeCheck = 0.3f;
    public float delayBeforeClear = 1.0f;

    [Header("Eventos (para Score/GameManager)")]
    public ReactionResultEvent OnMixResult;   // se dispara con el resultado (ReactionData) apenas se completa la mezcla
    public UnityEngine.Events.UnityEvent OnCauldronCleared; // se dispara cuando el recipiente vuelve a estar vacío

    [Header("Visual del recipiente")]
    [Tooltip("Si se deja vacío, se busca automáticamente en este mismo GameObject.")]
    public SpriteRenderer cauldronRenderer;
    [Tooltip("Sprite cuando ya se agregaron los 2 elementos (ej: vaso lleno / burbujeando).")]
    public Sprite fullSprite;
    [Tooltip("Sprite al que vuelve cuando se vacía. Si se deja vacío, se usa el sprite inicial del recipiente.")]
    public Sprite emptySprite;

    private readonly List<int> storedElementIds = new List<int>();
    private readonly List<DraggableElement> storedElements = new List<DraggableElement>();
    private bool isProcessing;
    private Collider2D col;

    void Awake()
    {
        if (reactionManager == null)
            reactionManager = FindObjectOfType<ReactionManager>();

        if (cauldronRenderer == null)
            cauldronRenderer = GetComponent<SpriteRenderer>();

        if (emptySprite == null && cauldronRenderer != null)
            emptySprite = cauldronRenderer.sprite;

        col = GetComponent<Collider2D>();
    }

    public bool CanAcceptElement()
    {
        return !isProcessing && storedElementIds.Count < maxElements;
    }

    /// <summary>Llamado por DraggableElement cuando se suelta dentro del recipiente.</summary>
    public void AddElement(DraggableElement element)
    {
        if (!CanAcceptElement())
        {
            element.ReturnToStart();
            return;
        }

        int slotIndex = storedElementIds.Count;
        storedElementIds.Add(element.elementId);
        storedElements.Add(element);

        Vector3 targetPos = (slotPositions != null && slotIndex < slotPositions.Length)
            ? slotPositions[slotIndex].position
            : transform.position;

        StartCoroutine(SnapElement(element, targetPos));

        if (storedElementIds.Count >= maxElements)
        {
            if (cauldronRenderer != null && fullSprite != null)
                cauldronRenderer.sprite = fullSprite;

            StartCoroutine(ProcessMix());
        }
    }

    IEnumerator SnapElement(DraggableElement element, Vector3 targetPos)
    {
        float t = 0f;
        Vector3 origin = element.transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            if (element == null) yield break;
            element.transform.position = Vector3.Lerp(origin, targetPos, t);
            yield return null;
        }
        if (element != null) element.transform.position = targetPos;
    }

    IEnumerator ProcessMix()
    {
        isProcessing = true;
        yield return new WaitForSeconds(delayBeforeCheck);

        ReactionData result = null;
        if (reactionManager != null && storedElementIds.Count == 2)
        {
            result = reactionManager.GetReaction(storedElementIds[0], storedElementIds[1]);
        }

        yield return StartCoroutine(PlayReactionFeedback());

        OnMixResult?.Invoke(result);

        yield return new WaitForSeconds(delayBeforeClear);
        ClearCauldron();
    }

    // Pequeña animación de "burbujeo" al completarse la mezcla (sin dependencias externas)
    IEnumerator PlayReactionFeedback()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 punch = originalScale * 1.15f;
        float duration = 0.15f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, punch, t / duration);
            yield return null;
        }
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(punch, originalScale, t / duration);
            yield return null;
        }
        transform.localScale = originalScale;
    }

    /// <summary>Vacía el recipiente y destruye los elementos que estaban dentro.</summary>
    public void ClearCauldron()
    {
        foreach (var element in storedElements)
        {
            if (element != null)
                element.ConsumeAndDestroy();
        }

        storedElements.Clear();
        storedElementIds.Clear();
        isProcessing = false;

        if (cauldronRenderer != null && emptySprite != null)
            cauldronRenderer.sprite = emptySprite;

        OnCauldronCleared?.Invoke();
    }

    // Ayuda visual en el editor para comprobar que los slots están bien
    // asignados sin tener que darle Play (útil en la prueba de asignación).
    void OnDrawGizmosSelected()
    {
        if (slotPositions == null) return;
        Gizmos.color = Color.cyan;
        foreach (var slot in slotPositions)
        {
            if (slot != null) Gizmos.DrawWireSphere(slot.position, 0.15f);
        }
    }

    /// <summary>
    /// Oculta o muestra el Perol (apaga también su collider, para que
    /// mientras esté oculto no se pueda soltar nada encima).
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (cauldronRenderer != null) cauldronRenderer.enabled = visible;
        if (col != null) col.enabled = visible;
    }

    /// <summary>
    /// Oculta o muestra TODOS los Peroles que haya en la escena (normalmente
    /// solo hay uno). Llamar con false al mostrar Perdiste o abrir un panel,
    /// y con true al cerrarlo / reiniciar.
    /// </summary>
    public static void SetAllVisible(bool visible)
    {
        Cauldron[] all = FindObjectsOfType<Cauldron>();
        foreach (Cauldron cauldron in all)
        {
            cauldron.SetVisible(visible);
        }
    }
}