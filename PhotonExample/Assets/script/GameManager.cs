using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null;

    //각 클라이언트 마다 생선된 플레이어 게임 오브젝트를 배열로 관리
    private List<GameObject> playerGoList = new List<GameObject>();
    private List<GameObject> leavePlayerGoList = new List<GameObject>(); //나간 플레이어 오브젝트의 정보를 담을 리스트생성
    

    private void Start()
    {

        for(int j = 0; j<leavePlayerGoList.Count; ++j)
        {
            if (playerPrefab != null && leavePlayerGoList[j].gameObject.name == photonView.Owner.NickName)
            {
                GameObject go = PhotonNetwork.Instantiate(leavePlayerGoList[j].name, leavePlayerGoList[j].transform.position, Quaternion.identity, 0);
            }

            else
                continue;
            }


        if (playerPrefab != null)
        {
            GameObject go = PhotonNetwork.Instantiate(
                playerPrefab.name,
                new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f)),
                Quaternion.identity,
                0);
            // 이미 생성된 캐릭터들은 구분없이 모두 플레이어이기 때문에 자신이 조작할 플레이으를 지정해주는 코드가 필요하다.
            go.GetComponent<PlayerCtrl>().SetMaterial(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    // PhotonNetwork.LeaveRooom 함수가 호출되면 호출
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        SceneManager.LoadScene("PhotonLauncher");
    }

    // 플레이어가 입장할 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Entered Room: {0}", otherPlayer.NickName);
        //누군가 접속하면 전체 클라이언트에서 누가 들어왔는지 알려주는 함수호출
        photonView.RPC("ApplyPlayerList", RpcTarget.All); //요부분을 어떤식으로 쓰는지 얼마나 적당히 잘쓰는지가 핵심이다.
        //Remote Procedure call : 원격으로 함수를 호출해준다. 포톤에서는 RpcTarget을 어떤식으로 처리하는지에 따라 클라이언트인지 P2P인지 등의 서버의 종류를 구분한다.
        //photonView : photon에서 유일하게 나의 네트워크 상태를 관찰할 수 있는기능
        //photonView는 Transfort처럼 따로 변수나 함수로 지정해주지 않아도 이미 등록되어있어 사용가능하다.
    }

    [PunRPC]
    public void ApplyPlayerList()
    {
        int playerCnt = PhotonNetwork.CurrentRoom.PlayerCount;
        // 플레이어 리스트가 최신이라면 건너뜀
        if (playerCnt == playerGoList.Count) return;

        // 현재 방에 접속해 있는 플레이어의 수
        Debug.LogError("CurrentRoom PlayerCount : " + playerCnt);

        // 현재 생성되어 있는 모든 포톤뷰 가져오기(포톤뷰 컴포넌트를 가지고있는 모든 아이들을 호출)
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // 매번 재정렬을 하는게 좋으므로 플레이어 게임오브젝트 리스트를 초기화
        playerGoList.Clear();

        // 현재 생성되어 있는 포톤뷰 전체와
        // 접속중인 플레이어들의 액터넘버를 비교해,
        // 플레이어 게임오브젝트 리스트에 추가
        for (int i = 0; i < playerCnt; ++i)
        {
            // 키는 0이 아닌 1부터 시작
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // 만약 PhotonNetwork.Instantiate를 통해서 생성된 포톤뷰가 아니라면 넘김
                if (photonViews[j].isRuntimeInstantiated == false) continue;

                // 만약 현재 키 값이 딕셔너리(플레이어목록) 내에 존재하지 않는다면 넘김
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // 포톤뷰의 액터넘버
                int viewNum = photonViews[j].Owner.ActorNumber;
                // 접속중인 플레이어의 액터넘버 (포톤뷰에서 만든 모든 플레이어는 엑터넘버라는 번호가 붙게되고 그번호가 붙어 있는 플레이어가 현재방에 있는 플레이어와 일치하는지 확인)
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;
                // 액터넘버가 같은 오브젝트가 있다면,

                if (viewNum == playerNum)
                {
                    // 게임오브젝트 이름도 알아보기 쉽게 변경
                    photonViews[j].gameObject.name = "Player_" + photonViews[j].Owner.NickName;
                    // 실제 게임오브젝트를 리스트에 추가
                    playerGoList.Add(photonViews[j].gameObject);
                }
                
            }
        }

        // 디버그용
        //PrintPlayerList();
    }

    // 플레이어가 나갈 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}",
                        otherPlayer.NickName);
    }

    public void LeaveRoom()
    {
        Debug.Log("Leave Room");
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        for (int i = 0; i < playerGoList.Count; ++i)
        {
            leavePlayerGoList.Add(playerGoList[i]);
        }

        PhotonNetwork.LeaveRoom();
    }
}


