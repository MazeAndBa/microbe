using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using System;

public class collectView : PunBehaviour, IPunTurnManagerCallbacks
{
   public GameObject ConnectUiView, GameUiView, cardgroup, card,WaitingUI;
   public Text question;
    public Image WinorLossImg;

    int[] i_optionRand;//該回合隨機的選項編號
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
        this.turnManager.TurnDuration = 5f;
        RefreshUIViews();
    }
	
	void Update () {
        if (PhotonNetwork.connected && this.ConnectUiView.GetActive())
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
        Debug.Log("OnTurnBegins() turn: " + turn);
        this.localSelection = "";
        this.remoteSelection = "";
        IsShowingResults = false;
        createCard(turn);
        cardgroup.SetActive(true);
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
            OnTurnCompleted(-1);
        }
    }
    #endregion

    private void UpdateScores()
    {
        PhotonPlayer remote = PhotonNetwork.player.GetNext();
        PhotonPlayer local = PhotonNetwork.player;

        switch (this.result)
        {
            case ResultType.LocalRAnsSTime:
                PhotonNetwork.player.AddScore((int)(DateDiff(this.localTime,remoteTime)*3 + local.GetScore() * 0.3));
                break;
            case ResultType.LocalRAnsLTime:
                PhotonNetwork.player.AddScore((int)(DateDiff(this.localTime, remoteTime) + local.GetScore() * 0.3));
                break;
            case ResultType.Draw:
                PhotonNetwork.player.AddScore((int)(local.GetScore()* 0.5));
                break;
            case ResultType.LocalWrongAns:
                PhotonNetwork.player.AddScore(0);
                break;
        }
    }

    private int DateDiff(DateTime DateTime1, DateTime DateTime2)
    {
        int dateDiff = 0;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();
        dateDiff = ts.Seconds;
        return dateDiff;
    }

    void createCard(int turn)
    {
        randomNum();
        quesInfo = collectConn.ques[turn].Split(',');
        Debug.Log(collectConn.ques[0]);
        int ans_pos = UnityEngine.Random.Range(0, 24);
        question.text = quesInfo[1];// ques_content

        for (int i = 0; i < 25; i++)
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
                optionInfo = collectConn.option[i_optionRand[i]].Split(',');
                cardObj.GetComponentInChildren<Text>().text = optionInfo[1];//other ans_content
                cardObj.name = optionInfo[1];
                //Debug.Log("options"+ optionInfo[1]);
            }
            cardObj.GetComponent<Button>().onClick.AddListener(delegate() { MakeTurn(cardObj.name); });
            cardObj.transform.SetParent(cardgroup.transform);
            cardObj.transform.localPosition = new Vector3(-540 + (i % 6) * 160, -280 + (i / 6) * 180, 0);
            cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    void randomNum()
    {
        int _random, j;
        i_optionRand = new int[24];
        for (int i = 0; i < 24; i++)
        {
            j = 0;
            _random = UnityEngine.Random.Range(0, collectConn.option.Length - 2);
            while (i > j)
            {
                while (_random == i_optionRand[j])
                {
                    _random = UnityEngine.Random.Range(0, collectConn.option.Length - 2);
                }
                j++;
            }
            i_optionRand[i] = _random;
        }
    }

    //set plaer's selection
    public void MakeTurn(string cardName)
    {
        this.turnManager.SendMove(cardName, true);
    }

    public void StartTurn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
            createCard(this.turnManager.Turn);
        }
    }

    public void OnEndTurn()
    {
        if (this.turnManager.Turn < 6)
        {
            this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
        }
        else //競賽結束，顯示本次雙方分數
        {
            PhotonPlayer remote = PhotonNetwork.player.GetNext();
            PhotonPlayer local = PhotonNetwork.player;
            Debug.Log("Your Score:"+local.GetScore()+" remote's Score: "+remote.GetScore());
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
        yield return new WaitForSeconds(2.0f);
        this.StartTurn();
    }

    void RefreshUIViews()
    {
        ConnectUiView.SetActive(!PhotonNetwork.inRoom);//如果還沒進房間則顯示連線畫面
        GameUiView.SetActive(PhotonNetwork.inRoom);
        //cardgroup.SetActive(PhotonNetwork.room != null ? PhotonNetwork.room.PlayerCount > 1 : false);//如果能get房號且房間人數大於1，則button才可互動
    }

    //計算得分
    private void CalculateWinAndLoss()
    {
        if (this.localSelection == "")
        {
            this.result = ResultType.LocalWrongAns;
            return;
        }

        if (this.remoteSelection == "")
        {
            this.result = ResultType.LocalRAnsSTime;
        }

        if (this.localSelection == quesInfo[2])
        {
            if (DateTime.Compare(localTime, remoteTime) < 0)
            {
                this.result = ResultType.LocalRAnsSTime;
            }
            else if (DateTime.Compare(localTime, remoteTime) > 0)
            {
                this.result = ResultType.LocalRAnsLTime;
            }
            else {
                this.result = ResultType.Draw;
            }
        }
        else {
                this.result = ResultType.LocalWrongAns;
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
            WaitingUI.SetActive(true);
        }
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        this.ConnectUiView.SetActive(true);
    }


}
