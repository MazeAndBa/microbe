using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour {

    public GameObject LearningState,CompeteState,LearningBadge, CompeteBadge,badgeObj;
    public Button btn_close;
    string[] s_LearningState, s_CompeteState;

    Xmlprocess xmlprocess;

    private void Awake()
    {
        xmlprocess = new Xmlprocess();
        s_LearningState = xmlprocess.getAchieveLearningState();
        s_CompeteState = xmlprocess.getAchieveCompeteState();
    }
    void Start () {
        btn_close.onClick.AddListener(closeAchieveUI);
        for (int i = 0; i < s_LearningState.Length; i++)
        {
            LearningState.GetComponentsInChildren<Text>()[i].text = s_LearningState[i];
            LearningState.GetComponentsInChildren<Text>()[i].text = s_CompeteState[i];
        }
    }

    void closeAchieveUI() {
        xmlprocess.setTouchACount();
        gameObject.SetActive(false);
    }


}
