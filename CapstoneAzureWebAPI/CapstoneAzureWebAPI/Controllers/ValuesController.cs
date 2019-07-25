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
using CapstoneAzureWebAPI.Models;

namespace CapstoneAzureWebAPI.Controllers
{
    [EnableCors(origins: "https://angularcapstoneproject.azurewebsites.net", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
        [HttpPost]
        //[Route("api/Values/CreateVM")]
       
        // POST api/values
        public HttpWebResponse CreateVM([FromBody]VirtualMachine vm)
        {
            //HttpWebResponse response;
            string s_Id = vm.subscriptionId;
            string rg_Name = vm.resourcegroupName;
            string vm_Name = vm.virtualmachineName;
            string api_Version = vm.apiVersion;


            //if ()
            //{
            //    //return response.Close();


            //    //response.Headers.Add("X-Custom-Header", "hello");

            //}
            return Request_login_microsoftonline_com(s_Id, rg_Name, vm_Name, api_Version);
        }
    

        // PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
        //[Route("api/values/makerequest")]
        public void Get()
        {
           
        }

        
        public HttpWebResponse Request_login_microsoftonline_com(string s_Id, string rg_Name, string vm_Name, string api_Version)
        {
            HttpWebResponse response = null;
            string getreaderjson = string.Empty;
            string token = string.Empty;
            HttpWebResponse res = null ;
            /*FOLLOWING IS THE CODE TO REQUEST BEARER TOKEN*/


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://login.microsoftonline.com/6f859493-8c64-4e94-a9bd-3b9529964f8c/oauth2/token");

                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("authorization", "Bearer <access_token>");

                request.Method = "POST";
                //request.ServicePoint.Expect100Continue = false;

                string body = @"grant_type=client_credentials
                                &client_id=ec3705e7-8eb6-49cf-9094-e398a554d14a
                                &client_secret=d443d04f-ddfd-4f06-905c-09f8c8c55052
                                &resource=https://management.azure.com/";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    getreaderjson = reader.ReadToEnd();
                }
                var obj = JObject.Parse(getreaderjson);
                 token = (string)obj.Last;

                /*FOLLOWING IS THE CODE TO CREATE VIRTUAL MACHINE*/

                res=MakeRequestsToCreateVM(token, s_Id, rg_Name, vm_Name, api_Version);


            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return response;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return response;
            }

            return res;
        }
       
        private HttpWebResponse MakeRequestsToCreateVM(string token, string s_Id, string rg_Name, string vm_Name, string api_Version)
        {
            //HttpWebResponse response;

            //if ()
            //{
            //    response.Close();
            //}
            return Request_management_azure_com(token, s_Id, rg_Name, vm_Name, api_Version);
        }
        
        private HttpWebResponse Request_management_azure_com(string token, string s_Id, string rg_Name, string vm_Name, string api_Version)
        {
            HttpWebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://management.azure.com/subscriptions/"+s_Id+"/resourceGroups/"+rg_Name+"/providers/Microsoft.Compute/virtualMachines/"+vm_Name+"?api-version="+api_Version);

                request.Headers.Set(HttpRequestHeader.Authorization, "Bearer "+token);
                request.ContentType = "application/json";
                request.Method = "PUT";

                string body = @"{
                                  ""location"": ""southindia"",
                                  ""properties"": {
                                    ""hardwareProfile"": {
                                      ""vmSize"": ""Standard_D12_v2""
                                    },
                                    ""storageProfile"": {
                                      ""imageReference"": {
                                        ""sku"": ""2016-Datacenter"",
                                        ""publisher"": ""MicrosoftWindowsServer"",
                                        ""version"": ""latest"",
                                        ""offer"": ""WindowsServer""
                                      },
                                      ""osDisk"": {
                                        ""caching"": ""ReadWrite"",
                                        ""managedDisk"": {
                                          ""storageAccountType"": ""Standard_LRS""
                                        },
                                        ""name"": ""myVMosdisk"",
                                        ""createOption"": ""FromImage""
                                      }
                                    },
                                    ""osProfile"": {
                                      ""adminUsername"": ""azvcmsfs"",
                                      ""computerName"": ""myVM"",
                                      ""adminPassword"": ""azvcmsfs22#$#$""
                                    },
                                    ""networkProfile"": {
                                      ""networkInterfaces"": [
                                        {
                                          ""id"": ""/subscriptions/8d7fc01f-bcea-4354-8ff6-ce2d070b2ac5/resourceGroups/azvcmsfsrg/providers/Microsoft.Network/networkInterfaces/azvcnic"",
                                          ""properties"": {
                                            ""primary"": true
                                          }
                                        }
                                      ]
                                    }
                                  }
                                }";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return response;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return response;
            }

            return response;
        }
    }
}
