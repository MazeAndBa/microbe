using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
using System.Text;
using edu.ncu.list.util;
using UnityEngine.UI;

public class UpdateSQL : MonoBehaviour {
    public Button test;

    protected Xmlprocess xmlprocess;
    MySQLAccess mySQLAccess;
    public XmlDocument xmlDoc;


    public int stateBG;
    static string host = "140.115.126.137";
    static string id = "maze";
    static string pwd = "106524006";
    static string database = "microbe";
    public static string result = "";

    string userID = "";

    void Start () {
        mySQLAccess = new MySQLAccess(host, id, pwd, database);
        test.onClick.AddListener(delegate() {StartCoroutine("ReloadXMLtoDB", 0.5F);});
    }


    IEnumerator ReloadXMLtoDB(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        xmlDoc = new XmlDocument();
        xmlDoc.Load(Xmlprocess.path + Xmlprocess._FileName);

        XmlNode node1 = xmlDoc.SelectSingleNode("Loadfile/User");
        XmlElement element11 = (XmlElement)node1;
        XmlAttribute attribute11 = element11.GetAttributeNode("ID");

        userID = attribute11.Value;


        /*
        XmlNode node1 = xmlDoc.SelectSingleNode("Ail_logfile/User");
        XmlElement element11 = (XmlElement)node1;
        XmlAttribute attribute11 = element11.GetAttributeNode("ID");

        string connectionString = "Server=" + host + ";Database=" + database + ";User ID=" + id + ";Password=" + pwd + ";Pooling=false;CharSet=utf8";
        openSqlConnection(connectionString);
        string sql1 = "UPDATE `member`    SET `money`=" + attribute12.Value + ", `background`='" + attribute15.Value +
    "', `class`='" + attribute13.Value + "', `number`='" + attribute14.Value + "' WHERE `ID`=" + attribute11.Value;
        MySqlCommand cmd1 = new MySqlCommand(sql1, dbConnection);
        cmd1.ExecuteNonQuery();
        */

    }


    public static void OnApplicationQuit()
    {
        MySQLAccess.Close();
    }



}
