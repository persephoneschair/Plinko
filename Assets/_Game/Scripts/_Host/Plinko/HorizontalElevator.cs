using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalElevator : MonoBehaviour
{
    public float changeRangeLower = 0.5f;
    public float changeRangeUpper = 2.5f;
    public float speed = 1f;
    public GameObject[] floors;

    private void Start()
    {
        GetComponentInChildren<Animator>().speed = this.speed;
        foreach(GameObject go in floors)
            StartCoroutine(CycleFloors(go));
    }

    IEnumerator CycleFloors(GameObject go)
    {
        while(true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(changeRangeLower, changeRangeUpper));
            go.SetActive(!go.activeInHierarchy);
        }
    }
}
