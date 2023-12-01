package com.microsoft.semantickernel.v1.assistants;

import com.microsoft.semantickernel.orchestration.SKFunction;

import reactor.core.publisher.Mono;

public interface AssistantThread extends SKFunction {

  public Mono addUserMessageAsync(String message);

}

