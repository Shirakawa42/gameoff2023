using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaScript : MonoBehaviour
{
    public int PointsToWin = 5;
    public float TimeToRespawn = 2.5f;

    private List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        PlayerManager pManager = Globals.playerManager;
        Globals.gameState = Globals.GameState.OnArena;
        pManager.OnArenaEnter();
        for (int i = 0; i < pManager.players.Length; i++)
        {
            if (pManager.players[i] == null)
                continue;

            ScalablePlayer player = pManager.players[i].GetComponent<ScalablePlayer>();
            player.OnPlayerDied.AddListener(OnPlayerDied);
        }

        GameObject spawn_points = transform.Find("spawn_points").gameObject;
        for (int i = 0; i < spawn_points.transform.childCount; i++)
            spawnPoints.Add(spawn_points.transform.GetChild(i));
    }

    private void OnPlayerDied(ScalablePlayer killed, ScalablePlayer killer)
    {
        Debug.Log("Player " + killer + " killed " + killed);
        // TODO add point to killer
        StartCoroutine(RespawnPlayer(killed.playerIndex));
    }

    IEnumerator RespawnPlayer(int playerIndex)
    {
        yield return new WaitForSeconds(TimeToRespawn);
        GameObject player = Globals.playerManager.players[playerIndex];
        player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        player.GetComponent<ScalablePlayer>().Reset();
        player.SetActive(true);
    }
}
