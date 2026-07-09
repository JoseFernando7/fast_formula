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

        // Debug.Log($"Reactions: {reactions.Count}");

        // Elements
        TextAsset elementsJson = Resources.Load<TextAsset>("Elements");

        ElementDatabase elementsDb = JsonUtility.FromJson<ElementDatabase>(elementsJson.text);

        elements = elementsDb.elements;

        // Debug.Log($"Elements: {elements.Count}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    public List<ReactionData> GetAllReactions()
    {
        return reactions;
    }
}
