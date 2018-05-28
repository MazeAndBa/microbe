using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompeteMain : PunBehaviour, IPunTurnManagerCallbacks
{
    public GameObject ConnectUI,Cards;

    private PunTurnManager turnManager;
    private bool IsShowingResults;
    private ResultType result;


    [SerializeField]
    public string localSelection;

    [SerializeField]
    public string remoteSelection;

    public enum CardResult
    {
        None = 0,//預設值為0
        CorrectAns,
        SameTime
    }

    public enum ResultType
    {
        None = 0,
        Draw,
        LocalWin,
        LocalLoss
    }

    void Start () {
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = 5f;
    }
	

	void Update () {

        // 連線的UI
        if (PhotonNetwork.connected && this.ConnectUI.GetActive())
        {
            this.ConnectUI.SetActive(false);
        }
        if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !this.ConnectUI.GetActive())
        {
            this.ConnectUI.SetActive(true);
        }
    }

    #region Implement IPunTurnManagerCallbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        IsShowingResults = false;
        Cards.SetActive(true);
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


    // when a player made the last/final move in a turn
    public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move.ToString());

        if (photonPlayer.IsLocal)
        {
            this.localSelection = move.ToString();
        }
        else
        {
            this.remoteSelection = move.ToString();
        }
    }



    public void OnTurnTimeEnds(int obj)
    {
        if (!IsShowingResults)
        {
            Debug.Log("OnTurnTimeEnds: Calling OnTurnCompleted");
            OnTurnCompleted(-1);
        }
    }

    private void UpdateScores()
    {
        if (this.result == ResultType.LocalWin)
        {
            PhotonNetwork.player.AddScore(1);   // this is an extension method for PhotonPlayer. you can see it's implementation
        }
    }

    #endregion

    private void CalculateWinAndLoss()
    {
        this.result = ResultType.None;
        if (this.localSelection == this.remoteSelection)
        {
            return;
        }

        if (this.localSelection == null)
        {
            this.result = ResultType.LocalLoss;
            return;
        }

        if (this.remoteSelection == null)
        {
            this.result = ResultType.LocalWin;
        }

       /* if (this.localSelection == Hand.Rock)
        {
            this.result = (this.remoteSelection == Hand.Scissors) ? ResultType.LocalWin : ResultType.LocalLoss;
        }
       */
    }
    public void StartTurn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
        }
    }

    public void OnEndTurn()
    {
        this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
    }

    public IEnumerator ShowResultsBeginNextTurnCoroutine()
    {
        Cards.SetActive(false);
        //Cards.interactable = false;
        IsShowingResults = true;
        // yield return new WaitForSeconds(1.5f);

        if (this.result == ResultType.Draw)
        {
         //平手的狀況   
        }
        else
        {
          //勝利或失敗的狀況  
        }
        
        yield return new WaitForSeconds(2.0f);
        this.StartTurn();
    }







}
