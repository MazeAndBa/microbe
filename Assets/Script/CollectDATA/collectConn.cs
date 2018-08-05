﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class collectConn : PunBehaviour
{

    public GameObject obj_gamestart;
    Button btn_start;
    InputField id, username;

    public static string[] ques, option;

    private string serverlink = "140.115.126.137/microbe/";
    string UserID;
    string previousRoomPlayerPrefKey = "Microbe:PreviousRoom";
    ///
    public string previousRoom;
    const string NickNamePlayerPrefsKey = "";
    CompeteManager cm;
    void Start () {


        btn_start = obj_gamestart.GetComponentInChildren<Button>();
        btn_start.onClick.AddListener(gamestart);
        id = obj_gamestart.GetComponentsInChildren<InputField>()[0];
        username = obj_gamestart.GetComponentsInChildren<InputField>()[1];
        //------------暫時註解以下三行以方便測試------------------
        //cm = new CompeteManager();
        //id.text = cm.playerInfo[0];
        //username.text = cm.playerInfo[1];
    }


    void gamestart() {
        //-----------暫時不使用xmlprocess以方便測試------------------
        createUser();
        //-----------------------------------------------------------
        obj_gamestart.gameObject.SetActive(false);
        if (PhotonNetwork.AuthValues == null)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
        }
        PhotonNetwork.AuthValues.UserId = id.text;
        Debug.Log("playerName: " + username.text + "AuthValues userID: " + PhotonNetwork.AuthValues.UserId);
        PhotonNetwork.playerName = username.text;
        PlayerPrefs.SetString(NickNamePlayerPrefsKey, username.text);
        PhotonNetwork.ConnectUsingSettings("0.5");
        PhotonHandler.StopFallbackSendAckThread();
        StartCoroutine(getQuestion());
        StartCoroutine(getOption());
    }

    void createUser() {
        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", id.GetComponentsInChildren<Text>()[1].text);
        phpform.AddField("user_name", username.GetComponentsInChildren<Text>()[1].text);
        new WWW(serverlink + "collectData", phpform);
        //Debug.Log("create");
    }

    IEnumerator getQuestion()
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", "getQuestion");
        WWW reg = new WWW(serverlink + "getQuestion", phpform);
        yield return reg;
        if (reg.error == null)
        {
            //Debug.Log(reg.text);
            ques = reg.text.Split(';');//最後一個是空的
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    IEnumerator getOption()
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", "getOption");
        WWW reg = new WWW(serverlink + "getOption", phpform);
        yield return reg;
        if (reg.error == null)
        {
            option = reg.text.Split(';');//最後一個是空的
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }
   



    public override void OnConnectedToMaster()
    {
        this.UserID = PhotonNetwork.player.UserId;
        //Debug.Log("UserID " + this.UserID);

        if (PlayerPrefs.HasKey(previousRoomPlayerPrefKey))//有先前的房間
        {
            this.previousRoom = PlayerPrefs.GetString(previousRoomPlayerPrefKey);
            PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
        }

        // 重新連回原本的房間
        if (!string.IsNullOrEmpty(this.previousRoom))
        {
            Debug.Log("ReJoining previous room: " + this.previousRoom);
            PhotonNetwork.ReJoinRoom(this.previousRoom);
            this.previousRoom = null; 
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();//隨機加入房間
        }
    }
    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)//如果先前房間不存在，則刪除key
    {
        Debug.Log("previousRoom isn't exist");
        this.previousRoom = null;
        PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined other room: " + PhotonNetwork.room.Name);
        this.previousRoom = PhotonNetwork.room.Name;
        PlayerPrefs.SetString(previousRoomPlayerPrefKey, this.previousRoom);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)//如果沒有其他房間可以加入，則創建房間
    {
        Debug.Log("CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 5, PlayerTtl = 15000 }, null);
    }




}
