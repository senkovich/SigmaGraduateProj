using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigmaGraduateProj.Models;
using static SigmaGraduateProj.Models.Currency;

namespace SigmaGraduateProj.Controllers
{
    [Controller]
    public class CurrencyController : Controller
    {
        private readonly TransactionsDBContext _dBContext;

        private readonly IConfiguration _configuration;

        private readonly string _nbuApi;

        private readonly ILogger<CurrencyController> _logger;

        private readonly HBCurrencyController _hBCurrencyController;

        public CurrencyController(TransactionsDBContext transactionsDBContext, IConfiguration configuration, ILogger<CurrencyController> logger, HBCurrencyController hBCurrencyController)
        {
            _dBContext = transactionsDBContext;
            _configuration = configuration;
            _nbuApi = configuration.GetConnectionString("NbuApiExchange");
            _logger = logger;
            _hBCurrencyController = hBCurrencyController;
        }

        /// <summary>
        /// Gets Currency exchange for date.
        /// </summary>
        /// <param name="currencyName">The Currency name you wish to get currency.</param>
        /// <param name="date">The date "dd.MM.yyyy" you wish to get currency.</param>
        /// <returns></returns>
        /// <response code="200">Currency returned</response>
        /// <response code="400">Invalid input params</response>  
        [HttpGet("[controller]/[action]/currencyName={currencyName}&date={date}")]
        [ProducesResponseType(typeof(Currency), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Currency>> Exchange(string currencyName, string date)
        {
            if (DateTime.TryParse(date, out DateTime dateParam) && _hBCurrencyController.CheckCurrencyExists(currencyName))
            {
                var currency = await getCurrExch(currencyName, dateParam);
                return Ok(currency);
            }
            return BadRequest("Invalid input params: " + HttpContext.Request.QueryString);
        }

        [NonAction]
        public async Task<Currency> getCurrExch(string currencyName, DateTime dateParam)
        {
            Currency exchange = _dBContext.Currencies.Where(ce => ce.CurrencyName == currencyName && ce.ExchangeDate == dateParam).FirstOrDefault();
            if (exchange != null)
            {
                return exchange;
            }
            else
            {
                string uri = $"{_nbuApi}valcode={currencyName}&date={dateParam:yyyyMMdd}&json";
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    string currencyExchJson = sr.ReadToEnd();
                    sr.Close();

                    var settings = new JsonSerializerSettings();
                    settings.ContractResolver = new CurrencyExchDateCustomContractResolver();
                    var currencyExch = JsonConvert.DeserializeObject<List<Currency>>(currencyExchJson, settings);

                    _dBContext.Currencies.AddRange(currencyExch);
                    await _dBContext.SaveChangesAsync();
                    return currencyExch.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }
        }
    }
}