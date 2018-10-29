using UnityEngine;
using System.Collections;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml;
using System.IO;
using System.Text;
using edu.ncu.list.util;
public class UpdateSQL : MonoBehaviour {

    protected Xmlprocess xmlprocess;
    private static MySqlConnection dbConnection;
    public XmlDocument xmlDoc;
    public string path, _FileName;
    public int stateBG;
    static string host = "140.115.126.137";
    static string id = "maze";
    static string pwd = "106524006";
    static string database = "microbe";
    public static string result = "";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
