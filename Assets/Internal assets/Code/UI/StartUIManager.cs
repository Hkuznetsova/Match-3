using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUIManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    private void Awake()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
