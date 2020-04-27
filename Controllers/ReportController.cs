using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SigmaGraduateProj.Models;

namespace SigmaGraduateProj.Controllers
{
    [Controller]
    public class ReportController : Controller
    {
        private readonly TransactionsDBContext _dBContext;

        private readonly CurrencyController _currencyExchDatesController;

        private readonly HBCurrencyController _hBCurrencyController;

        private readonly TransactionsController _transactionsController;

        public ReportController(TransactionsDBContext dBContext, 
                                CurrencyController currencyExchDatesController, 
                                HBCurrencyController hBCurrencyController, 
                                TransactionsController transactionsController)
        {
            _dBContext = dBContext;
            _currencyExchDatesController = currencyExchDatesController;
            _hBCurrencyController = hBCurrencyController;
            _transactionsController = transactionsController;
        }

        /// <summary>
        /// Make Quarter TAX report.
        /// </summary>
        /// <param name="Year">The year "yyyy" you wish to get TAX report.</param>
        /// <param name="Quarter">The quarter of Year "1-4" you wish to get TAX report.</param>
        /// <param name="type">The type of data you wish to get TAX report (json/html).</param>
        /// <returns></returns>
        /// <response code="200">Quarter TAX report returned</response>
        /// <response code="400">Bad request params</response>   
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> MakeQuarter(int Year, int Quarter, string type = "json")
        {
            var valRes = ValidateCreateRepParams(Year, Quarter, type);
            if (!String.IsNullOrEmpty(valRes))
            {
                return BadRequest(valRes);
            }

            QuarterToDateTime(Year, Quarter, out DateTime dt1, out DateTime dt2);

            Report selectedReport = await _dBContext.Reports.Where(r => r.Year == Year && r.Quarter == Quarter).Include(r => r.Tab).FirstOrDefaultAsync();

            if(selectedReport == null)
            {
                selectedReport = await CreateReport(Year, Quarter, dt1, dt2);
            }

            if (type == "json")
            {
                return Ok(selectedReport);
            }
            return Ok(formReportHtml(selectedReport));
        }

        /// <summary>
        /// Remake existing Quarter TAX report.
        /// </summary>
        /// <param name="Year">The year "yyyy" you wish to get TAX report.</param>
        /// <param name="Quarter">The quarter of Year "1-4" you wish to get TAX report.</param>
        /// <param name="type">The type of data you wish to get TAX report (json/html).</param>
        /// <returns></returns>
        /// <response code="200">Remade Quarter TAX report returned</response>
        /// <response code="400">Bad request params</response>      
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(typeof(string), 400)]        
        public async Task<IActionResult> ReMakeQuarter(int Year, int Quarter, string type = "json")
        {
            var valRes = ValidateCreateRepParams(Year, Quarter, type);
            if (!String.IsNullOrEmpty(valRes))
            {
                return BadRequest(valRes);
            }

            QuarterToDateTime(Year, Quarter, out DateTime dt1, out DateTime dt2);

            Report selectedReport = await _dBContext.Reports
                                        .Where(r => r.Year == Year && r.Quarter == Quarter)
                                        .Include(r => r.Tab)
                                        .FirstOrDefaultAsync();
            if (selectedReport == null)
            {
                return BadRequest($"Отчета за {Quarter} кв. {Year} - не существует");
            }
            _dBContext.Remove(selectedReport);
            await _dBContext.SaveChangesAsync();

            Report NewReport = await CreateReport(Year, Quarter, dt1, dt2);

            if (type == "json")
            {
                return Ok(NewReport);
            }
            return Ok(formReportHtml(NewReport));
        }

        private string ValidateCreateRepParams(int Year, int Quarter, string type)
        {
            string[] typesAvailible = { "json", "html" };
            if (Year < 0 || Year > 9999)
            {
                return $"Некорректный год {Year}";
            }
            else if (Quarter < 1 || Quarter > 4)
            {
                return $"Некорректный квартал {Quarter}";
            }
            else if (!typesAvailible.Contains(type))
            {
                return $"Некорректный тип {type}";
            }
            return string.Empty;
        } 

        private async Task<Report> CreateReport(int year, int quarter, DateTime dtfrom, DateTime dtto)
        {
            List<Transaction> transactions = _transactionsController.selectTransInterval(dtfrom, dtto);

            Report report = new Report();
            report.Quarter = quarter;
            report.Year = year;
            report.TotalSumUah = 0;

            List<ReportTAB> reportTABs = new List<ReportTAB>();
            foreach (var t in transactions)
            {
                var serialTransaction = JsonConvert.SerializeObject(t);
                ReportTAB reportTab = JsonConvert.DeserializeObject<ReportTAB>(serialTransaction);

                var exchange = _currencyExchDatesController.getCurrExch(reportTab.CurrencyName, reportTab.Date);
                reportTab.CurrencyExchRate = exchange.Result.ExchangeRate;
                reportTab.SumUah = decimal.Multiply(exchange.Result.ExchangeRate, reportTab.Sum);
                decimal.Round(reportTab.SumUah, 2);
                report.TotalSumUah = decimal.Add(report.TotalSumUah, reportTab.SumUah);
                reportTABs.Add(reportTab);
            }
            report.TaxSumUah = decimal.Multiply(report.TotalSumUah, (decimal)0.05);
            decimal.Round(report.TaxSumUah, 2);
            report.Tab = reportTABs;
            var reportEntity = _dBContext.Reports.Add(report);
            await _dBContext.SaveChangesAsync();
            return reportEntity.Entity;
        }

        private string formReportHtml(Report rdata)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("#", typeof(int)),
                    new DataColumn("Дата", typeof(string)),
                    new DataColumn("Сумма операции", typeof(decimal)),
                    new DataColumn("Валюта",typeof(string)),
                    new DataColumn("Курс", typeof(decimal)),
                    new DataColumn("Сумма, грн.", typeof(decimal)),
                    new DataColumn("Отправитель",typeof(string)),
                    new DataColumn("Комментарий",typeof(string)) });
            
            int i = 1;
            foreach (var r in rdata.Tab)
            {
                dt.Rows.Add(i, r.Date.ToString("dd.MM.yyyy"), r.Sum, r.CurrencyName, r.CurrencyExchRate, decimal.Round(r.SumUah, 2), r.Sender, r.Comment);
                i++;
            }

            StringBuilder sb = new StringBuilder();
            //Header
            sb.Append($"<p>Отчет Единого Налога третьей группы ФОП #{rdata.Id}</p>");
            sb.Append($"<p>Год: {rdata.Year}</p>");
            sb.Append($"<p>Квартал: {rdata.Quarter}</p>");
            //Table start.
            sb.Append("<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:Arial'>");

            //Adding HeaderRow.
            sb.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.ColumnName + "</th>");
            }
            sb.Append("</tr>");

            //Adding DataRow.
            foreach (DataRow row in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    sb.Append("<td style='width:100px;border: 1px solid #ccc'>" + row[column.ColumnName].ToString() + "</td>");
                }
                sb.Append("</tr>");
            }

            //Table end.
            sb.Append("</table>");

            //Bottom
            sb.Append($"<p>Итого Сумма, грн: {decimal.Round(rdata.TotalSumUah, 2)} грн.</p>");
            sb.Append($"<p>Сумма единого налога (5%): {decimal.Round(rdata.TaxSumUah, 2)} грн.</p>");
            return sb.ToString();
        }

        private void QuarterToDateTime(int year, int quarter, out DateTime dtfrom, out DateTime dtto)
        {
            if (quarter == 1)
            {
                DateTime dt1 = new DateTime(year, 1, 1);
                DateTime dt2 = new DateTime(year, 3, 31);
                dtfrom = dt1;
                dtto = dt2;
            }
            else if (quarter == 2)
            {
                DateTime dt1 = new DateTime(year, 4, 1);
                DateTime dt2 = new DateTime(year, 6, 30);
                dtfrom = dt1;
                dtto = dt2;
            }
            else if (quarter == 3)
            {
                DateTime dt1 = new DateTime(year, 7, 1);
                DateTime dt2 = new DateTime(year, 9, 30);
                dtfrom = dt1;
                dtto = dt2;
            }
            else
            {
                DateTime dt1 = new DateTime(year, 10, 1);
                DateTime dt2 = new DateTime(year, 12, 31);
                dtfrom = dt1;
                dtto = dt2;
            }            
        }
    }
}