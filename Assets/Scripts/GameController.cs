using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private TrashPickerBehaviour trashPickerBehaviourPrefab;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject highScoresScreen;
    [SerializeField] private GameObject gameOverHighScoreDisplay;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text gameOverHighScoreText;

    [SerializeField] private TMP_Text[] highScoreFields;

    [SerializeField] private Button newHumanGameButton;
    [SerializeField] private Button newAIGameButton;
    [SerializeField] private Button highScoresButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button gameOverReturnButton;
    [SerializeField] private Button highScoresReturnButton;

    [SerializeField] private float cameraSlideDuration = 1f;
    [SerializeField] private AnimationCurve cameraSlideCurve;
    [SerializeField] private Transform cameraSlideInStart;
    [SerializeField] private Transform cameraSlideInEnd;

    private Camera mainCamera;
    private TrashPickerBehaviour trashPickerBehaviour;

    private List<int> highScores;

    private void Start()
    {
        AttachButtonListeners();

        mainCamera = Camera.main;

        highScores = new List<int>();
    }

    private void StartHumanGame()
    {
        StartGame(false);
    }

    private void StartAIGame()
    {
        StartGame(true);
    }

    private void StartGame(bool aiPlaying)
    {
        mainMenu.SetActive(false);

        trashPickerBehaviour = Instantiate(trashPickerBehaviourPrefab);
        trashPickerBehaviour.OnGameOver.AddListener(HandleGameOver);
        trashPickerBehaviour.IsAI = aiPlaying;

        StartCoroutine(CameraSlideIn());
    }

    private void HandleGameOver(int score)
    {
        gameOverScreen.SetActive(true);

        gameOverScoreText.text = score.ToString();

        int placement = AddHighScore(score);
        bool showHighScore = false;

        if (placement > 0)
        {
            showHighScore = true;
            gameOverHighScoreText.text = $"#{placement}";
        }

        gameOverHighScoreDisplay.SetActive(showHighScore);
    }

    private void ShowHighScores()
    {
        mainMenu.SetActive(false);

        highScoresScreen.SetActive(true);
    }

    private int AddHighScore(int score)
    {
        int placement = 0;
        int index = -1;

        // If this score is greater than a score on the table,
        // store that index and insert the new score there
        for (int i = 0; i < highScores.Count; i++)
        {
            if (score > highScores[i])
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            highScores.Insert(index, score);
            placement = index + 1;
        }

        // If score did not beat any previous score and
        // there is space in the scores list, add score
        if (placement == 0 && highScores.Count < 6)
        {
            highScores.Add(score);
            placement = highScores.Count;
        }

        // Limit scores list size
        if (highScores.Count > 6)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        // Update view with current score list
        for (int i = 0; i < highScores.Count; i++)
        {
            highScoreFields[i].text = highScores[i].ToString();
        }

        return placement;
    }

    private void OpenMainMenu()
    {
        gameOverScreen.SetActive(false);
        highScoresScreen.SetActive(false);

        if (trashPickerBehaviour != null)
        {
            Destroy(trashPickerBehaviour.gameObject);
        }

        mainMenu.SetActive(true);
    }

    private void AttachButtonListeners()
    {
        newHumanGameButton.onClick.AddListener(StartHumanGame);
        newAIGameButton.onClick.AddListener(StartAIGame);
        highScoresButton.onClick.AddListener(ShowHighScores);
        gameOverReturnButton.onClick.AddListener(OpenMainMenu);
        highScoresReturnButton.onClick.AddListener(OpenMainMenu);
        quitButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator CameraSlideIn()
    {
        float timer = 0;

        while (timer < cameraSlideDuration)
        {
            float pct = cameraSlideCurve.Evaluate(timer / cameraSlideDuration);
            Vector3 position = Vector3.Lerp(cameraSlideInStart.position,
                cameraSlideInEnd.position, pct);
            mainCamera.transform.position = position;

            timer += Time.deltaTime;

            yield return null;
        }
    }
}
