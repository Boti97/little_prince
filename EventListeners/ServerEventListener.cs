using Bolt;
using System.Collections.Generic;
using UnityEngine;

public class ServerEventListener : GlobalEventListener
{
    //if a player dies and no more than one player remains, the game is over
    public override void OnEvent(PlayerDiedEvent evnt)
    {
        GameObjectManager.Instance.RemoveDeadPlayer(evnt.DeadPlayerId);

        if (BoltNetwork.IsServer)
        {
            if (GameObjectManager.Instance.Players.Count <= 1) 
                GameOverEvent.Create().Send();
        }
    }
}