using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpManager : SingletonMonoBehaviour<CameraLerpManager>
{
    private Camera cam;

    public enum CameraPosition { Machine, Result, Question };
    public Transform[] angles;
    public float defaultFieldOfView = 69f;
    public float defaultTransitionDuration = 0.75f;
    public CameraPosition currentPosition = CameraPosition.Question;

    private float elapsedTime;
    private Vector3 startPos;
    private Vector3 startRot;
    private float startFov;
    private Vector3 endPos;
    private Vector3 endRot;
    private float endFov;
    private bool isMoving;
    private float duration = 2f;

    #region Init

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    #endregion

    #region Public Functions

    [Button]
    public void ToggleMachineAndResult()
    {
        if (currentPosition == CameraPosition.Machine)
        {
            ZoomToPosition(CameraPosition.Result);
            currentPosition = CameraPosition.Result;
        }
        else if(currentPosition == CameraPosition.Result)
        {
            ZoomToPosition(CameraPosition.Machine);
            currentPosition = CameraPosition.Machine;
        }            
    }

    [Button]
    public void ToggleMachineAndLobby()
    {
        if (currentPosition == CameraPosition.Machine || currentPosition == CameraPosition.Result)
        {
            ZoomToPosition(CameraPosition.Question, defaultFieldOfView, 2f);
            currentPosition = CameraPosition.Question;
        }
        else if (currentPosition == CameraPosition.Question)
        {
            ZoomToPosition(CameraPosition.Machine, defaultFieldOfView, 2f);
            currentPosition = CameraPosition.Machine;
        }
    }

    public void ZoomToPosition(CameraPosition target, float fov, float transitionDuration)
    {
        if (isMoving)
            return;

        startPos = cam.transform.localPosition;
        startRot = cam.transform.localEulerAngles;
        startFov = cam.fieldOfView;

        endPos = angles[(int)target].transform.localPosition;
        endRot = angles[(int)target].transform.localEulerAngles;
        endFov = fov;

        elapsedTime = 0;
        duration = transitionDuration;
        isMoving = true;
        Invoke("EndLock", duration);
    }

    public void ZoomToPosition(CameraPosition target, float fov)
    {
        ZoomToPosition(target, fov, defaultTransitionDuration);
    }

    public void ZoomToPosition(CameraPosition target)
    {
        ZoomToPosition(target, defaultFieldOfView, defaultTransitionDuration);
    }

    #endregion

    #region Private Functions

    private void Update()
    {
        if (isMoving)
            PerformLerp();
    }

    private void PerformLerp()
    {
        elapsedTime += Time.deltaTime;

        float percentageComplete = elapsedTime / duration;

        this.gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, percentageComplete));

        float x = Mathf.LerpAngle(startRot.x, endRot.x, Mathf.SmoothStep(0, 1, percentageComplete));
        float y = Mathf.LerpAngle(startRot.y, endRot.y, Mathf.SmoothStep(0, 1, percentageComplete));
        float z = Mathf.LerpAngle(startRot.z, endRot.z, Mathf.SmoothStep(0, 1, percentageComplete));
        this.gameObject.transform.localEulerAngles = new Vector3(x, y, z);//Vector3.Lerp(startRot, endRot, Mathf.SmoothStep(0, 1, percentageComplete));

        cam.fieldOfView = Mathf.Lerp(startFov, endFov, Mathf.SmoothStep(0, 1, percentageComplete));
    }

    public void EndLock()
    {
        isMoving = false;
    }

    #endregion
}
