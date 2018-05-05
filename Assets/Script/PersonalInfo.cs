using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalInfo : MonoBehaviour {
    Xmlprocess xmlprocess;
	public Text userName;
    
	void Start(){
        xmlprocess = new Xmlprocess ();
        userName.text = xmlprocess.getName ();

	}
}
