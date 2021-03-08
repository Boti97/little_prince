using Bolt;
using Bolt.Matchmaking;
using Bolt.Photon;
using System;
using TMPro;
using UdpKit;
using UdpKit.Platform.Photon;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : GlobalEventListener
{
    //[SerializeField]
    //private GameObject exitJoiningGameButton;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject onlineGamePanel;

    [SerializeField]
    private GameObject roomPrefab;

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
            BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
            PhotonRoomProperties token = new PhotonRoomProperties();
            token.AddRoomProperty("roomName", GameObject.FindWithTag("NewRoomNameInputField").GetComponent<TMP_InputField>().text);
            BoltMatchmaking.CreateSession(sessionID: Guid.NewGuid().ToString(), sceneToLoad: "Game", token: token);
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        foreach (var session in sessionList)
        {
            PhotonSession photonSession = session.Value as PhotonSession;

            GameObject room = Instantiate(roomPrefab, roomListContent.transform);
            room.gameObject.SetActive(true);
            room.GetComponentInChildren<TextMeshProUGUI>().text = photonSession.Properties["roomName"] as string;
            room.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickJoinGame(photonSession));
        }
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickExitJoiningGame()
    {
        SlowerSun();
        //exitJoiningGameButton.SetActive(false);
        BoltLauncher.Shutdown();
    }

    public void OnClickOnlineGame()
    {
        onlineGamePanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        //exitJoiningGameButton.SetActive(false);
        BoltLauncher.StartClient();
    }

    public void OnClickBack()
    {
        onlineGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        BoltLauncher.Shutdown();
        SlowerSun();
    }

    public void OnValueChangeForNewRoomNameInputFieldText()
    {
        string nameOfRoom = GameObject.FindWithTag("NewRoomNameInputField").GetComponent<TMP_InputField>().text;
        if (nameOfRoom != null && !nameOfRoom.Equals(""))
        {
            GameObject.Find("HostButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("HostButton").GetComponent<Button>().interactable = false;
        }
    }

    private void FasterSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 10f;
    }

    private void SlowerSun()
    {
        GameObject.Find("Sun").GetComponentInChildren<SunBehaviour>().SelfRotationSpeed = 2f;
    }

    private void OnClickJoinGame(PhotonSession photonSession)
    {
        BoltMatchmaking.JoinSession(photonSession);
    }
}