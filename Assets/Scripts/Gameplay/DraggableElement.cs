using UnityEngine;

/// <summary>
/// JUEGO 100% 2D. Se coloca en el prefab de cada "elemento" que el jugador
/// puede arrastrar (el sprite del químico). Requiere un Collider2D (para
/// detectar el mouse, NO marcado como trigger) y un SpriteRenderer.
/// La cámara principal debe ser Orthographic (Unity la pone así por defecto
/// en un proyecto 2D).
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DraggableElement : MonoBehaviour
{
    [Header("Dato del elemento (coincide con el id en Elements.json)")]
    public int elementId;

    [Header("Configuración de arrastre")]
    public float returnSpeed = 12f;

    [Tooltip("Referencia al ElementSpawner que lo creó. Se usa para avisarle que ya puede generar otra copia en cuanto este elemento sale del estante.")]
    public ElementSpawner sourceSpawner;

    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging;
    private bool isReturning;
    private Camera cam;
    private SpriteRenderer sr;
    private Collider2D col;
    private int originalSortingOrder;

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalSortingOrder = sr.sortingOrder;
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        // No permitir seguir jugando una vez que salió la pantalla de Perdiste.
        if (ReactionLogger.IsGameOver) return;

        // Fallback por si el elemento se instancia antes de que la cámara
        // principal esté lista, o si cambia de cámara en runtime.
        if (cam == null) cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("DraggableElement: no se encontró una cámara con tag MainCamera.");
            return;
        }

        isDragging = true;
        isReturning = false;
        dragOffset = transform.position - GetMouseWorldPosition();
        sr.sortingOrder = 100; // se dibuja encima de todo mientras se arrastra

        // Apenas el elemento se levanta del estante, avisamos al spawner para
        // que ya pueda generar otra copia (incluso del mismo elemento).
        if (sourceSpawner != null)
        {
            sourceSpawner.NotifyPickedUp(gameObject);
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorldPosition() + dragOffset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        sr.sortingOrder = originalSortingOrder;

        Cauldron cauldron = FindCauldronUnderneath();

        if (cauldron != null && cauldron.CanAcceptElement())
        {
            cauldron.AddElement(this);
        }
        else
        {
            ReturnToStart();
        }
    }

    void Update()
    {
        if (isReturning)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * returnSpeed);
            if (Vector3.Distance(transform.position, startPosition) < 0.02f)
            {
                transform.position = startPosition;
                isReturning = false;
            }
        }
    }

    Cauldron FindCauldronUnderneath()
    {
        // Busca si, donde soltamos el elemento, hay un collider del Perol/Matraz
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (var hit in hits)
        {
            Cauldron cauldron = hit.GetComponent<Cauldron>();
            if (cauldron != null) return cauldron;
        }
        return null;
    }

    public void ReturnToStart()
    {
        isReturning = true;
    }

    public void SetStartPosition(Vector3 pos)
    {
        startPosition = pos;
    }

    /// <summary>Llamado por el Cauldron cuando el elemento ya fue procesado.</summary>
    public void ConsumeAndDestroy()
    {
        Destroy(gameObject);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mouseScreen);
    }

    /// <summary>
    /// Oculta o muestra este elemento sin destruirlo (apaga también su
    /// Collider2D para que mientras esté oculto no se pueda ni clickear ni
    /// arrastrar, así no interfiere con paneles abiertos encima).
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (sr != null) sr.enabled = visible;
        if (col != null) col.enabled = visible;
    }

    /// <summary>
    /// Oculta o muestra TODOS los elementos que estén sueltos en la escena
    /// en este momento. Llamar con false al abrir un panel (Lista, Pedido,
    /// etc.) y con true al cerrarlo, para que nunca se sobrepongan.
    /// </summary>
    public static void SetAllVisible(bool visible)
    {
        DraggableElement[] all = FindObjectsOfType<DraggableElement>();
        foreach (DraggableElement element in all)
        {
            element.SetVisible(visible);
        }
    }
}