package com.microsoft.semantickernel.v1;

import com.azure.ai.openai.OpenAIAsyncClient;
import com.azure.ai.openai.OpenAIClientBuilder;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.SKBuilders;
import com.azure.core.credential.KeyCredential;
import com.microsoft.semantickernel.chatcompletion.ChatCompletion;
import com.microsoft.semantickernel.chatcompletion.ChatHistory;
import com.microsoft.semantickernel.exceptions.ConfigurationException;
import com.microsoft.semantickernel.v1.semanticfunctions.SemanticFunction;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Map;

public class Main {

    final static String GPT_35_DEPLOYMENT_NAME = System.getenv("GPT_35_DEPLOYMENT_NAME");
    final static String GPT_4_DEPLOYMENT_NAME = System.getenv("GPT_4_DEPLOYMENT_NAME");
    final static String AZURE_OPENAI_ENDPOINT = System.getenv("AZURE_OPENAI_ENDPOINT");
    final static String AZURE_OPENAI_API_KEY = System.getenv("AZURE_OPENAI_API_KEY");
    final static String CURRENT_DIRECTORY = System.getProperty("user.dir");

    public static void main(String[] args) throws IOException {
        OpenAIAsyncClient client = new OpenAIClientBuilder()
                .credential(new KeyCredential(AZURE_OPENAI_API_KEY))
                .endpoint(AZURE_OPENAI_ENDPOINT)
                .buildAsyncClient();

        ChatCompletion<ChatHistory> gpt35Turbo = ChatCompletion.builder()
                .withOpenAIClient(client)
                .withModelId(GPT_35_DEPLOYMENT_NAME)
                .build();

        Kernel kernel = SKBuilders.kernel()
                .withDefaultAIService(gpt35Turbo)
                .build();

        SemanticFunction f = SemanticFunction.getFunctionFromYaml("Plugins/ChatPlugin/SimpleChat.prompt.yaml");
        f.registerOnKernel(kernel);

        ChatHistory chatHistory = gpt35Turbo.createNewChat("");

        BufferedReader bf = new BufferedReader(new InputStreamReader(System.in));
        while (true) {
            System.out.print("User > ");
            String input = bf.readLine();
            chatHistory.addMessage(ChatHistory.AuthorRoles.User, input);

            String assistantMessage = f.invokeAsync(Map.of("messages", chatHistory)).block().getResult();
            chatHistory.addMessage(ChatHistory.AuthorRoles.Assistant, assistantMessage);
            System.out.println("Assistant > " + assistantMessage);
        }
    }
}