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
    private float solarSystemRadius;

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

            GameObjectManager.Instance.Planets.ForEach(planet =>
            {
                int numberOfEnemies = UnityEngine.Random.Range(1, 4);
                Vector3 enemySpawnPos;
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
            });
        }

        SpawnPlayer();
    }

    //spawn the player to a random planet
    private void SpawnPlayer()
    {
        int randomPlanetIndex = UnityEngine.Random.Range(0, GameObjectManager.Instance.Planets.Count);

        Vector3 spawnPos = GameObjectManager.Instance.Planets[randomPlanetIndex].transform.position;
        spawnPos.x += 30;

        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    private void CreateSolarSystem()
    {
        BoltNetwork.Instantiate(sunPrefab);

        foreach (var planetPosition in getPlanetPositions())
        {
            BoltNetwork.Instantiate(planetPrefab, planetPosition, UnityEngine.Random.rotation);
        }

        GameObjectManager.Instance.RefreshPlanets();
    }

    private List<Vector3> getPlanetPositions()
    {
        List<Vector3> planetPositions = new List<Vector3>();
        planetPositions.Add(new Vector3(solarSystemRadius, 0, 0));

        bool algorithmFinished = false;
        Vector3 lastPlanetPosition = Vector3.negativeInfinity;

        //emergency limit
        int counter = 0;

        while (!algorithmFinished)
        {
            List<Vector3> newPlanetPositions = new List<Vector3>();
            if (lastPlanetPosition.Equals(Vector3.negativeInfinity))
            {
                //this is the first iteration
                //we just choose the first found intersection as the new planet position
                lastPlanetPosition = new Vector3(solarSystemRadius, 0, 0);
                planetPositions.Add(FindCircleCircleIntersections(
                    sunPrefab.transform.position,
                    solarSystemRadius,
                    new Vector3(solarSystemRadius, 0, 0),
                    105)[0]);
            }
            else
            {
                newPlanetPositions.AddRange(FindCircleCircleIntersections(
                    sunPrefab.transform.position,
                    solarSystemRadius,
                    planetPositions[planetPositions.Count - 1],
                    105));

                if ((lastPlanetPosition - newPlanetPositions[0]).sqrMagnitude < 0.01f)
                {
                    if (lastPlanetPosition.Equals(newPlanetPositions[1]))
                    {
                        Debug.LogError("Last center equals both of the new intersections!");
                    }
                    else
                    {
                        planetPositions.Add(newPlanetPositions[1]);
                        lastPlanetPosition = newPlanetPositions[1];
                    }
                }
                else if ((lastPlanetPosition - newPlanetPositions[1]).sqrMagnitude < 0.01f)
                {
                    planetPositions.Add(newPlanetPositions[0]);
                    lastPlanetPosition = planetPositions[planetPositions.Count - 2];
                }
                else
                {
                    Debug.LogError("Last center does not equal to a new intersection!");
                }
            }
            if (!((lastPlanetPosition - new Vector3(solarSystemRadius, 0, 0)).sqrMagnitude < 0.01f))
            {
                algorithmFinished = CheckIfAlgorithmFinished(lastPlanetPosition, planetPositions[0]);
            }
            if (counter > 100)
            {
                algorithmFinished = true;
            }
            counter++;
        }
        planetPositions.RemoveAt(planetPositions.Count - 1);

        return planetPositions;
    }

    private bool CheckIfAlgorithmFinished(Vector3 position1, Vector3 position2)
    {
        if (!position1.Equals(position2))
        {
            return Vector3.Distance(position1, position2) < 100;
        }
        return true;
    }

    private List<Vector3> FindCircleCircleIntersections(
        Vector3 circle0Center,
        float radius0,
        Vector3 circle1Center,
        float radius1)
    {
        List<Vector3> intersections = new List<Vector3>();

        // Find the distance between the centers
        float dx = circle0Center.x - circle1Center.x;
        float dz = circle0Center.z - circle1Center.z;
        double dist = Math.Sqrt(dx * dx + dz * dz);
        float distance = Vector3.Distance(circle0Center, circle1Center);

        // See how many solutions there are
        if (dist > radius0 + radius1)
        {
            Debug.LogError("No solutions, the circles are too far apart.");
            return intersections;
        }
        else if (dist < Math.Abs(radius0 - radius1))
        {
            Debug.LogError("No solutions, one circle contains the other.");
            return intersections;
        }
        else if ((dist == 0) && (radius0 == radius1))
        {
            Debug.LogError("No solutions, the circles coincide.");
            return intersections;
        }
        else if (dist == radius0 + radius1)
        {
            Debug.LogError("Only one solution, not supposed to happen!");
            return intersections;
        }
        else
        {
            // Find a and h
            double a = (radius0 * radius0 - radius1 * radius1 + dist * dist) / (2 * dist);
            double h = Math.Sqrt(radius0 * radius0 - a * a);

            // Find P2
            double cx2 = circle0Center.x + a * (circle1Center.x - circle0Center.x) / dist;
            double cz2 = circle0Center.z + a * (circle1Center.z - circle0Center.z) / dist;

            // Get the points P3
            intersections.Add(new Vector3(
                (float)(cx2 + h * (circle1Center.z - circle0Center.z) / dist),
                0f,
                (float)(cz2 - h * (circle1Center.x - circle0Center.x) / dist)));

            intersections.Add(new Vector3(
                (float)(cx2 - h * (circle1Center.z - circle0Center.z) / dist),
                0f,
                (float)(cz2 + h * (circle1Center.x - circle0Center.x) / dist)));
        }

        return intersections;
    }
}