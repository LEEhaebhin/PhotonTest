using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonTestLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string nickName = string.Empty;
    [SerializeField] private TMP_Text textCurrentPlayer = null; //�غ����� �÷��̾��
    [SerializeField] private Button ready = null; // �غ��ư ������ �濡 ����
    [SerializeField] private Button start = null; //���۹�ư ����ȯ

    [SerializeField] private string gameVersion = "0.0.1";
    [SerializeField] private byte maxPlayerPerRoom = 4;



    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        start.interactable = false;
    }

    public void Connect() //�����ư�� �����Ѵ�.
    {
        if (string.IsNullOrEmpty(nickName))
        {
            Debug.Log("NickName is empty");
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }

        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            PhotonNetwork.GameVersion = gameVersion;
            ////������ ���������� ���� �ʴ°�� Application.version�� ���Ͽ� ����Ƽ���� �����ϴ� �⺻ �������� ����� �� �ִ�.
            // ���� Ŭ���忡 ������ �����ϴ� ����
            // ���ӿ� �����ϸ� OnConnectedToMaster �޼��� ȣ��
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;
        Debug.Log("Nickname = " + nickName);
        // ���� �̸� ����
        PhotonNetwork.NickName = nickName;
    }

    public override void OnConnectedToMaster() // ����� �޾� ����ϴ� �Լ��̴�.
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);

        ready.interactable = false;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        ready.interactable = true;

        // ���� �����ϸ� OnJoinedRoom ȣ��
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        // �����Ͱ� ���ÿ� ������ �����ϰ��ϴ� ������ �ƴϱ� ������ ���� ���� �θ��� ��
        //PhotonNetwork.LoadLevel("Room"); //LoadLevel�� ȥ�ڸ� �����ϰ��ؾ��Ѵ�.
        TextCurrentPlayer();
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        ready.interactable = true;

        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        TextCurrentPlayer();
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            start.interactable = true;
        }
    }

    private void TextCurrentPlayer() //�����ư�� �����Ͽ� ������������ �ο��� ǥ�����ش�.
    {
        textCurrentPlayer.text = $"{ PhotonNetwork.CurrentRoom.PlayerCount}  / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestRoom");
    }
}
