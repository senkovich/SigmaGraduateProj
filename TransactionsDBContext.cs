using Microsoft.EntityFrameworkCore;
using SigmaGraduateProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaGraduateProj
{
    public class TransactionsDBContext : DbContext
    {
        public TransactionsDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<HBCurrency> HBCurrencies { get; set; }
        public DbSet<ReportTAB> ReportTABs { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}
