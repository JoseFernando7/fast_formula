using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;

    public int Score { get; private set; }

    public void Update()
    {
        scoreText.text = Score.ToString();
    }

    public void AddPoints(int points)
    {
        Score += points;

        Debug.Log($"Score: {Score}");
    }

    public void RemovePoints(int points)
    {
        Score -= points;

        if (Score < 0) Score = 0;

        Debug.Log($"Score: {Score}");
    }

    public void OnCorrectMix(float timeRemaining)
    {
        AddPoints(Mathf.CeilToInt(timeRemaining));
    }

    public void OnWrongMix(float timeRemaining)
    {
        RemovePoints(Mathf.CeilToInt(timeRemaining / 2f));
    }

    public void OnOrderExpired()
    {
        RemovePoints(15);
    }
}
