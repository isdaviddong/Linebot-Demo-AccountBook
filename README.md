Linebot-Demo-AccountBook
===

此Line bot範例為使用 LineBotSDK 建立 <br>
電子記帳本 <br>
用戶可以跟 bot 說 <br>
87 </B><br>
或 <br>
87 麥當勞 <br>
bot就會將此筆消費紀錄儲存起來 <br>
用戶可以說 /month 或 /today 取得 消費本月或今天的金額小記 <br>

如何使用
===
* 請 clone 之後，修改 web.config 中的 ChannelAccessToken
```xml
  <appSettings>
    <add key="ChannelAccessToken" value="請改成你自己的channel access token"/>
  </appSettings>
```
* 為了便於除錯，請修改 CopyCatController.cs 中的 Admin User Id
```csharp
   catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage("請改成你自己的Admin User Id", "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
```
* 建議使用Ngrok進行測試 <br/>
(可參考 https://youtu.be/kCga1_E-ijs ) 
* LINE Bot後台的WebHook設定，其位置為 Http://你的domain/api/LineAccountBook

資料庫
===
* 為了符合最低開發需求，此範例資料庫採用 SQL Express (已commit進去專案中) 以及 LinqToSql
* 開發環境需要安裝 SQL Express, 若要佈署至 Azure Web Site (或自行搭建的IIS)，需要自行改為使用Azure SQL DB
* 如果你的Visual Studio沒有安裝LinqToSQL，可以參考底下的畫面進行調整:
 ![](https://i.imgur.com/ew6acqd.png)
 

畫面
===
![](https://i.imgur.com/DKvVs4A.png)

線上課程
===
LineBotSDK線上教學課程: <br/>
https://www.udemy.com/line-bot <br/>