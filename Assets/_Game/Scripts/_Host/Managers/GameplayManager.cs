using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Linq;
using Control;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [Header("Rounds")]
    public RoundBase[] rounds;

    [Header("Question Data")]
    public Question CurrentQuestion;
    public TextMeshProUGUI questionMesh;
    public TextMeshProUGUI resultMesh;
    public TextMeshProUGUI timerMesh;

    public enum GameplayStage
    {
        OpenLobby,
        RunGame,
        DoNothing
    };
    public GameplayStage currentStage = GameplayStage.DoNothing;

    public enum Round { None };
    public Round currentRound = Round.None;
    public int roundsPlayed = 0;

    [Header("Gameplay Config")]
    public int timePerQ = 15;
    public int countdownToQ = 3;
    public int delayBetweenSections = 5;
    public int firstQOfMid = 4;
    public int firstQOfHigh = 7;
    public int finalQ = 10;

    [Button]
    public void ProgressGameplay()
    {
        switch (currentStage)
        {
            case GameplayStage.OpenLobby:
                LobbyManager.Get.OnOpenLobby();
                currentStage++;
                break;

            case GameplayStage.RunGame:
                StartCoroutine(RunGame());
                currentStage++;
                break;

            case GameplayStage.DoNothing:
                break;
        }
    }

    IEnumerator RunGame()
    {
        for(int i = 0; i < QuestionManager.GetRoundQCount(); i++)
        {
            if (i + 1 == firstQOfMid)
                PlinkoManager.Get.bucketLevel = PlinkoManager.BucketLevel.Medium;
            else if (i + 1 == firstQOfHigh)
                PlinkoManager.Get.bucketLevel = PlinkoManager.BucketLevel.High;

            CurrentQuestion = QuestionManager.GetQuestion();

            //Empty question reached
            //Do final question
            if(string.IsNullOrEmpty(CurrentQuestion.question))
            {
                PlinkoManager.Get.bucketLevel = PlinkoManager.BucketLevel.Gamble;
                CurrentQuestion = new Question()
                {
                    question = "PRESS GAMBLE TO PLAY DOUBLE OR NOTHING!",
                    answer = "GAMBLE"
                };
            }

            for(int j = 0; j < countdownToQ; j++)
            {
                questionMesh.text = (countdownToQ - j).ToString();
                foreach (PlayerObject pl in PlayerManager.Get.players)
                {
                    pl.wasCorrect = false;
                    pl.prefab.SetBorderColor(PlayerPrefab.BorderColor.Active);
                    HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"{countdownToQ - j}");
                }
                yield return new WaitForSeconds(1f);
            }

            questionMesh.text = CurrentQuestion.question;

            if(PlinkoManager.Get.bucketLevel == PlinkoManager.BucketLevel.Gamble)
            {
                foreach (PlayerObject pl in PlayerManager.Get.players)
                    HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.MultipleChoiceQuestion, $"{CurrentQuestion.question}|{timePerQ - 1}|GAMBLE!");
            }
            else
            {
                foreach (PlayerObject pl in PlayerManager.Get.players)
                    HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SimpleQuestion, $"{CurrentQuestion.question}|{timePerQ - 1}");
            }            

            for(int t = timePerQ; t >=0; t--)
            {
                timerMesh.text = t.ToString();
                yield return new WaitForSeconds(1f);
            }
            timerMesh.text = "";

            if(PlinkoManager.Get.bucketLevel != PlinkoManager.BucketLevel.Gamble)
            {
                string[] splitAns = CurrentQuestion.answer.Split(',');
                questionMesh.text += $"\n<size=175%>{splitAns.FirstOrDefault()}</size>";

                yield return new WaitForSeconds(1f);

                string[] ans = CurrentQuestion.answer.Split(',');
                foreach (PlayerObject pl in PlayerManager.Get.players)
                {
                    HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.SingleAndMultiResult, $"{ans.FirstOrDefault()}|{(pl.wasCorrect ? "CORRECT" : "INCORRECT")}");
                    if (pl.wasCorrect)
                        pl.prefab.SetBorderColor(PlayerPrefab.BorderColor.Correct);
                    else
                        pl.prefab.SetBorderColor(PlayerPrefab.BorderColor.Incorrect);
                }
            }
            else
            {
                foreach (PlayerObject pl in PlayerManager.Get.players)
                {
                    HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"{(pl.wasCorrect ? "GOOD LUCK!" : "YOUR PENNYS ARE SAFE!")}");
                    if (pl.wasCorrect)
                        pl.prefab.SetBorderColor(PlayerPrefab.BorderColor.Correct);
                    else
                        pl.prefab.SetBorderColor(PlayerPrefab.BorderColor.Answered);
                }
            }            

            yield return new WaitForSeconds(delayBetweenSections);

            CameraLerpManager.Get.ToggleMachineAndLobby();

            yield return new WaitForSeconds(3f);

            PlinkoManager.Get.GenerateNewBall();

            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => CameraLerpManager.Get.currentPosition == CameraLerpManager.CameraPosition.Question);
            yield return new WaitForSeconds(delayBetweenSections);

            if (PlinkoManager.Get.bucketLevel == PlinkoManager.BucketLevel.Gamble)
                break;
        }

        foreach (PlayerObject pl in PlayerManager.Get.players)
            HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"Thanks for playing Plinko\n\nYou have earned {pl.points * GameplayPennys.Get.multiplyFactor} Pennys!");

        GameplayPennys.Get.UpdatePennysAndMedals();
    }
}
