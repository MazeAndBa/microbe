using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompeteViewer : MonoBehaviour {

    public Button btn_search,btn_start;
    public GameObject enemyInfo;
    Text enemyName;
    CompeteManager cm;


    void Start () {
        cm = new CompeteManager();
        enemyName = enemyInfo.GetComponentsInChildren<Text>()[0];
        btn_search.onClick.AddListener(searchOther);
        btn_start.onClick.AddListener(gameStart);
	}
    void searchOther(){
        cm.searchTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        StartCoroutine(getEnemy());
        btn_search.gameObject.SetActive(false);
        btn_start.gameObject.SetActive(true);
    }

    
    
    IEnumerator getEnemy()
    {
        StartCoroutine(cm.requestMember());
        yield return new WaitForSeconds(0.1f);
        enemyName.text = cm.enemyInfo[1];
    }
    

    void gameStart() {
        cm.startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        cm.setEnemy();


    }


}
