package com.microsoft.semantickernel.v1.aiservices.responsetypes;

import java.util.UUID;

public interface BinaryFile {
    UUID getId();
    String contentType();
    byte[] getBytes();
}
