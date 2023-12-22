using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NNController : ControllerBase
    {
        public static Dictionary<int, string> Texts;
        public NeuralNetwork nn;

        public NNController(NeuralNetwork nn, Dictionary<int, string> TextsTemp)
        {
            this.nn = nn;
            Texts = TextsTemp;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int textId, string question)
        {
            if (question == null || question.Length == 0)
            {
                return BadRequest("Please, enter question");
            }
            var answer = await nn.AnswerQuestionAsync(Texts[textId], question);
            return Ok(answer.ToString());
        }

        [HttpPost]
        public ActionResult<string> PostText([FromBody] string text)
        {
            if (text == null || text.Length == 0)
            { 
                return BadRequest("Please, enter text");
            }
            int textId = GetUniqCode(text);
            Texts[textId] = text;
            return Ok(textId);
        }

        private int GetUniqCode(string text)
        {
            return text.GetHashCode();
        }
    }
}
