using NUnit.Framework;
using NUnitTestProject2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NUnitTestProject2.Tests
{
    public class TwitterAPI
    {
        [OneTimeSetUp]
        public void Setup()
        {
            //To support validation for the tests storing the validation details in a flat file as a one time setup, just for testing sake
            Utils.validationsetupdatageneration();
        }

        // Test to get the tweet content and validate the content
        [Test]
        public void TestTweetContent()
        {
            Dictionary<string, string> requestparam = new Dictionary<string, string>{ { "id", "1257326183101980673" } };
            string content = Utils.TwitterRequest("GET", "show", requestparam);
            var tweetContent = Utils.GetTweetContent(content);
            List<string> filecontent = File.ReadLines(@"C:\TwitterAPIAutomation\RunValidation.txt").ToList();
            Utils.Verify(((string)tweetContent["text"]).Contains(filecontent[0]), "Validate tweet content");
        }

        // Test to get the number of retweets and validate the count
        [Test]
        public void TestNumberOfReTweets()
        {
            Dictionary<string, string> requestparam = new Dictionary<string, string> { { "id", "1257326183101980673" } };
            string content = Utils.TwitterRequest("GET", "show", requestparam);
            var tweetContent = Utils.GetTweetContent(content);
            List<string> filecontent = File.ReadLines(@"C:\TwitterAPIAutomation\RunValidation.txt").ToList();
            Utils.Verify((string)tweetContent["retweet_count"] == filecontent[1], "Validate number of retweets");
        }

        // Test to get the retweeters ids and then validate the list of retweeter ids
        [Test]
        public void TestRetweetersIDs()
        {
            Dictionary<string, string> requestparam = new Dictionary<string, string> { { "id", "1257326183101980673" }, { "count", "100"} };
            string content = Utils.TwitterRequest("GET", "retweets", requestparam);
            List<string> retweeterslist = Utils.GetRetweetIDs(content);
            List<string> filecontent = File.ReadLines(@"C:\TwitterAPIAutomation\RunValidation.txt").ToList();
            filecontent.RemoveAt(0); // removing tweet content
            filecontent.RemoveAt(0); // removing retweet count
            bool result = true;
            Utils.Verify(filecontent.Count == retweeterslist.Count, "Validate the number of retweets");
            foreach (var retweeter in filecontent)
            {
                if (!retweeterslist.Contains(retweeter))
                    result = false;
            }
            Utils.Verify(result, "Validate the retweeters ids in the file matches the content obtained from API");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            //Deleting the file contents used for test validation, this is just for testing purpose
            Utils.clearfilesetup();
        }
    }
}
