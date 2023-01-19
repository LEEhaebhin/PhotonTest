using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null;

    //�� Ŭ���̾�Ʈ ���� ������ �÷��̾� ���� ������Ʈ�� �迭�� ����
    private List<GameObject> playerGoList = new List<GameObject>();
    private List<GameObject> leavePlayerGoList = new List<GameObject>(); //���� �÷��̾� ������Ʈ�� ������ ���� ����Ʈ����
    

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
            // �̹� ������ ĳ���͵��� ���о��� ��� �÷��̾��̱� ������ �ڽ��� ������ �÷������� �������ִ� �ڵ尡 �ʿ��ϴ�.
            go.GetComponent<PlayerCtrl>().SetMaterial(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    // PhotonNetwork.LeaveRooom �Լ��� ȣ��Ǹ� ȣ��
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        SceneManager.LoadScene("PhotonLauncher");
    }

    // �÷��̾ ������ �� ȣ��Ǵ� �Լ�
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Entered Room: {0}", otherPlayer.NickName);
        //������ �����ϸ� ��ü Ŭ���̾�Ʈ���� ���� ���Դ��� �˷��ִ� �Լ�ȣ��
        photonView.RPC("ApplyPlayerList", RpcTarget.All); //��κ��� ������� ������ �󸶳� ������ �߾������� �ٽ��̴�.
        //Remote Procedure call : �������� �Լ��� ȣ�����ش�. ���濡���� RpcTarget�� ������� ó���ϴ����� ���� Ŭ���̾�Ʈ���� P2P���� ���� ������ ������ �����Ѵ�.
        //photonView : photon���� �����ϰ� ���� ��Ʈ��ũ ���¸� ������ �� �ִ±��
        //photonView�� Transfortó�� ���� ������ �Լ��� ���������� �ʾƵ� �̹� ��ϵǾ��־� ��밡���ϴ�.
    }

    [PunRPC]
    public void ApplyPlayerList()
    {
        int playerCnt = PhotonNetwork.CurrentRoom.PlayerCount;
        // �÷��̾� ����Ʈ�� �ֽ��̶�� �ǳʶ�
        if (playerCnt == playerGoList.Count) return;

        // ���� �濡 ������ �ִ� �÷��̾��� ��
        Debug.LogError("CurrentRoom PlayerCount : " + playerCnt);

        // ���� �����Ǿ� �ִ� ��� ����� ��������(����� ������Ʈ�� �������ִ� ��� ���̵��� ȣ��)
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // �Ź� �������� �ϴ°� �����Ƿ� �÷��̾� ���ӿ�����Ʈ ����Ʈ�� �ʱ�ȭ
        playerGoList.Clear();

        // ���� �����Ǿ� �ִ� ����� ��ü��
        // �������� �÷��̾���� ���ͳѹ��� ����,
        // �÷��̾� ���ӿ�����Ʈ ����Ʈ�� �߰�
        for (int i = 0; i < playerCnt; ++i)
        {
            // Ű�� 0�� �ƴ� 1���� ����
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // ���� PhotonNetwork.Instantiate�� ���ؼ� ������ ����䰡 �ƴ϶�� �ѱ�
                if (photonViews[j].isRuntimeInstantiated == false) continue;

                // ���� ���� Ű ���� ��ųʸ�(�÷��̾���) ���� �������� �ʴ´ٸ� �ѱ�
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // ������� ���ͳѹ�
                int viewNum = photonViews[j].Owner.ActorNumber;
                // �������� �÷��̾��� ���ͳѹ� (����信�� ���� ��� �÷��̾�� ���ͳѹ���� ��ȣ�� �ٰԵǰ� �׹�ȣ�� �پ� �ִ� �÷��̾ ����濡 �ִ� �÷��̾�� ��ġ�ϴ��� Ȯ��)
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;
                // ���ͳѹ��� ���� ������Ʈ�� �ִٸ�,

                if (viewNum == playerNum)
                {
                    // ���ӿ�����Ʈ �̸��� �˾ƺ��� ���� ����
                    photonViews[j].gameObject.name = "Player_" + photonViews[j].Owner.NickName;
                    // ���� ���ӿ�����Ʈ�� ����Ʈ�� �߰�
                    playerGoList.Add(photonViews[j].gameObject);
                }
                
            }
        }

        // ����׿�
        //PrintPlayerList();
    }

    // �÷��̾ ���� �� ȣ��Ǵ� �Լ�
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


