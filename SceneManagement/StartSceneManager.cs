using Bolt;
using Bolt.Matchmaking;
using System;
using UdpKit;
using UnityEngine;

public class StartSceneManager : GlobalEventListener
{
    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }

    public void StartServer()
    {
        StartSun();
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
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