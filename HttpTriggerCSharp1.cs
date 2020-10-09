using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Twilio.TwiML;
using System.Net.Http;




namespace Company.Function
{
    

    public static class HttpTriggerCSharp1
    
    {
        [FunctionName("HttpTriggerCSharp1")]
        
                public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //var data = await req.Content.ReadAsStringAsync();
            var data = await new StreamReader(req.Body).ReadToEndAsync();
            var formValues = data.Split('&')
                .Select(value => value.Split('='))
                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "), 
                      pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));

    // Perform calculations, API lookups, etc. here

            var quest = formValues["Body"];
            log.LogInformation(quest); 

    ////////////////////////////

        var client = new HttpClient();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("question", quest);

        string json = JsonConvert.SerializeObject(dictionary);
        var requestData = new StringContent(json, Encoding.UTF8, "application/json");
        var url = "https://prod-81.eastus.logic.azure.com:443/workflows/b807d77ea57847669090f30f68d226a2/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=xxxx";
        var qaresponse = await client.PostAsync(String.Format(url), requestData);
        var result = await qaresponse.Content.ReadAsStringAsync();




    //////////////////////////////////



            log.LogInformation($"The message is {formValues["Body"]}");

            var response = new MessagingResponse()
              .Message(result);
            var twiml = response.ToString();
            twiml = twiml.Replace("utf-16", "utf-8");

            // var resp = new OkObjectResult(twiml);
 
            return new ContentResult
            {
                ContentType = "application/xml",
                Content = twiml,
                StatusCode = 200
            };
            // return new OkObjectResult(twiml).ContentTypes("application/xml");

            
        }
    }




    
}

