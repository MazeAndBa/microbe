using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeManager {
    private string serverlink = "140.115.126.137/microbe/";
    Xmlprocess xmlprocess;
    int level;
    public Dictionary<int, string> E_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文單字
    public Dictionary<int, string> T_vocabularyDic = new Dictionary<int, string>();//key=單字ID,val=英文中譯

    public PracticeManager(int level) {
        xmlprocess = new Xmlprocess();
        this.level = level;
    }

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
            }
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    ///<summary>
    ///將題目亂數重新排序
    ///</summary>
    public int[] randomQuestion() {

        int randomindex = 0, dicLength = E_vocabularyDic.Count;
        int[] i_indexRand = new int[dicLength];
        //亂數排列key(0~dicLength)
        for (int i = 0; i < dicLength; i++)
        {
            i_indexRand[i] = i;
        }
        int tmp =0;
        for (int i = 0; i < i_indexRand.Length; i++)
        {
            randomindex = UnityEngine.Random.Range(i, i_indexRand.Length- 1);
            tmp = i_indexRand[randomindex];
            i_indexRand[randomindex] = i_indexRand[i];
            i_indexRand[i] = tmp;
        }
        return i_indexRand;
    }

    ///<summary>
    ///根據選項數量進行n次亂數排列，randomOption[0]為正解(correctID)
    ///</summary>

    public int[] randomOption(int optionCount,int correctID)
    {
        int randomindex = 0, dicLength = T_vocabularyDic.Count;
        int[] i_indexRand = new int[dicLength];
        for (int i = 0; i < dicLength; i++)
        {
            //將正確答案ID移到陣列第一個
            if (i == correctID)
            {
                i_indexRand[0] = correctID;
                i_indexRand[i] = 0;
            }
            else
            {
                i_indexRand[i] = i;
            }
        }
        //將正確答案ID剔除後,進行optionCount-1次亂數排序
        int tmp = 0;
        for (int i = 1; i < optionCount; i++)
        {
            randomindex = UnityEngine.Random.Range(i, i_indexRand.Length - 1);
            tmp = i_indexRand[randomindex];
            i_indexRand[randomindex] = i_indexRand[i];
            i_indexRand[i] = tmp;
        }
        return i_indexRand;
    }
    /// <summary>
    /// 新增回合單字練習紀錄
    /// </summary>
    public void startLeaning() {
        xmlprocess.createLearningRecord(level);
    }

    /// <summary>
    /// 更新單字總練習次數
    /// </summary>
    /// <param name="eventname">要更新的attribute</param>
    public void setLearningTimes(string eventname) {
        xmlprocess.setLearningCount(eventname, level);
    }

    /// <summary>
    /// 回合單字練習的成績紀錄
    /// </summary>
    public void setLearningScore(int score)
    {
        xmlprocess.setLearningScoreRecord(level,score);
    }
}
