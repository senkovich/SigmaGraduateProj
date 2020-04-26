using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaGraduateProj.Models
{
    public class HBCurrency
    {
        //https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json  - список валют
        //https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?valcode=EUR&date=20200310&json
        //"r030":978,"txt":"Євро","rate":28.2336,"cc":"EUR","exchangedate":"10.03.2020"
        
        [Key]
        public int CurrencyCode { get; set; }

        public string CurrencyFullName { get; set; }

        public string CurrencyName { get; set; }
    }

    public class HBCurrencyCustomContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public HBCurrencyCustomContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>
            {
                {"CurrencyCode", "r030"},
                {"CurrencyFullName", "txt"},
                {"CurrencyName", "cc"}
            };
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
