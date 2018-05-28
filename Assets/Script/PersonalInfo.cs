using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfo : MonoBehaviour {
    Xmlprocess xmlprocess;
    string []userInfo;
	public Text userName;
    
	void Start(){
        xmlprocess = new Xmlprocess ();
        userInfo = xmlprocess.getUserInfo();
        userName.text = userInfo[1];

	}

}
