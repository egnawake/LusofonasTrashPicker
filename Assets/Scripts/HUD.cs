using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnText;

    public void ShowScore(int value)
    {
        scoreText.text = value.ToString();
    }

    public void ShowTurn(int turn, int maxTurns)
    {
        turnText.text = $"{turn}/{maxTurns}";
    }
}
