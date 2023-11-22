package com.microsoft.semantickernel.v1;

import com.microsoft.semantickernel.builders.BuildersSingleton;
import com.microsoft.semantickernel.textcompletion.TextCompletion;

public class SKBuilders {
    public static Kernel.Builder kernel() {
        return BuildersSingleton.INST.getInstance(Kernel.Builder.class);
    }

    public static TextCompletion.Builder textCompletion() {
        return BuildersSingleton.INST.getInstance(TextCompletion.Builder.class);
    }
}
