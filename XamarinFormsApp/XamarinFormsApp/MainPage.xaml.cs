using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new Usuario();
        }

        public Usuario Usuario { get; set; }

        private HttpClient _restClient;

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Ops", "Camera indisponível", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "DemoUX",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

            if (file == null)
                return;

            var stream = new MemoryStream();
            file.GetStream().CopyTo(stream);

            try
            {
                var url = "https://demofcnuvem.azurewebsites.net/api/Cadastro/read-from-doc";
                _restClient = _restClient ?? new HttpClient();

                var content = new MultipartFormDataContent();

                var t = new ByteArrayContent(stream.GetBuffer());
                t.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                content.Add(t, "file", "documento.jpg");

                var result = await _restClient.PostAsync(url, content);

                if (result.IsSuccessStatusCode)
                {
                    var jsonResult = await result.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Usuario>(jsonResult);

                    ((Usuario)BindingContext).Nome = obj.Nome;
                    ((Usuario)BindingContext).Rg = obj.Rg;
                    ((Usuario)BindingContext).OrgEmissor = obj.OrgEmissor;
                    ((Usuario)BindingContext).Cpf = obj.Cpf;
                    ((Usuario)BindingContext).DataNascimento = obj.DataNascimento;

                    await DisplayAlert("Sucesso", "Imagem carregada", "OK");
                } 
                else
                {
                    await DisplayAlert("Ops", await result.Content.ReadAsStringAsync(), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}
