using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : GlobalEventListener
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject leadSoldierPrefab;

    [SerializeField]
    private GameObject planetPrefab;

    [SerializeField]
    private GameObject sunPrefab;

    [SerializeField]
    private GameObject objectivePrefab;

    [SerializeField]
    private float solarSystemRadius;

    [SerializeField]
    private float planetToPlanetDistance;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
    }

    public override void SceneLoadLocalDone(string scene, IProtocolToken protocolToken)
    {
        if (BoltNetwork.IsServer)
        {
            CreateSolarSystem();
            SpawnEnemiesAndObjectives();
        }

        StartCoroutine(SpawnPlayer());
    }

    private void SpawnEnemiesAndObjectives()
    {
        //Vector3 enemySpawnPos;

        //enemySpawnPos = GameObjectManager.Instance.Planets[0].transform.position;
        //enemySpawnPos.x += 30;
        //BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos, Quaternion.identity);

        GameObjectManager.Instance.Planets.ForEach(planet =>
        {
            int numberOfEnemies = UnityEngine.Random.Range(1, 4);
            Vector3 enemySpawnPos;
            Vector3 objectiveSpawnPos;
            if (numberOfEnemies > 0)
            {
                enemySpawnPos = planet.transform.position;
                enemySpawnPos.x += 30;
                BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos, Quaternion.identity);
            }
            if (numberOfEnemies > 1)
            {
                enemySpawnPos = planet.transform.position;
                enemySpawnPos.y += 30;
                BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos, Quaternion.identity);
            }
            if (numberOfEnemies > 2)
            {
                enemySpawnPos = planet.transform.position;
                enemySpawnPos.z += 30;
                BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos, Quaternion.identity);
            }

            int probabilityOfObjectives = UnityEngine.Random.Range(1, 101);
            if (probabilityOfObjectives > 50)
            {
                objectiveSpawnPos = planet.transform.position;
                objectiveSpawnPos.z += 30;
                BoltNetwork.Instantiate(objectivePrefab, objectiveSpawnPos, Quaternion.identity);
            }
        });

        GameObjectManager.Instance.RefreshEnemies();
    }

    //spawn the player to a random planet
    private IEnumerator SpawnPlayer()
    {
        yield return StartCoroutine(GameObjectManager.Instance.RefreshPlanetsCoroutine());
        yield return StartCoroutine(GameObjectManager.Instance.RefreshEnemiesCoroutine());

        //TODO: change this to random planet, only for debugging purposes
        //int randomPlanetIndex = UnityEngine.Random.Range(0, GameObjectManager.Instance.Planets.Count - 1);
        int randomPlanetIndex = 0;
        GameObjectManager.Instance.RemoveEnemiesOnPlanet(GameObjectManager.Instance.Planets[randomPlanetIndex].GetComponentInChildren<PlanetNetworkState>().PlanetId);

        Vector3 spawnPos = GameObjectManager.Instance.Planets[randomPlanetIndex].transform.position;
        spawnPos.x += 30;

        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    private void CreateSolarSystem()
    {
        GameObjectManager.Instance.Sun = BoltNetwork.Instantiate(sunPrefab).gameObject;

        foreach (var planetPosition in GetPlanetPositions())
        {
            BoltNetwork.Instantiate(planetPrefab, planetPosition, UnityEngine.Random.rotation);
        }

        GameObjectManager.Instance.RefreshPlanets();
    }

    private List<Vector3> GetPlanetPositions()
    {
        return gameObject.GetComponent<PlanetPositionGenerator>().GeneratePlanetPositions();
    }
}