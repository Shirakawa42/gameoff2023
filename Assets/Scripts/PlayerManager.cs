using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public List<Image> playersImages;
    public List<Sprite> sprites;
    public GameObject[] players = new GameObject[4];

    private PlayerInputManager playerInputManager;

    void Awake()
    {
        Globals.playerManager = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += OnPlayerJoined;

        playerInputManager.EnableJoining();
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (Globals.gameState != Globals.GameState.OnMenu) return; // Re-activated player on respawn triggers OnPlayerJoined

        Debug.Log("Player " + playerInput.playerIndex + " joined");
        playersImages[playerInput.playerIndex].sprite = sprites[playerInput.playerIndex];
        players[playerInput.playerIndex] = playerInput.gameObject;
        playerInput.DeactivateInput();
        playerInput.gameObject.GetComponent<ScalablePlayer>().playerIndex = playerInput.playerIndex;
    }

    public void OnStartButton()
    {
        bool hasPlayer = false;
        foreach (GameObject player in players)
        {
            if (player)
            {
                hasPlayer = true;
                break;
            }
        }

        if (!hasPlayer) return;

        playerInputManager.DisableJoining();

        SceneManager.LoadScene("Arena");
    }

    private List<Transform> ShuffleList(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
        return list;
    }

    public void OnArenaEnter()
    {
        List<Transform> spawns = new();
        GameObject spawn_points = GameObject.FindGameObjectWithTag("Arena").transform.Find("spawn_points").gameObject;
        for (int i = 0; i < spawn_points.transform.childCount; i++)
            spawns.Add(spawn_points.transform.GetChild(i));

        spawns = ShuffleList(spawns);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                continue;

            players[i].transform.position = spawns[i].position;
            players[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void EnablePlayerInput(bool enable)
    {
        foreach (GameObject player in players)
        {
            if (!player) continue;
            if (enable)
                player.GetComponent<PlayerInput>().ActivateInput();
            else
                player.GetComponent<PlayerInput>().DeactivateInput();
        }
    }

    public void RemoveAllPlayers()
    {
        foreach (GameObject player in players)
        {
            if (!player) continue;
            Destroy(player);
        }
    }
}
