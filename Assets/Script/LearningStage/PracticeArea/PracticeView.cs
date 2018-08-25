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

    void Start () {
        pm = new PracticeManager();
        vocabularyID = 0;
        StartCoroutine(showReviewVocabulary());
        showReviewUI();
    }

    void showReviewUI() {

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
                Debug.Log(vocabularyID);
            }
            else
            {
                btn_gotonext.gameObject.SetActive(true);
                btn_gotonext.onClick.AddListener(showPracticeUI);
            }
        }
    }

    void showPracticeUI()
    {
        UIManager.Instance.ClosePanel("P_ReviewUI");
        UIManager.Instance.ShowPanel("P_PracticeUI");
    }
}
