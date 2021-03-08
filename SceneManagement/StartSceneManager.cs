using Bolt;
using Bolt.Matchmaking;
using System;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : GlobalEventListener
{
    [SerializeField]
    private GameObject exitJoiningGameButton;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject onlineGamePanel;

    [SerializeField]
    private Button roomPrefab;

    [SerializeField]
    private GameObject roomListContent;

    public void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;

        onlineGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        SlowerSun();
    }

    public void OnClickHost()
    {
        BoltLauncher.Shutdown();
        FasterSun();
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            BoltMatchmaking.CreateSession(sessionID: "test", sceneToLoad: "Game");
        }
    }

    public void OnClickJoin()
    {
        FasterSun();
        exitJoiningGameButton.SetActive(true);
        BoltLauncher.StartClient();
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            Button room = Instantiate(roomPrefab, roomListContent.transform);
            room.gameObject.SetActive(true);
            room.onClick.AddListener(() => OnClickJoinGame(photonSession));
        }
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickExitJoiningGame()
    {
        SlowerSun();
        exitJoiningGameButton.SetActive(false);
        BoltLauncher.Shutdown();
    }

    public void OnClickOnlineGame()
    {
        onlineGamePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        exitJoiningGameButton.SetActive(false);
        BoltLauncher.StartClient();
    }

    public void OnClickBack()
    {
        onlineGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        BoltLauncher.Shutdown();
        SlowerSun();
    }

    private void FasterSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 10f;
    }

    private void SlowerSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 2f;
    }

    private void OnClickJoinGame(UdpSession udpSession)
    {
        BoltMatchmaking.JoinSession(udpSession);
    }
}