using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void clickBackButton() {
        Destroy(GameObject.Find("WwiseGlobal"));
        Globals.playerManager.RemoveAllPlayers();
        Destroy(Globals.playerManager.gameObject);
        Globals.gameState = Globals.GameState.OnMenu;
        SceneManager.LoadScene("MainMenu");
    }
}
