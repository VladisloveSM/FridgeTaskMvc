using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using WebFridgeApp.Models;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Json;

namespace WebFridgeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string apiFridge;

        public IActionResult Index()
        {
            return View();
        }

        public HomeController(IConfiguration configuration)
        {
            this.apiFridge = configuration["FridgeApi"];
        }

        [HttpGet]
        public async Task<IActionResult> Fridge(Guid id)
        {
            using HttpClient client = new HttpClient();
            var result = await client.GetAsync($"{this.apiFridge}/Product/{id}");
            IEnumerable<ProductInFridge> products = ParseJson<IEnumerable<ProductInFridge>>(result.Content.ReadAsStringAsync().Result);

            var nameResult = await client.GetAsync($"{this.apiFridge}/Fridge/Name/{id}");

            ViewBag.Title = $"All Products in Fridge: \"{nameResult.Content.ReadAsStringAsync().Result}\".";

            var emptyResult = await client.GetAsync($"{this.apiFridge}/Product/Empty/{id}");
            IEnumerable<Product> productsEmpty = ParseJson<IEnumerable<Product>>(emptyResult.Content.ReadAsStringAsync().Result);
            if (productsEmpty.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Add: {productsEmpty.ElementAt(0).Name}");
                for (int i = 1; i < productsEmpty.Count(); i++)
                {
                    sb.Append($", {productsEmpty.ElementAt(i).Name}");
                }
                ViewBag.EmptyProducts = sb.ToString();
            }
            ViewBag.FridgeId = id;

            return View(products);
        }

        public async Task<IActionResult> List()
        {
            using HttpClient client = new HttpClient();
            var result = await client.GetAsync($"{this.apiFridge}/Fridge");

            return View(ParseJson<IEnumerable<Fridge>>(result.Content.ReadAsStringAsync().Result));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            using HttpClient client = new HttpClient();
            await client.DeleteAsync($"{this.apiFridge}/Fridge/{id}");

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult ProductForm(Guid id)
        {
            this.ViewBag.WebApiName = this.apiFridge;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductForm(ProductInFridge product)
        {
            product.Fridge_id = product.Id;
            this.ViewBag.WebApiName = this.apiFridge;
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                
                using HttpClient client = new HttpClient();

                await client.PostAsJsonAsync($"{this.apiFridge}/Product", product);

                return RedirectToAction("Fridge", new { id = product.Fridge_id });
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult FridgeForm()
        {
            ViewBag.WebApiName = this.apiFridge;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FridgeForm(Fridge fridge)
        {
            ViewBag.WebApiName = this.apiFridge;
            if (ModelState.IsValid)
            {
                fridge.Id = Guid.NewGuid();
                using HttpClient client = new HttpClient();

                await client.PostAsJsonAsync($"{this.apiFridge}/Fridge", fridge);

                return RedirectToAction("Fridge", new { fridge.Id });
            }

            return View(fridge);
        }

        [HttpGet]
        public IActionResult UpdateFridgeForm(Guid id)
        {
            ViewBag.WebApiName = this.apiFridge;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFridgeForm(Fridge fridge)
        {
            ViewBag.WebApiName = this.apiFridge;
            if (ModelState.IsValid)
            {
                using HttpClient client = new HttpClient();

                await client.PutAsJsonAsync($"{this.apiFridge}/Fridge", fridge);

                return RedirectToAction("List");
            }

            return View(fridge);
        }

        [HttpGet]
        public IActionResult UpdateProductForm(Guid id, Guid fridge_id)
        {
            ViewBag.ProductId = id;
            ViewBag.FridgeId = fridge_id;
            ViewBag.WebApiName = this.apiFridge;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductForm(ProductInFridge product)
        {
            ViewBag.FridgeId = product.Fridge_id;
            ViewBag.WebApiName = this.apiFridge;
            if (ModelState.IsValid)
            {
                using HttpClient client = new HttpClient();

                await client.PutAsJsonAsync($"{this.apiFridge}/Product", product);

                return RedirectToAction("Fridge", new { id = product.Fridge_id });
            }

            return View(product);
        }

        public async Task<IActionResult> GetFridgeWithEmptyProducts()
        {
            using HttpClient client = new HttpClient();
            var result = await client.GetAsync($"{this.apiFridge}/Fridge");

            IEnumerable<Guid> fridgesId = ParseJson<IEnumerable<Fridge>>(result.Content.ReadAsStringAsync().Result).Select(i => i.Id);
            foreach (Guid id in fridgesId)
            {
                var emptyResult = await client.GetAsync($"{this.apiFridge}/Product/Empty/{id}");
                IEnumerable<Product> productsEmpty = ParseJson<IEnumerable<Product>>(emptyResult.Content.ReadAsStringAsync().Result);
                if (productsEmpty.Any())
                {
                    return RedirectToAction("Fridge", new { id });
                }
            }

            return RedirectToAction("List");
        }

        public async Task<IActionResult> RemoveProduct(Guid id, Guid fridge_id)
        {
            using HttpClient client = new HttpClient();
            await client.DeleteAsync($"{this.apiFridge}/Product/{id}");

            return RedirectToAction("Fridge", new { id = fridge_id });
        }

        private T ParseJson<T>(string jsonValue)
        {
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(jsonValue, option);
        }

    }
}
