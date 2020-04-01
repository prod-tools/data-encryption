using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CSV_data_manipulation
{
    class Program
    {
        static void Main(params string[] args)
        {
            List<int> hashIndex = null;

            bool isHeader = true;
            int flushSize = 500;
            int counter = 0;
            using(StreamReader reader = new StreamReader(File.OpenRead(args[0])))
            {
                using(StreamWriter writer = File.CreateText(args[1]))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (isHeader)
                        {
                            hashIndex = 
                                GetHashColumnIndexes(args[1].Split(',', StringSplitOptions.RemoveEmptyEntries), 
                                line.Split(',', StringSplitOptions.RemoveEmptyEntries));
                            writer.WriteLine(line);
                            isHeader = false;
                        }
                        else
                        {
                            var items = line.Split(',');
                            foreach (int index in hashIndex)
                                items[index] = ComputeSha256Hash(items[index]);

                            writer.WriteLine(string.Join(',', items));
                        }

                        counter++;
                        if (counter % flushSize == 0)
                            writer.Flush();
                    }
                }
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static List<int> GetHashColumnIndexes(string[] hashColumnHeaders, string[] columnHeaders)
        {
            List<int> hashIndex = new List<int>();
            foreach (string header in hashColumnHeaders)
            {
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    if (string.Compare(header, columnHeaders[i], true) == 0)
                        hashIndex.Add(i);
                }
            }
            return hashIndex;
        }
    }
}
