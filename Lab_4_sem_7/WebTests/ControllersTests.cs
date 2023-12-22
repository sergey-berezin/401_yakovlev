using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using WebApp;
using WebApp.Controllers;
using System.Text;
using System.Net.Http.Json;
using static System.Net.Mime.MediaTypeNames;

namespace WebAppTest
{
    public class ControllersTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> waf;
        public ControllersTests(WebApplicationFactory<Program> waf)
        {
            this.waf = waf;
        }
        [Fact]
        public async Task NonEmptyTextQuestion()
        {
            var client = waf.CreateClient();
            string path_to_text = "C:\\Users\\Yakov\\Downloads\\study_7_sem\\C#\\models\\hobbit.txt";
            string text = File.ReadAllText(path_to_text);
            var response_1 =  await client.PostAsJsonAsync("https://localhost:5001/api/nn", text);
            var textId = await response_1.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.OK, response_1.StatusCode);

            string question = "What is a hobbit?";
            var response_2 = await client.GetAsync($"https://localhost:5001/api/nn?textId={textId}&question={question}");
            var answer = await response_2.Content.ReadAsStringAsync();
            //провер€ем, что ответ не пустой и содержательный (нейронка отрабатывает и св€зана корректна)
            Assert.Equal(System.Net.HttpStatusCode.OK, response_2.StatusCode);
            Assert.NotNull(answer);
            Assert.NotEqual(answer, "[CLS]");
        }
        [Fact]
        public async Task EmptyText()
        {
            var client = waf.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:5001/api/nn", "");
            var textId = await response.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task EmptyQuestion()
        {
            var client = waf.CreateClient();
            string path_to_text = "C:\\Users\\Yakov\\Downloads\\study_7_sem\\C#\\models\\hobbit.txt";
            string text = File.ReadAllText(path_to_text);
            var response_1 = await client.PostAsJsonAsync("https://localhost:5001/api/nn", text);
            var textId = await response_1.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.OK, response_1.StatusCode);
            var question = "";
            var response_2 = await client.GetAsync($"https://localhost:5001/api/nn?textId={textId}" + "&" + "question=" + question);
            var answer = await response_2.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response_2.StatusCode);
        }
    }
}