using UnityEngine;

/// <summary>
/// Script TEMPORAL solo para ver en consola el resultado de la mezcla
/// mientras no hay UI/Score todavía. Engánchalo al evento OnMixResult
/// del Cauldron desde el Inspector.
/// </summary>
public class ReactionLogger : MonoBehaviour
{
    public void HandleReaction(ReactionData data)
    {
        if (data == null)
        {
            Debug.Log("Resultado: NULL (no se encontró ninguna reacción, ni siquiera la de fallo)");
            return;
        }

        if (data.id == -1)
        {
            Debug.Log($"❌ Mezcla fallida (tipo: {data.type})");
        }
        else
        {
            Debug.Log($"✅ Reacción exitosa: {data.resultName} (tipo: {data.type})");
        }
    }
}
