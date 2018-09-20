﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfo : MonoBehaviour {
    Xmlprocess xmlprocess;
    string []userInfo;
	public Text userName,level;
    public Image userImg;
    public Button btn_achievement;
    public GameObject AchieveUI;
	void Start(){
        xmlprocess = new Xmlprocess ();
        userInfo = xmlprocess.getUserInfo();
        userName.text = userInfo[1];
        level.text = userInfo[2];
        switch (userInfo[3]) {
            case "0":
                userImg.sprite = Resources.Load("Image/boy", typeof(Sprite)) as Sprite;
                break;
            case "1":
                userImg.sprite = Resources.Load("Image/girl", typeof(Sprite)) as Sprite;
                break;
        }
        btn_achievement.onClick.AddListener(showAchievementUI);

        //如果完成練習，顯示成就UI
        if (PracticeView.showAchieve)
        {
            showAchievementUI();
            PracticeView.showAchieve = false;
        }
        //如果離開對戰畫面，顯示成就UI
        if (Btn_BackEvent.showAchieve)
        {
            showAchievementUI();
            Btn_BackEvent.showAchieve = false;
        }
    }

    void showAchievementUI()
    {
        AchieveUI.SetActive(true);
    }
}
