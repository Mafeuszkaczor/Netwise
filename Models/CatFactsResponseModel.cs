using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace Netwise.Models
{
    public class CatFactsResponseModel
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("fact")]
        public string fact { get; set; }

        [JsonProperty("length")]
        public int length { get; set; }
    }
}

