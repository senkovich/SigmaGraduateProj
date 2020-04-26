using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaGraduateProj.Models
{
    public class Report
    {
        [Key]
        [BindNever]
        public int? Id { get; set; }

        public int Quarter { get; set; }

        public int Year { get; set; }

        public decimal TotalSumUah { get; set; }

        public decimal TaxSumUah { get; set; }

        public List<ReportTAB> Tab { get; set; }
    }
}
