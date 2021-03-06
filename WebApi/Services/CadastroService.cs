using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using WebApi.Models;

namespace WebApi.Services
{
    public class CadastroService : ICadastroService
    {
        private readonly IComputerVisionClient _cvs;

        private const int numberOfCharsInOperationId = 36;

        public CadastroService(IComputerVisionClient cvs)
        {
            _cvs = cvs;
        }

        public async Task<Usuario> PreencherDados(Stream streamDocumento)
        {
            var result = await GetData(streamDocumento);
            return PreencherUsuario(result);
        }

        private async Task<List<string>> GetData(Stream streamImage)
        {
            RecognizeTextInStreamHeaders headers = await _cvs.RecognizeTextInStreamAsync(streamImage, TextRecognitionMode.Printed);

            string operationId = headers.OperationLocation.Substring(
                headers.OperationLocation.Length - numberOfCharsInOperationId);

            TextOperationResult result =
               await _cvs.GetTextOperationResultAsync(operationId);

            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                await Task.Delay(1000);

                result = await _cvs.GetTextOperationResultAsync(operationId);
            }

            return result.RecognitionResult.Lines.Select(l => l.Text).ToList();
        }

        private Usuario PreencherUsuario(List<string> dados)
        {
            Usuario usuario = new Usuario();

            //CNH
            if (dados.Any(x => x.Contains("CARTEIRA") && x.Contains("NACIONAL") && x.Contains("HAB")))
            {
                usuario.Nome = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("NOME"))) + 1);
                usuario.RG = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("DOC. IDENT"))) + 1).Split(" ").ElementAt(0);
                usuario.OrgEmissor = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("DOC. IDENT"))) + 1).Split(" ").ElementAt(1);
                usuario.CPF = (dados.Where(x => Regex.IsMatch(x.Replace(" ", ""), @"^\d{3}\.\d{3}\.\d{3}-\d{2}$")).FirstOrDefault() ?? "").Replace(" ", "");
                try
                {
                    usuario.DataNascimento = DateTime.ParseExact(dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("DATA NASC"))) + 2), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                { }
            }
            else if (dados.Any(x => x.Contains("REGISTRO")) && dados.Any(x => x.Contains("GERAL")) && dados.Any(x => x.Contains("Delegado")))
            {
                usuario.Nome = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("NOME"))) + 1);
                usuario.RG = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("GERAL"))) + 1).Split(" ").ElementAt(0);
                var emissor = dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("DOC ORIGEM"))) + 1).Split(" ");
                usuario.OrgEmissor = emissor.ElementAt(0) + "-" + emissor.ElementAt(1);
                usuario.CPF = (dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("CPF"))) + 1) ?? "").Replace("/", "-");
                try
                {
                    usuario.DataNascimento = DateTime.ParseExact(dados.ElementAt(dados.IndexOf(dados.FirstOrDefault(x => x.Contains("DATA DE NASCIMENTO"))) + 2), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                }
            }

            return usuario;
        }
    }
}