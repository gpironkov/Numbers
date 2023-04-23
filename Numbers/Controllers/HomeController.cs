using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Models;
using System;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private const string NumbersKey = "Numbers";
        private const string CountKey = "Count";
        private const string SumKey = "Sum";

        public HomeController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            var viewModel = new NumberModel();

            viewModel.NumbersList = GetNumbersToList();
            viewModel.Counter = _contextAccessor.HttpContext.Session.GetInt32(CountKey) ?? 0;
            viewModel.Sum = _contextAccessor.HttpContext.Session.GetInt32(SumKey) ?? 0;

            return View(viewModel);
        }

        public IActionResult AddNumber()
        {
            var random = new Random();
            var number = random.Next(1, 1001);

            var numbersList = GetNumbersToList();
            numbersList.Add(number);
            var numbers = JsonConvert.SerializeObject(numbersList);

            HttpContext.Session.SetString(NumbersKey, numbers);

            var count = HttpContext.Session.GetInt32(CountKey) ?? 0;
            _contextAccessor.HttpContext.Session.SetInt32(CountKey, ++count);

            return RedirectToAction("Index");
        }

        public IActionResult SumNumbers()
        {
            var numbersList = GetNumbersToList();
            var sum = numbersList.Sum();

            _contextAccessor.HttpContext.Session.SetInt32(SumKey, sum);

            return RedirectToAction("Index");
        }

        public IActionResult ClearNumbers()
        {
            HttpContext.Session.Remove(NumbersKey);
            HttpContext.Session.Remove(CountKey);
            HttpContext.Session.Remove(SumKey);

            return RedirectToAction("Index");
        }

        private List<int> GetNumbersToList()
        {
            var numbersJson = HttpContext.Session.GetString(NumbersKey);
            var numbersList = numbersJson != null ? JsonConvert.DeserializeObject<List<int>>(numbersJson) : new List<int>();

            return numbersList;
        }
    }
}
