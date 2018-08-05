using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AccountManager {

    private string serverlink = "140.115.126.137/microbe/";
    string s_checksum;
    public int state;
    public string[] AccountInfo;
    Xmlprocess xmlprocess;

    public IEnumerator CheckLogin(string fileName, string[] str)
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", str[0]);
        phpform.AddField("user_pwd", str[1]);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        if (reg.error == null)
        {
            if (reg.text == "0")
            {
                state = 0;//帳密錯誤
            }
            else
            {
                AccountInfo = reg.text.Split(',');
                state = 1;
                xmlprocess = new Xmlprocess(AccountInfo[0]);
                //xmlprocess.setUserInfo(AccountInfo);
                xmlprocess.New_timeHistoryRecord("Login", DateTime.Now.ToString("yyyy-MM-dd"));
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    public IEnumerator CheckRegister(string fileName, string[] str)
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", str[0]);
        phpform.AddField("user_pwd", str[1]);
        phpform.AddField("user_name", str[2]);
        phpform.AddField("user_sex", str[3]);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        if (reg.error == null)
        {
            if (reg.text == "0")
            {
                AccountInfo = new string[]{str[0],str[2],"1", str[3] };
                state = 0;//帳號不重複
                Debug.Log(state+" "+ AccountInfo);
                xmlprocess = new Xmlprocess(AccountInfo[0]);
                xmlprocess.setUserInfo(AccountInfo);//將註冊資訊傳至XmlNode
                xmlprocess.timeHistoryRecord("Register");

            }
            else
            {
                state = 1;
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

}
