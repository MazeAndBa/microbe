using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeManager {
    private string serverlink = "140.115.126.137/microbe/";

    public Dictionary<object, string> E_vocabularyDic = new Dictionary<object, string>();//key=單字ID,val=英文單字
    public Dictionary<object, string> T_vocabularyDic = new Dictionary<object, string>();//key=單字ID,val=英文中譯
    public int volID;
    string volEng, volTran;

    public IEnumerator LoadVocabulary(string fileName,int currentLevel)
    {

        WWWForm phpform = new WWWForm();
        phpform.AddField("chooseLevel", currentLevel);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        string[] tmp,tmp2;
        if (reg.error == null)
        {
            tmp = reg.text.Split(';');//最後一個是空的
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                tmp2 = tmp[i].Split(',');

                E_vocabularyDic.Add(i, tmp2[0]);
                T_vocabularyDic.Add(i, tmp2[1]);
                //vocabularyList.Add(new string(){volID =tmp[0],volEng = tmp[1],volTran = tmp[2] });
                 Debug.Log(E_vocabularyDic[0]);
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    

}
