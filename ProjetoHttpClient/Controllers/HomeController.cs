using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoHttpClient.Dto;
using ProjetoHttpClient.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace ProjetoHttpClient.Controllers
{
    public class HomeController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7100/api");

        private readonly HttpClient _httpClient;
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseUrl;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar() { 
        
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {

            if(ModelState.IsValid)
            {
                ResponseModel<UsuarioModel> usuario = new ResponseModel<UsuarioModel>();

                var httpContent = new StringContent(JsonConvert.SerializeObject(usuarioCriacaoDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/register", httpContent);

                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuario = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                if(usuario.Status == false)
                {
                    TempData["MensagemErro"] = "Ocorreu um erro no processo, procure pelo suporte!";
                    return View(usuarioCriacaoDto);
                }

                TempData["MensagemSucesso"] = usuario.Mensagem;
                return RedirectToAction("Login");


            }
            else
            {
                TempData["MensagemErro"] = "Ocorreu um erro no processo, procure pelo suporte!";
                return View(usuarioCriacaoDto);
            }





        }

    }
}
