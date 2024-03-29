using Photon.Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameSceneManager : GlobalEventListener
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject planetObjectPrefab;

    [SerializeField]
    private GameObject leadSoldierPrefab;

    [SerializeField]
    private GameObject sunPrefab;

    [SerializeField]
    private GameObject objectivePrefab;

    [SerializeField]
    private float solarSystemRadius;

    [SerializeField]
    private int baseSeed;

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
            //SpawnEnemiesAndObjectives();
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
        //yield return StartCoroutine(GameObjectManager.Instance.RefreshEnemiesCoroutine());

        //TODO: change this to random planet, only for debugging purposes
        //int randomPlanetIndex = UnityEngine.Random.Range(0, GameObjectManager.Instance.Planets.Count - 1);
        int randomPlanetIndex = 0;
        //GameObjectManager.Instance.RemoveEnemiesOnPlanet(GameObjectManager.Instance.Planets[randomPlanetIndex].GetComponentInChildren<PlanetNetworkState>().PlanetId);

        Vector3 spawnPos = GameObjectManager.Instance.Planets[randomPlanetIndex].transform.position;
        spawnPos.x += 30;

        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    private void CreateSolarSystem()
    {
        //GameObjectManager.Instance.Sun = BoltNetwork.Instantiate(sunPrefab).gameObject;

        //set random generation seed for recoverability
        baseSeed = Random.Range(0, 10000);
        Random.InitState(baseSeed);

        List<Vector3> planetPositions = GetPlanetPositions();
        List<GameObject> planetSurfaces = GetPlanetSurfaces(planetPositions.Count);
        List<GameObject> planetObjects = InitiatePlanetObjects(planetPositions);

        SetUpPlanetSurfaceComponents(planetSurfaces, planetObjects);

        //TODO: more complex planet placement
        //for (int i = 0; i < planetPositions.Count; i++)
        //{
        //    GameObject planetX = BoltNetwork.Instantiate(planetSurfaces[i], planetPositions[i], Random.rotation).gameObject;
        //}

        GameObjectManager.Instance.RefreshPlanets();
    }

    private List<GameObject> InitiatePlanetObjects(List<Vector3> planetPositions)
    {
        List<GameObject> planetObjects = new List<GameObject>();
        for (int i = 0; i < planetPositions.Count; i++)
        {
            GameObject planetObject = BoltNetwork.Instantiate(planetObjectPrefab, planetPositions[i], Random.rotation).gameObject;
            planetObject.name = "Planet" + i;
            planetObjects.Add(planetObject);
        }
        return planetObjects;
    }

    private void SetUpPlanetSurfaceComponents(List<GameObject> planetSurfaces, List<GameObject> planetObjects)
    {
        for (int i = 0; i < planetSurfaces.Count; i++)
        {
            //assaign unique bolt entity to planet
            planetSurfaces[i].transform.parent = planetObjects[i].transform;
            planetSurfaces[i].transform.localPosition = Vector3.zero;

            //set up planet core
            //planetCore.GetComponent<SphereCollider>().radius = 10;
            //planetCore.transform.parent = planetSurfaces[i].transform;

            //set up bolt entity 
            //BoltEntity source = boltEntityPrefab.GetComponent<BoltEntity>();
            //BoltEntity target = planets[i].AddComponent<BoltEntity>();
            //CopyClassValues(source, target);

            //set up network state
            planetSurfaces[i].AddComponent<PlanetNetworkState>();

            //set up planet behaviour
            //planets[i].AddComponent<PlanetBehaviour>();

            //tag planet
            planetSurfaces[i].tag = "Planet";

            //set Ground layer
            planetSurfaces[i].layer = 8;

            //add Mesh collider
            planetSurfaces[i].AddComponent<MeshCollider>();
        }
    }

    private List<Vector3> GetPlanetPositions()
    {
        return gameObject.GetComponent<PlanetPositionGenerator>().GeneratePlanetPositions();
    }

    private List<GameObject> GetPlanetSurfaces(int numberOfPlanets)
    {
        return gameObject.GetComponent<PlanetSurfaceGenerator>().GeneratePlanets(numberOfPlanets, baseSeed);
    }
}