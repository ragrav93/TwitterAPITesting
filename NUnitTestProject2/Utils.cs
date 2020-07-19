using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NUnitTestProject2
{
    static class Utils
    {
        /// <summary>
        /// Returns the response content for the url
        /// </summary>
        /// <param name="url">The API url request to retireve a specific information</param>
        /// <param name="action">action whether it is GET, POST, PUT or DELETE</param>
        /// <returns>response for the url hit</returns>
        public static string ResponseToUrl(string url, string action)
        {
            WebRequest request = WebRequest.Create(url);
            //The below authorization can be switch cased if there is requirement to include Basic or Oauth authorization
            //When you are running the suite replace the below line with your bearer token
            request.Headers["Authorization"] = "Bearer " + "use your bearer token";
            request.Method = action;
            WebResponse response = request.GetResponse();
            Stream receive_stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(receive_stream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            return content;
        }

        /// <summary>
        /// Returns the full url based on route and query parameters
        /// </summary>
        /// <param name="action">action whether it is GET, POST, PUT or DELETE</param>
        /// <param name="route">route for the specific action</param>
        /// <param name="queryparam">query parameters for the specific action</param>
        /// <param name="mainurl">Optional main url</param>
        /// <returns>Returns full url with route and query parameters</returns>
        public static string TwitterRequest(string action, string route, Dictionary<string, string> queryparam, string mainurl = "https://api.twitter.com/1.1/statuses")
        {
            string fullurl = $"{mainurl}/{route}/{queryparam["id"]}.json";
            if(queryparam.Keys.Count > 1)
            {
                fullurl = fullurl + "?";
                foreach (var keys in queryparam.Keys)
                {
                    if (keys == "id")
                        continue;
                    fullurl = fullurl + $"{keys}={queryparam[keys]}&";
                }
                fullurl = fullurl.TrimEnd('&');
            }
            return ResponseToUrl(fullurl, action);
        }

        /// <summary>
        /// Returns the tweet content after parsing the response
        /// </summary>
        /// <param name="content">Raw reponse content</param>
        /// <returns>returns the parsed repsonse content as Hashtable</returns>
        //This method can be expanded as the required ones from tweet content expands
        public static Hashtable GetTweetContent(string content)
        {
            Hashtable tweetcontent = new Hashtable();
            var rawtweetContent =  JObject.Parse(content);
            tweetcontent.Add("text", (string)rawtweetContent["text"]);
            tweetcontent.Add("retweet_count", (string)rawtweetContent["retweet_count"]);
            return tweetcontent;
        }

        /// <summary>
        /// Returs the retweeters ids as a list after parsing the response 
        /// </summary>
        /// <param name="content">Raw reponse content</param>
        /// <returns>List of retweeters ids</returns>
        //This method can be expanded as the required ones from retweet content expands
        public static List<string> GetRetweetIDs(string content)
        {
            var rawretweetcontent = JArray.Parse(content);
            List<string> retweetlist = new List<string>();
            foreach (var retweet in rawretweetcontent)
            {
                retweetlist.Add((string)retweet["user"]["screen_name"]);
            }
            return retweetlist;
        }

        /// <summary>
        /// Assert the test based on the result of the validation
        /// </summary>
        /// <param name="result">boolean after validation</param>
        /// <param name="message">message for the corresponding test case</param>
        public static void Verify(bool result, string message)
        {
            if (result)
                Console.WriteLine("Passed:" + message);
            else
            {
                Console.WriteLine("Failed:" + message);
                Assert.Fail();
            }     
        }

        /// <summary>
        /// Setting up the file content which will be used for validation, just for testing sake
        /// </summary>
        public static void validationsetupdatageneration()
        {
            clearfilesetup(); //Safely delete in case tear down failed during previous run
            var filepath = @"C:\TwitterAPIAutomation";
            Directory.CreateDirectory(filepath);
            Dictionary<string, string> requestparam = new Dictionary<string, string> { { "id", "1257326183101980673" } };
            string content = Utils.TwitterRequest("GET", "show", requestparam);
            var tweetContent = Utils.GetTweetContent(content);
            File.AppendAllText(@"C:\TwitterAPIAutomation\RunValidation.txt", (string)tweetContent["text"] + "\n");
            File.AppendAllText(@"C:\TwitterAPIAutomation\RunValidation.txt", (string)tweetContent["retweet_count"] + "\n");
            requestparam.Add("count", "100");
            string retweetContent = Utils.TwitterRequest("GET" ,"retweets", requestparam);
            List<string> retweeterslist = Utils.GetRetweetIDs(retweetContent);
            foreach (var retweeter in retweeterslist)
            {
                File.AppendAllText(@"C:\TwitterAPIAutomation\RunValidation.txt", retweeter + "\n");
            }
        }

        /// <summary>
        /// Clears the validation file after entire run suite, just for testing sake
        /// </summary>
        public static void clearfilesetup()
        {
            if (File.Exists(@"C:\TwitterAPIAutomation\RunValidation.txt"))
                File.Delete(@"C:\TwitterAPIAutomation\RunValidation.txt");
        }
    }
}
