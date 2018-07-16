using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using System;

public class collectView : PunBehaviour, IPunTurnManagerCallbacks
{
   public GameObject ConnectUiView, GameUiView,ResultUIView, cardgroup, card,WaitingUI;
   public Text question, RemotePlayerText, LocalPlayerText,Turn, TimeText;
    public Image WinorLossImg;
    float currentTime;
    int[] i_option;//該回合的選項
    string[] quesInfo,optionInfo;
    private PunTurnManager turnManager;
    private ResultType result;
    private bool IsShowingResults;
    private string localSelection, remoteSelection;
    private DateTime localTime, remoteTime;

    public enum ResultType
    {
        None = 0,
        Draw,
        LocalRAnsSTime,
        LocalRAnsLTime,
        LocalWrongAns
    }

    void Start () {
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = 10f;

        RefreshUIViews();
    }

    

    void Update () {
        if (PhotonNetwork.connected && this.ConnectUiView.GetActive())
        {
            this.ConnectUiView.SetActive(false);
            if (PhotonNetwork.room.PlayerCount > 1 && this.WaitingUI.GetActive())
            {
                this.WaitingUI.SetActive(false);
            }
        }
        else if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !this.ConnectUiView.GetActive())
        {
            this.ConnectUiView.SetActive(true);
        }

        this.UpdatePlayerTexts();

    }


    #region Implement IPunTurnManagerCallbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    /// 
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        this.StartCoroutine("initialTurn");
        this.TimeText.text = this.turnManager.TurnDuration.ToString();
        currentTime = 0f;
        InvokeRepeating("timer", 1f, 1f);

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
            if (cardgroup.GetComponent<Button>().IsInteractable()) {
                cardgroup.GetComponent<Button>().interactable = false;
            }
            OnTurnCompleted(-1);
        }
    }
    #endregion

    //set plaer's selection
    public void MakeTurn(string cardName)
    {
        this.turnManager.SendMove(cardName, true);
    }

    public void StartTurn()
    {
        Debug.Log("start");
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
            this.turnManager.selectQues(collectConn.ques);
            this.turnManager.randomOptions(collectConn.option);
        }
        this.UpdatePlayerTexts();

    }


    public void OnEndTurn()
    {
        if (this.turnManager.Turn < 5)
        {
            this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");

        }
        else //競賽結束，顯示本次雙方分數
        {
            GameUiView.SetActive(false);
            ResultUIView.SetActive(true);
            PhotonPlayer remote = PhotonNetwork.player.GetNext();
            PhotonPlayer local = PhotonNetwork.player;
            ResultUIView.GetComponentsInChildren<Text>()[1].text = local.GetScore().ToString();
            ResultUIView.GetComponentsInChildren<Text>()[3].text = remote.GetScore().ToString();
            Debug.Log("Your Score:" + local.GetScore() + " remote's Score: " + remote.GetScore());

        }
    }

    void RefreshUIViews()
    {
        ConnectUiView.SetActive(!PhotonNetwork.inRoom);//如果還沒進房間則顯示連線畫面
        GameUiView.SetActive(PhotonNetwork.inRoom);
        //cardgroup.SetActive(PhotonNetwork.room != null ? PhotonNetwork.room.PlayerCount > 1 : false);//如果能get房號且房間人數大於1，則button才可互動

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

    //時間郵戳
    private int DateDiff(DateTime DateTime1, DateTime DateTime2)
    {
        int dateDiff = 0;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();
        dateDiff = ts.Seconds;
        return dateDiff;
    }

    //建立卡牌
    void createCard()
    {
        int ans_pos = UnityEngine.Random.Range(0, 23);
        if ( quesInfo!= null && quesInfo.Length > 0)
        {
            this.question.text = quesInfo[1];// ques_content

            for (int i = 0, j = 0; i < 24; i++)
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
                    optionInfo = collectConn.option[i_option[j]].Split(',');
                    cardObj.GetComponentInChildren<Text>().text = optionInfo[1];//other ans_content
                    cardObj.name = optionInfo[1];
                    j++;
                    //Debug.Log("options"+ optionInfo[1]);
                }
                cardObj.GetComponent<Button>().onClick.AddListener(delegate () { MakeTurn(cardObj.name); });
                cardObj.transform.SetParent(cardgroup.transform);
                cardObj.transform.localPosition = new Vector3(-400 + (i % 6) * 160, -250 + (i / 6) * 150, 0);
                cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }


    public IEnumerator initialTurn()
    {
        yield return new WaitForSeconds(1.0f);
        quesInfo = this.turnManager.TurnQues;
        i_option = this.turnManager.TurnOption;
        Turn.text = this.turnManager.Turn.ToString();
        //if (this.turnManager.Turn == 1)
        //{
            createCard();
            cardgroup.SetActive(true);
        //}
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
        yield return new WaitForSeconds(2.0f);
        this.StartTurn();
    }


    //判斷結果
    private void CalculateWinAndLoss()
    {
        if (this.localSelection == "")
        {
            this.result = ResultType.LocalWrongAns;
            Debug.Log("You don't select");
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

        if (local != null)
        {
            // should be this format: "YOU   00"
            this.LocalPlayerText.text ="Your Name:"+local.NickName + "　Score:" + local.GetScore().ToString("D2");
        }
    }



    public override void OnJoinedRoom()
    {
        RefreshUIViews();


        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {
                // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                this.StartTurn();
            }
        }
        else
        {
            Debug.Log("Waiting for another player");
            this.WaitingUI.SetActive(true);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Other player arrived");

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {
                // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                this.WaitingUI.SetActive(false);
                this.StartTurn();
            }
        }
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        OnJoinedRoom();
        Debug.Log("Other player disconnected! " + otherPlayer.ToStringFull());
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        this.ConnectUiView.SetActive(true);
    }


}
