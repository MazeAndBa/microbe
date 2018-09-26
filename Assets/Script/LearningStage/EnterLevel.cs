using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterLevel : MonoBehaviour {

    Button btn_practice, btn_compete;
    Xmlprocess xmlprocess;

    void Start () {
        xmlprocess = new Xmlprocess();
        btn_practice = GetComponentsInChildren<Button>()[0];
        btn_compete = GetComponentsInChildren<Button>()[1];

        btn_practice.onClick.AddListener(goPractice);
        if (!xmlprocess.getLearningState())
        {
            btn_compete.interactable = false;
            btn_compete.image.color = Color.gray;
        }
        else {
            btn_compete.interactable = true;
            btn_compete.onClick.AddListener(goCompete);
        }
    }

    void goPractice() {

        //xmlprocess.New_timeHistoryRecord(levelName + "_Practice", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord( "Learning", DateTime.Now.ToString("HH:mm:ss"));
        SceneManager.LoadScene("LearningArea");
    }

    void goCompete()
    {
        //xmlprocess.New_timeHistoryRecord(levelName + "_Compete", System.DateTime.Now.ToString("HH-mm-ss"));
        xmlprocess.ScceneHistoryRecord( "Compete", DateTime.Now.ToString("HH:mm:ss"));
        SceneManager.LoadScene("CompeteArea");
    }

}
