using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompeteManager {
    Xmlprocess xmlprocess;
    private string serverlink = "140.115.126.137/microbe/";
    public string searchTime, startTime;
    public string[] enemyInfo, playerInfo;


    public CompeteManager() {
        xmlprocess = new Xmlprocess();
        playerInfo = xmlprocess.getUserInfo();
    }

    /*適用於非同步競賽
    * 
    public IEnumerator requestMember()
    {

        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", playerInfo[0]);
        phpform.AddField("level", playerInfo[2]);

        WWW reg = new WWW(serverlink + "searchEnemy", phpform);
        yield return reg;
        if (reg.error == null)
        {
            enemyInfo = reg.text.Split(',');
            xmlprocess.New_timeHistoryRecord("SearchEnemy", searchTime);
        }
        else
        {
            Debug.Log( "error msg" + reg.error );
        }
    }
    */
    public IEnumerator loadingQues()
    {

        WWWForm phpform = new WWWForm();
        phpform.AddField("level", playerInfo[2]);

        WWW reg = new WWW(serverlink + "searchEnemy", phpform);
        yield return reg;
        if (reg.error == null)
        {
            enemyInfo = reg.text.Split(',');
            xmlprocess.New_timeHistoryRecord("SearchEnemy", searchTime);
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }


    public void setEnemy()
    {
        xmlprocess.setEnemy(enemyInfo[0], searchTime, startTime);
    }

    public void createRoom() {
           
    }

}
