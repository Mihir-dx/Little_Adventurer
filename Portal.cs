using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
