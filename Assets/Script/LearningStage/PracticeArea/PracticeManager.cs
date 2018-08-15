using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeManager : MonoBehaviour {
    private string serverlink = "140.115.126.137/microbe/";
    int currentLevel;
    public Dictionary<int, string> E_vocabularyDic, T_vocabularyDic;//E_vocabularyDic:存放英文單字；T_vocabularyDic:存放單字中譯

    public int volID;
    string volEng, volTran;

    void Start () {
        currentLevel = Home.getLevel();
        E_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文單字
        T_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文中譯
    }

    public IEnumerator LoadVocabulary(string fileName)
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
                E_vocabularyDic.Add(i, tmp2[1]);
                T_vocabularyDic.Add(i, tmp2[2]);
                //vocabularyList.Add(new string(){volID =tmp[0],volEng = tmp[1],volTran = tmp[2] });
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    

}
