using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace XamarinFormsApp
{
    public class Usuario : INotifyPropertyChanged
    {
        private string _nome;
        private string _rg;
        private string _orgEmissor;
        private string _cpf;
        private DateTime? _dataNascimento;

        public string Nome { get => _nome; set { _nome = value; OnPropertyChanged(); } }
        public string Rg { get => _rg; set { _rg = value; OnPropertyChanged(); } }
        public string OrgEmissor { get => _orgEmissor; set { _orgEmissor = value; OnPropertyChanged(); } }
        public string Cpf { get => _cpf; set { _cpf = value; OnPropertyChanged(); } }
        public DateTime? DataNascimento { get => _dataNascimento; set { _dataNascimento = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
