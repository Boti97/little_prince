using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameObjectManager : MonoBehaviour
{
    private Slider staminaBar;
    private Slider healthBar;
    private Slider thrustBar;
    private GameObject gameOverText;
    private CinemachineFreeLook cinemachineVirtualCamera;
    private List<GameObject> planets;

    private static readonly object padlock = new object();
    private static GameObjectManager instance = null;

    private GameObjectManager()
    {
    }

    public static GameObjectManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GameObjectManager();
                }
                return instance;
            }
        }
    }

    public Slider StaminaBar { get => staminaBar; set => staminaBar = value; }
    public Slider HealthBar { get => healthBar; set => healthBar = value; }
    public GameObject GameOverText { get => gameOverText; set => gameOverText = value; }
    public CinemachineFreeLook CinemachineVirtualCamera { get => cinemachineVirtualCamera; set => cinemachineVirtualCamera = value; }
    public Slider ThrustBar { get => thrustBar; set => thrustBar = value; }
    public List<GameObject> Planets { get => planets; set => planets = value; }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        StaminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<Slider>();
        ThrustBar = GameObject.FindGameObjectWithTag("ThrustBar").GetComponent<Slider>();
        HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        GameOverText = GameObject.FindGameObjectWithTag("GameOver");
        CinemachineVirtualCamera = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();

        Planets = new List<GameObject>();
        Planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));

        DeactivateUnnecessaryGameObjects();
    }

    public void RefreshPlanets()
    {
        Planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
    }

    private void DeactivateUnnecessaryGameObjects()
    {
        GameOverText.SetActive(false);
    }
}