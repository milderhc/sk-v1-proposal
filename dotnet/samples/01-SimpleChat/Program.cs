﻿
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Handlebars;
using ISKFunction = Microsoft.SemanticKernel.ISKFunction;
using IKernel = Microsoft.SemanticKernel.IKernel;

string Gpt35TurboDeploymentName = Env.Var("AzureOpenAI:Gpt35TurboDeploymentName")!;
string Gpt4DeploymentName = Env.Var("AzureOpenAI:Gpt4DeploymentName")!;
string AzureOpenAIEndpoint = Env.Var("AzureOpenAI:Endpoint")!;
string AzureOpenAIApiKey = Env.Var("AzureOpenAI:ApiKey")!;
string currentDirectory = Directory.GetCurrentDirectory();

// Initialize the required functions and services for the kernel
ISKFunction chatFunction = SemanticFunction.GetFunctionFromYaml(currentDirectory + "/Plugins/ChatPlugin/SimpleChat.prompt.yaml");
IChatCompletion gpt35Turbo = new AzureOpenAIChatCompletion("gpt-3.5-turbo", AzureOpenAIEndpoint, AzureOpenAIApiKey, Gpt35TurboDeploymentName);
IChatCompletion gpt4 = new AzureOpenAIChatCompletion("gpt-4", AzureOpenAIEndpoint, AzureOpenAIApiKey, Gpt4DeploymentName);

// Create a new kernel
IKernel kernel = new Kernel(
    aiServices: new () { gpt35Turbo, gpt4 },
    promptTemplateEngines: new () {new HandlebarsPromptTemplateEngine()}
);

// Start the chat
ChatHistory chatHistory = gpt35Turbo.CreateNewChat();
while(true)
{
    Console.Write("User > ");
    chatHistory.AddUserMessage(Console.ReadLine()!);

    // Run the simple chat
    // The simple chat function uses the messages variable to generate the next message
    // see Plugins/ChatPlugin/SimpleChat.prompt.yaml for the full prompt
    var result = await kernel.RunAsync(
        chatFunction,
        variables: new() {{ "messages", chatHistory }},
        streaming: true
    );

    Console.Write("Assistant > ");
    await foreach(var message in result.GetStreamingValue<string>()!)
    {
        Console.Write(message);
    }
    Console.WriteLine();
    chatHistory.AddAssistantMessage(await result.GetValueAsync<string>()!);
}
