﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    public GameObject victoryText;

    [Tooltip("Container for Matches UI")]
    [SerializeField]
    private CanvasRenderer matchesUI;

    [Tooltip("Search for match button")]
    [SerializeField]
    private Button searchForMatch;

    public Image victoryImg;
    public Sprite tie;
    public Sprite win;
    public Sprite lose;

    private const float MatchSearchPollTime = 2.0f;
    private const float CurrentStatusPollTime = 1.0f;
    private const string MatchSearchable = "Start Battle!";
    private const string SearchingForMatch = "Searching for match...";

    void Awake()
    {
    }

    void Start()
    {
    }

    public void FindMatch()
    {
        fight.gameObject.SetActive(true);
        var startMatchButtonText = searchForMatch.GetComponentInChildren<Text>();
        if (startMatchButtonText)
        {
            startMatchButtonText.text = SearchingForMatch;
        }
        searchForMatch.interactable = false;
        searchForMatch.gameObject.SetActive(false);
        var player = CreatePlayer();
        ServerManager.Instance.FindMatch(player, OnMatchFound, MatchSearchPollTime);
    }
    public Animator fight;
    private void OnMatchFound(Match match)
    {
        var playerData = match.player0.id == FitbitRestClient.Instance.GetUserId() ? match.player0.playerdata : match.player1.playerdata;
        var opponentData = match.player1.id == FitbitRestClient.Instance.GetUserId() ? match.player0.playerdata : match.player1.playerdata;
        //AdventureStats.SetStat(FoodType.Dairy, playerData.dairy - opponentData.dairy);
        //AdventureStats.SetStat(FoodType.Fruit, playerData.fruit - opponentData.fruit);
        //AdventureStats.SetStat(FoodType.Grain, playerData.grain - opponentData.grain);
        //AdventureStats.SetStat(FoodType.Protien, playerData.protein - opponentData.protein);
        //AdventureStats.SetStat(FoodType.Sweets, playerData.sweets - opponentData.sweets);
        //AdventureStats.SetStat(FoodType.Vegetable, playerData.vegetable - opponentData.vegetable);
        //int total = AdventureStats.Dairy + AdventureStats.Fruit + AdventureStats.Grain + AdventureStats.Protein + AdventureStats.Sweets + AdventureStats.Vegetable;
        int total = playerData.sweets - opponentData.sweets;
        victoryImg.gameObject.SetActive(true);
        if (total > 0)
        {
            Debug.Log("WIN");
            victoryImg.sprite = win;
        }
        else if (total == 0)
        {
            Debug.Log("TIE");
            victoryImg.sprite = tie;
        }
        else
        {
            Debug.Log("LOSE");
            victoryImg.sprite = lose;
        }
        fight.gameObject.SetActive(false);
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.dairy - opponentData.dairy));
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.fruit - opponentData.fruit));
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.grain - opponentData.grain));
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.protein - opponentData.protein));
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.sweets - opponentData.sweets));
        AdventureStats.SetStat(FoodType.Dairy, Mathf.Max(0, playerData.vegetable - opponentData.vegetable));
        // TODO: Determine the winner
        // Play the animation
        //var startMatchButtonText = searchForMatch.GetComponentInChildren<Text>();
        //if (startMatchButtonText)
        //{
        //    startMatchButtonText.text = MatchSearchable;
        //}
        //searchForMatch.interactable = true;
        //matches.Add(match);
    }

    private Player CreatePlayer()
    {
        var player = new Player();
        player.id = FitbitRestClient.Instance.GetUserId();
        player.playerdata = PlayerDataHelper.GetPlayerData();
        return player;
    }
}