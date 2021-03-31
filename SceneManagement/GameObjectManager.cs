using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject headstonePrefab;

    private Slider staminaBar;
    private Slider healthBar;
    private Slider thrustBar;
    private GameObject gameOverText;
    private GameObject sun;
    private CinemachineFreeLook cinemachineVirtualCamera;
    private List<GameObject> planets;
    private List<GameObject> players;
    private List<GameObject> enemies;

    private static readonly object padlock = new object();
    private static GameObjectManager instance = null;

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
    public List<GameObject> Players { get => players; set => players = value; }
    public List<GameObject> Enemies { get => enemies; set => enemies = value; }
    public GameObject Sun { get => sun; set => sun = value; }

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

        Players = new List<GameObject>();
        Players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        Enemies = new List<GameObject>();
        Enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        Sun = GameObject.FindGameObjectWithTag("Sun");

        DeactivateUnnecessaryGameObjects();
    }

    //----------------------------------- PLAYER RELATED METHODS -----------------------------------

    public void RefreshPlayers()
    {
        Players.Clear();
        Players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    public void RemovePlayer(Guid id)
    {
        GameObject deadPlayer = GetPlayerById(id);
        Instantiate(headstonePrefab, deadPlayer.transform.position, Quaternion.identity);
        Players.Remove(deadPlayer);
        Destroy(deadPlayer);
    }

    public bool IsPlayerOwned(Guid id)
    {
        return GetOwnedPlayerId() == id;
    }

    public GameObject GetPlayerById(Guid id)
    {
        return Players.Find(player => player.GetComponent<PlayerNetworkState>().GetGuid() == id);
    }

    public Guid GetOwnedPlayerId()
    {
        return Players.Find(player => player.GetComponent<PlayerNetworkState>().entity.IsOwner).GetComponent<PlayerNetworkState>().GetGuid();
    }

    public bool IsOwnedPlayerAlive()
    {
        return Players.Find(player => player.GetComponent<PlayerNetworkState>().entity.IsOwner) != null;
    }

    public List<GameObject> FindPlayersOnPlanet(Guid planetId)
    {
        return Players.FindAll(player => player.GetComponent<PlayerBehaviour>().planetId == planetId);
    }

    //----------------------------------- ENEMY RELATED METHODS -----------------------------------
    public IEnumerator RefreshEnemiesCoroutine()
    {
        while (Enemies.Count.Equals(0))
        {
            yield return new WaitForSeconds(1f);
            Enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        }
    }

    public List<GameObject> FindEnemiesOnPlanet(Guid planetId)
    {
        return Enemies.FindAll(enemy => enemy.GetComponent<EnemyBehaviour>().planetId == planetId);
    }

    public void RemoveEnemiesOnPlanet(Guid planetId)
    {
        FindEnemiesOnPlanet(planetId).ForEach(enemy =>
        {
            Enemies.Remove(enemy);
            Destroy(enemy);
        });
    }

    public void RefreshEnemies()
    {
        Enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    //----------------------------------- PLANET RELATED METHODS -----------------------------------
    public IEnumerator RefreshPlanetsCoroutine()
    {
        while (Planets.Count.Equals(0))
        {
            yield return new WaitForSeconds(1f);
            Planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
        }
    }

    public void RefreshPlanets()
    {
        Planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
    }

    public GameObject GetPlanetById(Guid id)
    {
        return Planets.Find(planet => planet.GetComponent<PlanetNetworkState>().PlanetId == id);
    }

    //----------------------------------- OTHER METHODS -----------------------------------
    public bool IsGameOver()
    {
        return GameOverText.activeSelf;
    }

    //----------------------------------- PRIVATE METHODS -----------------------------------
    private void DeactivateUnnecessaryGameObjects()
    {
        GameOverText.SetActive(false);
    }
}