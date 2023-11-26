using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MenuManager
{
	public static bool IsInitialised { get; private set;}
	public static GameObject launchScreen, mainMenu, htpScreen, creditScreen;

	public static void Init(){
		GameObject canvas = GameObject.Find("Canvas");
		launchScreen = canvas.transform.Find("LaunchScreen").gameObject;
		mainMenu = canvas.transform.Find("MainMenu").gameObject;
		htpScreen = canvas.transform.Find("HowToPlay").gameObject;
		creditScreen = canvas.transform.Find("Credits").gameObject;

		IsInitialised = true;
	}

	public static void OpenMenu(Menu menu, GameObject callingMenu){
		if (!IsInitialised)
			Init();

		switch(menu){
			case Menu.LAUNCH_SCREEN:
				launchScreen.SetActive(true);
				break;
			case Menu.MAIN_MENU:
				mainMenu.SetActive(true);
				break;
			case Menu.HTP:
				htpScreen.SetActive(true);
				break;
			case Menu.CREDITS:
				creditScreen.SetActive(true);
				break;
		}

		callingMenu.SetActive(false);

	}
}