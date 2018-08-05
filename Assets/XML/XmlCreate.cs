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
			xmlDoc.AppendChild(Loadfile);//root Node

			XmlElement User = xmlDoc.CreateElement("User");
			User.SetAttribute("ID", "");
			User.SetAttribute("name","");
			User.SetAttribute("level", "");
            User.SetAttribute("sex", "");
            User.SetAttribute("money", "100");
            Loadfile.AppendChild(User);


			XmlElement current_icon = xmlDoc.CreateElement("current_icon");//目前頭像//the same level with user
			current_icon.SetAttribute("icon","");
			User.AppendChild(current_icon);

            XmlElement compete = xmlDoc.CreateElement("compete");
            User.AppendChild(compete);
            XmlElement enemy = xmlDoc.CreateElement("enemy");
            compete.AppendChild(enemy);




        //20160221
        XmlElement learningtask  = xmlDoc.CreateElement("learningtask");
			User.AppendChild(learningtask);
			learningtask.SetAttribute("star_all_count",Strcount);//聽說次數的總和
			learningtask.SetAttribute("listen_all_count",Strcount);
			learningtask.SetAttribute("speak_all_count",Strcount);
			learningtask.SetAttribute("state_count", "1");	

				XmlElement task1 = xmlDoc.CreateElement("task1");
				learningtask.AppendChild(task1);
				task1.SetAttribute("time",""); //紀錄任務完成時間
				task1.SetAttribute("star",Strcount); //紀錄該單元的星數
		        task1.SetAttribute("listen_count_learn",Strcount);
				task1.SetAttribute("listen_count_practice",Strcount);
		        task1.SetAttribute("speak_count",Strcount);
				task1.SetAttribute("state", "1");
					
			XmlElement move_progress = xmlDoc.CreateElement("practice_move_progress");//紀錄各個任務每道題目的練習次數
			User.AppendChild(move_progress);
				XmlElement move_task1 = xmlDoc.CreateElement("task1");
				move_progress.AppendChild(move_task1);
				move_task1.SetAttribute("move_count_practice",Strcount);
				XmlElement move_task2 = xmlDoc.CreateElement("task2");
				move_progress.AppendChild(move_task2);
			    move_task2.SetAttribute("move_count_practice",Strcount);
				XmlElement move_task3 = xmlDoc.CreateElement("task3");
				move_progress.AppendChild(move_task3);
			    move_task3.SetAttribute("move_count_practice",Strcount);


			XmlElement speak_progress = xmlDoc.CreateElement("practice_speak_progress");//紀錄各個任務每道題目的口說次數
			User.AppendChild(speak_progress);
				XmlElement speak_task1 = xmlDoc.CreateElement("task1");
				speak_progress.AppendChild(speak_task1);
				speak_task1.SetAttribute("speak_count_practice",Strcount);
				XmlElement speak_task2 = xmlDoc.CreateElement("task2");
				speak_progress.AppendChild(speak_task2);
				speak_task2.SetAttribute("speak_count_practice",Strcount);
				XmlElement speak_task3 = xmlDoc.CreateElement("task3");
				speak_progress.AppendChild(speak_task3);
				speak_task3.SetAttribute("speak_count_practice",Strcount);

			
			XmlElement challenge_progress = xmlDoc.CreateElement("challenge_progress");//紀錄挑戰關卡的挑戰次數與最高分數
			User.AppendChild(challenge_progress);
			XmlElement ch_card = xmlDoc.CreateElement("challengeticket");
			challenge_progress.AppendChild(ch_card);
			ch_card.SetAttribute("count","1");
			ch_card.SetAttribute("use",Strcount);
				XmlElement ch_task1 = xmlDoc.CreateElement("task1");
				challenge_progress.AppendChild(ch_task1);
				ch_task1.SetAttribute("challenge_count",Strcount);
				ch_task1.SetAttribute("state",Strcount);
				ch_task1.SetAttribute("hight_score",Strcount);
				XmlElement ch_task2 = xmlDoc.CreateElement("task2");
				challenge_progress.AppendChild(ch_task2);				
				ch_task2.SetAttribute("challenge_count",Strcount);
				ch_task2.SetAttribute("state",Strcount);
				ch_task2.SetAttribute("hight_score",Strcount);
				XmlElement ch_task3 = xmlDoc.CreateElement("task3");
				challenge_progress.AppendChild(ch_task3);
				ch_task3.SetAttribute("challenge_count",Strcount);
				ch_task3.SetAttribute("state",Strcount);
				ch_task3.SetAttribute("hight_score",Strcount);	


			/*-- 獎章紀錄--*/
			/*
			XmlElement reward = xmlDoc.CreateElement("reward");
				reward.SetAttribute("copperTouchTimes", Strcount);
				reward.SetAttribute("total", Strcount);
				reward.SetAttribute("isEnable", "1");
				User.AppendChild(reward);
					XmlElement reward_learn = xmlDoc.CreateElement("reward_learn"); //reward badge learn 2016/02/18
					reward.AppendChild(reward_learn);
					reward_learn.SetAttribute("time","");
					reward_learn.SetAttribute("kind",Strcount);
					XmlElement reward_listen = xmlDoc.CreateElement("reward_listen"); //reward badge listen 2016/02/18
					reward.AppendChild(reward_listen);
					reward_listen.SetAttribute("time","");
					reward_listen.SetAttribute("kind",Strcount);
					XmlElement reward_speak = xmlDoc.CreateElement("reward_speak"); //reward badge speak 2016/02/18
					reward.AppendChild(reward_speak);
					reward_speak.SetAttribute("time","");
					reward_speak.SetAttribute("kind",Strcount);
					XmlElement reward_unlock = xmlDoc.CreateElement("reward_unlock"); //reward badge unlock 2016/02/18
					reward.AppendChild(reward_unlock);
					reward_unlock.SetAttribute("time","");
					reward_unlock.SetAttribute("kind",Strcount);
					XmlElement reward_challengecard = xmlDoc.CreateElement("reward_challengecard"); //reward badge challengecard 2016/02/18
					reward.AppendChild(reward_challengecard);
					reward_challengecard.SetAttribute("time","");
					reward_challengecard.SetAttribute("kind",Strcount);
					XmlElement reward_shop = xmlDoc.CreateElement("reward_shop"); //reward badge shop 2016/02/18
					reward.AppendChild(reward_shop);
					reward_shop.SetAttribute("time","");
					reward_shop.SetAttribute("kind",Strcount);					
					XmlElement star_listen = xmlDoc.CreateElement("star_listen"); //reward star listen 2016/02/18
					reward.AppendChild(star_listen);
				    star_listen.SetAttribute("time","");
					star_listen.SetAttribute("kind",Strcount);			
					XmlElement star_speak = xmlDoc.CreateElement("star_speak"); //reward star speak 2016/02/18
					reward.AppendChild(star_speak);
					star_speak.SetAttribute("time","");
					star_speak.SetAttribute("kind",Strcount);	
				*/	
			

			XmlElement goods = xmlDoc.CreateElement("goods_item"); //商店物品the same level with user
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

			XmlElement buy_list = xmlDoc.CreateElement("buy_list");//購物清單the same level with user
			Loadfile.AppendChild(buy_list);
			XmlElement show_list = xmlDoc.CreateElement("show_list");//顯示清單the same level with user
			Loadfile.AppendChild(show_list);

			XmlElement log_record = xmlDoc.CreateElement("log_record");
			Loadfile.AppendChild(log_record);
			XmlElement time_history = xmlDoc.CreateElement("time_history");//log記錄-登入時間與場景
			log_record.AppendChild(time_history);
				XmlElement time_history_day = xmlDoc.CreateElement("time_history_day");
				time_history.AppendChild(time_history_day);					
				time_history_day.SetAttribute("day", DateTime.Now.ToString("yyyy-MM-dd"));
				XmlElement time_history_record = xmlDoc.CreateElement("time_history_record");
				time_history_day.AppendChild(time_history_record);
				time_history_record.SetAttribute("scence", "login");
				time_history_record.SetAttribute("startTime", DateTime.Now.ToString("HH:mm:ss"));
				time_history_record.SetAttribute("duration", "");

			XmlElement touch = xmlDoc.CreateElement("touch");//log記錄-每個場景點擊次數
			log_record.AppendChild(touch);
				XmlElement touch_space = xmlDoc.CreateElement("touch_space");
				XmlElement touch_rank = xmlDoc.CreateElement("touch_rank");
				XmlElement touch_shop = xmlDoc.CreateElement("touch_shop");
				XmlElement touch_learningtask = xmlDoc.CreateElement("touch_learningtask");
					XmlElement touch_task1 = xmlDoc.CreateElement("touch_task1");
					XmlElement touch_task2 = xmlDoc.CreateElement("touch_task2");
					XmlElement touch_task3 = xmlDoc.CreateElement("touch_task3");
				XmlElement touch_reward = xmlDoc.CreateElement("touch_reward");
					XmlElement touch_reward_badge = xmlDoc.CreateElement("touch_reward_badge");
						XmlElement touch_badge_learn = xmlDoc.CreateElement("touch_badge_learn");
						XmlElement touch_badge_listen = xmlDoc.CreateElement("touch_badge_listen");
						XmlElement touch_badge_speak = xmlDoc.CreateElement("touch_badge_speak");
						XmlElement touch_badge_unlock = xmlDoc.CreateElement("touch_badge_unlock");
						XmlElement touch_badge_challengecard = xmlDoc.CreateElement("touch_badge_challengecard");
						XmlElement touch_badge_shop = xmlDoc.CreateElement("touch_badge_shop");
					XmlElement touch_reward_listen = xmlDoc.CreateElement("touch_reward_listen");
					XmlElement touch_reward_speak = xmlDoc.CreateElement("touch_reward_speak");
				XmlElement touch_ali = xmlDoc.CreateElement("touch_ali");
				touch.AppendChild(touch_space);
				touch.AppendChild(touch_rank);
				touch.AppendChild(touch_shop);
				touch.AppendChild(touch_learningtask);
					touch_learningtask.AppendChild(touch_task1);
					touch_learningtask.AppendChild(touch_task2);
					touch_learningtask.AppendChild(touch_task3);
				touch.AppendChild(touch_reward);
					touch_reward.AppendChild(touch_reward_badge);
						touch_reward_badge.AppendChild(touch_badge_learn);
						touch_reward_badge.AppendChild(touch_badge_listen);
						touch_reward_badge.AppendChild(touch_badge_speak);
						touch_reward_badge.AppendChild(touch_badge_unlock);
						touch_reward_badge.AppendChild(touch_badge_challengecard);
						touch_reward_badge.AppendChild(touch_badge_shop);
					touch_reward.AppendChild(touch_reward_listen);
					touch_reward.AppendChild(touch_reward_speak);
				touch.AppendChild(touch_ali);
				touch_space.SetAttribute("count", Strcount);
				touch_rank.SetAttribute("count", Strcount);
				touch_shop.SetAttribute("count", Strcount);
				touch_learningtask.SetAttribute("count", Strcount);
					touch_task1.SetAttribute("count",Strcount);
					touch_task2.SetAttribute("count",Strcount);
					touch_task3.SetAttribute("count",Strcount);
				touch_reward.SetAttribute("count", Strcount);
					touch_reward_badge.SetAttribute("count", Strcount);
						touch_badge_learn.SetAttribute("count", Strcount);
						touch_badge_listen.SetAttribute("count", Strcount);
						touch_badge_speak.SetAttribute("count", Strcount);
						touch_badge_unlock.SetAttribute("count", Strcount);
						touch_badge_challengecard.SetAttribute("count", Strcount);
						touch_badge_shop.SetAttribute("count", Strcount);
					touch_reward_listen.SetAttribute("count", Strcount);
					touch_reward_speak.SetAttribute("count", Strcount);
				touch_ali.SetAttribute("count", Strcount);

            xmlDoc.Save(path+ filename);//存檔
		}
}