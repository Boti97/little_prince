using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoined : Bolt.EntityBehaviour<IPlayerState>
{
    public override void Attached()
    {
        PlayerJoinedEvent.Create().Send();
    }
}