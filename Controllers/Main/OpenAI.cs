using Devdiscourse.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Devdiscourse.Controllers.Main
{
    public class OpenAI : Controller
    {
        private static readonly List<OpenAIModel> conversation = new();
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Conversation = conversation;
            return View(new OpenAIModel { Question = "" });
        }
        [HttpPost]
        public async Task<IActionResult> Index(OpenAIModel openAIModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string endpoint = "https://api.openai.com/v1/chat/completions";
                    var messages = new[] { new { role = "user", content = openAIModel.Question } };
                    var data = new
                    {
                        model = "gpt-3.5-turbo",
                        messages
                    };
                    string jsonString = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    HttpClient client = new();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
                    var response = await client.PostAsync(endpoint, content);
                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);
                    openAIModel.Answer = jsonResponse["choices"][0]["message"]["content"].Value<string>();

                    conversation.Add(openAIModel);
                    ViewBag.Conversation = conversation;

                    return View(new OpenAIModel { Question = "" });
                }
                catch (HttpRequestException e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(openAIModel);
                }
            }
            else return View(openAIModel);
        }
    }
}
