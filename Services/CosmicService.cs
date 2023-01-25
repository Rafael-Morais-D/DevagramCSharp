using DevagramCSharp.Dtos;
using System.Net.Http.Headers;

namespace DevagramCSharp.Services
{
    public class CosmicService
    {
        public string EnviarImagem(ImagemDto imagemdto)
        {
            Stream imagem = imagemdto.Imagem.OpenReadStream();

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "d0iAzC8si24eDTl3tQRAIrzElYKIuQi6tqrf7JFB2Liw9Ngky3");

            var request = new HttpRequestMessage(HttpMethod.Post, "file");

            var conteudo = new MultipartFormDataContent
            {
                { new StreamContent(imagem), "media", imagemdto.Nome }
            };

            request.Content = conteudo;

            var retornoreq = client.PostAsync("https://upload.cosmicjs.com/v2/buckets/devagram-rafael-devagram/media", request.Content).Result;

            var urlRetorno = retornoreq.Content.ReadFromJsonAsync<CosmicRespostaDto>();

            return urlRetorno.Result.media.url;
        }
    }
}
