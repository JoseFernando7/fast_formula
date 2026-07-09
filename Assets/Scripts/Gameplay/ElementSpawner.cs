using UnityEngine;

/// <summary>
/// Va en cada botón/ícono del estante de elementos (Sprite/Image elementos
/// químicos, según tu diagrama de UI). Al hacer click, genera una copia
/// arrastrable de ese elemento para que el jugador la lleve al Perol.
///
/// Se puede llamar desde un EventTrigger (PointerDown) o desde un Button.
/// </summary>
public class ElementSpawner : MonoBehaviour
{
    [Header("Configuración del elemento a generar")]
    public int elementId;
    public Sprite icon;
    public GameObject draggableElementPrefab;
    public Transform spawnPoint;

    private GameObject currentInstance;
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Permite clickear directo el ícono del estante (requiere Collider2D en este mismo GameObject)
    void OnMouseDown()
    {
        SpawnElement();
    }

    public void SpawnElement()
    {
        // No generar más elementos si ya se mostró la pantalla de Perdiste.
        if (ReactionLogger.IsGameOver) return;

        // Evita generar una segunda copia mientras la anterior sigue en el
        // estante sin haberse movido todavía (para no amontonar sprites).
        if (currentInstance != null) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        currentInstance = Instantiate(draggableElementPrefab, pos, Quaternion.identity);

        DraggableElement draggable = currentInstance.GetComponent<DraggableElement>();
        if (draggable != null)
        {
            draggable.elementId = elementId;
            draggable.SetStartPosition(pos);
            draggable.sourceSpawner = this;
        }

        // Aplica el sprite real del elemento (si no se asigna, se queda con el sprite por defecto del prefab)
        if (icon != null)
        {
            SpriteRenderer sr = currentInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = icon;
            }
        }
    }

    /// <summary>
    /// Llamado por el DraggableElement en cuanto el jugador empieza a
    /// arrastrarlo (ya salió del estante). Libera este spawner para poder
    /// generar otra copia del mismo elemento, permitiendo mezclas que
    /// necesitan el mismo elemento repetido (ej: H+H, C+C, Au+Au).
    /// </summary>
    public void NotifyPickedUp(GameObject instance)
    {
        if (currentInstance == instance)
            currentInstance = null;
    }

    /// <summary>
    /// Oculta o muestra este ícono del estante (apaga sprite y collider, así
    /// mientras está oculto tampoco se puede clickear para generar más).
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (sr != null) sr.enabled = visible;
        if (col != null) col.enabled = visible;
    }

    /// <summary>
    /// Oculta o muestra TODOS los íconos del estante (los 10 elementos).
    /// Llamar con false al mostrar Perdiste o abrir un panel (Lista, etc.),
    /// y con true al cerrarlo / reiniciar.
    /// </summary>
    public static void SetAllVisible(bool visible)
    {
        ElementSpawner[] all = FindObjectsOfType<ElementSpawner>();
        foreach (ElementSpawner spawner in all)
        {
            spawner.SetVisible(visible);
        }
    }
}