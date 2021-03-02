using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEventListener : Bolt.GlobalEventListener
{
    //if a player joins we need to inform the enemies about it
    public override void OnEvent(PlayerJoinedEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            List<GameObject> enemies = new List<GameObject>();
            enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            enemies.ForEach(enemy => enemy.GetComponent<EnemyBehaviour>().RefreshPlayerList());
        }
    }
}