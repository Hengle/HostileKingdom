﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class InputManager : MonoBehaviour
{
    [SerializeField]
    LevelLogic levelLogic;
    LevelLoader levelLoader;
    bool wasLoaded;
    [SerializeField]
    StatsManager statsManager; 

    public bool gamePause;

    [Header("MouseInputsManager")]
    [SerializeField]
    Player mouse;
    public static Vector3 mousePosition;

    [Header("CameraInputs")]
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    CameraRotation cameraRotation;
    [SerializeField]
    CameraZoom cameraZoom;
    float scrollAxis;
    float rotationAxis;
    float mouseAxis;
    Vector2 inputAxis;

    [Header("GodMode")]
    [SerializeField]
    bool isGodModeEnabled;

    void Update()
    {
        mousePosition = Input.mousePosition;
        if (wasLoaded)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                gamePause = !gamePause;
                if (gamePause == false)
                {
                    statsManager.UnPaused();
                }
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                isGodModeEnabled = !isGodModeEnabled;
                Debug.Log("GOD Mode enabled");
            }
            if (isGodModeEnabled)
            {
                GodModeUpdate();
                return;
            }

            if (gamePause == true)
            {
                statsManager.Paused();
                Time.timeScale = 0.0f;
                Paused();
            }
            else
            {
				statsManager.UnPaused();
                Time.timeScale = 1.0f;
                NoPaused();
            }
        }
        else
        {
            SearchLevelLoader(); 
        }
    }

    public void SearchLevelLoader()
    {
        if (GameObject.FindGameObjectWithTag("LevelLoader") != null)
        {
            if (!wasLoaded) SetGameValues();
        }
    }

    void SetGameValues()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        mouse = levelLoader.mouse;
        cameraController = levelLoader.cameraController;
        cameraRotation = levelLoader.cameraRotation;
        cameraZoom = levelLoader.cameraZoom;
		statsManager = levelLoader.statsManager;
        wasLoaded = true;
    }

    void Paused()
    {

    }

    void NoPaused()
    {
        mouse.SetMousePosition(mousePosition); 

        #region CameraControllerAndZoom
        inputAxis.x = Input.GetAxis("Horizontal");
        inputAxis.y = Input.GetAxis("Vertical");
        rotationAxis = Input.GetAxis("Rotation");
        mouseAxis = Input.GetAxis("Mouse X");
        scrollAxis = Input.GetAxis("Mouse ScrollWheel");

        cameraController.SetInputAxis(inputAxis, mousePosition);
        cameraRotation.SetRotationAxis(rotationAxis);
        cameraZoom.SetAxis(scrollAxis);

        if (Input.GetButton("Fire3"))
        {
            cameraRotation.SetMouseRotationAxis(mouseAxis);
        }
        #endregion

        #region Selection&MovementBehaviours
        if (Input.GetMouseButtonDown(0))
        {
            mouse.ClickState();
        }
        #endregion
    }

    void GodModeUpdate()
    {
        mouse.SetMousePosition(mousePosition); 

        if (Input.GetMouseButtonDown(0))
        {
            mouse.ClickState();
        }

        #region CameraControllerAndZoom
        inputAxis.x = Input.GetAxis("Horizontal");
        inputAxis.y = Input.GetAxis("Vertical");
        rotationAxis = Input.GetAxis("Rotation");
        mouseAxis = Input.GetAxis("Mouse X");
        scrollAxis = Input.GetAxis("Mouse ScrollWheel");

        cameraController.SetInputAxis(inputAxis, mousePosition);
        cameraRotation.SetRotationAxis(rotationAxis);
        cameraZoom.SetAxis(scrollAxis);

        if (Input.GetButton("Fire3"))
        {
            cameraRotation.SetMouseRotationAxis(mouseAxis);
        }
        #endregion
    }
}
