using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OvelayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> hudPlayers;
    [SerializeField] private GameObject readyRound;
    [SerializeField] private GameObject startRound;
    [SerializeField] private GameObject endRound;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI winnerPlayerText;


    // OnArenaEnter()
    /*
     * Afficher le nbr de joueur de hud
     * Afficher le Ready X seconde 
     * Cacher Ready
     * Afficher Fight 1 seconde
     * Cacher Fight
     *
     */

    private void Start()
    {
        Globals.ovelayManager = this;
        Setup();
    }

    public void OnArenaEnter()
    {
        StartCoroutine(BeginRoundCoroutine());
    }

    public void OnGameEnded(int winnerId)
    {
        Globals.gameState = Globals.GameState.OnMenu;
        StartCoroutine(DisplayEndGameCoroutine(winnerId));
    }

    // Hide every overlay element
    void Setup()
    {
        foreach (GameObject hud in hudPlayers) hud.SetActive(false);
        readyRound.SetActive(false);
        startRound.SetActive(false);
        endRound.SetActive(false);
        winScreen.SetActive(false);
    }

    
    public void DisplayHUDPlayer(int idNumber)
    {
        hudPlayers[idNumber].SetActive(true);
    }
    
    private void DisplayWinScreen(int winnerId)
    {
        winnerPlayerText.text = "Player " + winnerId + " win !";
        winScreen.SetActive(true);
    } 


    IEnumerator BeginRoundCoroutine()
    {
        readyRound.SetActive(true);
        yield return new WaitForSeconds(2f);
        readyRound.SetActive(false);
        StartCoroutine(StartFightCoroutine());
    }

    IEnumerator StartFightCoroutine()
    {
        startRound.SetActive(true);
        yield return new WaitForSeconds(1f);
        startRound.SetActive(false);

        // Allows players to move
        Globals.playerManager.EnablePlayerInput(true);
    }

    IEnumerator DisplayEndGameCoroutine(int winnerId)
    {
        endRound.SetActive(true);
        yield return new WaitForSeconds(2f);

        // display win screen 
        DisplayWinScreen(winnerId);
    }
}
