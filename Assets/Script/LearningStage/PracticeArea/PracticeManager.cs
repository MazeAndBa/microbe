using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeManager {
    private string serverlink = "140.115.126.137/microbe/";

    public Dictionary<int, string> E_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文單字
    public Dictionary<int, string> T_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文中譯
    public string []volEng;

    public IEnumerator LoadVocabulary(string fileName,int currentLevel)
    {

        WWWForm phpform = new WWWForm();
        phpform.AddField("chooseLevel", currentLevel);
        WWW reg = new WWW(serverlink + fileName, phpform);
        yield return reg;
        string[] tmp,tmp2;
        if (reg.error == null)
        {
            volEng = reg.text.Split(';');//最後一個是空的

            tmp = reg.text.Split(';');//最後一個是空的
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                tmp2 = tmp[i].Split(',');

                E_vocabularyDic.Add(i, tmp2[0]);
                T_vocabularyDic.Add(i, tmp2[1]);
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    

}
