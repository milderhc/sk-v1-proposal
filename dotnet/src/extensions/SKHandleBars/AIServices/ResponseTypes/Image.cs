﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Handlebars;

/// <summary>
/// HTTP Schema for completion response.
/// </summary>
public sealed class Image : BinaryFile
{
    public override string ToString() {
        return $"Image: {ContentType} ({Bytes?.Length} bytes)";
    }
}


