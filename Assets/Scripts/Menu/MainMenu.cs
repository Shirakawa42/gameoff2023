using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void OnClick_Battle(){
		SceneManager.LoadScene("Menu");
	}

	public void OnClick_HTP(){
		MenuManager.OpenMenu(Menu.HTP, gameObject);
	}

	public void OnClick_Credits(){
		MenuManager.OpenMenu(Menu.CREDITS, gameObject);
	}

	public void OnClick_Quit(){
		Debug.Log ("QUIT");
		Application.Quit();
	}

}
