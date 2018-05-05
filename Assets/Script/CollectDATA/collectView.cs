using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class collectView : MonoBehaviour {

    public Text question;
    public GameObject obj_gamestart, cardgroup, card;
    Button btn_start;
    InputField id, username;
    string[] ques, quesInfo, option, optionInfo;
    int [] i_optionRand;//該回合隨機的選項編號
    static int round = 0;//第幾回合
    private string serverlink = "140.115.126.137/microbe/";

    void Start () {
        btn_start = obj_gamestart.GetComponentInChildren<Button>();
        btn_start.onClick.AddListener(gamestart);
        id = obj_gamestart.GetComponentsInChildren<InputField>()[0];
        username = obj_gamestart.GetComponentsInChildren<InputField>()[1];
    }


    void gamestart() {
        createUser();
        obj_gamestart.gameObject.SetActive(false);
        StartCoroutine(getQuestion());
        StartCoroutine(getOption());

    }

    void createUser() {
        WWWForm phpform = new WWWForm();
        phpform.AddField("user_id", id.GetComponentsInChildren<Text>()[1].text);
        phpform.AddField("user_name", username.GetComponentsInChildren<Text>()[1].text);
        new WWW(serverlink + "collectData", phpform);
        Debug.Log("create");
    }

    IEnumerator getQuestion() {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", "getQuestion");
        WWW reg = new WWW(serverlink + "getQuestion", phpform);
        yield return reg;
        if (reg.error == null)
        {
            //Debug.Log(reg.text);
            ques = reg.text.Split(';');//最後一個是空的
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
    }

    IEnumerator getOption()
    {
        WWWForm phpform = new WWWForm();
        phpform.AddField("action", "getOption");
        WWW reg = new WWW(serverlink + "getOption", phpform);
        yield return reg;
        if (reg.error == null)
        {
            option = reg.text.Split(';');//最後一個是空的
        }
        else
        {
            Debug.Log("error msg" + reg.error);
        }
        randomNum();
        createCard();
    }


    void randomNum() {
        int _random,j;
        i_optionRand = new int[24];
        for (int i = 0; i < 24; i++)
        {
            j = 0;
            _random = Random.Range(0, option.Length - 2);
            while (i > j)
            {
                while (_random == i_optionRand[j])
                {
                    _random = Random.Range(0, option.Length - 2);
                }
                j++;
            }
            i_optionRand[i] = _random;
        }
    }


    void createCard()
    {
        randomNum();
        quesInfo = ques[round].Split(',');
        int ans_pos = Random.Range(0,24);
        question.text = quesInfo[1];

        for (int i = 0; i < 25; i++)
        {
            GameObject cardObj = Instantiate(card);
            cardObj.gameObject.SetActive(true);
            if (i == ans_pos)
            {
                cardObj.GetComponentInChildren<Text>().text = quesInfo[2];
            }
            else
            {
                optionInfo = option[i_optionRand[i]].Split(',');
                cardObj.GetComponentInChildren<Text>().text = optionInfo[1];
                //Debug.Log("options"+ optionInfo[1]);
            }
            cardObj.transform.SetParent(cardgroup.transform);
            cardObj.transform.localPosition = new Vector3(-540 + (i % 6) * 160, -280 + (i / 6) * 180, 0);
            cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        round++;
    }
}
