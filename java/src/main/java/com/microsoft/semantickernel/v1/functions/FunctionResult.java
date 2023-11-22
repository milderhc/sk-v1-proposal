package com.microsoft.semantickernel.v1.functions;

public class FunctionResult {

    private String result;

    public FunctionResult () {
    }

    public FunctionResult(String result) {
        this.result = result;
    }

    public String getResult () {
        return this.result;
    }
}
