using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using edu.ncu.list.util;
using System.Collections.Generic;

public class Xmlprocess
{
    public XmlDocument xmlDoc;
    XmlCreate xmlCreate;
    public int count_onetime = 0;
    public string Strtime = (System.DateTime.Now).ToString();
    public static string path, _FileName;

    ///<summary>
    ///initial file,search the same xml file with the same userID
    ///</summary>
    public Xmlprocess(string filename)
    { //database initial
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Constants.DATABASE_PATH;
        }
        else
        {
            path = Application.dataPath + "/Resources/";
        }
        _FileName = filename + ".xml";

        if (isExits())
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(path + _FileName);
        }
    }

    public Xmlprocess()
    {
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
            xmlCreate = new XmlCreate(path, _FileName);//若檔案不存在，則創建xml
        }
        return true;
    }



    public void saveData()
    {
        xmlDoc.Save(path + _FileName);
    }

    //---------------------------------個人狀態--------------------------------------

    ///<summary>
    ///When registering,initial set userInfo.
    ///</summary>
    public void setUserInfo(string[] userInfo)
    {
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
            XmlAttribute[] attribute = { element.GetAttributeNode("ID"), element.GetAttributeNode("name"), element.GetAttributeNode("level"), element.GetAttributeNode("sex"), element.GetAttributeNode("money") };
            for (int i = 0; i < info.Length; i++)
            {
                info[i] = attribute[i].Value.ToString();

            }

            return info;
        }
        return null;
    }

    public int getmoney(int money, bool cost)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("money");
            int count = XmlConvert.ToInt32(attribute.Value);
            if (cost)
            {
                attribute.Value = money.ToString();
                xmlDoc.Save(path + _FileName);
                return count;
            }
            else if (!cost)
            {
                return count;//how mach you have.
            }
            return 0;
        }
        return 0;
    }

    public void clickword_getmoney()
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("money");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    public void getChallengeMoney(int money, bool kind)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("money");
            int count = XmlConvert.ToInt32(attribute.Value);
            if (kind == false)
            {
                count = count + money;
                attribute.Value = count.ToString();
            }
            saveData();
        }
    }
    //------------------------------金幣使用次數----------------------------------

    public int dosomething(string something, bool state)
    {
        XmlNode nodeLast = null;
        string lastDay = "";

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/log_record/" + something);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("count");
            int count = XmlConvert.ToInt32(attribute.Value);

            if (something == "ali/get_money" && !element.HasChildNodes)
            {
                XmlElement ali_DayGet = xmlDoc.CreateElement("ali_DayGet");
                ali_DayGet.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                ali_DayGet.SetAttribute("count", "0");
                element.AppendChild(ali_DayGet);
            }

            if (!state)
            {
                if (something == "ali/get_money")
                {
                    //Find the last Node in <get_money>
                    XmlNodeList nodelist = xmlDoc.SelectNodes("//ali_DayGet");
                    foreach (XmlNode item_File in nodelist)
                    {
                        XmlAttributeCollection xAT = item_File.Attributes;
                        for (int i = 0; i < xAT.Count; i++)
                        {
                            if (xAT.Item(i).Name == "day") lastDay = xAT.Item(i).Value;
                            nodeLast = item_File;
                        }
                    }

                    //Day is change
                    if (lastDay != DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        XmlElement ali_DayGet = xmlDoc.CreateElement("ali_DayGet");
                        ali_DayGet.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
                        ali_DayGet.SetAttribute("count", "0");
                        element.AppendChild(ali_DayGet);

                        //Refind the last node
                        nodelist = xmlDoc.SelectNodes("//ali_DayGet");
                        foreach (XmlNode item_File in nodelist)
                        {
                            XmlAttributeCollection xAT = item_File.Attributes;
                            for (int i = 0; i < xAT.Count; i++)
                            {
                                if (xAT.Item(i).Name == "day") lastDay = xAT.Item(i).Value;
                                nodeLast = item_File;
                            }
                        }
                    }
                    // ************************************************************************************					

                    XmlElement elementLast = (XmlElement)nodeLast;
                    XmlAttribute attributeLast = elementLast.GetAttributeNode("count");
                    attributeLast.Value = (Convert.ToInt32(attributeLast.Value) + 1).ToString();
                }

                count++;
                attribute.Value = count.ToString();
                saveData();
                return count;
            }
        }
        return 0;
    }


    public int starstate(string kind)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute1 = element.GetAttributeNode("star");
            return XmlConvert.ToInt32(attribute1.Value);
            //xmlDoc41.Save(path + _FileName);
        }
        return 0;
    }

    public int badge_speak_count(string kind, int count)
    {

        if (isExits())
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute1 = element.GetAttributeNode("count");
            attribute1.Value = count.ToString();
            saveData();
            return XmlConvert.ToInt32(attribute1.Value);
        }
        return 0;
    }

    // reward speakcount
    public int reward_speakcount(string kind)
    {

        if (isExits())
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute1 = element.GetAttributeNode("speak_count");
            return XmlConvert.ToInt32(attribute1.Value);
        }
        return 0;
    }


    // write learningtask star && practice speak
    public int write_star2(string kind, int star, int listen_count_practice, int speak_count)
    {

        if (System.IO.File.Exists(path + _FileName))
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("star");           //xml讀出來的星星數      
            XmlAttribute attribute1 = element.GetAttributeNode("listen_count_practice");//xml讀出來的聽力次數
            XmlAttribute attribute2 = element.GetAttributeNode("speak_count");
            int Nowstar = XmlConvert.ToInt32(attribute.Value);
            int countlisten = XmlConvert.ToInt32(attribute1.Value);
            int countspeak = XmlConvert.ToInt32(attribute2.Value);
            if (star > Nowstar)
            {                         //star系統傳過來新的星星數		
                attribute.Value = star.ToString();     //如果xml讀出來的星星數<獲得星星數，將星星數更改為系統獲得數量
                countlisten = countlisten + 1;
                countspeak = countspeak + 1;
                attribute1.Value = countlisten.ToString();
                attribute2.Value = countspeak.ToString();
            }
            else
            {
                star = Nowstar;
                attribute.Value = Nowstar.ToString();
                countlisten = countlisten + 1;
                countspeak = countspeak + 1;
                attribute1.Value = countlisten.ToString();
                attribute2.Value = countspeak.ToString();
            }
            saveData();
        }
        return 0;
    }
    // write learningteask unlock
    public int write_unlockstate(string kind, int state)
    {

        if (isExits())
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learningtask/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("state");
            int Newstate = XmlConvert.ToInt32(attribute.Value);
            if (Newstate < state)
            {
                Newstate = state;
                attribute.Value = Newstate.ToString();
            }
            else
            {
                attribute.Value = Newstate.ToString();
            }
            saveData();
        }
        return 0;
    }

    //write star all count
    public int star_allcount(string kind, int star_count)
    {

        if (isExits())
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("star_all_count");
            int Nowcount = XmlConvert.ToInt32(attribute.Value);
            Nowcount = star_count;
            attribute.Value = Nowcount.ToString();
            saveData();
        }
        return 0;
    }



    // 解鎖新關卡	
    public int write_allstarstate(string kind, int state)
    {

        if (isExits())
        {

            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("state_count");
            int Newstate = XmlConvert.ToInt32(attribute.Value);
            if (Newstate < state)
            {
                Newstate++;
                attribute.Value = Newstate.ToString();
            }
            else
            {
                attribute.Value = Newstate.ToString();
            }
            saveData();
        }
        return 0;
    }

    // 讀取目前解鎖關卡	
    public int unlockstate(string kind)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("state_count");
            return XmlConvert.ToInt32(attribute.Value);
        }
        return 0;
    }

    //move_progress 20160224
    public int write_move_progress(string kind, int question, int moveNum, int duration, string level)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_move_progress/" + kind);
            XmlElement list = xmlDoc.CreateElement("list");
            list.SetAttribute("QNum", question.ToString());
            list.SetAttribute("moveNum", moveNum.ToString());
            list.SetAttribute("duration", duration.ToString());
            list.SetAttribute("level", level);
            node.AppendChild(list);
            saveData();
        }
        return 0;
    }
    // move allcount 20160224
    public int write_movecount(string kind, int count)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_move_progress/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("move_count_practice");
            int Nowcount = XmlConvert.ToInt32(attribute.Value);
            Nowcount = Nowcount + 1;
            attribute.Value = Nowcount.ToString();
            saveData();
        }
        return 0;
    }

    //speak_progress 20160224
    public int write_speak_progress(string kind, string Asw, string score)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_speak_progress/" + kind);
            XmlElement list = xmlDoc.CreateElement("list");
            list.SetAttribute("Answer", Asw);
            list.SetAttribute("score", score);
            node.AppendChild(list);
            saveData();
        }
        return 0;
    }
    // speak allcount 20160224
    public int write_speakcount(string kind, int count)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/practice_speak_progress/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("speak_count_practice");
            int Nowcount = XmlConvert.ToInt32(attribute.Value);
            Nowcount = Nowcount + 1;
            attribute.Value = Nowcount.ToString();
            saveData();
        }
        return 0;
    }


    // read challengeticket
    public int challengeticket_state(string kind)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("use");
            return XmlConvert.ToInt32(attribute.Value);
        }
        return 0;
    }

    //challenge_progress
    public int write_challenge_progress(string kind, int choicetime, int movetime, int speaktime, int choicescore, int movescore, int SpeakScore, int AddScore)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/" + kind);
            XmlElement list = xmlDoc.CreateElement("list");
            list.SetAttribute("Choice_duration", choicetime.ToString());
            list.SetAttribute("Move_duration", movetime.ToString());
            list.SetAttribute("Speak_duration", speaktime.ToString());
            list.SetAttribute("Choice_score", choicescore.ToString());
            list.SetAttribute("Move_score", movescore.ToString());
            list.SetAttribute("speak_score", SpeakScore.ToString());
            list.SetAttribute("Final_score", AddScore.ToString());
            node.AppendChild(list);
            saveData();
        }
        return 0;
    }

    public int write_challengescore(string kind, int score)
    {

        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/challenge_progress/" + kind);
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("hight_score");
            int Newscore = XmlConvert.ToInt32(attribute.Value);
            if (Newscore < score)
            {
                Newscore = score;
                attribute.Value = Newscore.ToString();
            }
            else
            {
                attribute.Value = Newscore.ToString();
            }
            saveData();
        }
        return 0;
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

    /// <summary>
    /// 取得目前個人學習區練習次數
    /// </summary>
    /// <returns></returns>
    public bool getLearningState()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("learning_count");
            int learningcount = XmlConvert.ToInt32(attribute.Value);
            if (learningcount > 0) { return true; }
            return false;
        }
        return false;
    }

    /// <summary>
    /// 更新單字瀏覽次數與練習次數
    /// </summary>
    /// <param name="attributeName">review_count或是learning_count</param>
    public void setLearningCount(string attributeName)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            if (attributeName=="learning_count") {
                badgeLearningCounts(count);
            }
            saveData();
        }
    }

    /// <summary>
    /// 新增每回單字學習紀錄
    /// </summary>
    public void createLearningRecord()
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

            learning_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            learning_record.SetAttribute("score", "0");
            learning_record.SetAttribute("endTime", "");

            saveData();
        }
    }

    /// <summary>
    /// 更新每回單字學習紀錄的分數與結束時間
    /// </summary>
    public void setLearningScoreRecord(int score)
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
            updateHighScore(score);
            saveData();
        }
    }

    void updateHighScore(int score)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");
        int highscore = XmlConvert.ToInt32(attribute.Value);
        if (score > highscore)
        {
            attribute.Value = score.ToString();
            int improveCounts = LearningScoreImprove();//當前進步次數
            badgeLearningImprove(improveCounts);
            badgeLearningHighScore(score);
        }
        saveData();
    }

    /// <summary>
    /// 更新練習進步總次數
    /// </summary>
    /// <returns></returns>
    int LearningScoreImprove()
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/learning");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("learningImprove");
        int count = XmlConvert.ToInt32(attribute.Value);
        count = count + 1;
        attribute.Value = count.ToString();
        saveData();

        return count;
    }


    /*===============================---同儕對戰區的紀錄---===============================*/

    /// <summary>
    /// 更新個人對戰的次數
    /// </summary>
    /// <param name="attributeName"></param>
    public void setCompeteCount(string attributeName)
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode(attributeName);
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            if (attributeName == "compete_count")
            {
                badgeCompeteCounts(count);
            }

            saveData();
        }
    }

    /// <summary>
    /// 新增一筆對戰紀錄
    /// </summary>
    public void createCompeteRecord()
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

            compete_record.SetAttribute("startTime", DateTime.Now.ToString("HH: mm:ss"));
            compete_record.SetAttribute("endTime", "");
            compete_record.SetAttribute("hint_LA", "0");//使用提示再聽一次的總次數
            compete_record.SetAttribute("hint_ST", "0");//使用提示中譯的總次數
            compete_record.SetAttribute("score", "0");
            compete_record.SetAttribute("rank", "0");//本次對戰排名
            saveData();
        }
    }

    /// <summary>
    ///對戰結束更新對戰紀錄
    /// </summary>
    public void setCompeteScoreRecord(int hintLACount, int hintSTCount, int score, int rank)
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
            XmlAttribute attr_hintLA = element.GetAttributeNode("hint_LA");
            XmlAttribute attr_hintST = element.GetAttributeNode("hint_ST");
            XmlAttribute attr_rank = element.GetAttributeNode("rank");
            attr_hintLA.Value = hintLACount.ToString();
            attr_hintST.Value = hintSTCount.ToString();
            attr_score.Value = score.ToString();
            attr_endTime.Value = DateTime.Now.ToString("HH: mm:ss");
            attr_rank.Value = rank.ToString();
            updateCompeteHighScore(score);
            saveData();
        }
    }

    void updateCompeteHighScore(int score)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("highscore");
        int highscore = XmlConvert.ToInt32(attribute.Value);
        if (score > highscore)
        {
            attribute.Value = score.ToString();
            int improveCounts = CompeteScoreImprove();//當前進步次數
            badgeCompeteImprove(improveCounts);
            badgeCompeteHighScore(score);

        }
        saveData();
    }

    /// <summary>
    /// 對戰進步次數
    /// </summary>
    /// <returns></returns>
    int CompeteScoreImprove()
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/User/compete");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("competeImprove");
        int count = XmlConvert.ToInt32(attribute.Value);
        count = count + 1;
        attribute.Value = count.ToString();
        saveData();

        return count;
    }


    /// <summary>
    /// 新增每回合的對戰紀錄
    /// </summary>
    /// <param name="quesID">題號</param>
    public void createRoundRecord(string quesID)
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
            round_record.SetAttribute("ans_state", "");//作答正確或錯誤
            round_record.SetAttribute("duration", "0");//作答時間
            round_record.SetAttribute("hint_LA", "0");//提示再聽一次的次數
            round_record.SetAttribute("hint_ST", "0");//提示中譯的次數
            round_record.SetAttribute("score", "0");//作答時間
            round_record.SetAttribute("rank", "0");//當回合的排名
            saveData();
        }
    }

    /// <summary>
    /// 設置當回合的答案
    /// </summary>
    /// <param name="ans_state">正確或錯誤</param>
    /// <param name="duration">花費時間</param>
    public void setRoundAns(string ans_state, int duration)
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
        XmlAttribute attr_ansState = round_record.GetAttributeNode("ans_state");
        XmlAttribute attr_duration = round_record.GetAttributeNode("duration");
        attr_ansState.Value = ans_state;
        attr_duration.Value = duration.ToString();

        saveData();
    }

    /// <summary>
    /// 當回合使用提示次數
    /// </summary>
    /// <param name="hintName">提示名稱</param>
    public void setRoundHintcount(string hintName)
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
        XmlAttribute attr_hint = round_record.GetAttributeNode(hintName);
        int count = XmlConvert.ToInt32(attr_hint.Value);
        count = count + 1;
        attr_hint.Value = count.ToString();

        saveData();
    }

    /// <summary>
    /// 取得回合使用提示的次數
    /// </summary>
    /// <param name="hintName">提示名稱</param>
    /// <returns></returns>
    public int getRoundHintcount(string hintName)
    {
        if (isExits())
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
            XmlAttribute attr_hint = round_record.GetAttributeNode(hintName);
            int count = XmlConvert.ToInt32(attr_hint.Value);
            return count;
        }
        return 0;
    }

    /// <summary>
    /// 設置當前回合的分數
    /// </summary>
    /// <param name="score">分數</param>
    /// <param name="rank">當前排名</param>
    public void setRoundScore(int score, int rank)
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

    /*===============================---點擊紀錄---===============================*/
    /// <summary>
    ///成就UI點擊次數
    /// </summary>
    /// <param name="attributeName"></param>
    public void setTouchACount()
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
    public void setTouchLCount()
    {
        if (isExits())
        {
            XmlNode node = xmlDoc.SelectSingleNode("Loadfile/touch_history/touch_leaderboard");
            XmlElement element = (XmlElement)node;
            XmlAttribute attribute = element.GetAttributeNode("count");
            int count = XmlConvert.ToInt32(attribute.Value);
            count = count + 1;
            attribute.Value = count.ToString();
            saveData();
        }
    }

    /*===============================---場景紀錄---===============================*/

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

            XmlElement element = (XmlElement)nodeLast;
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

    /*===============================---成就頁面---===============================*/
    /// <summary>
    ///Get學習狀況
    /// </summary>
    public string[] getAchieveLearningState()
    {
        if (isExits())
        {
            string[] _tmp = new string[2];
            XmlNode learningNode = xmlDoc.SelectSingleNode("Loadfile/User/learning");
            XmlElement learningElement = (XmlElement)learningNode;
            XmlAttribute learning_highscore = learningElement.GetAttributeNode("highscore");
            XmlAttribute learning_count = learningElement.GetAttributeNode("learning_count");
            _tmp[0] = learning_highscore.Value;
            _tmp[1] = learning_count.Value;

            return _tmp;
        }
        return null;

    }
    /// <summary>
    ///Get學習獎章狀況
    /// </summary>
    public int[] getAchieveLearningBadges(int learningBadgeCount)
    {
        if (isExits())
        {
            int[] badgesLevel = new int[learningBadgeCount]; ;
            for (int badgeID = 1; badgeID <= learningBadgeCount; badgeID++)
            {
                string badgeid = "badge" + badgeID;
                XmlNode competeBadgesNode = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/" + badgeid);
                XmlElement competeBadgesElement = (XmlElement)competeBadgesNode;
                XmlAttribute level = competeBadgesElement.GetAttributeNode("level");
                int targetLevel = XmlConvert.ToInt32(level.Value) + 1;//目標獎章為目前階段的下一階段
                badgesLevel[badgeID - 1] = targetLevel;//因為陣列索引值初始為0，故減1

            }
            return badgesLevel;
        }
        return null;

    }

    /// <summary>
    ///Get對戰狀況
    /// </summary>
    public string[] getAchieveCompeteState()
    {
        if (isExits())
        {
            string[] _tmp = new string[2];
            XmlNode competeNode = xmlDoc.SelectSingleNode("Loadfile/User/compete");
            XmlElement competeElement = (XmlElement)competeNode;
            XmlAttribute compete_highscore = competeElement.GetAttributeNode("highscore");
            XmlAttribute compete_count = competeElement.GetAttributeNode("compete_count");

            _tmp[0] = compete_highscore.Value;
            _tmp[1] = compete_count.Value;

            return _tmp;
        }
        return null;

    }

    /// <summary>
    ///Get對戰獎章狀況
    /// </summary>
    /// <param name="initialIndex">ID起始值</param>
    /// <param name="totalBadgeCount">獎章總數</param>
    public int[] getAchieveCompeteBadges(int initialIndex, int totalBadgeCount)
    {
        if (isExits())
        {
            int[] badgesLevel = new int[(totalBadgeCount - initialIndex) + 1]; ;
            for (int badgeID = initialIndex; badgeID <= totalBadgeCount; badgeID++)
            {
                string badgeid = "badge" + badgeID;
                XmlNode competeBadgesNode = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/" + badgeid);
                XmlElement competeBadgesElement = (XmlElement)competeBadgesNode;
                XmlAttribute level = competeBadgesElement.GetAttributeNode("level");
                int targetLevel = XmlConvert.ToInt32(level.Value) + 1;//目標獎章為目前階段的下一階段
                badgesLevel[badgeID - initialIndex] = targetLevel;

            }
            return badgesLevel;
        }
        return null;

    }

    /*===============================---獎章資料---===============================*/

    /// <summary>
    /// badge1練習次數獎章
    /// </summary>
    /// <param name="learningCounts">當前練習總次數</param>
    void badgeLearningCounts(int learningCounts) {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge1");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        switch (learningCounts)
        {
            case 1:
                _level = 1;
                break;
            case 5:
                _level = 2;
                break;
            case 10:
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
    }

    /// <summary>
    /// badge2練習分數達標獎章
    /// </summary>
    /// <param name="highscore">最高分數</param>
    void badgeLearningHighScore(int highscore) {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge2");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        if (highscore >= 500 && highscore < 2000)
        {
            _level = 1;
        }
        else if (highscore >= 2000 && highscore < 4000)
        {
            _level = 2;
        }
        else if (highscore >= 4000)
        {
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
    }

    /// <summary>
    /// badge3練習進步獎章
    /// </summary>
    /// <param name="improveCounts">當前進步總次數</param>
    void badgeLearningImprove(int improveCounts)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_learning/badge3");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        switch (improveCounts)
        {
            case 1:
                _level = 1;
                break;
            case 3:
                _level = 2;
                break;
            case 5:
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
    }
    /// <summary>
    /// badge8對戰次數獎章
    /// </summary>
    /// <param name="competeCounts">當前對戰總次數</param>
    void badgeCompeteCounts(int competeCounts)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge8");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        switch (competeCounts)
        {
            case 1:
                _level = 1;
                break;
            case 5:
                _level = 2;
                break;
            case 10:
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
    }
    /// <summary>
    /// badge9對戰分數達標獎章
    /// </summary>
    /// <param name="highscore">最高分數</param>
    void badgeCompeteHighScore(int highscore) {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge9");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        if (highscore >=30 && highscore<60) {
            _level = 1;
        }else if (highscore >= 60 && highscore<100) {
            _level = 2;
        }else if (highscore >= 100)
        {
            _level = 3;
        }
        attribute.Value = _level.ToString();
        saveData();
    }

    /// <summary>
    /// badge11對戰進步獎章
    /// </summary>
    /// <param name="improveCounts">當前進步總次數</param>
    void badgeCompeteImprove(int improveCounts)
    {
        XmlNode node = xmlDoc.SelectSingleNode("Loadfile/badge_record/badge_compete/badge11");
        XmlElement element = (XmlElement)node;
        XmlAttribute attribute = element.GetAttributeNode("level");
        int _level = 0;
        switch (improveCounts)
        {
            case 1:
                _level = 1;
                break;
            case 3:
                _level = 2;
                break;
            case 5:
                _level = 3;
                break;
        }
        attribute.Value = _level.ToString();
        saveData();
    }
}
