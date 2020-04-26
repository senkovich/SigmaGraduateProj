using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaGraduateProj.Models
{
    public class Transaction
    {
        [BindNever]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Sum { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Название валюты состоит из 3 символов")]
        public string CurrencyName { get; set; }

        public string Sender { get; set; }

        public string Comment { get; set; }
    }
}
