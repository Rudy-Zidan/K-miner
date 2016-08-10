using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using K_miner.Core.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace K_miner
{
    class K_miner
    {
        private DataBase db;
        private bool QueueStatus;
        private string Domain;
        private string currentURI;
        public K_miner()
        {
            db = new DataBase();
        }
        public void Start()
        {
            DataTable Queue = db.Select("select top 1 URI from dbo.Queue", null, false);
            if (Queue.Rows.Count > 0)
            {
                QueueStatus = true;
                Fetch(Queue.Rows[0][0].ToString());
            }
            else
            {
                QueueStatus = false;
                Domain = "wikipedia.com";
                Fetch("http://wikipedia.com");
            }
        }
        private void Fetch(string uri)
        {
            try
            {

                Uri url = NormalizeURI(uri);
                currentURI = url.AbsoluteUri;
                Domain = url.Host;
                Console.WriteLine("Fetching: " + url.AbsoluteUri);

                HttpWebRequest request = HttpWebRequest.CreateHttp(url.AbsoluteUri);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(stream);
                if (doc.DocumentNode.FirstChild!=null)
                {
                    Console.WriteLine("Entering Queue ---------------------------------:>");
                    foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                    {
                        HtmlAgilityPack.HtmlAttribute att = link.Attributes["href"];
                        if (!att.Value.StartsWith("#") && !att.Value.StartsWith("javascript"))
                        {
                            Console.WriteLine(att.Value);
                            Uri URLQueue = NormalizeURI(att.Value);
                            if (URLQueue != null)
                            {
                                EnQueue(URLQueue);
                            }
                        }
                    }
                    Console.WriteLine("End of Queue -----------------------------------:>");
                    Console.WriteLine("Saving: " + url.AbsoluteUri);
                    if (SaveDB(url))
                    {
                        if (QueueStatus == true)
                        {
                            if (DeQueue(url))
                            {
                                Start();
                            }
                        }
                        else
                        {
                            Start();
                        }
                    }
                    else
                    {
                        if (DeQueue(url))
                        {
                            Start();
                        }
                    }
                }
                else
                {
                    if (DeQueue(url))
                    {
                        Start();
                    }
                }
            }
            catch (WebException e)
            {
                WebResponse response = e.Response;
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    if (DeQueue(new Uri(currentURI)))
                    {
                        Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
        private Uri NormalizeURI(string uri)
        {
            Uri baseUri = new Uri("http://"+Domain);
            Uri myUri = new Uri(baseUri, uri);
            return myUri;
        }
        private bool SaveDB(Uri url)
        {
            bool flag = false;
            List<SqlQueryParameter> param = new List<SqlQueryParameter>();
            param.Add(new SqlQueryParameter("@URL", url.AbsoluteUri));
            DataTable result = db.Select("select * from URLs where URL=@URL",param,false);
            if (result.Rows.Count == 0)
            {
                param.Add(new SqlQueryParameter("@URL_Domain", url.Host));
                if (db.Manipulate("Insert into URLs values(@URL,@URL_Domain)", param, false) == true)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }
        private void EnQueue(Uri url)
        {
            List<SqlQueryParameter> param = new List<SqlQueryParameter>();
            param.Add(new SqlQueryParameter("@URI", url.AbsoluteUri));
            DataTable result = db.Select("select * from Queue where URI=@URI", param, false);
            if (result.Rows.Count == 0)
            {
                db.Manipulate("Insert into Queue values(@URI)", param, false);
            }
        }
        private bool DeQueue(Uri url)
        {
            bool flag = false;
            try
            {
                List<SqlQueryParameter> param = new List<SqlQueryParameter>();
                param.Add(new SqlQueryParameter("@URI", url.AbsoluteUri));
                flag = db.Manipulate("delete from Queue where URI=@URI",param,false);
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }
        ~K_miner(){}
    }
    
}
