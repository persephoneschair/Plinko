using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlinkoManager : SingletonMonoBehaviour<PlinkoManager>
{
    public GameObject ballToSpawn;

    public enum BucketLevel
    {
        Low,
        Medium,
        High,
        Gamble
    }
    public BucketLevel bucketLevel = BucketLevel.Low;

    public int[] lowLevelBuckets = new int[7] { 1, 1, 2, 2, 3, 3, 5 };
    public int[] midLevelBuckets = new int[7] { 3, 3, 5, 10, 20, 200, 300 };
    public int[] highLevelBuckets = new int[9] { 10, 10, 20, 200, 50, 300, 100, 100, 500 };

    [Button]
    public void GenerateNewBall()
    {
        CamSwitcher.movedToQuestion = false;
        CamSwitcher.movedToResult = false;
        var b = Instantiate(ballToSpawn, this.transform);
        b.transform.localPosition = new Vector3(UnityEngine.Random.Range(-20f, 20f), 17f, 1.5f);
    }
}
