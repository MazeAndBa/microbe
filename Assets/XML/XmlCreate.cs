using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using edu.ncu.list.util;

//原檔名叫做xmlfile
public class XmlCreate{
	public int count = 0;	
	public XmlDocument xmlDoc;

	public XmlCreate(string path,string filename) {
		//檔案不存在，因此初次創建table

			string Strcount = count.ToString();
			xmlDoc = new XmlDocument();	

			XmlElement Loadfile = xmlDoc.CreateElement("Loadfile");
			xmlDoc.AppendChild(Loadfile);

        /*====================================---基本資料---====================================*/
			XmlElement User = xmlDoc.CreateElement("User");
            Loadfile.AppendChild(User);
            User.SetAttribute("ID", "");
			User.SetAttribute("name","");
			User.SetAttribute("level", "");
            User.SetAttribute("sex", "");
            User.SetAttribute("money", "100");

			XmlElement current_icon = xmlDoc.CreateElement("current_icon");//目前頭像編號
            User.AppendChild(current_icon);
            current_icon.SetAttribute("icon","");

        /*===============================---每一關卡的練習狀況---===============================*/
            XmlElement learning  = xmlDoc.CreateElement("learning");
		    User.AppendChild(learning);
            for (int i = 1; i <= 3; i++)
            {
                XmlElement level = xmlDoc.CreateElement("level"+i);
                learning.AppendChild(level);
                level.SetAttribute("review_count", Strcount);//查看單字的次數
                level.SetAttribute("learning_count", Strcount);//完成練習的次數
                level.SetAttribute("score", Strcount);
            }
        /*===============================---每一關卡的對戰狀況---===============================*/
            XmlElement compete = xmlDoc.CreateElement("compete");
		    User.AppendChild(compete);
            for (int i = 1; i <= 3; i++)
            {
                XmlElement level = xmlDoc.CreateElement("level" + i);
                learning.AppendChild(level);
                level.SetAttribute("compete_count", Strcount);//對戰的次數
                level.SetAttribute("highscore", Strcount);//對戰的最高分數
                level.SetAttribute("update_count", Strcount);//最高分數刷新次數
            }

        /* ----商店物品
			XmlElement goods = xmlDoc.CreateElement("goods_item");
			goods.SetAttribute("count",Strcount);
			Loadfile.AppendChild(goods);
				XmlElement _baby = xmlDoc.CreateElement("baby");
				_baby.SetAttribute("price","2400");
				_baby.SetAttribute("state", "0");
				_baby.SetAttribute("time", "");
				goods.AppendChild(_baby);
				XmlElement _glass = xmlDoc.CreateElement("glass");
				_glass.SetAttribute("price","400");
				_glass.SetAttribute("state", "0");
				_glass.SetAttribute("time", "");
				goods.AppendChild(_glass);
				XmlElement _guitar = xmlDoc.CreateElement("guitar");
				_guitar.SetAttribute("price","500");
				_guitar.SetAttribute("state", "0");
				_guitar.SetAttribute("time", "");
				goods.AppendChild(_guitar);
    -----*/


        /*====================================---Log紀錄---====================================*/
            XmlElement log_record = xmlDoc.CreateElement("log_record");
		    Loadfile.AppendChild(log_record);
            log_record.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));//Log紀錄的日期

            /*-------學習紀錄---------*/
            XmlElement learning_history = xmlDoc.CreateElement("learning_history");
            log_record.AppendChild(learning_history);
            XmlElement learning_record = xmlDoc.CreateElement("learning_record");
            learning_history.AppendChild(learning_record);
            learning_record.SetAttribute("level","");//本次練習程度
            learning_record.SetAttribute("startTime", "");
            learning_record.SetAttribute("endTime", "");
            learning_record.SetAttribute("duration", "");
            learning_record.SetAttribute("score", "");//本次練習分數


            /*-------對戰紀錄---------*/
            XmlElement compete_history = xmlDoc.CreateElement("compete_history");
            log_record.AppendChild(compete_history);
            XmlElement compete_record = xmlDoc.CreateElement("compete_record");
            compete_history.AppendChild(compete_record);
            compete_record.SetAttribute("level", "");
            compete_record.SetAttribute("startTime", "");//對戰開始時間
            compete_record.SetAttribute("endTime", "");
            compete_record.SetAttribute("duration", "");
            compete_record.SetAttribute("score", "");//本次對戰分數
            compete_record.SetAttribute("rank", "");//本次對戰排名

            /*-------當前對戰的回合紀錄---------*/
            XmlElement round_record = xmlDoc.CreateElement("round_record");
            compete_record.AppendChild(round_record);
            round_record.SetAttribute("ques_id", "");//題號
            round_record.SetAttribute("ans_state", "");//作答正確或錯誤
            round_record.SetAttribute("duration", "");//作答時間
            round_record.SetAttribute("rank", "");//當回合的排名

            /*-------排行榜更新紀錄---------*/
            XmlElement rank_history = xmlDoc.CreateElement("rank_history");
            log_record.AppendChild(rank_history);
            XmlElement rank_record = xmlDoc.CreateElement("rank_record");
            rank_history.AppendChild(rank_record);
            rank_record.SetAttribute("level", "");
            rank_record.SetAttribute("highscore", "");//目前最高分數
            rank_record.SetAttribute("rank", "");//目前排名
            rank_record.SetAttribute("updateTime", "");//刷新最高分數的時間

            /*-------點擊UI紀錄---------*/
            XmlElement touch_history = xmlDoc.CreateElement("touch_history");
            log_record.AppendChild(touch_history);
            XmlElement touch_rank = xmlDoc.CreateElement("touch_rank");//點擊排行榜的次數
            touch_history.AppendChild(touch_rank);
            touch_rank.SetAttribute("count", Strcount);

            /*-------場景載入與離開紀錄---------*/
            XmlElement scene_history = xmlDoc.CreateElement("scene_history");
            log_record.AppendChild(scene_history);
            XmlElement scene_record = xmlDoc.CreateElement("scene_record");
            scene_history.AppendChild(scene_record);
            scene_record.SetAttribute("scence", "");
            scene_record.SetAttribute("startTime", "");
            scene_record.SetAttribute("endTime", "");


            xmlDoc.Save(path+ filename);//存檔
		}
}