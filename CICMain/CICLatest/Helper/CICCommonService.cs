using CICLatest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace CICLatest.Helper
{
    public class CICCommonService
    {
        private readonly UserManager<UserModel> _userManager;
        public CICCommonService(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        public bool removeSession(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
               

            }
            return false;
        }

        public void sendSMS(string emailId, string body)
        {
            string PhoneNumber = "";
            var user =  _userManager.FindByEmailAsync(emailId);
            if (user != null)
            {
                PhoneNumber = user.Result.PhoneNumber;
            }
                
            var c = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("from", "CIC"),
                        new KeyValuePair<string, string>("body", body),
                        new KeyValuePair<string, string>("touser", PhoneNumber),

                    });

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.PostAsync("https://ciccommunicationapi.azurewebsites.net/api/Sms?from=CIC&body= " + body + "&touser=" + PhoneNumber, c).Result;
                response.EnsureSuccessStatusCode();

            }
        }

        public string GetFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString().Substring(2);
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "/" + NexYear;
            else
                FinYear = PreYear + "/" + CurYear;

            return FinYear.Trim();
        }

    }
}
