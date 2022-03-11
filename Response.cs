using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString="";
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        /*StatusCode code;
        List<string> headerLines = new List<string>();*/
        string httpversion;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath, string httpversion,bool isHead)
        {
            this.httpversion = httpversion;
            string stat = GetStatusLine(code);
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            responseString += stat + "\r\n";
            responseString += "Content-Type: "+contentType + "\r\n";
            responseString += "Content-Length: "+ content.Length + "\r\n";
            responseString += "Date: "+DateTime.Now.ToString()+ "\r\n";
            if(redirectoinPath.Length!=0)
            responseString += "Location:"+redirectoinPath + "\r\n";
            responseString +="\r\n";
            if (!isHead)
            {
                responseString += content + "\r\n";
            }

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
        
            statusLine = httpversion;
            if (code == StatusCode.BadRequest)
                statusLine+= " 400 BadRequest";
            else if (code == StatusCode.NotFound)
                statusLine+= " 404 NotFound";
            else if (code == StatusCode.InternalServerError)
                statusLine+= " 500 InternalServerError";
            else if (code == StatusCode.Redirect)
                statusLine+= " 301 Moved Permanently";
            else if (code == StatusCode.OK)
                statusLine+= " 200 OK";
            return statusLine;
        }
    }
}
