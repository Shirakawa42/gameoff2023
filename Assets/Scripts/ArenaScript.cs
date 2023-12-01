using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaScript : MonoBehaviour
{
    public int PointsToWin = 5;
    public float TimeToRespawn = 2.5f;

    private List<Transform> spawnPoints = new List<Transform>();
    private int[] players_score = new int[4];
    private PlayerManager pManager = null;

    void Start()
    {
        pManager = Globals.playerManager;
        OvelayManager overlayManager = Globals.ovelayManager;
        Globals.gameState = Globals.GameState.OnArena;
        pManager.OnArenaEnter();
        //TODO reset players_score
        for (int i = 0; i < pManager.players.Length; i++)
        {
            if (pManager.players[i] == null)
                continue;

            ScalablePlayer player = pManager.players[i].GetComponent<ScalablePlayer>();
            player.OnPlayerDied.AddListener(OnPlayerDied);
            player.OnScaleChanged.AddListener(Globals.hud.SetSize);
            player.Reset();

            overlayManager.DisplayHUDPlayer(i);
        }

        overlayManager.OnArenaEnter();

        GameObject spawn_points = transform.Find("spawn_points").gameObject;
        Debug.Log("spawn points: " + spawn_points);
        for (int i = 0; i < spawn_points.transform.childCount; i++)
            spawnPoints.Add(spawn_points.transform.GetChild(i));
    }

    private void OnPlayerDied(ScalablePlayer killed, ScalablePlayer killer)
    {
        Globals.hud.AddPoint(killer.playerIndex);
        players_score[killer.playerIndex] += 1;
        if (players_score[killer.playerIndex] >= PointsToWin)
        {
            WinGame(killer.playerIndex);
        }
        else
        {
            StartCoroutine(RespawnPlayer(killed.playerIndex));
        }
    }

    private void WinGame(int winnerIndex)
    {
        Debug.Log("Player " + winnerIndex + " wins !");
        Globals.gameState = Globals.GameState.MatchEnd;
        // Deactivate all players
        foreach (GameObject player in pManager.players)
        {
            if (!player) continue;
            player.SetActive(false);
        }
        // Show win screen
        Globals.ovelayManager.OnGameEnded(winnerIndex+1);
    }

    public void BackToMenu()
    {
        Destroy(GameObject.Find("WwiseGlobal"));
        pManager.RemoveAllPlayers();
        Destroy(pManager.gameObject);
        Globals.gameState = Globals.GameState.OnMenu;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator RespawnPlayer(int playerIndex)
    {
        yield return new WaitForSeconds(TimeToRespawn);
        GameObject player = Globals.playerManager.players[playerIndex];
        player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        player.GetComponent<ScalablePlayer>().Reset();
        Globals.hud.ResetHp(playerIndex);
        player.SetActive(true);
    }
}
