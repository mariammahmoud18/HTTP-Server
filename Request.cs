using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        int index_blank = 0;
        bool post_flag = false;
        public bool isHead=false;
        public bool exception = false;
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;

        }
        
        string[] Lines;
        public bool ParseRequest()
        {
            

            Lines = Regex.Split(requestString, "\r\n");
           
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (Lines.Length < 3) return false;

            // Parse Request line
            if (!ParseRequestLine()) return false;
            // Validate blank line exists
            if (!ValidateBlankLine()) return false;
            if (post_flag)
            {
                if (!ValidateContent()) return false;
            }
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines()) return false;

            for (int i = Lines[0].IndexOf('/') + 1; i < Lines[0].Length; i++)
            {
                if (Lines[0][i] == ' ') break;
                relativeURI += Lines[0][i];
            }
            return true;
        }

        private bool ParseRequestLine()
        {

            Regex regex = new Regex(@"^GET \W[a-zA-Z0-9]+\.html HTTP/[0-9]\.[0-9]$", RegexOptions.Compiled);           
            Regex regex1 = new Regex(@"^POST \W[a-zA-Z0-9]+\.html HTTP/[0-9]\.[0-9]$", RegexOptions.Compiled);
            Regex regex2 = new Regex(@"^HEAD \W[a-zA-Z0-9]+\.html HTTP/[0-9]\.[0-9]$", RegexOptions.Compiled);
            if (regex.IsMatch(Lines[0]))
            {
                return true;
            }
            else if (regex1.IsMatch(Lines[0]))
            {
                post_flag = true;
                return true;
            }
            else if (regex2.IsMatch(Lines[0]))
            {
                isHead = true;
                return true;
            }


           // exception = true;
            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool ValidateContent()
        {

            bool flag = false;
            for (int i = index_blank + 1; i < Lines.Length; i++)
            {
                if (Lines[i].Length > 0)
                {
                    using (StreamWriter sw = File.AppendText("data.txt"))
                    {
                        sw.WriteLine(Lines[i]);
                    }
                    flag = true;
                }

                }
            
            return flag;
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            string key = "", val = "";
            int counter = 0;
            for (int i = 1; i < Lines.Length; i++)
            {

                if (Lines[i].Length == 0) break;
                int x;
                for (x = 0; x < Lines[i].Length; x++)
                {
                    if (Lines[i][x] == ':')
                    {
                        key += Lines[i][x];
                        break;
                    }
                    key += Lines[i][x];
                }
                val = Lines[i].Remove(0, x + 1);

                headerLines.Add(key, val);
                key = "";
                val = "";
                counter++;
            }
            if (counter == 0) return false;
            return true;


        }

        private bool ValidateBlankLine()
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i].Length == 0)
                {
                    index_blank = i;
                    return true;

                }
            }
            return false;

        }

    }
}
