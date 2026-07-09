using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public InputAction successAction;
    public InputAction failedAction;

    private ReactionManager reactionManager;
    private ScoreManager scoreManager;
    public ReactionData currentOrder;
    private List<ReactionData> validReactions;

    public float timeRemaining = 15.0f;
    public bool isDone = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        successAction.Enable();
        failedAction.Enable();

        reactionManager = GetComponent<ReactionManager>();
        scoreManager = GetComponent<ScoreManager>();

        validReactions = reactionManager.GetAllReactions().FindAll(reaction => reaction.id != -1 && reaction.type == "Success");

        GenerateOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDone) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            OrderFailed();
        }

        if (successAction.triggered)
        {
            OrderSuccess();
        }

        if (failedAction.triggered)
        {
            OrderIncorrect();
        }
    }

    public void GenerateOrder()
    {
        currentOrder = validReactions[Random.Range(0, validReactions.Count)];

        timeRemaining = 15f;
        isDone = false;

        Debug.Log("Nuevo Pedido: " + currentOrder.resultName);
    }

    void OrderFailed()
    {
        isDone = true;

        Debug.Log("El cliente se fue");
        scoreManager.OnOrderExpired();

        Invoke(nameof(GenerateOrder), 2f);
    }

    void OrderSuccess()
    {
        isDone = true;

        Debug.Log("El pedido fue entregado correctamente");
        scoreManager.OnCorrectMix(timeRemaining);

        Invoke(nameof(GenerateOrder), 2f);
    }

    void OrderIncorrect()
    {
        isDone = true;

        Debug.Log("El pedido fue entregado pero la mezcla es incorrecta");
        scoreManager.OnWrongMix(timeRemaining);

        Invoke(nameof(GenerateOrder), 2f);
    }
}
