﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform cameraRotationTransform; 

    Transform camTransform;

    [Header("Pan")]
    [SerializeField]
    float panShiftSpeed;
    [SerializeField]
    float panNormalSpeed; 
    float panSpeed;
    [SerializeField]
    float panBorderThickness ;

    [Header("Limits")]
    [SerializeField]
    Vector2 panLimit;

    [Header("Inputs")]
    float rotationAxis;
    float mouseRotationAxis; 
    Vector2 inputAxis;

    void Start()
    {
        camTransform = transform;
    }

    void MovementUpdate(Vector3 mousePosition)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            panSpeed = panShiftSpeed;
        }
        else
        {
            panSpeed = panNormalSpeed; 
        }

        Vector3 newPosition = new Vector3(inputAxis.x, 0, inputAxis.y);

        if (mousePosition.y >= Screen.height - panBorderThickness) newPosition = Vector3.back;
        else if (mousePosition.y <= panBorderThickness) newPosition = Vector3.forward;
        else if (mousePosition.x >= Screen.width - panBorderThickness) newPosition = Vector3.left;
        else if (mousePosition.x <= panBorderThickness) newPosition = Vector3.right;

        newPosition *= panSpeed * Time.deltaTime;
        newPosition = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * newPosition;
        newPosition = camTransform.InverseTransformDirection(newPosition);

        camTransform.Translate(newPosition, cameraRotationTransform);
    }

    void LimitPosition()
    {
        camTransform.position = new Vector3(Mathf.Clamp(camTransform.position.x, -panLimit.x, panLimit.x),
                                            camTransform.position.y,
            Mathf.Clamp(camTransform.position.z, -panLimit.y, panLimit.y));
    }

    public void SetInputAxis(Vector2 newAxis, Vector3 mousePosition)
    {
        inputAxis = newAxis;
        MovementUpdate(mousePosition);
        LimitPosition();
    }
}
