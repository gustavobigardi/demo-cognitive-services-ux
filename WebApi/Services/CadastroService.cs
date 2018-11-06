using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using WebApi.Models;

namespace WebApi.Services
{
    public class CadastroService : ICadastroService
    {
        private readonly IComputerVisionClient _cvs;

        public CadastroService(IComputerVisionClient cvs)
        {
            _cvs = cvs;
        }

        public async Task<Usuario> PreencherDados(Stream streamDocumento)
        {
            var result = await GetData(streamDocumento);
            return new Usuario();
        }

        private async Task<Dictionary<string, string>> GetData(Stream streamImage)
        {
            OcrResult ocrResult = await _cvs.RecognizePrintedTextInStreamAsync(true, streamImage, OcrLanguages.Pt);
            var result = new Dictionary<string, string>();

            result.Add("Key", OcrResultsToString(ocrResult));

            return result;
        }

        string OcrResultsToString(OcrResult result)
        {
            return string.Join("\n",
                result.Regions.ToList().Select(region =>
                    string.Join(" ", region.Lines.ToList().Select(line =>
                         string.Join(" ", line.Words.ToList().Select(word =>
                             word.Text).ToArray())).ToArray())).ToArray());
        }
    }
}