using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0.0.1";
    [SerializeField] private byte maxPlayerPerRoom = 4;

    [SerializeField] private string nickName = string.Empty;

    [SerializeField] private Button connectButton = null;


    private void Awake()
    {
        // 마스터가 PhotonNetwork.LoadLevel()을 호출하면,
        // 모든 플레이어가 동일한 레벨을 자동으로 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = true;
    }

    // ConnectButton이 눌러지면 호출

    public void Connect()
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



    // InputField_NickName과 연결해 닉네임을 가져옴
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

        connectButton.interactable = false;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;

        // 방을 생성하면 OnJoinedRoom 호출
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        // 마스터가 동시에 게임을 시작하게하는 구조가 아니기 때문에 각자 씬을 부르면 됨
        //PhotonNetwork.LoadLevel("Room"); //LoadLevel은 혼자만 가능하게해야한다.
        SceneManager.LoadScene("Room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        connectButton.interactable = true;

        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}
// 출처] 포톤 엔진(Photon Engine) 네트워크 - 접속 | 작성자 구름고래

