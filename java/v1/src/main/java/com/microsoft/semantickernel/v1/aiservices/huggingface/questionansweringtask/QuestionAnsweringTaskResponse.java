package com.microsoft.semantickernel.v1.aiservices.huggingface.questionansweringtask;

import com.fasterxml.jackson.annotation.JsonProperty;

public interface QuestionAnsweringTaskResponse {

    @JsonProperty("score")
    double getScore();

    @JsonProperty("start")
    int getStart();

    @JsonProperty("end")
    int getEnd();

    @JsonProperty("answer")
    String getAnswer();
    
}
