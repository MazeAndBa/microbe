using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PracticeView : MonoBehaviour {

    PracticeManager pm;
    int currentLevel,vocabularyID;
    static int p_score;
    Text text_score;
    public GameObject UI_showAnsfeedback;

    #region ReviewVocabulary UI
    Text text_English,text_Translation;
    Button btn_pronun,btn_pre, btn_next, btn_gotonext;
    #endregion
    #region PracticeMuitiselect UI
    Text text_totalQues,text_Question;
    Button[] btn_option;
    int quesID,correctOption;//quesID:題數;correctOption:正確的選項編號
    int[] randomQuestion, randomOption;//隨機排列後的題目與選項
    #endregion

    #region PracticeCompose UI
    Button btn_alphabet,btn_clear,btn_submit;
    Text text_quescontent;
    GameObject[] CollectBtnObj;
    string userAns;
    #endregion

    void Start () {
        pm = new PracticeManager();
        text_score = GetComponentsInChildren<Text>()[5];
        p_score = 0;
        vocabularyID = 0;
        StartCoroutine(showReviewVocabulary());
        UIManager.Instance.CloseAllPanel();

        showReviewUI();
    }
    #region Review function
    
    void showReviewUI()
    {
        UIManager.Instance.ShowPanel("P_ReviewUI");
        text_English = GetComponentsInChildren<Text>()[6];
        text_Translation = GetComponentsInChildren<Text>()[7];
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

    #region PracticeMuitiselect function

    void showPracticeUI()
    {
        btn_option = new Button[4];
        UIManager.Instance.TogglePanel("P_ReviewUI",false);
        UIManager.Instance.ShowPanel("P_PracticeUI");
        text_totalQues =  GetComponentsInChildren<Text>()[6];
        text_Question = GetComponentsInChildren<Text>()[7];
        for(int i = 0; i < btn_option.Length; i++)
        {
            btn_option[i] = GetComponentsInChildren<Button>()[i+2];
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
        text_totalQues.text = (quesID+1).ToString()+"/"+ pm.E_vocabularyDic.Count;
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
        if (correctOption.Equals(optionID))
        {
            StartCoroutine(showfeedback(0));
            p_score += (int)(p_score * 0.5) + 30;
            text_score.text = p_score.ToString();
        }
        else {
            StartCoroutine(showfeedback(1));
        }

        if (quesID >= pm.E_vocabularyDic.Count - 1)
        {
            StartCoroutine(PracticeEnd());
        }
        else
        {
            quesID++;
            showPracticeQues(quesID);
        }
    }
    IEnumerator PracticeEnd()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.TogglePanel("P_PracticeUI", false);
        UIManager.Instance.ShowPanel("P_ComposeUI");
        showComposeUI();
    }

    #endregion

    #region PracticeCompose function
    void showComposeUI() {

        btn_alphabet = Resources.Load("UI/Btn_Alphabet", typeof(Button)) as Button;
        text_totalQues = GetComponentsInChildren<Text>()[6];
        text_Question = GetComponentsInChildren<Text>()[7];
        text_quescontent = GetComponentsInChildren<Text>()[8];
        btn_submit = GetComponentsInChildren<Button>()[3];
        btn_submit.onClick.AddListener(compareComposeAns);

        quesID = 0;
        randomQuestion = pm.randomQuestion();
        showComposeQues(quesID);
    }

    //初始化題目
    void initialComposeQuestion(int quesID)
    {
        text_totalQues.text = "1/" + pm.E_vocabularyDic.Count;
        text_quescontent.text = "";//初始化挖空題目
        for (int i = 0; i < CollectBtnObj.Length; ++i)//刪除所有字母按鈕
        {
            if (CollectBtnObj[i] != null)
            {
                Destroy(CollectBtnObj[i].gameObject);
            }
        }
        showComposeQues(quesID);
    }

    //更新每回合的題目
    void showComposeQues(int quesID)
    {
        text_totalQues.text = (quesID + 1).ToString() + "/" + pm.T_vocabularyDic.Count;
        text_Question.text = pm.T_vocabularyDic[randomQuestion[quesID]];
        for (int i = 0; i < pm.E_vocabularyDic[randomQuestion[quesID]].Length; i++) {
            text_quescontent.text += "_ ";
        }
        randomSort(randomQuestion[quesID]);
    }

    //重新排列字母
    void randomSort(int index) {
        int random;
        char tmp;
        char []randomAns= pm.E_vocabularyDic[randomQuestion[quesID]].ToCharArray();
        for (int i = 0; i < randomAns.Length; i++) {
            random = Random.Range(i, randomAns.Length-1);
            tmp = randomAns[random];
            randomAns[random] = randomAns[i];
            randomAns[i] = tmp;
        }
        //生成按鈕
        creatAlphabetBtn(randomAns);
    }

    void creatAlphabetBtn(char[] randomAns) {
        int pointer = 0;//當前字母指標
        CollectBtnObj = new GameObject[randomAns.Length];

        while (pointer<randomAns.Length) {
        Button g_btnObj = Instantiate(btn_alphabet);//Options
        g_btnObj.transform.SetParent(GameObject.Find("Content").transform);
        g_btnObj.GetComponentInChildren<Text>().text = randomAns[pointer].ToString();
        g_btnObj.transform.localPosition = new Vector3(0 + pointer * 150, 0.0f, 0.0f);
        g_btnObj.transform.localScale = Vector3.one;
        g_btnObj.name = randomAns[pointer].ToString();
        g_btnObj.onClick.AddListener(() => clickAlphabet(g_btnObj));
        CollectBtnObj[pointer] = g_btnObj.gameObject;
        pointer++;
        }
    }

    void clickAlphabet(Button _trigger)
    {
        userAns += _trigger.name;//將點擊的選項存入usrAns
        setQuesContent(_trigger.name);
        Destroy(_trigger.gameObject);//按鈕點擊後消失
    }

    void setQuesContent(string alphabet)
    {
        int underline_index = text_quescontent.text.IndexOf('_');
        //Debug.Log(underline_index);
        if (underline_index != -1)
        {
            text_quescontent.text = text_quescontent.text.Remove(underline_index, 1);
        }
        text_quescontent.text = text_quescontent.text.Insert(underline_index, alphabet);
    }
    void compareComposeAns() {

        if (userAns == pm.E_vocabularyDic[randomQuestion[quesID]])
        {
            StartCoroutine(showfeedback(0));
            p_score += (int)(p_score *(currentLevel+0.5)) + 30;
            text_score.text = p_score.ToString();

        }
        else {
            Debug.Log("你的答案:" + userAns);
            Debug.Log("正確答案:" + pm.E_vocabularyDic[randomQuestion[quesID]]);
            StartCoroutine(showfeedback(1));
        }

        if (quesID >= pm.E_vocabularyDic.Count - 1)
        {
            StartCoroutine(ComposeEnd());
        }
        else
        {
            quesID++;
            initialComposeQuestion(quesID);
        }
    }

    IEnumerator ComposeEnd() {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.CloseAllPanel();
        SceneManager.LoadScene("LearningStage");
    }
    #endregion

    IEnumerator showfeedback(int _state)
    {
        GameObject fb  = Instantiate(UI_showAnsfeedback);//Options
        fb.transform.SetParent(this.gameObject.transform);
        fb.transform.localPosition = Vector3.zero;
        fb.transform.localScale = Vector3.one;
        Image img_correct = fb.GetComponentsInChildren<Image>()[0];
        Image img_wrong = fb.GetComponentsInChildren<Image>()[1];
        img_correct.gameObject.SetActive(false);
        img_wrong.gameObject.SetActive(false);

        if (_state == 0)
        {
            img_correct.gameObject.SetActive(true);
            Debug.Log("Correct");
        }
        else
        {
            img_wrong.gameObject.SetActive(true);
            Debug.Log("Wrong");

        }
        yield return new WaitForSeconds(0.1f);
        Destroy(fb);
    }

}
