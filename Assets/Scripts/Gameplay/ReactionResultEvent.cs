using UnityEngine.Events;

/// <summary>
/// UnityEvent tipado para poder engancharlo desde el Inspector (por ejemplo,
/// el ScoreManager o el compañero de UI puede arrastrar su método aquí sin
/// tocar código).
/// </summary>
[System.Serializable]
public class ReactionResultEvent : UnityEvent<ReactionData> { }
