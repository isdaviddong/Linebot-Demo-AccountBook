using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBookMain
{
    public class Utility
    {


        /// <summary>
        /// 取得最常用type
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        static public List<string> GetTypes(string UserId)
        {
            List<string> items = new List<string>();

            var db = new Models.MainDBDataContext();
            var ret = (from c in db.AccountBooks
                       where c.UserId == UserId
                       orderby c.SpendDT descending
                       select c.SpendType).Distinct().Take(9); 
            if (ret.Count() <= 0)
            {
                //如果沒有資料
                items.Add("餐費");
                items.Add("交通費");
                items.Add("娛樂費");
                items.Add("服裝費");

                return items;
            }

            items = ret.ToList();

            if ((!items.Contains("餐費")))
                items.Add("餐費");
            if ((!items.Contains("交通費")))
                items.Add("交通費");
            if ((!items.Contains("娛樂費")))
                items.Add("娛樂費");
            if ((!items.Contains("服裝費")))
                items.Add("服裝費");

            return items;
        }

        static public float GetTodaySpend(string UserId)
        {
            var db = new Models.MainDBDataContext();
            var ret = from c in db.AccountBooks
                      where c.SpendDT.StartsWith(DateTime.Now.ToString("yyyy/MM/dd"))
                      select c.Amount;
            if (ret.Count() <= 0) return 0;
            var amount = ret.Sum();
            return (float)amount;
        }

        static public float GetThisMonthSpend(string UserId)
        {
            var db = new Models.MainDBDataContext();
            var ret = from c in db.AccountBooks
                      where c.SpendDT.StartsWith(DateTime.Now.ToString("yyyy/MM"))
                      select c.Amount;
            if (ret.Count() <= 0) return 0;
            var amount = ret.Sum();
            return (float)amount;
        }

        static public bool SaveToDB(string UserId, float num, string AccountType, string Memo)
        {
            var db = new Models.MainDBDataContext();
            var rec = new Models.AccountBook();
            rec.UserId = UserId;
            rec.Amount = (decimal)num;
            rec.SpendType = AccountType;
            rec.SpendDT = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            rec.Memo = Memo;
            db.AccountBooks.InsertOnSubmit(rec);
            db.SubmitChanges();
            return true;
        }

        static public void SetState(string Key, string Value)
        {
            System.Web.HttpContext.Current.Application[Key] = Value;
        }

        static public string GetState(string Key)
        {
            if (System.Web.HttpContext.Current.Application [Key] == null) return null;
            return System.Web.HttpContext.Current.Application[Key].ToString();
        }
    }
}