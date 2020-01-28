using System.Collections.Generic;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using Newtonsoft.Json.Linq;

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

            //using Newtonsoft
            JObject obs = JObject.Parse(response.Content);

            //below is without using Newtonsoft
            //var deserialise = new JsonDeserializer();
            //var output = deserialise.Deserialize<Dictionary<string, string>>(response);
            //var writer = output["author"];

            Assert.That(obs["author"].ToString(), Is.EqualTo("George BB"), "Author is not correct");
        }
    }
}
