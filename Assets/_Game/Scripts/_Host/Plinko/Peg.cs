using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    private Renderer rend;
    private float lightDuration = 1.5f;
    public Material offMat;
    public Material hitMat;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = offMat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Ball")
        {
            rend.material = hitMat;
            Invoke("SwitchOff", lightDuration);
        }
    }

    private void SwitchOff()
    {
        rend.material = offMat;
    }
}
