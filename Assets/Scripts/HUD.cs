using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private TrashPickerBehaviour gameBehaviour;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnText;

    private void Start()
    {
        gameBehaviour.OnAction.AddListener(HandleAction);
    }

    private void HandleAction(int turn, int maxTurns, int score)
    {
        turnText.text = $"{turn}/{maxTurns}";
        scoreText.text = score.ToString();
    }
}
