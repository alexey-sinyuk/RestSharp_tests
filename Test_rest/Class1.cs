using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test_rest
{
    [TestFixture]
    public class Tests
    {

        public string url = "http://172.30.23.9:8080/ServletToDoList/";
        public RestClient client;
        public CookieContainer cookieJar = new CookieContainer();
        public string username = "asinuk";
        public string password = "P@ssw0rd";
        public string listname = "NewList_" + new Random().Next();
        public string date = "2016-01-01";

        [SetUp]
        public void Setup()
        {
            client = new RestClient(url);
            client.CookieContainer = cookieJar;
            var request = new RestRequest("Login", Method.POST);            
            string body = string.Format("name={0}&password={1}", username, password);
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            client.Execute(request);            
        }

        [Test]
        public void GetLists()
        {   
            var request = new RestRequest("GetLists", Method.GET);
            request.AddHeader("Accept", "application/json");
            var queryResult = client.Execute(request);            
            //Verify status code
            Assert.AreEqual(200, (int)queryResult.StatusCode);
            //Verify headers
            Assert.AreEqual("application/json", queryResult.Headers.ToList()
                .Find(x => x.Name == "Content-Type")
                .Value.ToString());
            //Verify body            
            Assert.IsTrue(queryResult.Content.Contains("{\"" + username + "\""));
        }

        [Test]
        public void CreateList()
        {
            var request = new RestRequest("CreateList", Method.POST);
            request.AddHeader("Content-Type", "applicatin/json");
            string body = string.Format("{{\"{0}\":{{\"{1}\":\"{2}\"}}}}", username, listname, date);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            //request.AddBody(body);
            var queryResult = client.Execute(request);
            Console.WriteLine(queryResult.Content);
            //Verify status code
            Assert.AreEqual(201, (int)queryResult.StatusCode);
            //Verify headers
            Assert.NotNull(queryResult.Headers.ToList()
                .Find(x => x.Name == "Content-Length"));
            //Verify body
            Assert.AreEqual(body, queryResult.Content);
        }

        [Test]
        public void DeleteList()
        {
            var request = new RestRequest("DeleteList", Method.POST);
            string query = string.Format("{0}:{1}:{2}",username, listname, date);            
            request.AddQueryParameter("tasks", query);
            var queryResult = client.Execute(request);
            //Verify status code
            Assert.AreEqual(200, (int)queryResult.StatusCode);
            Console.WriteLine(queryResult.Content);
        }

    }
}
