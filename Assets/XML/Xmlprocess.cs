using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using edu.ncu.list.util;
using System.Collections.Generic;

public class Xmlprocess{
	public XmlDocument xmlDoc;
	XmlCreate xmlCreate;
	public int count_onetime = 0;
	public string Strtime = (System.DateTime.Now).ToString();
	public static string path, _FileName;


	public Xmlprocess(string filename) { //database initial
		if (Application.platform == RuntimePlatform.Android) {
			path = Constants.DATABASE_PATH ;
		} else {
			path = Application.dataPath + "/Resources/";
		}
        _FileName = filename + ".xml";

        if (isExits ()) {
			xmlDoc = new XmlDocument ();
			xmlDoc.Load (path + _FileName);
		}
	}

    public Xmlprocess(){
        if (isExits())
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path + _FileName);
        }
    }

    private Boolean isExits()
	{
		if (!System.IO.File.Exists(path + _FileName))
		{
			xmlCreate = new XmlCreate (path,_FileName);//若檔案不存在，則創建xml
		}
		return true;
	}



	public void saveData()
	{
		xmlDoc.Save(path + _FileName);
	}
	//---------------------------------個人狀態--------------------------------------


	public void setUserInfo(string []userInfo){
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute[] attributeList = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level") };
            for (int i = 0; i < userInfo.Length; i++)
            {
                attributeList[i].Value = userInfo[i];
            }
            saveData();
        }
	}


	public string getID() {
		if(isExits() ) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("ID");
			return attribute.Value.ToString();
		}
		return "0" ;
	}


	public string getName() {
		if(isExits() ) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("name");
			return attribute.Value.ToString();
		}
		return "0" ;
	}

    public string getLevel()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("level");
            return attribute.Value.ToString();
        }
        return "0";
    }

    public void setEnemy(string enemy_id,string searchtime,string starttime) {
        if (isExits()) {
                XmlNode competenode = xmlDoc.SelectSingleNode("Loadfile/User/compete");
                XmlElement compete = (XmlElement)competenode;
                XmlElement enemyList = xmlDoc.CreateElement("enemy");
                enemyList.SetAttribute("enemy_id", enemy_id);
                enemyList.SetAttribute("searchTime", searchtime);
                enemyList.SetAttribute("startTime", starttime);
                compete.AppendChild(enemyList);
                 saveData();
        }
    }

    public int getmoney(int money, bool cost) {

		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("money");
			int count = XmlConvert.ToInt32(attribute.Value);
			if(cost) {
				attribute.Value = money.ToString();
				xmlDoc.Save(path+_FileName);
				return count;
			} else if(!cost) {
				return count;//how mach you have.
			}
			return 0;
		}
		return 0;
	}	

	public void clickword_getmoney() {

		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("money");
			int count = XmlConvert.ToInt32(attribute.Value);
			count = count + 1;
			attribute.Value = count.ToString();
			saveData();
		}
	}

	public void getChallengeMoney(int money, bool kind) {

		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("money");
			int count = XmlConvert.ToInt32(attribute.Value);
			if(kind == false) {
				count = count + money; 
				attribute.Value = count.ToString();
			}
			saveData();
		}
	}	
	//------------------------------金幣使用次數----------------------------------

	public int dosomething(string something, bool state) {
		XmlNode nodeLast = null;
		string lastDay = "";
		
		if(isExits() ) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/log_record/" + something);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("count");
			int count = XmlConvert.ToInt32(attribute.Value);
			
			if(something == "ali/get_money" && !element.HasChildNodes) {
				XmlElement ali_DayGet = xmlDoc.CreateElement("ali_DayGet");
				ali_DayGet.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
				ali_DayGet.SetAttribute("count", "0");		
				element.AppendChild(ali_DayGet);
			}

			if(!state) {
				if(something == "ali/get_money") {
					//Find the last Node in <get_money>
					XmlNodeList nodelist = xmlDoc.SelectNodes("//ali_DayGet");
					foreach (XmlNode item_File in nodelist) {
						XmlAttributeCollection xAT = item_File.Attributes;
						for (int i = 0 ; i < xAT.Count ; i++) {
							if (xAT.Item(i).Name == "day") lastDay = xAT.Item(i).Value ;	
							nodeLast = item_File;
				        }					
					}		
					
					//Day is change
					if(lastDay != DateTime.Now.ToString("yyyy-MM-dd")) {
						XmlElement ali_DayGet = xmlDoc.CreateElement("ali_DayGet");
						ali_DayGet.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
						ali_DayGet.SetAttribute("count", "0");		
						element.AppendChild(ali_DayGet);
						
						//Refind the last node
						nodelist = xmlDoc.SelectNodes("//ali_DayGet");
						foreach (XmlNode item_File in nodelist) {
							XmlAttributeCollection xAT = item_File.Attributes;
							for (int i = 0 ; i < xAT.Count ; i++) {
								if (xAT.Item(i).Name == "day") lastDay = xAT.Item(i).Value ;	
								nodeLast = item_File;
					        }					
						}						
					} 		
					// ************************************************************************************					
					
					XmlElement elementLast = (XmlElement)nodeLast;
					XmlAttribute attributeLast = elementLast.GetAttributeNode("count");
					attributeLast.Value = ( Convert.ToInt32(attributeLast.Value) + 1 ).ToString();
				}
				
				count++;
				attribute.Value = count.ToString();
				saveData();
				return count;
			} 
		}
		return 0;
	}

    
	public int starstate(string kind){
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute1 = element.GetAttributeNode("star");
			return XmlConvert.ToInt32(attribute1.Value);
			//xmlDoc41.Save(path + _FileName);
		}
		return 0 ;
	}
		
	public int badge_speak_count(string kind, int count){
		
		if(isExits()) {

			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute1 = element.GetAttributeNode("count");
			attribute1.Value=count.ToString();
			saveData();
			return XmlConvert.ToInt32(attribute1.Value);}
		return 0 ;
	}

	// reward speakcount
	public int reward_speakcount(string kind){
		
		if(isExits()) {

			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute1 = element.GetAttributeNode("speak_count");
			return XmlConvert.ToInt32(attribute1.Value);
		}
		return 0 ;
	}
		

	// write learningtask star && practice speak
	public int write_star2(string kind, int star, int listen_count_practice, int speak_count){

		if (System.IO.File.Exists (path + _FileName)) {

			XmlNode node = xmlDoc.SelectSingleNode ("Loadfile/User/learningtask/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode ("star");           //xml讀出來的星星數      
			XmlAttribute attribute1 = element.GetAttributeNode ("listen_count_practice");//xml讀出來的聽力次數
			XmlAttribute attribute2 = element.GetAttributeNode ("speak_count");
			int Nowstar = XmlConvert.ToInt32 (attribute.Value); 
			int countlisten = XmlConvert.ToInt32 (attribute1.Value);  
			int countspeak = XmlConvert.ToInt32 (attribute2.Value); 
			if ( star > Nowstar ) {	                	  //star系統傳過來新的星星數		
				attribute.Value=star.ToString();     //如果xml讀出來的星星數<獲得星星數，將星星數更改為系統獲得數量
				countlisten=countlisten+1;
				countspeak=countspeak+1;
				attribute1.Value=countlisten.ToString();
				attribute2.Value=countspeak.ToString();
			}
			else {
				star = Nowstar;
				attribute.Value=Nowstar.ToString();
				countlisten=countlisten+1;
				countspeak=countspeak+1;
				attribute1.Value=countlisten.ToString();
				attribute2.Value=countspeak.ToString();
			}
			saveData();
		}
		return 0 ;
	}
	// write learningteask unlock
	public int write_unlockstate(string kind, int state){

		if(isExits()) {

			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("state");
			int Newstate = XmlConvert.ToInt32(attribute.Value);
			if( Newstate < state ){
				Newstate = state;
				attribute.Value = Newstate.ToString();
			}else{
				attribute.Value = Newstate.ToString();
			}
			saveData();
		}
		return 0 ;
	}

	//write star all count
	public int star_allcount(string kind, int star_count){
		
		if(isExits()) {

			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("star_all_count");
			int Nowcount = XmlConvert.ToInt32 (attribute.Value);
			Nowcount = star_count;
			attribute.Value=Nowcount.ToString();
			saveData();
		}
		return 0 ;
	}
	


	// 解鎖新關卡	
	public int write_allstarstate(string kind, int state){
		
		if(isExits()) {

			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("state_count");
			int Newstate = XmlConvert.ToInt32(attribute.Value);
			if( Newstate < state ){
				Newstate++;
				attribute.Value = Newstate.ToString();
			}else{
				attribute.Value = Newstate.ToString();
			}
			saveData();
		}
		return 0 ;
	}
		
	// 讀取目前解鎖關卡	
	public int unlockstate(string kind){
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("state_count");
			return XmlConvert.ToInt32(attribute.Value);
		}
		return 0 ;
	}

	//move_progress 20160224
	public int write_move_progress(string kind, int question, int moveNum, int duration, string level) {
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_move_progress/"+kind);
			XmlElement list = xmlDoc.CreateElement("list");
			list.SetAttribute("QNum",question.ToString());
			list.SetAttribute("moveNum",moveNum.ToString());
			list.SetAttribute("duration",duration.ToString());
			list.SetAttribute("level", level);
			node.AppendChild(list);
			saveData();
		}
		return 0 ;
	}
	// move allcount 20160224
	public int write_movecount(string kind, int count){
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_move_progress/"+kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("move_count_practice");
			int Nowcount = XmlConvert.ToInt32 (attribute.Value);
			Nowcount=Nowcount+1;
			attribute.Value=Nowcount.ToString();
			saveData();
		}
		return 0 ;
	}

	//speak_progress 20160224
	public int write_speak_progress(string kind, string Asw, string score) {
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_speak_progress/"+kind);
			XmlElement list = xmlDoc.CreateElement("list");
			list.SetAttribute("Answer",Asw);
			list.SetAttribute("score",score);
			node.AppendChild(list);
			saveData();
		}
		return 0 ;
	}
	// speak allcount 20160224
	public int write_speakcount(string kind, int count){
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_speak_progress/"+kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("speak_count_practice");
			int Nowcount = XmlConvert.ToInt32 (attribute.Value);
			Nowcount=Nowcount+1;
			attribute.Value=Nowcount.ToString();
			saveData();
		}
		return 0 ;
	}


	// read challengeticket
	public int challengeticket_state(string kind){

		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("use");
			return XmlConvert.ToInt32(attribute.Value);
		}
		return 0 ;
	}

	//challenge_progress
	public int write_challenge_progress(string kind, int choicetime, int movetime, int speaktime, int choicescore, int movescore, int SpeakScore, int AddScore) {
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/"+kind);
			XmlElement list = xmlDoc.CreateElement("list");
			list.SetAttribute("Choice_duration",choicetime.ToString());
			list.SetAttribute("Move_duration",movetime.ToString());
			list.SetAttribute("Speak_duration",speaktime.ToString());
			list.SetAttribute("Choice_score",choicescore.ToString());
			list.SetAttribute("Move_score",movescore.ToString());
			list.SetAttribute("speak_score",SpeakScore.ToString());
			list.SetAttribute("Final_score",AddScore.ToString());
			node.AppendChild(list);
			saveData();
		}
		return 0 ;
	}

	public int write_challengescore(string kind, int score){
		
		if(isExits()) {
			XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/" + kind);
			XmlElement element = (XmlElement)node;
			XmlAttribute attribute = element.GetAttributeNode("hight_score");
			int Newscore = XmlConvert.ToInt32(attribute.Value);
			if( Newscore < score ){
				Newscore = score;
				attribute.Value = Newscore.ToString();
			}else{
				attribute.Value = Newscore.ToString();
			}
			saveData();
		}
		return 0 ;
	}

	//初次建立Log紀錄
	public void timeHistoryRecord(string scence) {
		if (isExits ()) {
			XmlNode nodeLast = null;
			XmlNode nodeLast_Previous = null;

			// Find the previous scence start time ********************************************************************************************
			XmlNodeList nodelist_Previous = xmlDoc.SelectNodes ("//time_history_record");
			foreach (XmlNode item_File_Previous in nodelist_Previous) {
				XmlAttributeCollection xAT2 = item_File_Previous.Attributes;
				for (int j = 0; j < xAT2.Count; j++) {
					nodeLast_Previous = item_File_Previous;
				}					
			}
			XmlElement elementLast_Previous = (XmlElement)nodeLast_Previous;
			XmlAttribute attributeLast_Previous = elementLast_Previous.GetAttributeNode ("startTime");
			XmlAttribute attributeLast_Duration = elementLast_Previous.GetAttributeNode ("duration");

			DateTime nowTime = Convert.ToDateTime (DateTime.Now.ToString ("HH:mm:ss"));
			DateTime getTime = Convert.ToDateTime (attributeLast_Previous.Value.ToString ());

			System.TimeSpan diff = nowTime.Subtract (getTime);
			int timerNum = (int)diff.TotalSeconds;

			attributeLast_Duration.Value = (timerNum / 60).ToString () + ":" + (timerNum % 60).ToString ();
			// *********************************************************************************************************************************************
			
			XmlNodeList nodelist = xmlDoc.SelectNodes ("//time_history_day");
			foreach (XmlNode item_File in nodelist) {
				XmlAttributeCollection xAT = item_File.Attributes;
				for (int i = 0; i < xAT.Count; i++) {
					nodeLast = item_File;
				}					
			}

			XmlNode node = xmlDoc.SelectSingleNode ("Loadfile/log_record/time_history");
			XmlElement element = (XmlElement)node;			
			XmlElement elementLast = (XmlElement)nodeLast;
			XmlAttribute attributeLast = elementLast.GetAttributeNode ("day");

			if (attributeLast.Value.ToString () != DateTime.Now.ToString ("yyyy-MM-dd")) {			
				XmlElement time_history_day = xmlDoc.CreateElement ("time_history_day");
				time_history_day.SetAttribute ("day", DateTime.Now.ToString ("yyyy-MM-dd"));	
				element.AppendChild (time_history_day);

				//Refind the last node
				/*
				 * nodelist = xmlDoc.SelectNodes ("//time_history_day");
					foreach (XmlNode item_File in nodelist) {
						XmlAttributeCollection xAT = item_File.Attributes;
						for (int i = 0; i < xAT.Count; i++) {
							nodeLast = item_File;
						}					
					}
				*/			
				elementLast = (XmlElement)nodeLast;
			}	

			XmlElement time_history_record = xmlDoc.CreateElement ("time_history_record");
			elementLast.AppendChild (time_history_record);
			time_history_record.SetAttribute ("scence", scence);
			time_history_record.SetAttribute ("startTime", DateTime.Now.ToString ("HH:mm:ss"));
			time_history_record.SetAttribute ("duration", "");

			saveData();
			
		}
	}


	//Log紀錄
	public void New_timeHistoryRecord(string scence, string starttime){
		XmlNode nodeLast = null;
		if (isExits()) {
			XmlNodeList nodelist = xmlDoc.SelectNodes("//time_history_day");
			foreach (XmlNode item_File in nodelist) {
				XmlAttributeCollection xAT = item_File.Attributes;
				for (int i = 0 ; i < xAT.Count ; i++) {
					nodeLast = item_File;
				}					
			}

			XmlNode node = xmlDoc.SelectSingleNode ("Loadfile/log_record/time_history");
			XmlElement element = (XmlElement)node;
			XmlElement elementLast = (XmlElement)nodeLast;
			XmlAttribute attributeLast = elementLast.GetAttributeNode ("day");
					
			if (attributeLast.Value.ToString () != DateTime.Now.ToString ("yyyy-MM-dd")) {			
				XmlElement time_history_day = xmlDoc.CreateElement ("time_history_day");
				time_history_day.SetAttribute ("day", DateTime.Now.ToString ("yyyy-MM-dd"));	
				element.AppendChild (time_history_day);
				elementLast = (XmlElement)nodeLast;
			}

			XmlElement time_history_record = xmlDoc.CreateElement("time_history_record");;
			elementLast.AppendChild(time_history_record);
			time_history_record.SetAttribute("scence", scence);
			time_history_record.SetAttribute("startTime", starttime);
			time_history_record.SetAttribute("endTime", DateTime.Now.ToString("HH:mm:ss"));

			saveData();
		}
	}

	

}
