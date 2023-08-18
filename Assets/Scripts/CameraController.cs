using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

public class CameraController : MonoBehaviour {

    [Header("Cam stuff")]
    [SerializeField] Transform camParent;

    public Transform CamTransform => cam.transform;
    [SerializeField] Camera cam;

    [Header("misc")]
    [SerializeField] float mouseSensitivity = 5.0f;

    [Header("Virtual Joystick")]
    [SerializeField] VariableJoystick variableJoystick;

    public bool AllowShootingProjectile => allowShootingBall;
    bool allowShootingBall = false;

    void Awake()
    {
        variableJoystick.AxisOptions = AxisOptions.Vertical;
        variableJoystick.gameObject.SetActive(false);
        ToggleAllowShootingBall(false);
    }

    void ToggleAllowShootingBall(bool target) {
        allowShootingBall = target;
    }

    void Update()
    {
        if(allowShootingBall == false) {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject() == false) {
            UseMouse();
        }
        else {
            UseVirtualJoystick();
        }
    }

    float minCamClampAngle = -30f;
    float maxCamClampAngle = 10f;
    void UseMouse() {
        if (Input.GetMouseButton(0)) {
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            float angle = Mathf.Repeat(cam.transform.eulerAngles.x + 180, 360) - 180;
            if (mouseY > 0f) {
                if (angle > minCamClampAngle) {
                    cam.transform.Rotate(Vector3.left, mouseY);
                }
            } else if (mouseY < 0f) {
                if (angle < maxCamClampAngle) {
                    cam.transform.Rotate(Vector3.left, mouseY);
                }
            }
        }
    }

    void UseVirtualJoystick() {
        float vJoystick = variableJoystick.Vertical * 0.075f;
        float angle = Mathf.Repeat(cam.transform.eulerAngles.x + 180, 360) - 180;
        if (vJoystick > 0f) {
            if (angle > minCamClampAngle) {
                cam.transform.Rotate(Vector3.left, vJoystick);
            }
        } else if (vJoystick < 0f) {
            if (angle < maxCamClampAngle) {
                cam.transform.Rotate(Vector3.left, vJoystick);
            }
        }
    }

    public void RotateToLookAt(GameObject target) {
        StopCamParentRotation();
        Vector3 targetPoint = new Vector3(target.transform.position.x, camParent.position.y, target.transform.position.z) - camParent.position;
        camParentRotationIenum = RotateCamParent(targetPoint);
        StartCoroutine(camParentRotationIenum);
    }

    public void StopCamParentRotation() {
        if (camParentRotationIenum != null) {
            StopCoroutine(camParentRotationIenum);
        }
    }

    IEnumerator camParentRotationIenum;
    IEnumerator RotateCamParent(Vector3 targetPoint) {
        ToggleAllowShootingBall(false);
        variableJoystick.gameObject.SetActive(false);
        //camParent.LookAt(new Vector3(targetPoint.x, camParent.position.y, targetPoint.z));

        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        while (camParent.rotation != targetRotation) {
            camParent.rotation = Quaternion.Slerp(camParent.rotation, targetRotation, Time.deltaTime * 3.0f);
            yield return null;
        }

        variableJoystick.gameObject.SetActive(true);
        ToggleAllowShootingBall(true);
    }
}
