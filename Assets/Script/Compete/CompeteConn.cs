using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class CompeteViewer : PunBehaviour
{

    public Button btn_search,btn_start;
    public GameObject enemyInfo,playerInfo;
    //Text enemyName;
    CompeteManager cm;
    string playerID,NickName, UserID;//UserID連線後給予的ID
    string previousRoomPlayerPrefKey;
    public string previousRoom;


    void Start () {
        cm = new CompeteManager();
        playerID = cm.playerInfo[0];
        NickName = cm.playerInfo[1];
        playerInfo.GetComponentsInChildren<Text>()[0].text = NickName;

        //enemyName = enemyInfo.GetComponentsInChildren<Text>()[0];
        btn_search.onClick.AddListener(ApplyUserIdAndConnect);
        //btn_start.onClick.AddListener(gameStart);
	}


    void searchOther(){
        cm.searchTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        cm.setEnemy();

        //StartCoroutine(getEnemy());
        //btn_search.gameObject.SetActive(false);
        //btn_start.gameObject.SetActive(true);
    }

    public void ApplyUserIdAndConnect()
    {
        if (PhotonNetwork.AuthValues == null)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
        }
        PhotonNetwork.AuthValues.UserId = playerID;
        Debug.Log("playerName: " + NickName + "AuthValues userID: " + PhotonNetwork.AuthValues.UserId);
        PhotonNetwork.playerName = NickName;
        PhotonNetwork.ConnectUsingSettings("0.5");
        // this way we can force timeouts by pausing the client (in editor)
        PhotonHandler.StopFallbackSendAckThread();
    }

    public override void OnConnectedToMaster()
    {
        // after connect 
        this.UserID = PhotonNetwork.player.UserId;
        Debug.Log("UserID " + this.UserID);

        if (PlayerPrefs.HasKey(previousRoomPlayerPrefKey))//有先前的房間
        {
            Debug.Log("getting previous room from prefs");
            this.previousRoom = PlayerPrefs.GetString(previousRoomPlayerPrefKey);
            PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
        }

        // after timeout: re-join "old" room (if one is known)重新連回原本的房間
        if (!string.IsNullOrEmpty(this.previousRoom))
        {
            Debug.Log("ReJoining previous room: " + this.previousRoom);
            PhotonNetwork.ReJoinRoom(this.previousRoom);
            this.previousRoom = null;       // we only will try to re-join once. if this fails, we will get into a random/new room
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();//隨機加入房間
        }
    }
    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)//如果先前房間不存在，則刪除key
    {
        Debug.Log("OnPhotonJoinRoomFailed");
        this.previousRoom = null;
        PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
    }

    public override void OnJoinedLobby()
    {
        OnConnectedToMaster(); // this way, it does not matter if we join a lobby or not
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.room.Name);
        this.previousRoom = PhotonNetwork.room.Name;
        PlayerPrefs.SetString(previousRoomPlayerPrefKey, this.previousRoom);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)//如果沒有其他房間可以加入，則創建房間
    {
        Debug.Log("CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 20000 }, null);
    }



    /*適用於非同步競賽
     * IEnumerator getEnemy()
    {
        StartCoroutine(cm.requestMember());
        yield return new WaitForSeconds(0.1f);
        enemyName.text = cm.enemyInfo[1];
    }
    */


    void gameStart() {
        cm.startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        cm.setEnemy();

    }


}
