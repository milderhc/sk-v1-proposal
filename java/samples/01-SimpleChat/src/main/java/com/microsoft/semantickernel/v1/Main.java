package com.microsoft.semantickernel.v1;

import com.azure.ai.openai.OpenAIAsyncClient;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.SKBuilders;
import com.microsoft.semantickernel.chatcompletion.ChatCompletion;
import com.microsoft.semantickernel.chatcompletion.ChatHistory;
import com.microsoft.semantickernel.connectors.ai.openai.chatcompletion.OpenAIChatHistory;
import com.microsoft.semantickernel.connectors.ai.openai.util.OpenAIClientProvider;
import com.microsoft.semantickernel.exceptions.ConfigurationException;
import com.microsoft.semantickernel.textcompletion.TextCompletion;
import com.microsoft.semantickernel.v1.functions.FunctionResult;
import com.microsoft.semantickernel.v1.semanticfunctions.SemanticFunction;
import com.microsoft.semantickernel.v1.templateengine.HandlebarsPromptTemplateEngine;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Map;

public class Main {
    public static void main(String[] args) throws ConfigurationException, IOException {
        OpenAIAsyncClient client = OpenAIClientProvider.getClient();

        Kernel kernel = SKBuilders.kernel()
                .withDefaultAIService(SKBuilders.chatCompletion()
                        .withOpenAIClient(client)
                        .withModelId("gpt-35-turbo")
                        .build())
                .withPromptTemplateEngine(new HandlebarsPromptTemplateEngine())
                .build();

        ChatCompletion<OpenAIChatHistory> chat = kernel.getService(null, ChatCompletion.class);
        ChatHistory chatHistory = chat.createNewChat("  You are a helpful assistant.");

        SemanticFunction f = SemanticFunction.getFunctionFromYaml("Plugins/ChatPlugin/SimpleChat.prompt.yaml");
        //TODO register automatically
        f.registerOnKernel(kernel);

        String userInput;
        BufferedReader bf = new BufferedReader(new InputStreamReader(System.in));

        while (true) {
            String assistantMessage = f.invokeAsync(Map.of("messages", chatHistory)).block().getResult();
            chatHistory.addMessage(ChatHistory.AuthorRoles.Assistant, assistantMessage);
            messageOutputAsync(chatHistory);

            userInput = bf.readLine();
            f.invokeAsync(Map.of("messages", chatHistory)).block().getResult();
            chatHistory.addMessage(ChatHistory.AuthorRoles.User, userInput);
            messageOutputAsync(chatHistory);
        }
    }

    private static void messageOutputAsync(ChatHistory chatHistory) {
        ChatHistory.Message message = chatHistory.getMessages().get(chatHistory.getMessages().size() - 1);

        System.out.println(message.getAuthorRoles() + ": " + message.getContent());
        System.out.println("------------------------");
    }
}