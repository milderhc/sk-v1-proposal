
package com.microsoft.semantickernel.v1.assistants;

import javax.annotation.Nullable;

import java.util.List;
import java.util.Collection;

import com.microsoft.semantickernel.plugin.Plugin;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.services.AIService;
import com.microsoft.semantickernel.templateengine.PromptTemplateEngine;
import com.microsoft.semantickernel.orchestration.SKFunction;
import com.microsoft.semantickernel.textcompletion.CompletionSKFunction;
import com.microsoft.semantickernel.semanticfunctions.SemanticFunctionConfig;
import com.microsoft.semantickernel.orchestration.ContextVariables;
import com.microsoft.semantickernel.orchestration.SKContext;
import com.microsoft.semantickernel.orchestration.FunctionResult;
import com.microsoft.semantickernel.KernelResult;
import com.microsoft.semantickernel.memory.SemanticTextMemory;
import com.microsoft.semantickernel.KernelConfig;

import reactor.core.publisher.Mono;

public class AssistantKernel implements Kernel, Plugin {

    public static AssistantKernel FromConfiguration(
        String configurationFile,
        @Nullable List<AIService> aiServices,
        @Nullable List<Plugin> plugins,
        @Nullable List<PromptTemplateEngine> promptTemplateEngines
    ) {
			return null;
    }

		public SKFunction getFunction(String skill, String function) {
			return null;
    }

		public <FunctionType extends SKFunction>FunctionType registerSemanticFunction(FunctionType func) {
			return null;
    }

    public CompletionSKFunction registerSemanticFunction(
			String skillName, String functionName, SemanticFunctionConfig functionConfig) {
			return null;
		}

		public <T extends AIService> T getService(String serviceId, Class<T> clazz) {
			return null;
		}

    public CompletionSKFunction.Builder getSemanticFunctionBuilder() {
			return null;
    }

		public Mono<KernelResult> runAsync(boolean b, ContextVariables variables, SKFunction... pipeline) {
			return null;
		}

		public Mono<KernelResult> runAsync(ContextVariables variables, SKFunction... pipeline) {
			return null;
		}

		public Mono<KernelResult> runAsync(String s, SKFunction... pipeline) {
			return null;
		}

		public Mono<KernelResult> runAsync(SKFunction... pipeline) {
			return null;
		}

		public SemanticTextMemory getMemory() {
			return null;
		}

    public PromptTemplateEngine getPromptTemplateEngine() {
			return null;
		}

		public KernelConfig getConfig() {
			return null;
		}

		public Collection<SKFunction> functions() {
			return null;
		}

		public String description() {
			return null;
		}

		public String name() {
			return null;
		}

		public Mono<AssistantThread> createThreadAsync() {
			return null;
		}
}
