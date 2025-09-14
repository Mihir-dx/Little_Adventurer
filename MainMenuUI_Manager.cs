using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI_Manager : MonoBehaviour
{
    public AudioSource ButtonClick;
    public GameObject instructionsPanel;

    public void Button_Start()
    {
        StartCoroutine(PlayThen(() =>
        {
            SceneManager.LoadScene("GameScene");
        }));
    }

    public void Button_Quit()
    {
        StartCoroutine(PlayThen(() =>
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }));
    }

    private IEnumerator PlayThen(System.Action after)
    {
        if (ButtonClick != null && ButtonClick.clip != null)
        {
            ButtonClick.Play();
            yield return new WaitForSeconds(ButtonClick.clip.length); // Wait for the sound clip to finish
        }
        after?.Invoke();
    }

    public void ShowInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }
    }

    public void HideInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }
    }
}
