using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager GM;
    public TMPro.TextMeshProUGUI CoinText;
    public Slider HealthSlider; // Reference to the UI Slider for health 0 - 1.
    public GameObject UI_Pause;
    public GameObject UI_GameIsOver;
    public GameObject UI_GameIsFinished;
    public AudioSource ButtonClick;

    private enum GameUI_State
    {
        Gameplay, Pause, GameOver, GameFinished
    }

    GameUI_State currentState;

    private void Start()
    {
        SwitchUIState(GameUI_State.Gameplay);
    }

    void Update()
    {
        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        CoinText.text = GM.playerCharacter.Coin.ToString();
    }

    private void SwitchUIState(GameUI_State state)
    {
        UI_Pause.SetActive(false);
        UI_GameIsOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        Time.timeScale = 1f; //So game is not paused

        switch(state)
        {
            case GameUI_State.Gameplay:
                break;

            case GameUI_State.Pause:
                Time.timeScale = 0f;
                UI_Pause.SetActive(true);
                break;

            case GameUI_State.GameOver:
                UI_GameIsOver.SetActive(true);
                break;

            case GameUI_State.GameFinished:
                UI_GameIsFinished.SetActive(true);
                break;
        }

        currentState = state;
    }

    public void TogglePauseUI()
    {
        if (currentState == GameUI_State.Gameplay)
            SwitchUIState(GameUI_State.Pause);
        else if (currentState == GameUI_State.Pause)
            SwitchUIState(GameUI_State.Gameplay);
    }

    public void Button_MainMenu()
    {
        GM.ReturnToMainMenu();
    }

    public void Button_Restart()
    {
        GM.Restart();
    }

    public void ShowGameOverUI()
    {
        SwitchUIState(GameUI_State.GameOver);
    }

    public void ShowGameFinishedUI()
    {
        SwitchUIState(GameUI_State.GameFinished);
    }
}
