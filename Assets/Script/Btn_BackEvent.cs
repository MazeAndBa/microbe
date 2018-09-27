using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Btn_BackEvent : MonoBehaviour {
    public static bool showAchieve;

    public void BackToScene(string SceneName) {
        UIManager.Instance.CloseAllPanel();
        SceneManager.LoadScene(SceneName);
    }

    public void BackToSceneFromCompete(string SceneName)
    {
        showAchieve = true;
        UIManager.Instance.CloseAllPanel();
        SceneManager.LoadScene(SceneName);
    }
}
