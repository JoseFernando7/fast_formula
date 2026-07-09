using System.Collections.Generic;
using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    private List<ReactionData> reactions;
    private List<ElementData> elements;

    void Awake()
    {
        // Reactions
        TextAsset reactionsJson = Resources.Load<TextAsset>("Reactions");

        ReactionDatabase reactionDb = JsonUtility.FromJson<ReactionDatabase>(reactionsJson.text);

        reactions = reactionDb.reactions;

        Debug.Log($"Reactions: {reactions.Count}");

        // Elements
        TextAsset elementsJson = Resources.Load<TextAsset>("Elements");

        ElementDatabase elementsDb = JsonUtility.FromJson<ElementDatabase>(elementsJson.text);

        elements = elementsDb.elements;

        Debug.Log($"Elements: {elements.Count}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Mezclaste {elements[0].name} y {elements[1].name} y obtuviste {GetReaction(0, 1).resultName}");
        Debug.Log($"Mezclaste {elements[1].name} y {elements[0].name} y obtuviste {GetReaction(1, 0).resultName}");
        Debug.Log($"Mezclaste {elements[3].name} y {elements[4].name} y obtuviste {GetReaction(3, 4).resultName}");
        Debug.Log($"Mezclaste {elements[0].name} y {elements[5].name} y obtuviste {GetReaction(0, 5).resultName}");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public ReactionData GetReaction(int a, int b)
    {
        foreach (ReactionData reaction in reactions)
        {
            if (reaction.id == -1)
                continue;

            if (
                (reaction.ingredients[0] == a && reaction.ingredients[1] == b) ||
                (reaction.ingredients[0] == b && reaction.ingredients[1] == a)
                )
            {
                return reaction;
            }
        }

        foreach (ReactionData reaction in reactions)
        {
            if (reaction.id == -1) return reaction;
        }

        return null;
    }
}
