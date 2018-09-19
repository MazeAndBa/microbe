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

    //<summary>
    //initial file,search the same xml file with the same userID
    //</summary>
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

    //<summary>
    //When registering,initial set userInfo.
    //</summary>
    public void setUserInfo(string []userInfo){
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute[] attributeList = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex") };
            for (int i = 0; i < userInfo.Length; i++)
            {
                attributeList[i].Value = userInfo[i];
            }
            saveData();
        }
	}

    ///<summary>
    ///return an array, 0=ID,1=name,2=level,3=sex, 4=money
    ///</summary>
    public string[] getUserInfo()
    {
        if (isExits())
        {
            string[] info = new string[4];
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute []attribute = { element.GetAttributeNode("ID") , element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex"), element.GetAttributeNode("money") };
            for (int i = 0; i < info.Length; i++)
            {
                info[i] = attribute[i].Value.ToString();

            }

            return info;
        }
        return null;
    }

    //<summary>
    //record competeInfo
    //</summary>
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
			saveData();
		}
	}

    public void ExitTimeHistoryRecord(string endTime)
    {
        if (isExits())
        {
            //XmlNode nodeLast = null;
            XmlNode nodeLast_Previous = null;

            // Find the previous scence start time ********************************************************************************************
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//time_history_day");
            foreach (XmlNode item_File_Previous in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = item_File_Previous.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLast_Previous = item_File_Previous;
                }
            }
            XmlElement elementLast_Previous = (XmlElement)nodeLast_Previous;
            XmlAttribute attributeLast_StartTime = elementLast_Previous.GetAttributeNode("startTime");
            XmlAttribute attributeLast_EndTime = elementLast_Previous.GetAttributeNode("endTime");
            XmlAttribute attributeLast_Duration = elementLast_Previous.GetAttributeNode("duration");

            DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
            DateTime getTime = Convert.ToDateTime(attributeLast_StartTime.Value.ToString());

            System.TimeSpan diff = nowTime.Subtract(getTime);
            int timerNum = (int)diff.TotalSeconds;

            attributeLast_EndTime.Value = getTime.ToString();
            attributeLast_Duration.Value = (timerNum / 60).ToString() + ":" + (timerNum % 60).ToString();
            // *********************************************************************************************************************************************
            saveData();
        }
    }

    /*-------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public bool getLearningState(int level) {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning/level" + level);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("learning_count");
            int learningcount = XmlConvert.ToInt32(attribute.Value);
            if (learningcount > 0) { return true; }
            return false;
        }
        return false;
    }


    /// <summary>
    /// 更新單字瀏覽與學習次數紀錄
    /// </summary>
    /// <param name="attributeName"></param>
    public void setLearningCount(string attributeName, int level)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning/level"+level);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    //新增每回單字學習紀錄
    public void createLearningRecord(int level)
    {
        XmlNode nodeLast = null;
        XmlElement learning_history = null;

        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element = (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);//創新log與learning節點

                XmlElement learninghistory = xmlDoc.CreateElement("learning_history"); ;
                log_record.AppendChild(learninghistory);
                learning_history = learninghistory;
            }
            else
            {
                XmlNode n_learning_history = nodeLast.SelectSingleNode("learning_history");//在最近一筆的log下。找到learning節點
                learning_history = (XmlElement)n_learning_history;
            }

            XmlElement learning_record = xmlDoc.CreateElement("learning_record");
            learning_history.AppendChild(learning_record);

            learning_record.SetAttribute("level", level.ToString());
            learning_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            learning_record.SetAttribute("score", "0");
            learning_record.SetAttribute("endTime", "");

            saveData();
        }
    }


    //更新每回單字學習紀錄
    public void setLearningScoreRecord(int level,int score)
    {
        if (isExits())
        {
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//learning_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_score = element.GetAttributeNode("score");
            XmlAttribute attr_endTime = element.GetAttributeNode("endTime");
            attr_score.Value = score.ToString();
            attr_endTime.Value = DateTime.Now.ToString("HH: mm:ss");
            updateHighScore(level, score);
            saveData();
        }
    }

    void updateHighScore(int level,int score)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning/level" + level);
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");    
        int highscore = XmlConvert.ToInt32(attribute.Value);
        if (score > highscore) {
            attribute.Value = score.ToString();
        }
        saveData();
    }


    /// <summary>
    /// 更新對戰次數紀錄
    /// </summary>
    /// <param name="attributeName"></param>
    public void setCompeteCount(string attributeName, int level)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete/level" + level);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    //新增一筆對戰紀錄
    public void createCompeteRecord(int level)
    {
        XmlNode nodeLast = null;
        XmlElement compete_history = null;

        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element = (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);//創新log與learning節點

                XmlElement competehistory = xmlDoc.CreateElement("compete_history"); ;
                log_record.AppendChild(competehistory);
                compete_history = competehistory;
            }
            else
            {
                XmlNode n_compete_history = nodeLast.SelectSingleNode("compete_history");//在最近一筆的log下。找到learning節點
                compete_history = (XmlElement)n_compete_history;
            }

            XmlElement compete_record = xmlDoc.CreateElement("compete_record");
            compete_history.AppendChild(compete_record);

            compete_record.SetAttribute("level", level.ToString());
            compete_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            compete_record.SetAttribute("endTime", "");
            compete_record.SetAttribute("score", "0");
            compete_record.SetAttribute("rank", "0");//本次對戰排名
            saveData();
        }
    }

    //新增每回合的對戰紀錄
    public void createRoundRecord(string quesID,string ans_state,int duration)
    {
        XmlNode nodeLast = null;
        if (isExits())
        {

            XmlNodeList nodelist = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement compete_record = (XmlElement)nodeLast;
            XmlElement round_record = xmlDoc.CreateElement("round_record");
            compete_record.AppendChild(round_record);
            round_record.SetAttribute("ques_id", quesID.ToString());//題號
            round_record.SetAttribute("ans_state", ans_state);//作答正確或錯誤
            round_record.SetAttribute("duration", duration.ToString());//作答時間
            round_record.SetAttribute("score", "0");//作答時間
            round_record.SetAttribute("rank", "0");//當回合的排名
            saveData();
        }
    }

   public void setRoundScore(int score,int rank)
    {
        XmlNode nodeLast = null;
        XmlNodeList nodelist = xmlDoc.SelectNodes("//round_record");
        foreach (XmlNode item_File in nodelist)
        {
            XmlAttributeCollection xAT = item_File.Attributes;
            for (int i = 0; i < xAT.Count; i++)
            {
                nodeLast = item_File;
            }
        }
        XmlElement round_record = (XmlElement)nodeLast;
        XmlAttribute attr_score = round_record.GetAttributeNode("score");
        XmlAttribute attr_rank = round_record.GetAttributeNode("rank");
        attr_score.Value = score.ToString();
        attr_rank.Value = rank.ToString();

        saveData();
    }

    //更新每回對戰紀錄
    public void setCompeteScoreRecord(int level, int score,int rank)
    {
        if (isExits())
        {
            XmlNode nodeLastLearning = null;
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//compete_record");
            foreach (XmlNode itemsNode in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = itemsNode.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    nodeLastLearning = itemsNode;
                }
            }
            XmlElement element = (XmlElement)nodeLastLearning;
            XmlAttribute attr_score = element.GetAttributeNode("score");
            XmlAttribute attr_endTime = element.GetAttributeNode("endTime");
            XmlAttribute attr_rank = element.GetAttributeNode("rank");
            attr_score.Value = score.ToString();
            attr_endTime.Value = DateTime.Now.ToString("HH: mm:ss");
            attr_rank.Value = rank.ToString();
            updateCompeteHighScore(level, score);
            saveData();
        }
    }

    void updateCompeteHighScore(int level, int score)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete/level" + level);
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");
        int highscore = XmlConvert.ToInt32(attribute.Value);
        if (score > highscore)
        {
            attribute.Value = score.ToString();
        }
        saveData();
    }

    /// <summary>
    ///成就UI點擊次數
    /// </summary>
    /// <param name="attributeName"></param>
    public void setTouchCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_achieve");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("count");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    /// <summary>
    ///排行榜UI點擊次數
    /// </summary>
    /// <param name="attributeName"></param>
    public void setTouchCount(int level)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_leaderboard/touch_level" + level);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("count");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    //0830場景進入紀錄
    public void ScceneHistoryRecord(string scence, string starttime)
    {
        XmlNode nodeLast = null;
        XmlElement scene_history = null;
        if (isExits())
        {
            XmlNodeList nodelist = xmlDoc.SelectNodes("//log_record");
            foreach (XmlNode item_File in nodelist)
            {
                XmlAttributeCollection xAT = item_File.Attributes;
                for (int i = 0; i < xAT.Count; i++)
                {
                    nodeLast = item_File;
                }
            }

            XmlElement element= (XmlElement)nodeLast;
            XmlAttribute attributeLast = element.GetAttributeNode("day");
            if (attributeLast.Value.ToString() != DateTime.Now.ToString("yyyy-MM-dd"))//如果最近一筆紀錄不是今天的日期
            {

                XmlNode n_Loadfile = xmlDoc.SelectSingleNode("Loadfile/");
                XmlElement loadfile = (XmlElement)n_Loadfile;
                XmlElement log_record = xmlDoc.CreateElement("log_record");
                log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                loadfile.AppendChild(log_record);

                XmlElement scenehistory = xmlDoc.CreateElement("scene_history"); ;
                log_record.AppendChild(scenehistory);
                scene_history = scenehistory;
            }
            else
            {
                XmlNode n_scene_history = nodeLast.SelectSingleNode("scene_history");
                scene_history = (XmlElement)n_scene_history;
            }

            XmlElement scene_record = xmlDoc.CreateElement("scene_record"); ;
            scene_history.AppendChild(scene_record);
            scene_record.SetAttribute("scence", scence);
            scene_record.SetAttribute("startTime", starttime);
            saveData();
        }
    }


    //0830場景離開紀錄
    public void ExitSceneRecord(string sceneName)
    {
        if (isExits())
        {
            XmlNode nodeLast_Previous = null;
            string tmp_sceneName= "";
            // 抓取最近一次進入的場景紀錄
            XmlNodeList nodelist_Previous = xmlDoc.SelectNodes("//scene_record");

            foreach (XmlNode item_File_Previous in nodelist_Previous)
            {
                XmlAttributeCollection xAT2 = item_File_Previous.Attributes;
                for (int j = 0; j < xAT2.Count; j++)
                {
                    if (xAT2.Item(j).Name == "scene") tmp_sceneName = xAT2.Item(j).Value;
                    nodeLast_Previous = item_File_Previous;
                }
            }
            if (tmp_sceneName == sceneName)
            {
                XmlElement elementLast_Previous = (XmlElement)nodeLast_Previous;
                elementLast_Previous.SetAttribute("endTime", DateTime.Now.ToString("HH:mm:ss"));
                saveData();
            }
        }
    }

    /// <summary>
    ///Get 各難易度的學習狀況
    /// </summary>
    public string[] getAchieveState(int level)
    {
        if (isExits())
        {
            string[] _tmp = new string[4];
            XmlNode learningNode = xmlDoc.SelectSingleNode("Loadfile/User/learning/level"+level);
            XmlNode competeNode = xmlDoc.SelectSingleNode("Loadfile/User/compete/level" + level);
            XmlElement learningElement = (XmlElement)learningNode;
            XmlElement competeElement = (XmlElement)competeNode;
            XmlAttribute learning_count = learningElement.GetAttributeNode("learning_count");
            XmlAttribute learning_highscore = learningElement.GetAttributeNode("highscore");
            XmlAttribute compete_count = competeElement.GetAttributeNode("compete_count");
            XmlAttribute compete_highscore = competeElement.GetAttributeNode("highscore");
            _tmp[0] = learning_count.Value;
            _tmp[1] = learning_highscore.Value;
            _tmp[2] = compete_count.Value;
            _tmp[3] = compete_highscore.Value;

            return _tmp;
        }
        return null;

    }

}
