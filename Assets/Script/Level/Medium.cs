using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Medium : MonoBehaviour {
    Button btn_practice, btn_compete, btn_back;
    Xmlprocess xmlprocess;

    void Start () {
        btn_practice = GetComponentsInChildren<Button>()[1];
        btn_compete = GetComponentsInChildren<Button>()[2];
        btn_back = GetComponentsInChildren<Button>()[3];

        btn_practice.onClick.AddListener(delegate { goPractice(Home.getLevel()); });
        btn_compete.onClick.AddListener(delegate { goPractice(Home.getLevel()); });
        btn_back.onClick.AddListener(BackMainmenu);

    }

    void goPractice(string level) {
        SceneManager.LoadScene(level + "_P");
    }

    void BackMainmenu()
    {
        SceneManager.LoadScene("home");
    }

}
