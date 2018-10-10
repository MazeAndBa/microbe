﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class collectView : PunBehaviour, IPunTurnManagerCallbacks
{
    public GameObject ConnectUiView, WaitingUI, GameStartUI,ShowResultMes, ResultUIView, cardgroup, card;
    public Text question, RemotePlayerText, LocalPlayerText, TurnText, TimeText;
    public AudioSource vol_pronun;
    Button btn_gamestart, btn_exit,btn_hintLA,btn_hintST;
    bool timerflag = false;

    int currentTime;
    int cardCount;//卡牌數量
    int c_hintLA_count, c_hintST_count;//當前使用提示的次數
    int hintLA_count, hintST_count;//使用提示的最大次數
    string[] s_option;//該回合的選項
    string[] quesInfo, optionInfo;
    DateTime TurnStartTime;


    private PunTurnManager turnManager;
    private string localSelection, remoteSelection;
    private DateTime localTime;
    private DateTime remoteTime;
    private ResultType result;
    private bool IsShowingResults;

    #region 記錄資料
    Xmlprocess xmlprocess;
    #endregion

    public enum ResultType
    {
        None = 0,
        CorrectAns,
        WrongAns
    }

    void Start() {
        xmlprocess = new Xmlprocess();
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = 15f;
        cardCount = 16;
        RefreshConnectUI();
    }



    void Update() {
        if (PhotonNetwork.connected)
        {
            this.ConnectUiView.SetActive(false);
        }

        if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !this.ConnectUiView.GetActive())
        {
            this.ConnectUiView.SetActive(true);
        }
        if (timerflag)
        {
            currentTime = (int)turnManager.RemainingSecondsInTurn;
            this.TimeText.text = currentTime.ToString();//顯示倒數秒數
        }
    }


    #region Implement IPunTurnManagerCallbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    /// 
    public void OnTurnBegins(int turn)
    {

        //回合初始化
        this.StartCoroutine("initialTurn");
    }

    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);
        timerflag = false;
        this.CalculateWinAndLoss();
        this.UpdateScores();
    }

    public void OnTurnTimeEnds(int obj)
    {
        if (!IsShowingResults)
        {
            Debug.Log("Time's up!");
            OnTurnCompleted(-1);
        }
    }

    // when a player moved (but did not finish the turn)
    public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move.ToString());
        throw new NotImplementedException();
    }


    //when a player made the last/final move in a turn
    public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move.ToString());

        if (photonPlayer.IsLocal)
        {
            this.localTime = DateTime.Now;
            this.localSelection = move.ToString();
        }
        else
        {
            this.remoteTime = DateTime.Now;
            this.remoteSelection = move.ToString();
        }
    }

    public void OnEndTurn()
    {
        if (this.turnManager.Turn < 5)
        {
            this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
        }
        else //競賽結束，顯示本次雙方分數
        {
            GameObject[] PlayerLists = GameObject.FindGameObjectsWithTag("PlayerLists");//抓取玩家名單的物件，方便銷毀
            GameStartUI.SetActive(false);
            ResultUIView.SetActive(true);
            PhotonPlayer[] player = PhotonNetwork.playerList;
            PhotonPlayer local = PhotonNetwork.player;
            int localRank = 0;

            for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
            {
                if (player[i].NickName == local.NickName) localRank = i+1;
                ResultUIView.GetComponentsInChildren<Text>()[1].text +=(i+1)+"\t"+player[i].NickName + "　分數:" + player[i].GetScore().ToString("D2")+"\n";
            }
            ResultUIView.GetComponentsInChildren<Text>()[2].text = c_hintLA_count.ToString();
            ResultUIView.GetComponentsInChildren<Text>()[3].text = c_hintST_count.ToString();
            xmlprocess.setCompeteScoreRecord(c_hintLA_count,c_hintST_count,local.GetScore(), localRank);
            this.StartCoroutine(gameover(PlayerLists));
        }

    }

    #endregion


    public void StartTurn()
    {
        Debug.Log("start");
        if (this.turnManager.Turn == 0) {
            InitialGameUI();
        }
        //房主抓取題目、選項、當前回合數
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
            this.turnManager.selectQues(collectConn.ques);
            this.turnManager.randomOptions(collectConn.option);
        }
        currentTime = (int)this.turnManager.TurnDuration ;
        this.TimeText.text = currentTime.ToString();
        this.localSelection = "";
        this.remoteSelection = "";
        IsShowingResults = false;
    }

    public IEnumerator initialTurn()//回合初始化
    {
        yield return new WaitForSeconds(1.0f);
        //存取新題目、選項、當前回合數
        quesInfo = this.turnManager.TurnQues;
        s_option = this.turnManager.TurnOption;
        TurnText.text = this.turnManager.Turn.ToString();
        xmlprocess.createRoundRecord(quesInfo[0]);//創建新的回合紀錄

        //銷毀上一回合的卡片
        GameObject[] tmp_cards = GameObject.FindGameObjectsWithTag("card");
        if (tmp_cards.Length > 0)
        {
            for (int i = 0; i < tmp_cards.Length; i++)
            {
                Destroy(tmp_cards[i]);
            }
        }
        //產生卡牌
        createCard();
        cardgroup.SetActive(true);
        //播放聲音
        vol_pronun.clip = Resources.Load("Sound/" + quesInfo[2], typeof(AudioClip)) as AudioClip;
        vol_pronun.Play();

        timerflag = true;
        TurnStartTime = DateTime.Now;
        //提示按鈕監聽事件
        btn_hintLA.onClick.AddListener(ListenAgain);
        btn_hintST.onClick.AddListener(ShowTranslation);

    }

    //建立卡牌
    void createCard()
    {
        int ans_pos = UnityEngine.Random.Range(0, cardCount-1);
        if (quesInfo != null && quesInfo.Length > 0)
        {

            for (int i = 0, j = -1; i < cardCount; i++)
            {
                GameObject cardObj = Instantiate(card);
                cardObj.gameObject.SetActive(true);

                do {//如果選項與答案相同,則跳過抓下一個選項
                    j++;
                    optionInfo = s_option[j].Split(',');
                } while (optionInfo[1] == quesInfo[2]);

                if (i == ans_pos)//如果當前位置為答案位置
                {
                    cardObj.GetComponentInChildren<Text>().text = quesInfo[2];
                    cardObj.name = quesInfo[2];
                }
                else
                {
                    cardObj.GetComponentInChildren<Text>().text = optionInfo[1];
                    cardObj.name = optionInfo[1];
                    //Debug.Log("options: " + optionInfo[1]);
                }
                    cardObj.GetComponent<Button>().onClick.AddListener(delegate () { MakeTurn(cardObj.name); });
                    cardObj.transform.SetParent(cardgroup.transform);
                    cardObj.transform.localPosition = new Vector3(-350 + (i % 4) * 160, (i / 4) * -150+150, 0);
                    cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    //set plaer's selection
    public void MakeTurn(string cardName)
    {
        this.turnManager.SendMove(cardName, true);
    }

    //作答耗費的時間
    private int DateDiff(DateTime DateTime1, DateTime DateTime2)
    {
        int dateDiff = 0;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();
        dateDiff = ts.Seconds;
        return dateDiff;
    }


    //判斷輸贏結果
    private void CalculateWinAndLoss()
    {
        if (this.localSelection == "")
        {
            this.result = ResultType.None;
            Debug.Log("You hadn't select");
            return;
        }

        if (this.remoteSelection == "")
        {
            this.result = ResultType.None;
        }

        if (this.localSelection == quesInfo[2])
        {
           this.result = ResultType.CorrectAns;
            Debug.Log("Correct answer!");
        }
        else {
            this.result = ResultType.WrongAns;
            Debug.Log("Wrong answer");
        }
    }

    //顯示當回合的競賽結果
    public IEnumerator ShowResultsBeginNextTurnCoroutine()
    {
        //ButtonCanvasGroup.interactable = false;
        IsShowingResults = true;
        GameObject ResultMes = Instantiate(ShowResultMes);
        ResultMes.transform.SetParent(GameStartUI.transform);
        ResultMes.transform.localPosition = Vector3.zero;
        ResultMes.transform.localScale = Vector3.one;
        Image imgResult = ResultMes.GetComponentsInChildren<Image>()[1];
        Text textResult = ResultMes.GetComponentInChildren<Text>();


        switch (this.result)
        {
            case ResultType.None:
                imgResult.sprite = Resources.Load("Image/none", typeof(Sprite)) as Sprite;
                textResult.text = "你沒有選擇卡牌";
                break;
            case ResultType.CorrectAns:
                imgResult.sprite = Resources.Load("Image/correct", typeof(Sprite)) as Sprite;
                textResult.text = "答對囉！";

                break;
            case ResultType.WrongAns:
                imgResult.sprite = Resources.Load("Image/wrong", typeof(Sprite)) as Sprite;
                textResult.text = "正確答案:"+ quesInfo[2];

                break;
        }
        yield return new WaitForSeconds(1.0f);
        Destroy(ResultMes);
        this.StartTurn();
    }

    //計算得分
    private void UpdateScores()
    {
        PhotonPlayer local = PhotonNetwork.player;
        int spendTime = DateDiff(this.localTime, TurnStartTime);
        int restTime = (int)this.turnManager.TurnDuration - spendTime;//剩餘時間
        int _hintLA = xmlprocess.getRoundHintcount("hint_LA");//當回合使用提示再聽一次的次數
        int _hintST = xmlprocess.getRoundHintcount("hint_ST");//當回合使用提示中譯的次數
        string resultState = "";
        switch (this.result)
        {
            case ResultType.CorrectAns:
                PhotonNetwork.player.AddScore((int)(restTime * 1.5 + local.GetScore() * 0.3 - (_hintLA * 1) - (_hintST *3)+ (PhotonNetwork.room.PlayerCount * 0.5) ));//剩餘時間*1.5+原本分數*0.3-使用提示+房間人數*0.5
                resultState = "correct";
                break;
            case ResultType.None:
                PhotonNetwork.player.AddScore(-5);
                resultState = "none";
                break;
            case ResultType.WrongAns:
                PhotonNetwork.player.AddScore(0);
                resultState = "wrong";
                break;
        }
        //Debug.Log("花費時間: "+ spendTime);
        xmlprocess.setRoundAns(resultState, spendTime);
        StartCoroutine(UpdatePlayerTexts());
    }

//更新即時排名
    IEnumerator UpdatePlayerTexts()
    {
        Debug.Log("Refresh the leadboard!");
        yield return new WaitForSeconds(0.1f);
        PhotonPlayer local = PhotonNetwork.player;
        PhotonPlayer[] player = PhotonNetwork.playerList;
        int localRank = 0;
        //依分數排序玩家清單
        for (int i = 0; i < PhotonNetwork.room.PlayerCount-1; i++)
        {
            for (int j = i + 1; j < PhotonNetwork.room.PlayerCount; j++)
            {
                if (player[i].GetScore() < player[j].GetScore())
                {
                    PhotonPlayer tmp = player[j];
                    player[j] = player[i];
                    player[i] = tmp;
                }
            }
        }
        //更新排行榜UI與自己畫面的分數
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            GameObject GameRank = GameObject.FindGameObjectWithTag("GameRank");
            GameRank.GetComponentsInChildren<Text>()[i].text = player[i].NickName + "　Score:" + player[i].GetScore().ToString("D2");
            if (local.NickName == player[i].NickName) localRank = i+1;
        }
        if (local != null)
        {
            this.LocalPlayerText.text = local.GetScore().ToString("D2");
        }
        xmlprocess.setRoundScore(local.GetScore(),localRank);
        //回合結束
        this.OnEndTurn();
    }

    #region Recheck connect and Initialize UI
    void RefreshConnectUI()
    {
        this.ConnectUiView.SetActive(!PhotonNetwork.inRoom);//如果還沒進房間則顯示連線畫面

        this.WaitingUI.SetActive(PhotonNetwork.inRoom);
        if (GameStartUI.GetActive())
        {
            this.GameStartUI.SetActive(false);
        }
    }

    void RefreshWaitUI() {
        btn_gamestart = this.WaitingUI.GetComponentsInChildren<Button>()[0];
        btn_gamestart.onClick.AddListener(ClickGameStart);
        btn_exit = this.WaitingUI.GetComponentsInChildren<Button>()[1];
        btn_exit.onClick.AddListener(ExitGame);

        PhotonPlayer hostPlayer = PhotonNetwork.masterClient;
        GameObject HostInfo = GameObject.FindGameObjectWithTag("Host");
        HostInfo.GetComponentsInChildren<Text>()[0].text = hostPlayer.NickName;
        switch (xmlprocess.getUserInfo()[3])
        {
            case "0":
                HostInfo.GetComponentsInChildren<Image>()[0].sprite = Resources.Load("Image/boy", typeof(Sprite)) as Sprite;
                break;
            case "1":
                HostInfo.GetComponentsInChildren<Image>()[0].sprite = Resources.Load("Image/girl", typeof(Sprite)) as Sprite;
                break;
        }

        //Initialize players'name
        for (int i = 1; i < 5; i++)
        {
            PhotonPlayer[] player = PhotonNetwork.playerList;
            GameObject PlayerInfo = GameObject.FindGameObjectWithTag("Player" + i);
            PlayerInfo.GetComponentsInChildren<Text>()[0].text = "";
            PlayerInfo.GetComponentsInChildren<Image>()[0].sprite = Resources.LoadAll<Sprite>("compete")[1];

        }
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            for (int i = 1; i < PhotonNetwork.room.PlayerCount; i++)
            {
                PhotonPlayer[] player = PhotonNetwork.playerList;
                GameObject PlayerInfo = GameObject.FindGameObjectWithTag("Player" + i);
                PlayerInfo.GetComponentsInChildren<Text>()[0].text = player[i].NickName;
                switch (xmlprocess.getUserInfo()[3])
                {
                    case "0":
                        PlayerInfo.GetComponentsInChildren<Image>()[0].sprite = Resources.Load("Image/boy", typeof(Sprite)) as Sprite;
                        break;
                    case "1":
                        PlayerInfo.GetComponentsInChildren<Image>()[0].sprite = Resources.Load("Image/girl", typeof(Sprite)) as Sprite;
                        break;
                }
            }
        }

    }

    void InitialGameUI() {
        //初次進入進行遊戲畫面初始化

        btn_hintLA = this.GameStartUI.GetComponentsInChildren<Button>()[0];
        btn_hintST = this.GameStartUI.GetComponentsInChildren<Button>()[1];
        hintLA_count = 3;hintST_count = 3;
        c_hintLA_count = 0; c_hintST_count = 0;
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            PhotonPlayer local = PhotonNetwork.player;
            LocalPlayerText.text = local.GetScore().ToString("D2");
            PhotonPlayer[] player = PhotonNetwork.playerList;
            Text remote = Instantiate(RemotePlayerText);
            GameObject GameRank = GameObject.FindGameObjectWithTag("GameRank");
            remote.transform.SetParent(GameRank.transform);
            remote.transform.localPosition = new Vector3(25, - i * 80+165, 0);
            remote.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            remote.name = (i+1)+"";
            remote.text = player[i].NickName + "\n分數:" + player[i].GetScore().ToString("D2");
        }
    }

    IEnumerator gameover(GameObject [] PlayerLists) {

        yield return new WaitForSeconds(3.0f);
        ResultUIView.SetActive(false);
        //遊戲結束重置玩家分數
        PhotonPlayer[] player = PhotonNetwork.playerList;
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            player[i].SetScore(0);
        }

        //銷毀排行榜的玩家名單物件
        if (PlayerLists.Length > 0)
        {
            for (int i = 0; i < PlayerLists.Length; i++)
            {
                Destroy(PlayerLists[i]);
            }
        }
        ExitGame();
    }

    #endregion

    #region Button Event

    public void ClickGameStart()
    {
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            xmlprocess.ScceneHistoryRecord("StartCompete", DateTime.Now.ToString("HH:mm:ss"));
            xmlprocess.setCompeteCount("compete_count");
            xmlprocess.createCompeteRecord();
            this.photonView.RPC("GameStart", PhotonTargets.All);
        }
        else
        {
            Debug.Log("Player isn't enough.");
        }
    }

    void ListenAgain() {
        if (c_hintLA_count < hintLA_count)
        {
            vol_pronun.Play();
            c_hintLA_count++;
            btn_hintLA.GetComponentsInChildren<Text>()[0].text = (hintLA_count- c_hintLA_count) + "/"+ hintLA_count;
            xmlprocess.setRoundHintcount("hint_LA");
        }
        else {
            btn_hintLA.interactable = false;
        }
    }

    void ShowTranslation() {
        if (c_hintST_count < hintST_count)
        {
            this.question.text = quesInfo[1];
            c_hintST_count++;
            btn_hintST.GetComponentsInChildren<Text>()[0].text = (hintST_count - c_hintST_count) + "/" + hintST_count;
            xmlprocess.setRoundHintcount("hint_ST");
        }
        else {
            btn_hintST.interactable = false;
        }
    }

    void ExitGame()
    {
        //PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.Disconnect();
    }

    [PunRPC]
    void GameStart()
    {
        if (this.turnManager.Turn == 0)
        {
            // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
            this.WaitingUI.SetActive(false);
            this.GameStartUI.SetActive(true);
            this.StartTurn();
        }
    }

    #endregion

    public override void OnJoinedRoom()
    {
        RefreshConnectUI();
        RefreshWaitUI();
        if (PhotonNetwork.room.PlayerCount <= 5)
        {

            if (PhotonNetwork.isMasterClient)
            {
                //房主才有遊戲開始的按鈕
                btn_gamestart.gameObject.SetActive(true);
            }
            else
            {
                btn_gamestart.gameObject.SetActive(false);
            }
            Debug.Log("Waiting for another player");
        }

    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");
        RefreshConnectUI();

    }

    public override void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitchedto: " + PhotonNetwork.masterClient.NickName);
        RefreshWaitUI();
        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.IsLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.NickName + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }



    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Other player arrived");

        GameObject PlayerInfo = GameObject.FindGameObjectWithTag("Player"+(PhotonNetwork.room.PlayerCount-1));
        PlayerInfo.GetComponentsInChildren<Text>()[0].text = newPlayer.NickName;
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        RefreshConnectUI();
        RefreshWaitUI();
        Debug.Log("Other player disconnected! " + otherPlayer.ToStringFull());
    }

    public override void OnDisconnectedFromPhoton()
    {
        RefreshConnectUI();
        Debug.Log("OnFailedToConnectToPhoton");
    }


}
