using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchScreen : MonoBehaviour
{
   public void OnClick_Enter(){
	   MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
	}
}
