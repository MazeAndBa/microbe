﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour {
    public Button btn_easy, btn_medium, btn_hard;
    Xmlprocess xmlprocess;
    static int chooseLevel;

	void Start () {
        xmlprocess = new Xmlprocess();
        btn_easy.onClick.AddListener(delegate { goScene("Easy",0); });
        btn_medium.onClick.AddListener(delegate { goScene("Medium",1); });
        btn_hard.onClick.AddListener(delegate { goScene("Hard", 2); });

    }
    void goScene(string sceneName, int Level) {
        chooseLevel = Level;
        //Debug.Log("chooseLevel:"+Level);
        string startTime = (System.DateTime.Now).ToString("HH:mm:ss");
        xmlprocess.ScceneHistoryRecord(sceneName, startTime);
        SceneManager.LoadScene("LearningStage");
    }

    public static int getLevel() {
        return chooseLevel;
    }
}
