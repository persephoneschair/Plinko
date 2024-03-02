using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketGenerator : SingletonMonoBehaviour<BucketGenerator>
{
    public GameObject bucketObj;
    public float generationFrequency = 1f;
    public Color[] bucketColors;
    private int currentBoxCol = 0;
    private int fiftyFiftyBox = 0;

    void Start()
    {
        StartCoroutine(GenerateBuckets());
    }

    IEnumerator GenerateBuckets()
    {
        while(true)
        {
            yield return new WaitForSeconds(generationFrequency);
            var bObj = Instantiate(bucketObj, this.transform);
            Bucket buck = bObj.GetComponent<Bucket>();

            switch(PlinkoManager.Get.bucketLevel)
            {
                case PlinkoManager.BucketLevel.Low:
                    buck.OnCreate(Bucket.Operation.Addition, Extensions.PickRandom(PlinkoManager.Get.lowLevelBuckets), bucketColors[currentBoxCol]);
                    break;

                case PlinkoManager.BucketLevel.Medium:
                    int value = Extensions.PickRandom(PlinkoManager.Get.midLevelBuckets);
                    Bucket.Operation op = Bucket.Operation.Addition;
                    if (value > 100)
                    {
                        value /= 100;
                        op = Bucket.Operation.Multiplication;
                    }                        
                    buck.OnCreate(op, value, bucketColors[currentBoxCol]);
                    break;

                case PlinkoManager.BucketLevel.High:
                    value = Extensions.PickRandom(PlinkoManager.Get.highLevelBuckets);
                    op = Bucket.Operation.Addition;
                    if (value > 100)
                    {
                        value /= 100;
                        op = Bucket.Operation.Multiplication;
                    }
                    buck.OnCreate(op, value, bucketColors[currentBoxCol]);
                    break;

                case PlinkoManager.BucketLevel.Gamble:
                    buck.OnCreate(fiftyFiftyBox == 0 ? Bucket.Operation.Double : Bucket.Operation.Wipeout, 0, bucketColors[currentBoxCol]);
                    break;
            }
            currentBoxCol = (currentBoxCol + 1) % bucketColors.Length;
            fiftyFiftyBox = (fiftyFiftyBox + 1) % 2;
        }
    }
}
