package com.microsoft.semantickernel.v1;

import com.microsoft.semantickernel.v1.semanticfunctions.SemanticFunction;

import java.io.IOException;

public interface Kernel extends com.microsoft.semantickernel.Kernel {
    public SemanticFunction getFunctionFromYaml(String filePath) throws IOException;
}
