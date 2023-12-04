package com.microsoft.semantickernel.v1.aiservices.huggingface.summarizationtask;

import com.fasterxml.jackson.annotation.JsonProperty;

public interface SummarizationTaskRequest {
    
    interface SummarizationTaskInputs {
        
        @JsonProperty("question")
        String getQuestion();
        
        @JsonProperty("context")
        String getContext();
    }

    @JsonProperty("inputs")
    SummarizationTaskInputs getInputs();

}
