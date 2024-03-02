using Control;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerObject
{
    public string playerClientID;
    public Player playerClientRef;
    public Podium podium;
    public string otp;
    public string playerName;

    public PlayerPrefab prefab;

    public string twitchName;
    public Texture profileImage;

    public int points;
    public bool wasCorrect;

    public PlayerObject(Player pl, string name)
    {
        playerClientRef = pl;
        otp = OTPGenerator.GenerateOTP();
        playerName = name;
        points = 0;
        //podium = Podiums.GetPodiums.podia.FirstOrDefault(x => x.containedPlayer == null);
        //podium.containedPlayer = this;
    }

    public void ApplyProfilePicture(string name, Texture tx, bool bypassSwitchAccount = false)
    {
        //Player refreshs and rejoins the same game
        if (PlayerManager.Get.players.Count(x => (!string.IsNullOrEmpty(x.twitchName)) && x.twitchName.ToLowerInvariant() == name.ToLowerInvariant()) > 0 && !bypassSwitchAccount)
        {
            PlayerObject oldPlayer = PlayerManager.Get.players.FirstOrDefault(x => x.twitchName.ToLowerInvariant() == name.ToLowerInvariant());
            if (oldPlayer == null)
                return;

            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.SecondInstance, "");

            oldPlayer.playerClientID = playerClientID;
            oldPlayer.playerClientRef = playerClientRef;
            oldPlayer.playerName = playerName;
            //oldPlayer.podium.playerNameMesh.text = playerName;

            otp = "";
            //podium.containedPlayer = null;
            //podium = null;
            playerClientRef = null;
            playerName = "";

            if (PlayerManager.Get.pendingPlayers.Contains(this))
                PlayerManager.Get.pendingPlayers.Remove(this);

            HostManager.Get.SendPayloadToClient(oldPlayer, EventLibrary.HostEventType.Validated, $"{oldPlayer.playerName}|{oldPlayer.points.ToString()}|{oldPlayer.twitchName}");
            //HostManager.Get.UpdateLeaderboards();
            return;
        }
        otp = "";
        twitchName = name.ToLowerInvariant();
        profileImage = tx;
        HostManager.Get.SendPayloadToClient(this, EventLibrary.HostEventType.Validated, $"{playerName}|{points.ToString()}|{twitchName}");
        PlayerManager.Get.OnPlayerHasValidated(this);
        SaveManager.BackUpData();
    }

    public void HandlePlayerScoring(string[] submittedAnswers)
    {
        wasCorrect = Extensions.Spellchecker(submittedAnswers.FirstOrDefault(), GameplayManager.Get.CurrentQuestion.answer.Split(',').ToList());
        prefab.SetBorderColor(PlayerPrefab.BorderColor.Answered);
    }
}
