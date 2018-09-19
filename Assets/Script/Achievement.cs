using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour {

    public GameObject[] levelObj = new GameObject[3];
    Button btn_close;
    Xmlprocess xmlprocess;

    void Start () {
        string[] _tmp;
        xmlprocess = new Xmlprocess();
        btn_close = GetComponentsInChildren<Button>()[0];
        btn_close.onClick.AddListener(closeAchieveUI);
        for (int j = 0; j < 3; j++)
        {
            _tmp = xmlprocess.getAchieveState(j);
            for (int i = 0; i < _tmp.Length; i++)
            {
                levelObj[j].GetComponentsInChildren<Text>()[i].text = _tmp[i];
            }
        }
    }

    void closeAchieveUI() {
        xmlprocess.setTouchCount();
        gameObject.SetActive(false);

    }

}
