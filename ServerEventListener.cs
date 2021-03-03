using Bolt;
using System.Collections.Generic;
using UnityEngine;

public class ServerEventListener : GlobalEventListener
{
    [SerializeField]
    protected GameObject headstonePrefab;

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

    //if a player dies and no more than one player remains, the game is over
    public override void OnEvent(PlayerDiedEvent evnt)
    {
        Instantiate(headstonePrefab, evnt.PlayerPosition, Quaternion.identity);

        if (BoltNetwork.IsServer)
        {
            List<GameObject> enemies = new List<GameObject>();
            enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            enemies.ForEach(enemy => enemy.GetComponent<EnemyBehaviour>().RefreshPlayerList());

            List<GameObject> players = new List<GameObject>();
            players.AddRange(GameObject.FindGameObjectsWithTag("Player"));

            if (players.Count <= 1) GameOverEvent.Create().Send();
        }
    }
}