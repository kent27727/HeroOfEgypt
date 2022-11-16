using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControlButton : MonoBehaviour
{
    enum TargetScene
	{
        Next,
        Previous,
        MainMenu,
	}

    [SerializeField] TargetScene targetScene;
    Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
		switch (targetScene)
		{
            case TargetScene.MainMenu:
                button.onClick.AddListener(() => SceneController.LoadMainSecene());
                break;

            case TargetScene.Next:
                button.onClick.AddListener(() => SceneController.LoadNextScene());
                break;

            case TargetScene.Previous:
                button.onClick.AddListener(() => SceneController.LoadPreviousScene());
                break;
        }
	}

    public void Play()
	{
        SceneManager.LoadScene("Level");
	}
    public void QuitGame()
	{
        Application.Quit();
	}

    
}
