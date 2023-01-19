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
    [SerializeField] private TMP_Text textCurrentPlayer = null; //준비중인 플레이어수
    [SerializeField] private Button ready = null; // 준비버튼 누르면 방에 들어간다
    [SerializeField] private Button start = null; //시작버튼 씬전환

    [SerializeField] private string gameVersion = "0.0.1";
    [SerializeField] private byte maxPlayerPerRoom = 4;



    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        start.interactable = false;
    }

    public void Connect() //레디버튼과 연결한다.
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
            ////별도의 버전관리를 하지 않는경우 Application.version을 통하여 유니티에서 제공하는 기본 버전으로 사용할 수 있다.
            // 포톤 클라우드에 접속을 시작하는 지점
            // 접속에 성공하면 OnConnectedToMaster 메서드 호출
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;
        Debug.Log("Nickname = " + nickName);
        // 유저 이름 지정
        PhotonNetwork.NickName = nickName;
    }

    public override void OnConnectedToMaster() // 상속을 받아 사용하는 함수이다.
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);

        ready.interactable = false;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        ready.interactable = true;

        // 방을 생성하면 OnJoinedRoom 호출
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        // 마스터가 동시에 게임을 시작하게하는 구조가 아니기 때문에 각자 씬을 부르면 됨
        //PhotonNetwork.LoadLevel("Room"); //LoadLevel은 혼자만 가능하게해야한다.
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

    private void TextCurrentPlayer() //레디버튼과 연결하여 현재접속중인 인원을 표시해준다.
    {
        textCurrentPlayer.text = $"{ PhotonNetwork.CurrentRoom.PlayerCount}  / { PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestRoom");
    }
}
