using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CICLatest.Helper
{
    public class AzureTablesData
    {
        //public static int GetAllEntitiesWithContToken()
        //{
        //    var acc = new CloudStorageAccount(
        //                 new StorageCredentials("account name", "account key"), true);
        //    var tableClient = acc.CreateCloudTableClient();
        //    var table = tableClient.GetTableReference("table name");
        //    TableContinuationToken token = null;
        //    var entities = new List<object>();
        //    do
        //    {
        //        var queryResult = table.ExecuteQuerySegmentedAsync(new TableQ uery<>(), token);
        //        entities.AddRange(queryResult.Results);
        //        token = queryResult.ContinuationToken;
        //    } while (token != null);
        //}
        public static EntityResponse GetAllEntityWithContinuationToken(string storageAccount, string accessKey, string resourcePath)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath;
            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            var entityResponse = new EntityResponse
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = string.Empty,
                Message = string.Empty,
                NextPartitionKey = string.Empty,
                NextRowKey = string.Empty
            };

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    entityResponse.NextPartitionKey = response.Headers["x-ms-continuation-NextPartitionKey"];
                    entityResponse.NextRowKey = response.Headers["x-ms-continuation-NextRowKey"];
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        entityResponse.Data = r.ReadToEnd();
                        return entityResponse;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    entityResponse.Data = sr.ReadToEnd();
                    // Log res if required
                }

                return entityResponse;
            }
        }

        public static int GetAllEntity(string storageAccount, string accessKey, string resourcePath, out string jsonData)
        {
            //string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=RowKey%20eq%20" + "'form2'";
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath;
            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntity(string storageAccount, string accessKey, string resourcePath, string rowKey, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=RowKey%20eq%20'" + rowKey + "'";
            
            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyBusinessName(string storageAccount, string accessKey, string resourcePath, string businessName, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=BusinessName%20eq%20'" + businessName + "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyCreatedBy(string storageAccount, string accessKey, string resourcePath, string createdBy, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CreatedBy%20eq%20'" + createdBy + "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyRowPartition(string storageAccount, string accessKey, string resourcePath, string partitionkey, string rowkey, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=PartitionKey%20eq%20'" + partitionkey + "'%20and%20RowKey%20eq%20'" + rowkey+ "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static EntityResponse GetEntitybyNextRowPartition(string storageAccount, string accessKey, string resourcePath, string partitionkey, string rowkey)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?NextPartitionKey=" + partitionkey + "&NextRowKey=" + rowkey;

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            var entityResponse = new EntityResponse
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = string.Empty,
                Message = string.Empty,
                NextPartitionKey = string.Empty,
                NextRowKey = string.Empty
            };
            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    entityResponse.NextPartitionKey = response.Headers["x-ms-continuation-NextPartitionKey"];
                    entityResponse.NextRowKey = response.Headers["x-ms-continuation-NextRowKey"];
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        entityResponse.Data = r.ReadToEnd();
                        return entityResponse;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    entityResponse.Data = sr.ReadToEnd();
                    // Log res if required
                }

                return entityResponse;
            }
        }
        public static int GetEntitybyLoginId(string storageAccount, string accessKey, string resourcePath, string CreatedBy, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CreatedBy%20eq%20'" + CreatedBy + "'%20and%20CreatedDate%20gt%20''";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyLoginIdwithForm(string storageAccount, string accessKey, string resourcePath, string CreatedBy,string formname, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CreatedBy%20eq%20'" + CreatedBy + "'%20and%20CreatedDate%20gt%20''%20and%20FormName%20eq%20'" + formname + "'" ;

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetSubContractors(string storageAccount, string accessKey, string resourcePath, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CertificateNo%20gt%20''";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyCertificate(string storageAccount, string accessKey, string resourcePath, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CertificateNo%20gt%20''";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetSubcontractDetailsJson(string storageAccount, string accessKey, string resourcePath, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CertificateNo%20eq%20''";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }

        public static int GetEntitybyCertificateJson(string storageAccount, string accessKey, string resourcePath, string certno, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=CertificateNo%20eq%20'" + certno + "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }
        public static string InsertEntity(string storageAccount, string accessKey, string tableName, string jsonData)
        {
            string jsonResponse = "";
            string host = string.Format(@"https://{0}.table.core.windows.net/", storageAccount);

            string resource = string.Format(@"{0}", tableName);
            string uri = host + resource;
        
            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request = getRequestHeaders("POST", request, storageAccount, accessKey, resource, jsonData.Length);


            // Write Entity's JSON data into the request body
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        r.ReadToEnd();
                        jsonResponse =Convert.ToString(response.StatusCode);
                        //return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonResponse = sr.ReadToEnd();
                    // Log res if required
                }

                // return (int)ex.Status;
            }

            return jsonResponse;
        }

        public static HttpWebRequest getRequestHeaders(string requestType, HttpWebRequest Newrequest, string storageAccount, string accessKey, string resource, int Length = 0)
        {
            HttpWebRequest request = Newrequest;

            switch (requestType.ToUpper())
            {
                case "GET":
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.ContentLength = Length;
                    request.Accept = "application/json;odata=nometadata";
                    request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2015-04-05");
                    request.Headers.Add("Accept-Charset", "UTF-8");
                    request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
                    request.Headers.Add("DataServiceVersion", "1.0;NetFx");
                    break;
                case "POST":
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = Length;
                    request.Accept = "application/json;odata=nometadata";
                    request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2015-04-05");
                    request.Headers.Add("Accept-Charset", "UTF-8");
                    request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
                    request.Headers.Add("DataServiceVersion", "1.0;NetFx");
                    break;
                case "PUT":
                    request.Method = "PUT";
                    request.ContentLength = Length;
                    request.ContentType = "application/json";
                    request.Accept = "application/json;odata=nometadata";
                    request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2015-04-05");
                    //request.Headers.Add("If-Match", "*");
                    request.Headers.Add("Accept-Charset", "UTF-8");
                    request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
                    request.Headers.Add("DataServiceVersion", "1.0;NetFx");
                    break;
                case "DELETE":
                    request.Method = "DELETE";
                    request.ContentType = "application/json";
                    request.Accept = "application/json;odata=nometadata";
                    request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
                    request.Headers.Add("x-ms-version", "2015-04-05");
                    request.Headers.Add("If-Match", "*");
                    request.Headers.Add("Accept-Charset", "UTF-8");
                    request.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
                    request.Headers.Add("DataServiceVersion", "1.0;NetFx");
                    break;
            }

            string sAuthorization = getAuthToken(request, storageAccount, accessKey, resource);
            request.Headers.Add("Authorization", sAuthorization);
            return request;
        }

        public static string getAuthToken(HttpWebRequest request, string storageAccount, string accessKey, string resource)
        {
            try
            {
                string sAuthTokn = "";

                string stringToSign = request.Headers["x-ms-date"] + "\n";

                stringToSign += "/" + storageAccount + "/" + resource;

                HMACSHA256 hasher = new HMACSHA256(Convert.FromBase64String(accessKey));

                sAuthTokn = "SharedKeyLite " + storageAccount + ":" + Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

                return sAuthTokn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string UpdateEntity(string storageAccount, string accessKey, string tableName, string jsonData, string partitionKey, string RowKey)
        {            
            string jsonResponse = "";
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + tableName + "(PartitionKey='" + partitionKey + "',RowKey='" + RowKey +"')";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            req.Method = "PUT";
            string mxdate = DateTime.UtcNow.ToString("R");
            string storageServiceVersion = "2015-12-11";
            //specify request header

            var StorageAccountName = storageAccount;
            var StorageKey = accessKey;
            var requestUri = new Uri(uri);
            var canonicalizedStringToBuild = string.Format("{0}\n{1}", mxdate, $"/{StorageAccountName}/{requestUri.AbsolutePath.TrimStart('/')}");
            string signature;
            using (var hmac = new HMACSHA256(Convert.FromBase64String(StorageKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
                signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
            }

            string authorizationHeader = string.Format($"{StorageAccountName}:" + signature);
            String authorization = String.Format("{0} {1}", "SharedKeyLite", authorizationHeader);
            req.Headers.Add("Authorization", authorization);
            req.Headers.Add("x-ms-date", mxdate);
            req.Headers.Add("x-ms-version", storageServiceVersion);
            req.ContentType = "application/json";
            req.Headers.Add("Accept-Charset", "UTF-8");
            req.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
            req.Headers.Add("DataServiceVersion", "3.0;NetFx");
            req.Headers.Add("If-Match", "*");
            req.Accept = "application/json;odata=nometadata";
            req.ContentLength = jsonData.Length;

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    r.ReadToEnd();
                    jsonResponse = Convert.ToString(response.StatusCode);                    
                }
            }
            return jsonResponse;
        }

        public static string DeleteEntity(string storageAccount, string accessKey, string resourcePath, string partitionKey, string rowKey, string jsonData)
        {
            string jsonResponse = "";
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "(PartitionKey='" + partitionKey +"',RowKey='" + rowKey +"')";            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            req.Method = "DELETE";
            string mxdate = DateTime.UtcNow.ToString("R");
            string storageServiceVersion = "2015-12-11";
            //specify request header

            var StorageAccountName = storageAccount;
            var StorageKey = accessKey;
            var requestUri = new Uri(uri);
            var canonicalizedStringToBuild = string.Format("{0}\n{1}", mxdate, $"/{StorageAccountName}/{requestUri.AbsolutePath.TrimStart('/')}");
            string signature;
            using (var hmac = new HMACSHA256(Convert.FromBase64String(StorageKey)))
            {
                byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
                signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
            }

            string authorizationHeader = string.Format($"{StorageAccountName}:" + signature);
            String authorization = String.Format("{0} {1}", "SharedKeyLite", authorizationHeader);
            req.Headers.Add("Authorization", authorization);            
            req.Headers.Add("x-ms-date", mxdate);
            req.Headers.Add("x-ms-version", storageServiceVersion);
            req.ContentType = "application/json";
            req.Headers.Add("Accept-Charset", "UTF-8");
            req.Headers.Add("MaxDataServiceVersion", "3.0;NetFx");
            req.Headers.Add("DataServiceVersion", "3.0;NetFx");
            req.Headers.Add("If-Match", "*");
            req.Accept = "application/json;odata=nometadata";

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    r.ReadToEnd();
                    jsonResponse = Convert.ToString(response.StatusCode);
                    //return (int)response.StatusCode;
                }
            }
            return jsonResponse;
        }

        public static string generateAuthorizationHeader(string storageAccount, string accessKey, string mxdate)
        {
            mxdate = DateTime.UtcNow.ToString("R");

            string canonicalizedResource = $"/{storageAccount}/cicform1(PartitionKey='Clerk', RowKey='Firm1')";

            string contentType = "application/json";

            string stringToSign = $"DELETE\n\n{contentType}\n{mxdate}\n{canonicalizedResource}";

            //string stringToSign = $"{ mxdate }" + "\n";

            //stringToSign += "/" + storageAccount + "/cicform1";

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(accessKey));

            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            String authorization = String.Format("{0} {1}:{2}","SharedKey", storageAccount,signature);

            return authorization;
        }
        public static int GetEntitybyFilterDashboard(string storageAccount, string accessKey, string resourcePath, string searchfield, string serachvalue, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=" + searchfield + "%20eq%20'" + serachvalue + "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }


        public static int GetEntitybyDate(string storageAccount, string accessKey, string resourcePath, string searchfield, DateTime serachvalue1, DateTime serachvalue2, out string jsonData)
        {
            string uri = @"https://" + storageAccount + ".table.core.windows.net/" + resourcePath + "?$filter=Timestamp%20ge%20datetime'" + serachvalue1 + "'%20and%20Timestamp%20lt%20datetime'" + serachvalue2 + "'";

            // Web request 
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            int query = 0;// resourcePath.IndexOf("?");
            if (query > 0)
            {
                resourcePath = resourcePath.Substring(0, query);
            }

            request = getRequestHeaders("GET", request, storageAccount, accessKey, resourcePath);

            // Execute the request
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader r = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        jsonData = r.ReadToEnd();
                        return (int)response.StatusCode;
                    }
                }
            }
            catch (WebException ex)
            {
                // get the message from the exception response
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                {
                    jsonData = sr.ReadToEnd();
                    // Log res if required
                }

                return (int)ex.Status;
            }
        }
    }
}
