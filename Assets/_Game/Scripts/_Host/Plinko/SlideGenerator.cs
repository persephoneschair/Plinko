using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlideGenerator : MonoBehaviour
{
    public GameObject slideToInstance;
    private List<GameObject> instancedSlides = new List<GameObject>();

    public float instantiationFrequency = 3f;
    public float delayFrequency = 1f;
    public int maximumSlides = 3;

    private void Start()
    {
        StartCoroutine(GenerateSlides());
    }

    IEnumerator GenerateSlides()
    {
        while(true)
        {
            yield return new WaitForSeconds(delayFrequency);
            if (instancedSlides.Count >= maximumSlides)
            {
                var x = instancedSlides.FirstOrDefault();
                Destroy(x);
                instancedSlides.Remove(x);
            }
            var newSlide = Instantiate(slideToInstance, this.gameObject.transform);
            newSlide.transform.localScale = GetScale();
            newSlide.transform.localEulerAngles = GetRotation();
            newSlide.transform.localPosition = GetPosition();
            instancedSlides.Add(newSlide);
            yield return new WaitForSeconds(instantiationFrequency);
        }
    }

    private Vector3 GetScale()
    {
        float[] yScale = new float[3] { 4f, 7.5f, 11f };
        return new Vector3(0.4f, Extensions.PickRandom(yScale), 2.5f);
    }

    private Vector3 GetRotation()
    {
        return new Vector3(0, 0, UnityEngine.Random.Range(0, 2) == 0 ? -29f : 29f);
    }

    private Vector3 GetPosition()
    {
        float[] yPos = new float[3] { -3.25f, 0f, 3.25f };
        return new Vector3((-18.85f + (1.75f * UnityEngine.Random.Range(0, 22))), Extensions.PickRandom(yPos), 0f);
    }
}
