using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CadastroController : ControllerBase
    {
        private readonly ICadastroService _cadastroService;

        public CadastroController(ICadastroService cadastroService)
        {
            _cadastroService = cadastroService;
        }

        [HttpPost]
        [Route("read-from-doc")]
        public async Task<IActionResult> ReadFromDoc(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new JsonResult(new Usuario());
            }

            var result = await _cadastroService.PreencherDados(file.OpenReadStream());

            return new JsonResult(result);
        }
    }
}