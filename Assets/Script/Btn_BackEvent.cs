using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Btn_BackEvent : MonoBehaviour {
    Xmlprocess xmlprocess;
    public static bool showAchieve;

    public void BackToScene(string SceneName) {
        xmlprocess = new Xmlprocess();
        //xmlprocess.ExitSceneRecord();
        UIManager.Instance.CloseAllPanel();
        SceneManager.LoadScene(SceneName);
    }

    public void BackToSceneFromCompete(string SceneName)
    {
        showAchieve = true;
        xmlprocess = new Xmlprocess();
        UIManager.Instance.CloseAllPanel();
        SceneManager.LoadScene(SceneName);
    }
}
