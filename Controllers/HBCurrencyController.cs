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

        [HttpGet]
        public async Task<IActionResult> FillHBCurrency()
        {
            List<HBCurrency> currencies = new List<HBCurrency>();

            if (_dBContext.HBCurrencies.Count() > 0)
            {
                return Ok("HBCurrencies is not empty");
            }
            else
            {
                string currencyJson;
                var filepath = Path.Combine("wwwroot", "curr.json");
                if (System.IO.File.Exists(filepath))
                {
                    currencyJson = System.IO.File.ReadAllText(filepath);
                }
                else
                {
                    try
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_nbuApi + "json");
                        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                        StreamReader sr = new StreamReader(resp.GetResponseStream());
                        currencyJson = sr.ReadToEnd();
                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new HBCurrencyCustomContractResolver();
                currencies = JsonConvert.DeserializeObject<List<HBCurrency>>(currencyJson, settings);

                _dBContext.HBCurrencies.AddRange(currencies);
                await _dBContext.SaveChangesAsync();
                return Ok(currencies);
            }
        }
    }
}