using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeView : MonoBehaviour {

    PracticeManager pm;
    #region ReviewVocabulary UI
    Text text_English,text_Translation;
    Button btn_pronun,btn_pre, btn_next;
    #endregion

    void Start () {
        ShowReviewVocabulary();
    }
    void ShowReviewVocabulary(){
        int vocabularyID = 0;
        StartCoroutine(pm.LoadVocabulary("loadVocabulary.php"));

        UIManager.Instance.ShowPanel("P_ReviewUI");
        text_English = GetComponentsInChildren<Text>()[0];
        text_Translation = GetComponentsInChildren<Text>()[1];
        btn_pronun = GetComponentsInChildren<Button>()[0];
        btn_pre = GetComponentsInChildren<Button>()[1];
        btn_next = GetComponentsInChildren<Button>()[2];

        text_English.text = pm.E_vocabularyDic[vocabularyID];
        text_Translation.text = pm.T_vocabularyDic[vocabularyID];

        //btn_pronun.onClick.AddListener(confirmregister);
        btn_pre.onClick.AddListener(delegate() { changeVocabularyID(vocabularyID - 1); });
        btn_next.onClick.AddListener(delegate () { changeVocabularyID(vocabularyID + 1); });
    }

    void changeVocabularyID(int vocabularyID) {

        text_English.text = pm.E_vocabularyDic[vocabularyID];
        text_Translation.text = pm.T_vocabularyDic[vocabularyID];
    }
}
