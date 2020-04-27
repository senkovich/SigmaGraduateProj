using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SigmaGraduateProj.Models;

namespace SigmaGraduateProj.Controllers
{
    [Controller]
    public class HBCurrencyController : Controller
    {
        TransactionsDBContext _dBContext;

        IConfiguration _configuration;

        private readonly string _nbuApi;

        public HBCurrencyController(TransactionsDBContext transactionsDBContext, IConfiguration configuration)
        {
            _dBContext = transactionsDBContext;
            _configuration = configuration;
            _nbuApi = configuration.GetConnectionString("NbuApiExchange");
        }

        /// <summary>
        /// Gets List of availible Currencies.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">List of Currencies returned</response>
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(typeof(List<HBCurrency>), 200)]
        public async Task<ActionResult> List()
        {
            return Ok(await _dBContext.HBCurrencies.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetHBCurrencyByCode(int? currencyCode)
        {
            if (currencyCode == null)
            {
                return Ok(_dBContext.HBCurrencies);
            }
            return Ok(_dBContext.HBCurrencies.Where(c => c.CurrencyCode == currencyCode));
        }

        [HttpGet]
        public IActionResult GetHBCurrencyByName(string? currencyName)
        {
            if (string.IsNullOrEmpty(currencyName))
            {
                return Ok(_dBContext.HBCurrencies);
            }
            return Ok(_dBContext.HBCurrencies.Where(c => c.CurrencyName.Contains(currencyName)));
        }

        [NonAction]
        public bool CheckCurrencyExists(string currencyName)
        {
            var currency = _dBContext.HBCurrencies.Where(c => c.CurrencyName.Contains(currencyName));
            if(currency.Count() == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Updates list of availible Currencies from https://bank.gov.ua/.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">List of availible Currencies from https://bank.gov.ua/ returned</response>
        /// <response code="400">Error connecting to https://bank.gov.ua/</response>
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(typeof(List<HBCurrency>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateHBCurrency()
        {
            var hbcurrencies = await _dBContext.HBCurrencies.ToListAsync();
            _dBContext.HBCurrencies.RemoveRange(hbcurrencies);
            await _dBContext.SaveChangesAsync();

            string nbuCurrencyJson;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_nbuApi + "json");
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(resp.GetResponseStream());
                nbuCurrencyJson = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var jssettings = new JsonSerializerSettings();
            jssettings.ContractResolver = new HBCurrencyCustomContractResolver();
            List<HBCurrency> currencies = JsonConvert.DeserializeObject<List<HBCurrency>>(nbuCurrencyJson, jssettings);

            _dBContext.HBCurrencies.AddRange(currencies);
            await _dBContext.SaveChangesAsync();
            return Ok(currencies);            
        }
    }
}