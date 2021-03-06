﻿using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using RestSharpDemo.Model;
using System.Threading.Tasks;
using System;

namespace RestSharpDemo {
    [TestFixture]
    public class UnitTest1 {
        [Test]
        public void TestMethod1() {
            var client = new RestClient("http://localhost:3000/");
            //will be passing post/1
            var request = new RestRequest("posts/{postid}", Method.GET);
            request.AddUrlSegment("postid", 1);
            var response = client.Execute(request);

            //below is without using Newtonsoft
            //var deserialise = new JsonDeserializer();
            //var output = deserialise.Deserialize<Dictionary<string, string>>(response);
            //var writer = output["author"];

            //using Newtonsoft
            JObject obs = JObject.Parse(response.Content);
            Assert.That(obs["author"].ToString(), Is.EqualTo("George BB"), "Author is not correct");
        }

        [Test]
        public void PostWithAnonymousBody() {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts/{postid}/profile", Method.POST);

            //Add Body to the request. Below commented out code no longer works as AddBody is deprecated
            //request.RequestFormat = DataFormat.Json;
            //request.AddBody(new { name = "Sam" });
            request.AddJsonBody(new { name = "Sam" });
            request.AddUrlSegment("postid", 1);

            //old way without using Newtonsoft library
            //var deserialise = new JsonDeserializer();
            //var output = deserialise.Deserialize<Dictionary<string, string>>(client.Execute(request));
            //var result = output["name"];
            //Assert.That(result, Is.EqualTo("Sam"), "Profile is not correct");

            //below is using Newtonsoft
            JObject result = JObject.Parse(client.Execute(request).Content);
            Assert.That(result["name"].ToString(), Is.EqualTo("Sam"), "Profile is not correct");
        }

        [Test]
        public void PostWithTypeClassBody() {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);

            //Add Body to the request. Below commented out code no longer works as AddBody is deprecated
            //request.RequestFormat = DataFormat.Json;
            //request.AddBody(new { name = "Sam" });
            request.AddJsonBody(new Posts() { id = "20", author = "Execute Automation", title = "RestSharp Demo Course" });

            //old way without using Newtonsoft library
            //pass a Posts model class to automatically deserialise the value
            var response = client.Execute<Posts>(request);
            //var deserialise = new JsonDeserializer();
            //var output = deserialise.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];
            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "Author is not correct");

            //below is using Newtonsoft
            //JObject result = JObject.Parse(client.Execute<Posts>(request).Content);
            //Assert.That(result["author"].ToString(), Is.EqualTo("Execute Automation"), "Profile is not correct");
        }


        [Test]
        public void PostWithAsync() {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);

            //Add Body to the request. Below commented out code no longer works as AddBody is deprecated
            //request.RequestFormat = DataFormat.Json;
            //request.AddBody(new { name = "Sam" });
            request.AddJsonBody(new Posts() { id = "21", author = "Execute Automation", title = "RestSharp Demo Course" });

            //old way without using Newtonsoft library
            //pass a Posts model class to automatically deserialise the value
            //var response = client.Execute<Posts>(request);
            var response = ExecuteAsyncRequest<Posts>(client, request).GetAwaiter().GetResult();
            //var deserialise = new JsonDeserializer();
            //var output = deserialise.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];
            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "Author is not correct");

            //below is using Newtonsoft
            //JObject result = JObject.Parse(client.Execute<Posts>(request).Content);
            //Assert.That(result["author"].ToString(), Is.EqualTo("Execute Automation"), "Profile is not correct");
        }

        private async Task <IRestResponse<T>> ExecuteAsyncRequest<T> (RestClient client, IRestRequest request) where T: class, new() {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, restResponse => {
                if (restResponse.ErrorException != null) {
                    const string message = "Error retrieving response.";
                    throw new ApplicationException(message, restResponse.ErrorException);
                }
                taskCompletionSource.SetResult(restResponse);
            });

            return await taskCompletionSource.Task;

        }
    }
}
