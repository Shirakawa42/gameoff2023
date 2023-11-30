using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvelayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> hudPlayers;
    [SerializeField] private GameObject readyRound;
    [SerializeField] private GameObject startRound;
    [SerializeField] private GameObject endRound;
    [SerializeField] private GameObject winScreen;


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
        Globals.gameState = Globals.GameState.OnMenu;
        StartCoroutine(BeginRoundCoroutine());
    }

    public void OnGameEnded()
    {
        Globals.gameState = Globals.GameState.OnMenu;
        StartCoroutine(DisplayEndGameCoroutine());
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

    
    void DisplayWinScreen(int idPlayer)
    {
        Debug.Log("OverlayManager: DisplayWinScreen(): Do connection to display correct win screen");
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
        Globals.gameState = Globals.GameState.OnArena;

        // Debug
        StartCoroutine(DebugDisplayWinScreen(5f));
    }

    IEnumerator DisplayEndGameCoroutine()
    {
        endRound.SetActive(true);
        yield return new WaitForSeconds(2f);

        // display win screen 
        DisplayWinScreen(0);
        winScreen.SetActive(true);
    }

    IEnumerator DebugDisplayWinScreen(float secondsBeforeDebug)
    {
        yield return new WaitForSeconds(secondsBeforeDebug);

        OnGameEnded();
    }

}
