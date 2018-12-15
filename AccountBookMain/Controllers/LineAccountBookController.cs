using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AccountBookMain.Controllers
{
    public class LineAccountBookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/LineAccountBook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            try
            {
                //設定ChannelAccessToken(或不設定直接抓取Web.Config中key為ChannelAccessToken的AppSetting)
                //this.ChannelAccessToken = "!!!!! 改成自己的ChannelAccessToken !!!!!";
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                    responseMsg = $"你說了: {LineEvent.message.text}";
                else
                    responseMsg = $"收到 event : {LineEvent.type}";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage("!!!改成你的AdminUserId!!!", "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}
