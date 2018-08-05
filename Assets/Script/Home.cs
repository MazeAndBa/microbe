using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour {
    public Button btn_easy, btn_medium, btn_hard;
    Xmlprocess xmlprocess;
    static string chooseLevel;

	void Start () {
        xmlprocess = new Xmlprocess();
        btn_easy.onClick.AddListener(delegate { goScene("Easy_C"); });
        btn_medium.onClick.AddListener(delegate { goScene("Medium"); });
        //btn_hard.onClick.AddListener(delegate { goScene("Hard_C"); });

    }
    void goScene(string sceneName) {
        Debug.Log(sceneName);
        string startTime = (System.DateTime.Now).ToString("HH:mm:ss");
        xmlprocess.New_timeHistoryRecord(sceneName, startTime);
        chooseLevel = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    public static string getLevel() {
        return chooseLevel;
    }
}
