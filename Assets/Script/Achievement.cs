using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour {

    public GameObject levelObj;
    Button btn_close;
    Xmlprocess xmlprocess;

    void Start () {
        string[] _tmp;
        xmlprocess = new Xmlprocess();
        btn_close = GetComponentsInChildren<Button>()[0];
        btn_close.onClick.AddListener(closeAchieveUI);
        _tmp = xmlprocess.getAchieveState();
        for (int i = 0; i < _tmp.Length; i++)
        {
            levelObj.GetComponentsInChildren<Text>()[i].text = _tmp[i];
        }

    }

    void closeAchieveUI() {
        xmlprocess.setTouchACount();
        gameObject.SetActive(false);

    }

}
