using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;

//Class made in align to JSON Schema and for Search Service Model
namespace AzureSearchOnBlobJSON
{
    class MetaDataObj
    {
        [IsSearchable, IsFilterable]
        [Analyzer(AnalyzerName.AsString.ElMicrosoft)]
        [JsonProperty("name")]
        public string Name { get; set; }

        [IsSearchable, IsFilterable]
        [Analyzer(AnalyzerName.AsString.ElMicrosoft)]
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    class SampleEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        [JsonProperty("id")]
        public string ID { get; set; }

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.ElMicrosoft)]
        public string Answer { get; set; }

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.ElMicrosoft)]
        public string Source { get; set; }

        [IsSearchable,IsFilterable]
        [Analyzer(AnalyzerName.AsString.ElMicrosoft)]
        public string[] Questions { get; set; }

        public MetaDataObj[] Metadata { get; set; }

        [IsSortable, IsFilterable]
        public DateTimeOffset Createddate { get; set; }
    }


}
