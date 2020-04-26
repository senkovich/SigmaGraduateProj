using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SigmaGraduateProj.Models
{
    public class ReportTAB
    {
        [Key]
        [BindNever]
        public int Id { get; set; }

        public int ReportId { get; set; }

        public DateTime Date { get; set; }

        public decimal Sum { get; set; }

        public string CurrencyName { get; set; }

        public decimal CurrencyExchRate { get; set; }

        public decimal SumUah { get; set; }

        public string Sender { get; set; }

        public string Comment { get; set; }
    }
}
