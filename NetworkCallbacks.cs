using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class NetworkCallbacks : GlobalEventListener
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject leadSoldierPrefab;

    public override void SceneLoadLocalDone(string scene, IProtocolToken protocolToken)
    {
        Vector3 spawnPos = new Vector3(-19.08f + Random.Range(-10, 10), 108f, 456f);
        BoltNetwork.Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        if (BoltNetwork.IsServer)
        {
            Vector3 enemySpawnPos = new Vector3(-19.08f + Random.Range(-10, 10), 108f, 456f);
            BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos, Quaternion.identity);

            //Vector3 enemySpawnPos2 = new Vector3(-19.08f + Random.Range(-10, 10), 108f, 456f);
            //BoltNetwork.Instantiate(leadSoldierPrefab, enemySpawnPos2, Quaternion.identity);
        }
    }
}