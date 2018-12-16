using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualBasic;

namespace AccountBookMain.Controllers
{
    public class LineAccountBookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/LineAccountBook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            //設定ChannelAccessToken(或不設定直接抓取Web.Config中key為ChannelAccessToken的AppSetting)
            //this.ChannelAccessToken = "!!!!! 改成自己的ChannelAccessToken !!!!!";
            //取得Line Event
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
            try
            {
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                var responseMsg = "";
                //如果是純文字訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    //如果是特殊命令，就執行
                    if (!ProcessCommand(LineEvent))
                        //如果不是，就視為一般文字輸入
                        responseMsg = ProcessText(LineEvent);
                }
                else if (LineEvent.type.ToLower() == "postback")
                    responseMsg = ProcessPostback(LineEvent);
                else
                    responseMsg = $"收到 event : {LineEvent.type}";

                //回覆訊息
                if (!string.IsNullOrEmpty(responseMsg))
                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

        private bool ProcessCommand(isRock.LineBot.Event e)
        {
            var msg = e.message.text.Trim();

            if (msg.ToLower() == "/today")
            {
                //今日花費
                var n = Utility.GetTodaySpend(e.source.userId);
                //回覆QuickReply
                isRock.LineBot.Bot b = new isRock.LineBot.Bot();
                isRock.LineBot.TextMessage TextMessage = new isRock.LineBot.TextMessage(
                    $"您今天總花費金額為 ${n}");
                b.ReplyMessage(e.replyToken, TextMessage);
                return true;
            }


            if (msg.ToLower() == "/month")
            {
                //本月花費
                var n = Utility.GetThisMonthSpend(e.source.userId);
                //回覆QuickReply
                isRock.LineBot.Bot b = new isRock.LineBot.Bot();
                isRock.LineBot.TextMessage TextMessage = new isRock.LineBot.TextMessage(
                    $"您本月總花費金額為 ${n}");
                b.ReplyMessage(e.replyToken, TextMessage);
                return true;
            }

            return false;
         }
        private string ProcessText(isRock.LineBot.Event e)
        {
            const  string HelpMsg = "嗨，您好。\n如果希望我幫您記帳，你可以直接輸入金額唷...\n\n例如:\n 180 \n 或\n 180 麥當勞\n然後再選取或輸入分類即可。"; 

            //取得User Id
            var UserId = e.source.userId;
            var msg = e.message.text;
            //如果沒有狀態
            if (string.IsNullOrEmpty(Utility.GetState(UserId)))
            {
                //預期msg是金額數字
                //去除空白和不必要的字
                msg = msg.Replace("$", "").Replace("元", "").Trim();
                //全形轉半形
                //msg = Strings.StrConv(msg, VbStrConv.Narrow, 0);
                //如果當前沒有狀態，預期的是純數字
                //先以 ' ' 分割
                var item = msg.Split(' ');
                if (item.Length <= 0) return HelpMsg;
                float num = 0;
                string memo = "";
                if (float.TryParse(item[0], out num) == false)
                    return HelpMsg;
                if (item.Length == 2) memo = item[1].Trim();

                //如果資料沒問題, 保存起來
                Utility.SetState(UserId + "-Amount", num.ToString());
                Utility.SetState(UserId + "-Memo", memo);

                //回覆QuickReply
                isRock.LineBot.Bot b = new isRock.LineBot.Bot();
                isRock.LineBot.TextMessage TextMessage = new isRock.LineBot.TextMessage($"請選擇或直接輸入這筆金額'{num}'的記帳類別");

                var Types = Utility.GetTypes(UserId);
                foreach (var type in Types)
                {
                    TextMessage.quickReply.items.Add(new isRock.LineBot.QuickReplyMessageAction(type, type));
                }
                b.ReplyMessage(e.replyToken, TextMessage);
                //設定狀態
                Utility.SetState(UserId, "等待分類名稱");
                return "";
            }
            //如果有狀態，且為正等待分類名稱
            if (Utility.GetState(UserId) == "等待分類名稱")
            {
                //取回應該有的數字
                if (string.IsNullOrEmpty(Utility.GetState(UserId + "-Amount")))
                {
                    Utility.SetState(UserId, "");
                    return HelpMsg;
                }
                //取回數字
                float num = 0;
                if (float.TryParse(Utility.GetState(UserId + "-Amount"), out num) == false)
                    return HelpMsg;
                string memo = Utility.GetState(UserId + "-Memo");
             
                //有了數字num以及分類名稱
                var AccountType = msg.Trim();
                //清空狀態
                Utility.SetState(UserId, "");
                //紀錄
                if (Utility.SaveToDB(UserId, num, AccountType, memo))
                    return $"${num} 已記錄為 {AccountType}";
                else
                    return "記錄失敗";
            }
            return "";
        }
        private string ProcessPostback(isRock.LineBot.Event e)
        {
            return "";
        }
    }
}
