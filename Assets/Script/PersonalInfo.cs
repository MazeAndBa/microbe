using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfo : MonoBehaviour {
    Xmlprocess xmlprocess;
    string []userInfo;
	public Text userName,level;
    public Image userImg;
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
    }

}
