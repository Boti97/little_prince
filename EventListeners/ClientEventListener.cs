using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientEventListener : GlobalEventListener
{
    public override void OnEvent(GameOverEvent evnt)
    {
        GameObjectManager.Instance.GameOverText.SetActive(true);
    }
}