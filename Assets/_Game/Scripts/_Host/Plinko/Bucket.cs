using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    [Header("Lerp Properties")]
    public float duration = 12f;

    public enum Operation
    { 
        Addition,
        Multiplication,
        Double,
        Wipeout
    };

    [Header("Box Values")]
    public Operation operation;
    public int boxValue;
    private TextMeshPro boxValueLabel;

    [Header("Box Geometry")]
    public Renderer[] outerBox;


    private bool created;
    private float elapsedTime;
    private Vector3 startPos = new Vector3(24f, 0, -0.75f);
    private Vector3 endPos = new Vector3(-24f, 0, -0.75f);

    public void OnCreate(Operation op, int val, Color col)
    {
        foreach(Renderer r in outerBox)
            r.material.color = col;
        boxValueLabel = GetComponentInChildren<TextMeshPro>();
        operation = op;
        boxValue = val;

        switch(operation)
        {
            case Operation.Addition:
            case Operation.Multiplication:
                boxValueLabel.text = operation == Operation.Addition ? "+" : "x";
                boxValueLabel.text += boxValue.ToString();
                break;

            case Operation.Double:
            case Operation.Wipeout:
                boxValueLabel.text = operation == Operation.Double ? "<sprite=7>" : "<sprite=10>";
                break;
        }
        
        created = true;
        Invoke("EndLock", duration + 0.1f);
    }

    void Update()
    {
        if (created)
            PerformLerp();
    }

    private void PerformLerp()
    {
        elapsedTime += Time.deltaTime;

        float percentageComplete = elapsedTime / duration;

        this.gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, percentageComplete);
    }

    public void EndLock()
    {
        created = false;
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball")
        {
            switch (operation)
            {
                case Operation.Addition:
                    foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
                    {
                        pl.points += boxValue;
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"POINTS: {pl.points}");
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"YOU GOT +{boxValue} FOR THAT DROP!");
                        pl.prefab.playerScoreMesh.text = pl.points.ToString();
                    }

                    GameplayManager.Get.resultMesh.text = $"+{boxValue} POINTS";
                    Invoke("InvokeFade", 4f);
                    break;

                case Operation.Multiplication:
                    foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
                    {
                        pl.points *= boxValue;
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"POINTS: {pl.points}");
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"YOU GOT x{boxValue} FOR THAT DROP!");
                        pl.prefab.playerScoreMesh.text = pl.points.ToString();
                    }
                    GameplayManager.Get.resultMesh.text = $"x{boxValue} POINTS";
                    Invoke("InvokeFade", 4f);
                    break;

                case Operation.Double:
                    foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
                    {
                        pl.points *= 2;
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"POINTS: {pl.points}");
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"YOU DOUBLED UP!");
                        pl.prefab.playerScoreMesh.text = pl.points.ToString();
                    }
                    GameplayManager.Get.resultMesh.text = $"DOUBLE POINTS";
                    Invoke("InvokeFade", 4f);
                    break;

                case Operation.Wipeout:
                    foreach (PlayerObject pl in PlayerManager.Get.players.Where(x => x.wasCorrect))
                    {
                        pl.points = 0;
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.UpdateScore, $"POINTS: {pl.points}");
                        HostManager.Get.SendPayloadToClient(pl, EventLibrary.HostEventType.Information, $"YOU WIPED OUT!");
                        pl.prefab.playerScoreMesh.text = pl.points.ToString();
                    }
                    GameplayManager.Get.resultMesh.text = $"WIPEOUT";
                    Invoke("InvokeFade", 4f);
                    break;
            }
        }
    }

    private void InvokeFade()
    {
        GameplayManager.Get.resultMesh.GetComponent<TypewriterByCharacter>().StartDisappearingText();
    }
}
