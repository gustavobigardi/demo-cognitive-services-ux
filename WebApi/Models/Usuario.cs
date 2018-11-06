using System;

namespace WebApi.Models
{
    public class Usuario
    {
        public string Nome { get; set; }
        public string RG { get; set; }
        public string OrgEmissor { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}