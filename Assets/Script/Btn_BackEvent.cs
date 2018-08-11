using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Btn_BackEvent : MonoBehaviour {
    Xmlprocess xmlprocess;

    public void BackToScene(string SceneName) {
        xmlprocess = new Xmlprocess();
        xmlprocess.ExitTimeHistoryRecord(System.DateTime.Now.ToString("HH-mm-ss"));
        SceneManager.LoadScene(SceneName);
    }
}
