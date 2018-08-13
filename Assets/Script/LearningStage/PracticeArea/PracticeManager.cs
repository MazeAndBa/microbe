using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeManager : MonoBehaviour {
    private string serverlink = "140.115.126.137/microbe/";
    int currentLevel;
    List<string> vocabularyList;

    public int volID;
    string volEng, volTran;

    void Start () {
        currentLevel = Home.getLevel();
        vocabularyList = new List<string>();
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
                //vocabularyList.Add(new string(){volID =tmp[0],volEng = tmp[1],volTran = tmp[2] });
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

}
