using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using OpenAI_API;
using System.Net.Http;
using System.IO;
using System.Net;

namespace Test_Chat_GPT3
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region CONSTS

        // Replace YOUR_API_KEY with your actual API key
        private const string API_KEY = /**/"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";
        // Replace YOUR_MODEL with the name of the model you want to use (e.g. "text-davinci-002")
        private const string MODEL = "text-davinci-003";

        #endregion

        #region FIELDS

        OpenAIAPI openAiApi;

        string initialText = "...";

        OpenAIAPI api;

        #endregion

        #region CONSTRUCTOR

        public MainWindow()
        {
            InitializeComponent();

            //var api = new OpenAI_API.OpenAIAPI(engine: Engine.Davinci);
            api = new OpenAIAPI(API_KEY, Engine.Davinci);

            //Fill ComboBox
            FillComboBox();
            txtQuestion.Text = initialText;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Método para obtener una respuesta de ChatGPT3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task<string> GetGPT3Response(string input)
        {
            using (var client = new HttpClient())
            {
                // Set the API key in the request header
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_KEY);

                // Set the parameters for the API request
                var content = new FormUrlEncodedContent(new[]
                {
                    //new KeyValuePair<string, string>("api_key", API_KEY),
                    new KeyValuePair<string, string>("model", MODEL),
                    new KeyValuePair<string, string>("prompt", WebUtility.UrlEncode(input)),
                    new KeyValuePair<string, string>("max_tokens", "1024"),
                    new KeyValuePair<string, string>("temperature", "0.5"),
                });

                //var contentido = api.Completions.CreateCompletionAsync("\"\"\"\nUtil exposes the following:\nutil.openai() -> authenticates & returns the openai module, which has the following functions:\nopenai.Completion.create(\n    prompt=\"<my prompt>\", # The prompt to start completing from\n    max_tokens=123, # The max number of tokens to generate\n    temperature=1.0 # A measure of randomness\n    echo=True, # Whether to return the prompt in addition to the generated completion\n)\n\"\"\"\nimport util\n\"\"\"\nCreate an OpenAI completion starting from the prompt \"Once upon an AI\", no more than 5 tokens. Does not include the prompt.\n\"\"\"\n",
                //    64, 0, 1.0);
  

                // Send the API request and get the response
                var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse the response and return the generated text
                var json = JObject.Parse(responseString);
                return (string)json["choices"][0]["text"];
            }
        }

        /// <summary>
        /// Método para lanzar consulta a la API
        /// </summary>
        /// <returns></returns>
        private async Task LanzarConsultaAsync()
        {
            try
            {
                txtAnswer.Text = "";

                #region 

                //string filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\prompt.json";

                DataXml data = new DataXml();
                data.PromptP = new DataXml.Prompt();
                data.PromptP.StartText = "Hola";
                //data.ExportPrompt(filePath);

                string filePath = "";
                int maxTokensAnswer = 200;

                //data.LoadPrompt(filePath);
                if (cmbTopics.SelectedItem.ToString() == "Vision Pro Inputs")
                {
                    filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\Coms_OnValueChanged Prompt.txt";
                    maxTokensAnswer = 300;
                }
                else if (cmbTopics.SelectedItem.ToString() == "Vidi")
                {
                    filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\Vidi Prompt.txt";
                    maxTokensAnswer = 200;
                }
                else if (cmbTopics.SelectedItem.ToString() == "C#")
                {
                    filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\startText.txt";
                    maxTokensAnswer = 200;
                }
                else if (cmbTopics.SelectedItem.ToString() == "Vista Personalizada")
                {
                    filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\WPF Prompt.txt";
                    maxTokensAnswer = 200;
                }
                else
                {
                    filePath = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\startText.txt";
                    maxTokensAnswer = 200;
                }
                //string filePath2 = @"H:\Dev\OpenAI\Test\Test Chat-GPT3\Test Prompt.txt";

                // Lee el contenido del archivo y lo guarda en una variable de tipo string
                string texto = File.ReadAllText(filePath);

                //// codifica el string
                string textoCodificado = WebUtility.UrlEncode(texto);

                #endregion
                
                string[] stopSeq = new string[] { "Pregunta" };

                //txtAnswer.Text += res.ToString();
                await api.Completions.StreamCompletionAsync(
                    new CompletionRequest(texto + " " + txtQuestion.Text, maxTokensAnswer, 0, presencePenalty: 0, frequencyPenalty: 0, numOutputs: 1, echo: false, stopSequences: stopSeq),
                    res => txtAnswer.Text += res.ToString());

            }
            catch (Exception ex)
            {

            }
        }

        private void FillComboBox()
        {
            try
            {
                List<string> topicsList = new List<string>() { "Vidi", "Vision Pro Inputs", "Outputs Vision Pro", "Comunicación", "Vista Personalizada", "C#" };

                foreach (string topic in topicsList) { cmbTopics.Items.Add(topic); }

            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region EVENTS

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LanzarConsultaAsync();
        }
        
        private async void Button_Click_NOTUSED(object sender, EventArgs e)
        {
            try
            {
                // Get the text that the user entered
                string inputText = txtQuestion.Text;
                // Clear the input text box
                txtQuestion.Text = "";

                // Use the GPT-3 API to generate a response based on the user's input
                string response = await GetGPT3Response(inputText);

                // Display the response in the chat window
                txtAnswer.Text += "You: " + inputText + Environment.NewLine;
                txtAnswer.Text += "Assistant: " + response + Environment.NewLine;
            }catch(Exception ex)
            {

            }
        }

        private void TxtQuestion_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtQuestion.Text == initialText) txtQuestion.Text = "";
        }

        #endregion


    }
}
