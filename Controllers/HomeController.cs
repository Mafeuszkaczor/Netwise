using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Netwise.Models;
using Netwise.Database;
using Newtonsoft.Json;

namespace Netwise.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly NetwiseDbContext _context;

    private readonly string apiUrl = "https://catfact.ninja/fact";

    public HomeController(ILogger<HomeController> logger, NetwiseDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IActionResult CatFacts()
    {
        var allFacts = _context.facts.ToList();
        return View(allFacts);
    }

    [HttpPost]
    public IActionResult FindCatFact(int? maxLength)
    {
        string url = apiUrl;

        if (maxLength != null && maxLength > 0 )
        {
            url = $"{apiUrl}?max_length={maxLength}";
        }

        try
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = httpClient.Send(request);

            var jsonResponse = JsonConvert.DeserializeObject<CatFactsResponseModel>(response.Content.ReadAsStringAsync().Result);
            var catFactsResponse = new CatFactsResponseModel
            {
                fact = jsonResponse.fact,
                length = jsonResponse.length
            };

            _context.Add(catFactsResponse);
            _context.SaveChanges();
            _logger.LogInformation("Api fetched succesfully");
            TempData["maxlength"] = maxLength;
            return RedirectToAction("CatFacts");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = true;
            return RedirectToAction("CatFacts");
        }
    }

    public IActionResult SaveToFile()
    {
        try
        {
            var context = _context.facts.ToList();
            if (context.Count() > 0)
            {
                string fileContext = "Fact  Length\r\n";
                foreach (var i in context)
                {
                    CatFactsResponseModel element = i;
                    fileContext += $"{element.fact} {element.length}\r\n";
                }
                byte[] bytes = Encoding.UTF8.GetBytes(fileContext);
                _logger.LogInformation("File downloaded succesfully");
                return File(bytes, "text/txt", "CatFacts.txt");
            }
            else
            { return RedirectToAction("CatFacts"); }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["error"] = true;
            return RedirectToAction("CatFacts");
        }
    }

    public IActionResult Reset()
    {
        var fact = _context.facts.ToList();
        if (fact != null)
        {
            _context.facts.RemoveRange(fact);
            _context.SaveChanges();
        }
        _logger.LogInformation("Database cleared");
        return RedirectToAction("CatFacts");
    }

    public IActionResult Index()
    {
        return View();
    }
}

