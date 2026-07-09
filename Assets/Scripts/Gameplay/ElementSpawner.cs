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
    public GameObject draggableElementPrefab;
    public Transform spawnPoint;

    private GameObject currentInstance;

    // Permite clickear directo el ícono del estante (requiere Collider2D en este mismo GameObject)
    void OnMouseDown()
    {
        SpawnElement();
    }

    public void SpawnElement()
    {
        // Evita generar una segunda copia mientras la anterior sigue en pantalla
        if (currentInstance != null) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        currentInstance = Instantiate(draggableElementPrefab, pos, Quaternion.identity);

        DraggableElement draggable = currentInstance.GetComponent<DraggableElement>();
        if (draggable != null)
        {
            draggable.elementId = elementId;
            draggable.SetStartPosition(pos);
        }
    }
}
