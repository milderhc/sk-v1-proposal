﻿
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Handlebars;
using ISKFunction = Microsoft.SemanticKernel.ISKFunction;
using IKernel = Microsoft.SemanticKernel.IKernel;

string AzureOpenAIDeploymentName = Env.Var("AzureOpenAI:ChatCompletionDeploymentName")!;
string AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
string AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;
string BingApiKey = Env.Var("Bing:ApiKey")!;
string currentDirectory = Directory.GetCurrentDirectory();

// Initialize the required functions and services for the kernel
ISKFunction chatFunction = SemanticFunction.GetFunctionFromYaml(currentDirectory + "/Plugins/ChatPlugin/GroundedChat.prompt.yaml");
IChatCompletion gpt35Turbo = new AzureOpenAIChatCompletion("gpt-3.5-turbo", AzureOpenAIEndpoint, AzureOpenAIApiKey, AzureOpenAIDeploymentName);

// Create the search plugin
List<ISKFunction> searchPluginFunctions = NativeFunction.GetFunctionsFromObject(new Search(BingApiKey));
searchPluginFunctions.Add(SemanticFunction.GetFunctionFromYaml(currentDirectory + "/Plugins/SearchPlugin/GetSearchQuery.prompt.yaml"));
Plugin searchPlugin = new(
    "Search",
    functions: searchPluginFunctions
);

// Create new kernel
IKernel kernel = new Kernel(
    aiServices: new () { gpt35Turbo },
    plugins: new () { searchPlugin },
    promptTemplateEngines: new () {new HandlebarsPromptTemplateEngine()},
    entryPoint: chatFunction
);

// Start the chat
ChatHistory chatHistory = gpt35Turbo.CreateNewChat();
while(true)
{
    Console.Write("User > ");
    chatHistory.AddUserMessage(Console.ReadLine()!);

    // Run the chat function
    // The grounded chat function uses the search plugin to perform a Bing search to ground the response
    // See Plugins/ChatPlugin/GroundedChat.prompt.yaml for the full prompt
    var result = await kernel.RunAsync(variables: new() {
        { "persona", "You are a snarky (yet helpful) teenage assistant. Make sure to use hip slang in every response." },
        { "messages", chatHistory }
    });

    Console.WriteLine("Assistant > " + result);
    chatHistory.AddAssistantMessage(result.GetValue<string>()!);
}