using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoHttpClient.Dto;
using ProjetoHttpClient.Models;
using ProjetoHttpClient.Services;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace ProjetoHttpClient.Controllers
{
    public class HomeController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7100/api");

        private readonly HttpClient _httpClient;
        private readonly ISessaoInterface _sessaoInterface;
        public HomeController(HttpClient httpClient, ISessaoInterface sessaoInterface)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseUrl;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar() { 
        
            return View();
        }

        public IActionResult Logout()
        {
            _sessaoInterface.RemoverSessao();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> EditarUsuario(int id)
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if (usuario == null)
            {
                TempData["MensagemErro"] = "É necessário estar logado para acessar essa página!";
                return RedirectToAction("Login");
            }

            ResponseModel<UsuarioModel> usuarioResposta = new ResponseModel<UsuarioModel>();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "/Usuario/" + Convert.ToInt32(id)))
            {

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);


                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioResposta = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                var usuarioEditarDto = new UsuarioEditarDto
                {
                    Id = usuarioResposta.Dados.Id,
                    Nome = usuarioResposta.Dados.Nome,
                    Sobrenome = usuarioResposta.Dados.Sobrenome,
                    Email = usuarioResposta.Dados.Email,
                    Usuario = usuarioResposta.Dados.Usuario
                };

                return View(usuarioEditarDto);


            }

        }


        public async Task<IActionResult> RemoverUsuario(int id)
        {

            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if(usuario == null)
            {
                TempData["MensagemErro"] = "É necessário estar logado para acessar essa página!";
                return RedirectToAction("Login");
            }

            ResponseModel<UsuarioModel> usuarioResposta = new ResponseModel<UsuarioModel>();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, _httpClient.BaseAddress + "/Usuario?id=" + Convert.ToInt32(id)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioResposta = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                TempData["MensagemSucesso"] = usuarioResposta.Mensagem;
                return RedirectToAction("ListarUsuarios");
            };


        }


        public async Task<IActionResult> ListarUsuarios()
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if(usuario == null)
            {
                TempData["MensagemErro"] = "É necessário estar logado para acessar essa página!";
                return RedirectToAction("Login");
            }

            ResponseModel<List<UsuarioModel>> usuarios = new ResponseModel<List<UsuarioModel>>();

            using(var requestMessage = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "/Usuario"))
            {

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);


                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarios = JsonConvert.DeserializeObject<ResponseModel<List<UsuarioModel>>>(data);
                }

                return View(usuarios.Dados);

            }




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


        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioLoginDto)
        {
            if(ModelState.IsValid)
            {
                ResponseModel<UsuarioModel> usuario = new ResponseModel<UsuarioModel>();

                var httpContent = new StringContent(JsonConvert.SerializeObject(usuarioLoginDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/login", httpContent);

                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuario = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                if(usuario.Status == false)
                {
                    TempData["MensagemErro"] = "Credenciais inválidas!";
                    return View(usuarioLoginDto);
                }

                _sessaoInterface.CriarSessao(usuario.Dados);

                return RedirectToAction("ListarUsuarios");
            }
            else
            {
                TempData["MensagemErro"] = "Ocorreu um erro no processo, procure pelo suporte!";
                return View(usuarioLoginDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(UsuarioEditarDto usuarioEditarDto)
        {

            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if (usuario == null)
            {
                TempData["MensagemErro"] = "É necessário estar logado para acessar essa página!";
                return RedirectToAction("Login");
            }

            ResponseModel<UsuarioModel> usuarioResposta = new ResponseModel<UsuarioModel>();

            using(var requestMessage = new HttpRequestMessage(HttpMethod.Put, _httpClient.BaseAddress + "/Usuario"))
            {

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(usuarioEditarDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if(response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioResposta = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                TempData["MensagemSucesso"] = usuarioResposta.Mensagem;
                return RedirectToAction("ListarUsuarios");


            }

        }

    }
}
