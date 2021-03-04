using Bolt;
using Bolt.Matchmaking;
using System;
using UdpKit;
using UnityEngine;

public class StartSceneManager : GlobalEventListener
{
    [SerializeField]
    private GameObject exitJoiningGameButton;

    public void Awake()
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
        exitJoiningGameButton.SetActive(true);
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

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitJoiningGame()
    {
        StopSun();
        exitJoiningGameButton.SetActive(false);
        BoltLauncher.Shutdown();
    }

    private void StartSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 5f;
    }

    private void StopSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 0f;
    }
}