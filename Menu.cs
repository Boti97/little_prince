using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using System;

public class Menu : GlobalEventListener
{
    public void StartServer()
    {
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            StartSun();
            BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "Game");
        }
    }

    public void StartClient()
    {
        StartSun();
        BoltLauncher.StartClient();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }

    private void StartSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 10;
    }
}