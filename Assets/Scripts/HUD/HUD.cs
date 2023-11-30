using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Sprite points_empty;
    public Sprite points_full;

    public List<Image> player1_points;
    public List<Image> player2_points;
    public List<Image> player3_points;
    public List<Image> player4_points;

    public Slider player1_hpslider;
    public Slider player2_hpslider;
    public Slider player3_hpslider;
    public Slider player4_hpslider;

    public GameObject hud_player1;
    public GameObject hud_player2;
    public GameObject hud_player3;
    public GameObject hud_player4;

    private Slider[] sliders;
    private GameObject[] huds;
    private List<Image>[] points;

    void Awake()
    {
        Globals.hud = this;
    }

    void Start()
    {
        hud_player1.SetActive(false);
        hud_player2.SetActive(false);
        hud_player3.SetActive(false);
        hud_player4.SetActive(false);

        sliders = new Slider[4];
        sliders[0] = player1_hpslider;
        sliders[1] = player2_hpslider;
        sliders[2] = player3_hpslider;
        sliders[3] = player4_hpslider;

        huds = new GameObject[4];
        huds[0] = hud_player1;
        huds[1] = hud_player2;
        huds[2] = hud_player3;
        huds[3] = hud_player4;

        points = new List<Image>[4];
        points[0] = player1_points;
        points[1] = player2_points;
        points[2] = player3_points;
        points[3] = player4_points;

        PlayerManager pManager = Globals.playerManager;
        for (int i = 0; i < pManager.players.Length; i++)
        {
            if (pManager.players[i] == null)
                continue;

            huds[i].SetActive(true);
            sliders[i].value = sliders[i].maxValue;
            for (int j = 0; j < points[i].Count; j++)
                points[i][j].sprite = points_empty;
        }
    }

    public void AddPoint(int playerIndex)
    {
        for (int i = 0; i < points[playerIndex].Count; i++)
        {
            if (points[playerIndex][i].sprite == points_empty)
            {
                points[playerIndex][i].sprite = points_full;
                break;
            }
        }
    }

    public int TakeDamage(int playerIndex)
    {
        sliders[playerIndex].value -= 1;
        return (int)sliders[playerIndex].value;
    }

    public void ResetHp(int playerIndex)
    {
        sliders[playerIndex].value = sliders[playerIndex].maxValue;
    }
}
