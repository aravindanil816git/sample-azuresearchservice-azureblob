using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace AzureSearchOnBlobJSON
{
    class Program
    {
 
        static void Main(string[] args)
        {

            //Uncomment and  use generateFormattedJSON() to convert JSON to schema format
            //!IMPORTANT: Once Done , copy the contents into blob file.
            //generateFormattedJSON();

            SearchServices s = new SearchServices();
            //Below function should be one time activity only. If Any of dependencies already exists delete from Portal and run fn. again
            if (s.InitializeSearchServiceDependency())
            {
                /*Search Queries */
                //1. Return all documents by descending order of CreatedDate              
                var result1 = s.RunQuery(new[] { "Answer", "Questions", "Source", "Createddate", "Metadata" }, new[] { "Createddate desc" }, "*");
                documentLogger(result1);

                //1. Return all documents which contain 'BackTrack' keyword 
                var result2 = s.RunQuery(new[] { "Answer", "Questions", "Source", "Createddate" }, new[] { "Createddate asc" }, "Backtrack");
                documentLogger(result2);

                //1. Return all documents which contain qn101 as value in QUESTION field
                var result3 = s.RunQuery(new[] { "Answer", "Questions", "Source", "Createddate" }, new[] { "Createddate asc" }, "Questions eq 'qn101'", "*");
                documentLogger(result3);
            }
            Console.WriteLine("Could not initialize Search service dependency");
        }

        /// <summary>
        /// Logs Search Result into Console
        /// </summary>
        /// <param name="result">Retrieved Search Document</param>
        public static void documentLogger(DocumentSearchResult<Document> result)
        {
            if (result != null)
                Console.WriteLine(JsonConvert.SerializeObject(result.Results, Formatting.Indented));

            else
                Console.WriteLine("Search Not successful");
            Console.WriteLine("------------------------------------------------------------------");
        }
    }
}
