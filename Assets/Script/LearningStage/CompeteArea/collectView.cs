using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class collectView : PunBehaviour, IPunTurnManagerCallbacks
{
    public GameObject ConnectUiView, WaitingUI, GameStartUI, ResultUIView, cardgroup, card;
    public Text question, RemotePlayerText, LocalPlayerText, TurnText, TimeText;
    public Image WinorLossImg;
    Button btn_gamestart, btn_exit;


    float currentTime;
    string[] s_option;//該回合的選項
    string[] quesInfo, optionInfo;

    private PunTurnManager turnManager;
    private string localSelection, remoteSelection;
    private DateTime localTime;
    private DateTime remoteTime;
    private ResultType result;
    private bool IsShowingResults;

    public enum ResultType
    {
        None = 0,
        Draw,
        LocalRAnsSTime,
        LocalRAnsLTime,
        LocalWrongAns
    }

    void Start() {
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = 15f;
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

    }


    #region Implement IPunTurnManagerCallbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    /// 
    public void OnTurnBegins(int turn)
    {

        //回合初始化
        this.StartCoroutine("initialTurn");


        this.localSelection = "";
        this.remoteSelection = "";
        IsShowingResults = false;

    }


    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);

        this.CalculateWinAndLoss();
        this.UpdateScores();
        this.OnEndTurn();
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


    public void OnTurnTimeEnds(int obj)
    {
        if (!IsShowingResults)
        {
            Debug.Log("Time's up!");
            CancelInvoke("timer");
            if (cardgroup.GetComponent<Button>().IsInteractable()) {
                cardgroup.GetComponent<Button>().interactable = false;
            }
            OnTurnCompleted(-1);
        }
    }

    public void OnEndTurn()
    {
        if (this.turnManager.Turn < 5)
        {
            this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
            this.UpdatePlayerTexts();
            this.StartTurn();
        }
        else //競賽結束，顯示本次雙方分數
        {
            GameStartUI.SetActive(false);
            ResultUIView.SetActive(true);
            PhotonPlayer[] player = PhotonNetwork.playerList;
            //依分數排序
            for (int i = 0; i < PhotonNetwork.room.PlayerCount - 1; i++)
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
            for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
            {
                ResultUIView.GetComponentsInChildren<Text>()[1].text +=(i+1)+" "+player[i].NickName + "　Score:" + player[i].GetScore().ToString("D2")+"<br/>";
            }
            StartCoroutine(gameover());
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

        this.TimeText.text = this.turnManager.TurnDuration.ToString();
        currentTime = 0f;
        InvokeRepeating("timer", 1f, 1f);

    }

    //計時器
    private void timer()
    {
        if (currentTime != this.turnManager.TurnDuration)
        {
            currentTime += 1;
            this.TimeText.text = (this.turnManager.TurnDuration - currentTime) + "";//顯示倒數秒數
        }
        else
        {
            this.TimeText.text = "0";
        }
    }

    public IEnumerator initialTurn()
    {
        yield return new WaitForSeconds(1.0f);
        //存取新題目、選項、當前回合數
        quesInfo = this.turnManager.TurnQues;
        s_option = this.turnManager.TurnOption;
        TurnText.text = this.turnManager.Turn.ToString();

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

    }

    //建立卡牌
    void createCard()
    {
        int ans_pos = UnityEngine.Random.Range(0, 15);
        if (quesInfo != null && quesInfo.Length > 0)
        {
            this.question.text = quesInfo[1];// ques_content

            for (int i = 0, j = 0; i < s_option.Length+1; i++)
            {
                GameObject cardObj = Instantiate(card);
                cardObj.gameObject.SetActive(true);
                if (i == ans_pos)
                {
                    cardObj.GetComponentInChildren<Text>().text = quesInfo[2];//correct answer
                    cardObj.name = quesInfo[2];
                }
                else
                {
                    optionInfo = s_option[j].Split(',');
                    cardObj.GetComponentInChildren<Text>().text = optionInfo[1];//other ans_content
                    cardObj.name = optionInfo[1];
                    j++;
                    Debug.Log("options "+ optionInfo[1]);
                }
                cardObj.GetComponent<Button>().onClick.AddListener(delegate () { MakeTurn(cardObj.name); });
                cardObj.transform.SetParent(cardgroup.transform);
                cardObj.transform.localPosition = new Vector3(-350 + (i % 4) * 160, (i / 4) * -150, 0);
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
            this.result = ResultType.LocalWrongAns;
            Debug.Log("You hadn't select");
            return;
        }

        if (this.remoteSelection == "")
        {
            this.result = ResultType.LocalWrongAns;
        }

        if (this.localSelection == quesInfo[2])
        {
            if (DateTime.Compare(localTime, remoteTime) < 0)
            {
                this.result = ResultType.LocalRAnsSTime;
                Debug.Log("You fast");
            }
            else if (DateTime.Compare(localTime, remoteTime) > 0)
            {
                this.result = ResultType.LocalRAnsLTime;
                Debug.Log("You slow");
            }
            else {
                this.result = ResultType.Draw;
                Debug.Log("Draw");
            }
        }
        else {
            this.result = ResultType.LocalWrongAns;
            Debug.Log("You wong");
        }
    }

    //顯示當回合的競賽結果
    public IEnumerator ShowResultsBeginNextTurnCoroutine()
    {
        //ButtonCanvasGroup.interactable = false;
        IsShowingResults = true;
        switch (this.result)
        {
            case ResultType.LocalRAnsSTime:
                //WinorLossImg.sprite = ;
                break;
            case ResultType.LocalRAnsLTime:
                //WinorLossImg.sprite = ;
                break;
            case ResultType.Draw:
                //WinorLossImg.sprite = ;
                break;
            case ResultType.LocalWrongAns:
                //WinorLossImg.sprite = ;
                break;
        }
        yield return new WaitForSeconds(1.0f);

    }

    //計算得分
    private void UpdateScores()
    {
        PhotonPlayer remote = PhotonNetwork.player.GetNext();
        PhotonPlayer local = PhotonNetwork.player;
        
        switch (this.result)
        {
            case ResultType.LocalRAnsSTime:
                PhotonNetwork.player.AddScore((int)(DateDiff(this.localTime, remoteTime) * 3 + local.GetScore() * 0.3));
                break;
            case ResultType.LocalRAnsLTime:
                PhotonNetwork.player.AddScore((int)(DateDiff(this.localTime, remoteTime) + local.GetScore() * 0.3));
                break;
            case ResultType.Draw:
                PhotonNetwork.player.AddScore((int)(local.GetScore() * 0.5));
                break;
            case ResultType.LocalWrongAns:
                PhotonNetwork.player.AddScore(0);
                break;
        }
    }

    void UpdatePlayerTexts()//更新自己與對手狀態(名字+分數)
    {
        PhotonPlayer local = PhotonNetwork.player;
        PhotonPlayer[] player = PhotonNetwork.playerList;
        //依分數排序
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
        //更新排行榜
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            GameObject GameRank = GameObject.FindGameObjectWithTag("GameRank");
            GameRank.GetComponentsInChildren<Text>()[i].text = player[i].NickName + "　Score:" + player[i].GetScore().ToString("D2");
        }
        if (local != null)
        {
            // should be this format: "YOU   00"
            this.LocalPlayerText.text = "You: " + local.NickName + "　Score:" + local.GetScore().ToString("D2");
        }

        /*
        PhotonPlayer remote = PhotonNetwork.player.GetNextFor(local);
        if (remote != null)
        {
            this.RemotePlayerText.text = "Remote Name:" + remote.NickName + "　Score:" + remote.GetScore().ToString("D2");
        }
        else
        {
            this.RemotePlayerText.text = "Remote Name:";
            this.TimeText.text = "";
        }
        */

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
        //Initialize players'name
        for (int i = 1; i < 5; i++)
        {
            PhotonPlayer[] player = PhotonNetwork.playerList;
            GameObject PlayerInfo = GameObject.FindGameObjectWithTag("Player" + i);
            PlayerInfo.GetComponentsInChildren<Text>()[0].text = "";
        }
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            for (int i = 1; i < PhotonNetwork.room.PlayerCount; i++)
            {
                PhotonPlayer[] player = PhotonNetwork.playerList;
                GameObject PlayerInfo = GameObject.FindGameObjectWithTag("Player" + i);
                PlayerInfo.GetComponentsInChildren<Text>()[0].text = player[i].NickName;
            }
        }

    }

    void InitialGameUI() {
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            PhotonPlayer[] player = PhotonNetwork.playerList;
            Text remote = Instantiate(RemotePlayerText);
            GameObject GameRank = GameObject.FindGameObjectWithTag("GameRank");
            remote.transform.SetParent(GameRank.transform);
            remote.transform.localPosition = new Vector3(0, - i * 100, 0);
            remote.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            remote.name = (i+1)+"";
            remote.text = player[i].NickName + "　Score:" + player[i].GetScore().ToString("D2");
        }
    }

    IEnumerator gameover() {
        yield return new WaitForSeconds(3.0f);
        ResultUIView.SetActive(false);
        ExitGame();
    }

    #endregion

    #region Button Event

    public void ClickGameStart()
    {
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            this.photonView.RPC("GameStart", PhotonTargets.All);
        }
        else
        {
            Debug.Log("Player isn't enough.");
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
