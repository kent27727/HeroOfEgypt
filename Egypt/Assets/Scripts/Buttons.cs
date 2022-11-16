using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
	public Button restartButton;
	
	private void Start()
	{
		Time.timeScale = 0;
		

	}


	



	public void SetVariables()
	{
		Time.timeScale = 1;
		


	}

	public void Restart()
	{
		SceneManager.LoadScene("Level");
	}

	public void Shop()
	{
		SceneManager.LoadScene("Shop");
	}

	
}
