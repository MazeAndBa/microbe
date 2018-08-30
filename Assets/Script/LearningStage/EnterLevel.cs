using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterLevel : MonoBehaviour {

    Button btn_practice, btn_compete;
    Xmlprocess xmlprocess;
    int currentLevel;
    string levelName = "";

    void Start () {
        currentLevel = Home.getLevel();
        xmlprocess = new Xmlprocess();
        switch (currentLevel)
        {
            case 0:
                levelName = "Easy";
                break;
            case 1:
                levelName = "Medium";
                break;
            case 2:
                levelName = "Hard";
                break;
        }

        btn_practice = GetComponentsInChildren<Button>()[1];
        btn_compete = GetComponentsInChildren<Button>()[2];
        btn_practice.onClick.AddListener(goPractice);
        btn_compete.onClick.AddListener(goCompete);

    }

    void goPractice() {

        //xmlprocess.New_timeHistoryRecord(levelName + "_Practice", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord(levelName + "_Practice", DateTime.Now.ToString("HH-mm-ss"));
        SceneManager.LoadScene("PracticeArea");
    }

    void goCompete()
    {
        //xmlprocess.New_timeHistoryRecord(levelName + "_Compete", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord(levelName + "_Compete", DateTime.Now.ToString("HH-mm-ss"));
        SceneManager.LoadScene("CompeteArea");
    }

}
