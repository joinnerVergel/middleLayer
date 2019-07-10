using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace CapaMedia.Controllers
{
    public class ValuesController : ApiController
    {
        // POST api/values
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public JObject Post([FromBody]JObject value)
        {
            try
            {
                string url = value.GetValue("url").ToString();
                string postData = value.GetValue("objeto")!=null? value.GetValue("objeto").ToString():null;
                string metodo = value.GetValue("metodo")!=null? value.GetValue("metodo").ToString():null;
                string token = value.GetValue("token")!=null? value.GetValue("token").ToString():null;
                string responseFromServer = "";
                ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
                // Create a request using a URL that can receive a post.   
                WebRequest request = WebRequest.Create(url);
                // Set the ContentType property of the WebRequest.  
                
                request.ContentType = "application/json";
                
                if (token != null && token!="") {
                    //request.Headers.Add("Access-Control-Allow-Origin", "*");
                    //request.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                    request.Headers.Add("Authorization", "Bearer " + token);
                }

                if (metodo.ToUpper().Equals("GET"))
                {
                    request.Method = "GET";
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            responseFromServer = sr.ReadToEnd();
                        }
                    }
                }

                if (metodo.ToUpper().Equals("POST")) {
                    request.Method = "POST";
                    // Create POST data and convert it to a byte array.  
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentLength property of the WebRequest.  
                    request.ContentLength = byteArray.Length;

                    // Get the request stream.  
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.  
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.  
                    dataStream.Close();
                    // Get the response.  
                    WebResponse response = request.GetResponse();
                    // Display the status.  
                    Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                    // Get the stream containing content returned by the server.  
                    // The using block ensures the stream is automatically closed.
                    using (dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.  
                        StreamReader reader = new StreamReader(dataStream);
                        // Read the content.  
                        responseFromServer = reader.ReadToEnd();
                        // Display the content.  
                        Console.WriteLine(responseFromServer);
                    }

                    // Close the response.  
                    response.Close();
                }

                JObject json = JObject.Parse(responseFromServer);
                return json;
            }
            catch (Exception e)
            {
                return JObject.Parse("{\"error\":\""+e.Message+"\"}");
            }
            
            
            
        }
    }
}
