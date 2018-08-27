using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeView : MonoBehaviour {

    PracticeManager pm;
    int currentLevel,vocabularyID;
    #region ReviewVocabulary UI
    Text text_English,text_Translation;
    Button btn_pronun,btn_pre, btn_next, btn_gotonext;
    #endregion
    #region PracticeType1 UI
    Text text_Question;
    Button[] btn_option;
    int quesID,correctOption;//quesID:題數;correctOption:正確的選項編號
    int[] randomQuestion, randomOption;//隨機排列後的題目與選項
    #endregion


    void Start () {
        pm = new PracticeManager();
        btn_option = new Button[4];
        vocabularyID = 0;
        StartCoroutine(showReviewVocabulary());
        showReviewUI();
    }
    #region Review function

    void showReviewUI()
    {

        UIManager.Instance.ShowPanel("P_ReviewUI");
        
        text_English = GetComponentsInChildren<Text>()[5];
        text_Translation = GetComponentsInChildren<Text>()[6];
        btn_pronun = GetComponentsInChildren<Button>()[2];
        btn_pre = GetComponentsInChildren<Button>()[3];
        btn_next = GetComponentsInChildren<Button>()[4];
        btn_gotonext = GetComponentsInChildren<Button>()[5];
        btn_gotonext.gameObject.SetActive(false);

        btn_pre.onClick.AddListener(delegate () { changeVocabularyID(-1); });
        btn_next.onClick.AddListener(delegate () { changeVocabularyID(1); });
    }

    IEnumerator showReviewVocabulary(){
        currentLevel = Home.getLevel();
        StartCoroutine(pm.LoadVocabulary("loadVocabulary.php", currentLevel));
        yield return new WaitForSeconds(0.1f);
        changeVocabularyID(0);
    }

    void changeVocabularyID(int count) {

        if (vocabularyID >= 0)
        {
            if (pm.E_vocabularyDic.ContainsKey(vocabularyID + count))
            {
                btn_gotonext.gameObject.SetActive(false);

                vocabularyID += count;
                text_English.text = pm.E_vocabularyDic[vocabularyID];
                text_Translation.text = pm.T_vocabularyDic[vocabularyID];
            }
            else
            {
                btn_gotonext.gameObject.SetActive(true);
                btn_gotonext.onClick.AddListener(showPracticeUI);

            }
        }
    }
    #endregion

    #region PracticeType1 function

    void showPracticeUI()
    {
        UIManager.Instance.TogglePanel("P_ReviewUI",false);
        UIManager.Instance.ShowPanel("P_PracticeUI");
        text_Question = GetComponentsInChildren<Text>()[5];
        for(int i = 0; i < btn_option.Length; i++)
        {
            btn_option[i] = GetComponentsInChildren<Button>()[i+2];
            //btn_option[i].onClick.AddListener(delegate () { compareAns(i); });
        }
        btn_option[0].onClick.AddListener(delegate () { compareAns(0); });
        btn_option[1].onClick.AddListener(delegate () { compareAns(1); });
        btn_option[2].onClick.AddListener(delegate () { compareAns(2); });
        btn_option[3].onClick.AddListener(delegate () { compareAns(3); });


        initialQuestion();
    }

    void initialQuestion() {
        quesID = 0;
        randomQuestion = pm.randomQuestion();
        showPracticeQues(quesID);
    }

    void showPracticeQues(int quesID) {//更新每回合的題目與選項
        //Debug.Log("題號"+ quesID);

        text_Question.text = pm.E_vocabularyDic[randomQuestion[quesID]];
        showPracticeOption(randomQuestion[quesID]);
    }

    void showPracticeOption(int correctOptID)
    {
        randomOption = pm.randomOption(4, correctOptID);
        correctOption = UnityEngine.Random.Range(0, btn_option.Length-1);//隨機選擇正確答案的位置
        //Debug.Log("正確選項ID"+correctOption);
        for (int i = 0, randomOptionIndex = 1; i < btn_option.Length; i++)
        {
            if (i == correctOption)
            {
                btn_option[i].GetComponentInChildren<Text>().text = pm.T_vocabularyDic[correctOptID];
            }
            else
            {
                btn_option[i].GetComponentInChildren<Text>().text = pm.T_vocabularyDic[randomOption[randomOptionIndex]];
                randomOptionIndex++;
            }
        }
    }

    void compareAns(int optionID) {
        //Debug.Log(optionID);

        if (quesID < pm.E_vocabularyDic.Count-1)
        {
            if (correctOption.Equals(optionID))
            {
                Debug.Log("Correct");
            }
            else {
                Debug.Log("Wrong");
            }
            quesID++;
            showPracticeQues(quesID);
        }
        else {
            UIManager.Instance.ClosePanel("P_PracticeUI");
        }
    }
    #endregion
}
