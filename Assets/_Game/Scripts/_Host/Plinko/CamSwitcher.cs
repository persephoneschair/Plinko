using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitcher : MonoBehaviour
{
    public static bool movedToResult = false;
    public static bool movedToQuestion = false;

    public float delayLength = 3f;
    public bool doDelay = false;
    public bool toQuestion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Ball" || (movedToResult && !toQuestion) || movedToQuestion)
            return;

        if (!toQuestion)
            movedToResult = true;
        else
            movedToQuestion = true;
        
        Invoke("SwitchBack", doDelay ? delayLength : 0);
    }

    private void SwitchBack()
    {
        if(toQuestion)
            CameraLerpManager.Get.ToggleMachineAndLobby();
        else
            CameraLerpManager.Get.ToggleMachineAndResult();
    }
}
