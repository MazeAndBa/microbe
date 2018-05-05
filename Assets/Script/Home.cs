using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour {
    public Button compete;
    Xmlprocess xmlprocess;
	void Start () {
        xmlprocess = new Xmlprocess();
        compete.onClick.AddListener(delegate { goScene("compete"); });
	}
    void goScene(string sceneName) {
        Debug.Log(sceneName);
        string startTime = (System.DateTime.Now).ToString("HH:mm:ss");
        xmlprocess.New_timeHistoryRecord(sceneName, startTime);
        SceneManager.LoadScene(sceneName);
    }

}
