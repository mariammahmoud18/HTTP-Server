using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace HTTPServer
{
    class Server
    {
        string content;
        string uri = "";
        StatusCode stat;
        Response res;
        string httpversion = "HTTP/0.9";
        int dep = 0;
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEnd);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);           
                while (true)
                {
                    //TODO: accept connections and start thread for each accepted connection.
                    Socket clientSocket = serverSocket.Accept();
                    Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                    thread.Start(clientSocket);
                }
            
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;           
            clientSocket.ReceiveTimeout = 0; 

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] dataReceived = new byte[1024 * 1024];
                    int length = clientSocket.Receive(dataReceived);                    
                    if (length == 0) break;

                    //****** Get Data From Clieny And Put suitable Http Version
                    string data = Encoding.ASCII.GetString(dataReceived, 0, length);
                    Regex ver = new Regex(@"HTTP/1\.0");
                    Regex ver1 = new Regex(@"HTTP/1\.1");
                    Regex ver2 = new Regex(@"HTTP/0\.9");                   
                    if (ver.IsMatch(data))
                        httpversion = "HTTP/1.0";

                   else if (ver1.IsMatch(data))                    
                        httpversion = "HTTP/1.1";
                    
                    else if (ver2.IsMatch(data))                    
                        httpversion = "HTTP/0.9";

                    // Create Request And HandleRequest 
                    Request req = new Request(data);                   
                    Response res= HandleRequest(req);
                   
                    // TODO: Send Response back to client
                     clientSocket.Send(Encoding.ASCII.GetBytes(res.ResponseString));//check again
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }           

            // TODO: close client socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    stat = StatusCode.BadRequest;
                    content = LoadDefaultPage("BadRequest.html");
                   // if (!request.exception)
                    {
                        res = new Response(stat, "text/html", content, uri, httpversion,request.isHead);
                        return res;
                    }
                }
               
                //TODO: check file exists
                if (File.Exists(request.relativeURI))
                {
                    
                    stat = StatusCode.OK;
                    content = File.ReadAllText(request.relativeURI);
                    if (content.Length == 0)
                    {
                        Configuration.RedirectionRules.ContainsKey(null);
                    }

                }
               else  if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {                   
                    stat = StatusCode.Redirect;
                    content = LoadDefaultPage("Redirect.html");
                    if(content.Length==0)
                    {
                        
                        Configuration.RedirectionRules.ContainsKey(null);
                    }
                    uri = GetRedirectionPagePathIFExist(request.relativeURI);
                }

                else
                {
                    stat = StatusCode.NotFound;
                    content = LoadDefaultPage("NotFound.html");
                }

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class                
                stat = StatusCode.InternalServerError;
                content = LoadDefaultPage("InternalError.html");
                Logger.LogException(ex);
            }
            
            res = new Response(stat, "text/html", content, uri,httpversion,request.isHead);
            return res;
            }
        // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
        private string GetRedirectionPagePathIFExist(string relativePath)
        {

            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                  return Configuration.RedirectionRules[relativePath];
            else
                return string.Empty;
        }
        //Read Content Of Page
        private string LoadDefaultPage(string defaultPageName)
        {
            string val = "";
            try
            {
              //  string filePath = Path.Combine(Configuration.RootPath, defaultPageName);          
                if (File.Exists(defaultPageName))      
                {
                    string[] content = File.ReadAllLines(defaultPageName);

                    for (int i = 0; i < content.Length; i++)
                        val += content[i];
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return val;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(filePath);
                foreach(string l in lines)
                {
                    string[] files = l.Split(',');
                    Configuration.RedirectionRules.Add(files[0], files[1]);
                }
               
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
