using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownSharpTests
{
    public static class Utilities
    {
        /// <summary>
        /// removes any empty newlines and any leading spaces at the start of lines 
        /// all tabs, and all carriage returns
        /// </summary>
        public static string RemoveWhitespace(string s)
        {
            // Standardize line endings             
            s = s.Replace("\r\n", "\n");    // DOS to Unix
            s = s.Replace("\r", "\n");      // Mac to Unix

            // remove any tabs entirely
            s = s.Replace("\t", "");

            // remove empty newlines
            s = Regex.Replace(s, @"^\n", "", RegexOptions.Multiline);

            // remove leading space at the start of lines
            s = Regex.Replace(s, @"^\s+", "", RegexOptions.Multiline);

            // remove all newlines
            s = s.Replace("\n", "");

            return s;
        }

        /// <summary>
        /// returns CRC-16 of string as 4 hex characters
        /// </summary>
        public static string GetCrc16(string s)
        {
            if (String.IsNullOrEmpty(s)) return "";
            byte[] b = new Crc16().ComputeChecksumBytes(Encoding.UTF8.GetBytes(s));
            return b[0].ToString("x2") + b[1].ToString("x2");
        }


        /// <summary>
        /// returns the contents of the specified file as a string  
        /// assumes the file is relative to the root of the project
        /// </summary>
        public static string FileContents(string filename)
        {
            try
            {
                return File.ReadAllText(Path.Combine(ExecutingAssemblyPath, filename));
            }
            catch (FileNotFoundException)
            {
                return "";
            }

        }

        /// <summary>
        /// returns the root path of the currently executing assembly
        /// </summary>
        public static string ExecutingAssemblyPath
        {
            get
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // removes executable part
                path = Path.GetDirectoryName(path);
                // we're typically in \bin\debug or bin\release so move up two folders
                path = Path.Combine(path, "..");
                path = Path.Combine(path, "..");
                return path;
            }
        }
    }
}
