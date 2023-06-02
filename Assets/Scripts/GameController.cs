using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TrashPickerBehaviour trashPickerBehaviourPrefab;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject highScoresScreen;

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

    private void Start()
    {
        AttachButtonListeners();

        mainCamera = Camera.main;
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

    private void HandleGameOver()
    {
        gameOverScreen.SetActive(true);
    }

    private void ShowHighScores()
    {
        mainMenu.SetActive(false);

        highScoresScreen.SetActive(true);
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
