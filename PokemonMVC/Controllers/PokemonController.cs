using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PokemonMVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokemonMVC.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _httpClient;

        public PokemonController()
        {
            _httpClient = new HttpClient { BaseAddress = new System.Uri("https://pokeapi.co/api/v2/") };
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var response = await _httpClient.GetStringAsync($"pokemon?offset={(page - 1) * 20}&limit=20");
            var data = JObject.Parse(response);
            var results = data["results"].ToObject<List<JObject>>();

            List<string> pokemonNames = new List<string>();
            foreach (var result in results)
            {
                pokemonNames.Add(result["name"].ToString());
            }

            ViewBag.Page = page;
            ViewBag.HasNext = data["next"] != null;
            ViewBag.HasPrevious = data["previous"] != null;

            return View(pokemonNames);
        }

        public async Task<IActionResult> Details(string name)
        {
            var response = await _httpClient.GetStringAsync($"pokemon/{name}");
            var data = JObject.Parse(response);

            Pokemon pokemon = new Pokemon
            {
                Name = data["name"].ToString(),
                Moves = data["moves"].ToObject<List<JObject>>().ConvertAll(move => move["move"]["name"].ToString()),
                Abilities = data["abilities"].ToObject<List<JObject>>().ConvertAll(ability => ability["ability"]["name"].ToString())
            };

            return View(pokemon);
        }
    }
}
