package com.microsoft.semantickernel.v1;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.dataformat.yaml.YAMLFactory;
import com.microsoft.semantickernel.KernelConfig;
import com.microsoft.semantickernel.memory.SemanticTextMemory;
import com.microsoft.semantickernel.services.AIServiceProvider;
import com.microsoft.semantickernel.templateengine.PromptTemplateEngine;
import com.microsoft.semantickernel.v1.semanticfunctions.SemanticFunction;
import com.microsoft.semantickernel.v1.semanticfunctions.SemanticFunctionModel;
import com.microsoft.semantickernel.v1.templateengine.HandlebarsPromptTemplate;
import com.microsoft.semantickernel.v1.templateengine.HandlebarsPromptTemplateEngine;

import javax.annotation.Nullable;
import java.io.IOException;
import java.io.InputStream;

public class DefaultKernel extends com.microsoft.semantickernel.DefaultKernel implements Kernel {
    public DefaultKernel(KernelConfig kernelConfig, PromptTemplateEngine promptTemplateEngine, @Nullable SemanticTextMemory memoryStore, AIServiceProvider aiServiceProvider) {
        super(kernelConfig, promptTemplateEngine, memoryStore, aiServiceProvider);
    }

    public SemanticFunction getFunctionFromYaml(String filePath) throws IOException {
        ObjectMapper mapper = new ObjectMapper(new YAMLFactory());
        InputStream inputStream = SemanticFunction.class
                .getClassLoader()
                .getResourceAsStream(filePath);

        SemanticFunctionModel functionModel = mapper.readValue(inputStream, SemanticFunctionModel.class);

        SemanticFunction semanticFunction = new SemanticFunction.Builder()
                .withName(functionModel.getName())
                .withInputParameters(functionModel.getInputVariables())
                .withPromptTemplate(new HandlebarsPromptTemplate(functionModel.getTemplate(), new HandlebarsPromptTemplateEngine()))
                .withPluginName(functionModel.getName())
                .withExecutionSettings(functionModel.getExecutionSettings())
                .withDescription(functionModel.getDescription())
                .build();

        semanticFunction.registerOnKernel(this);
        return semanticFunction;
    }
}
