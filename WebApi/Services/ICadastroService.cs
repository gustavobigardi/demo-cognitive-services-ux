using System.IO;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface ICadastroService
    {
         Task<Usuario> PreencherDados(Stream streamDocumento);
    }
}