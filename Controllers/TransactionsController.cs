using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigmaGraduateProj.Models;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;

namespace SigmaGraduateProj.Controllers
{
    [Controller]
    public class TransactionsController : Controller
    {
        private readonly TransactionsDBContext _dBContext;

        private readonly CurrencyController _currencyExchDatesController;

        private readonly HBCurrencyController _hBCurrencyController;

        public TransactionsController(TransactionsDBContext dBContext, CurrencyController currencyExchDatesController, HBCurrencyController hBCurrencyController)
        {
            _dBContext = dBContext;
            _currencyExchDatesController = currencyExchDatesController;
            _hBCurrencyController = hBCurrencyController;
        }

        /// <summary>
        /// Gets last 100 transactions.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Transactions returned</response>      
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(typeof(List<Transaction>), 200)]
        public async Task<IActionResult> Get100()
        {
            var alltansactions = await _dBContext.Transactions.OrderByDescending(t => t.Date).Take(100).ToListAsync();
            return Ok(alltansactions);
        }

        /// <summary>
        /// Gets a transaction by id.
        /// </summary>
        /// <param name="id">The id of the transaction you wish to get.</param>
        /// <returns></returns>
        /// <response code="200">Transaction returned</response>
        /// <response code="404">Transaction not found</response>        
        [HttpGet("[controller]/[action]/id={id}")]
        [ProducesResponseType(typeof(Transaction), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var tansaction = await _dBContext.Transactions.Where(t => t.Id == id).FirstOrDefaultAsync();
            if(tansaction == null)
            {
                return NotFound();
            }
            return Ok(tansaction);
        }

        /// <summary>
        /// Gets a transactions per interval.
        /// </summary>
        /// <param name="fromDate">The starting date "dd.MM.yyyy" from which you wish to get transactions.</param>
        /// <param name="toDate">The ending date "dd.MM.yyyy" to which you wish to get transactions.</param>
        /// <param name="type">The type of data you wish to get transactions (json/html).</param>
        /// <returns></returns>
        /// <response code="200">Transactions returned</response>
        /// <response code="400">Invalid input params</response>  
        /// <response code="404">Transactions not found</response>   
        [HttpGet("[controller]/[action]/fromDate={fromDate}&toDate={toDate}&type={type}")]
        [ProducesResponseType(typeof(List<Transaction>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetInterval(string fromDate, string toDate, string type = "json")
        {
            string[] typesAvailible = { "json", "html" };
            if (DateTime.TryParse(fromDate, out DateTime fromDate1) && DateTime.TryParse(toDate, out DateTime toDate1) && typesAvailible.Contains(type))
            {
                var intervalTrans = selectTransInterval(fromDate1, toDate1);
                if (intervalTrans.Count == 0)
                {
                    return NotFound();
                }
                else if(type == "json")
                {
                    return Ok(intervalTrans);
                }
                else if(type == "html")
                {
                    var htmlTable = formTransactionHtml(intervalTrans);
                    return Ok(htmlTable);
                }
            }
            return BadRequest("Invalid input params");
        }

        [NonAction]
        public List<Transaction> selectTransInterval(DateTime dt1, DateTime dt2)
        {
           return _dBContext.Transactions.Where(t => t.Date >= dt1)
                                            .Where(t => t.Date <= dt2)
                                            .OrderByDescending(t => t.Date)
                                            .ToList();
        }

        private string formTransactionHtml(List<Transaction> tdata)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[6] {  new DataColumn("#", typeof(int)),
                    new DataColumn("Дата", typeof(string)),
                    new DataColumn("Сума операции", typeof(decimal)),
                    new DataColumn("Валюта",typeof(string)),
                    new DataColumn("Отправитель",typeof(string)),
                    new DataColumn("Комментарий",typeof(string)) });

            int i = 0;
            foreach(var t in tdata)
            {
                dt.Rows.Add(i, t.Date.ToString("dd.MM.yyyy"), t.Sum, t.CurrencyName, t.Sender, t.Comment);
                i++;
            }

            StringBuilder sb = new StringBuilder();
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
            return sb.ToString();
        }

        /// <summary>
        /// Creates new transaction.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Return created transaction</response>
        /// <response code="400">List invalid transaction params:</response>  
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(typeof(Transaction), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (!_hBCurrencyController.CheckCurrencyExists(transaction.CurrencyName))
            {
                ModelState.AddModelError("CurrencyName", $"Валюта Date не существует");
            }
            else if (transaction.Date>DateTime.Now)
            {
                ModelState.AddModelError("Date", $"Дата Транзакции {transaction.Date} не может быть больше текущей даты");
            }

            if (ModelState.IsValid)
            {
                await _currencyExchDatesController.Exchange(transaction.CurrencyName, transaction.Date.ToString());

                var entity = _dBContext.Transactions.Add(transaction);
                await _dBContext.SaveChangesAsync();
                return Ok(entity.Entity);
            }

            List<string> errList = new List<string>();
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errList.Add(error.ErrorMessage);
                }
            }
            return BadRequest(errList);
        }

        /// <summary>
        /// Delete transaction by id.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Transaction deleted</response>
        /// <response code="400">Delete error</response>  
        /// <response code="404">Transaction not found</response>  
        [HttpDelete("[controller]/[action]/id={id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(int), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return BadRequest(id);
            }

            Transaction transaction = _dBContext.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound(id);
            }

            try
            {
                _dBContext.Transactions.Remove(transaction);
                await _dBContext.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Replace transaction by id.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Transaction replaced</response>
        /// <response code="400">Bad request params</response>  
        /// <response code="404">Transaction not found</response>  
        [HttpPut("[controller]/[action]/id={id}")]
        [ProducesResponseType(typeof(Transaction), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(int), 404)]
        public async Task<IActionResult> Replace([FromRoute]int id, [FromQuery]Transaction transaction)
        {
            if (id == null)
            {
                return BadRequest(id);
            }
            if (ModelState.IsValid)
            {
                var transactionToUpdate = _dBContext.Transactions
                    .Where(t => t.Id == id)
                    .SingleOrDefault();

                if (transactionToUpdate == null)
                {
                    return NotFound(id);
                }

                if (await TryUpdateModelAsync<Transaction>(
                        transactionToUpdate,
                        "", 
                        t => t.Date, t => t.Sum, t => t.CurrencyName, t => t.Comment,  t => t.Sender))
                {
                    try
                    {
                        await _dBContext.SaveChangesAsync();
                        return Ok(transactionToUpdate);
                    }
                    catch (DbUpdateException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
            List<string> errList = new List<string>();
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    errList.Add(error.ErrorMessage);
                }
            }
            return BadRequest(errList);
        }
    }
}