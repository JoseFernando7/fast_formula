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

    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging;
    private bool isReturning;
    private Camera cam;
    private SpriteRenderer sr;
    private int originalSortingOrder;

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        originalSortingOrder = sr.sortingOrder;
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
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
}
