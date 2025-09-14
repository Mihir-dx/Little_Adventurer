using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    private bool isGameOver;
    public GameUI_Manager gameUI_Manager;
    public AudioSource ButtonClick;
    private bool isPaused = false;

    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();

    }

    private void Start()
    {
        HideCursor();
    }

    private void GameOver()
    {
        ShowCursor();
        gameUI_Manager.ShowGameOverUI();
    }

    public void GameisFinished()
    {
        ShowCursor();
        gameUI_Manager.ShowGameFinishedUI();
    }

    void Update()
    {
        if (isGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))

            if (isPaused)
            {
                isPaused = false;
                HideCursor();
                gameUI_Manager.TogglePauseUI();
            }
            else
            {
                isPaused = true;
                ShowCursor();
                gameUI_Manager.TogglePauseUI();
            }

        if (playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            isGameOver = true;
            GameOver();
        }
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(PlayThen(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }));
    }

    public void Restart()
    {
        StartCoroutine(PlayThen(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }));
    }

    private IEnumerator PlayThen(System.Action after)
    {
        if (ButtonClick != null && ButtonClick.clip != null)
        {
            ButtonClick.Play();
            yield return new WaitForSecondsRealtime(ButtonClick.clip.length); // Wait for the sound clip to finish
        }
        after?.Invoke();
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
