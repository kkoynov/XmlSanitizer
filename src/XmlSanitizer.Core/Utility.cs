using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace XmlSanitizer.Core
{
    public class Utility
    {
        /// <summary>
        /// Load from a csv like file the existing values.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="skipHeaders">Whether first row should be omitted. </param>
        /// <returns></returns>
        public static HashSet<string> LoadExistingValues(string filePath, bool skipHeaders = true)
        {
            var result = new HashSet<string>();
            using (var reader = new StreamReader(filePath))
            {
                if (skipHeaders && reader.EndOfStream == false)
                {
                    reader.ReadLine();
                }

                while(reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    result.Add(line);
                }
            }

            return result;
        }
    }
}