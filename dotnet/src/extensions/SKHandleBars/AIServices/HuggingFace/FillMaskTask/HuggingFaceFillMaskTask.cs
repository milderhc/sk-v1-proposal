﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Handlebars;

/// <summary>
/// HuggingFace text completion service.
/// </summary>
public sealed class HuggingFaceFillMaskTask : AIService
{
    private const string HuggingFaceApiEndpoint = "https://api-inference.huggingface.co/models";
    private readonly ModelRequestXmlConverter modelRequestXmlConverter = new();

    private readonly string _model;
    private readonly string? _endpoint;
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public HuggingFaceFillMaskTask(string model, string? apiKey = null, HttpClient? httpClient = null, string? endpoint = null): base(model)
    {
        this._model = model;
        this._apiKey = apiKey;
        this._httpClient = httpClient ?? new HttpClient();
        this._endpoint = endpoint;
    }

    public async override Task<FunctionResult> GetModelResultAsync(string pluginName, string name, string prompt)
    {
        ModelRequest modelRequest = modelRequestXmlConverter.ParseXml(prompt);

        var userMessages = modelRequest.Messages!.Where(x => x.Role == "user").ToList();

        if (userMessages.Count != 1)
        {
            throw new SKException("HuggingFaceSummarizationTask only supports a single user message");
        }

        var results = await this.ExecuteGetCompletionsAsync(userMessages[0].Content.ToString()!).ConfigureAwait(false);
        
        var result = new FunctionResult(name, pluginName, results[0].TokenStr);
        result.Metadata.Add(AIFunctionResultExtensions.ModelResultsMetadataKey, results);

        return result;
    }

    public override Task<FunctionResult> GetModelStreamingResultAsync(string pluginName, string name, string prompt)
    {
        throw new NotImplementedException();
    }

    #region private ================================================================================

    private async Task<List<FillMaskTaskResponse>> ExecuteGetCompletionsAsync(string text, CancellationToken cancellationToken = default)
    {
        var completionRequest = new FillMaskTaskRequest
        {
            Input = text
        };

        using var httpRequestMessage = HttpRequest.CreatePostRequest(this.GetRequestUri(), completionRequest);

        httpRequestMessage.Headers.Add("User-Agent", Telemetry.HttpUserAgent);
        if (!string.IsNullOrEmpty(this._apiKey))
        {
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {this._apiKey}");
        }

        using var response = await this._httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        List<FillMaskTaskResponse>? taskResponse = JsonSerializer.Deserialize<List<FillMaskTaskResponse>>(body);

        if (taskResponse is null)
        {
            throw new SKException("Unexpected response from model")
            {
                Data = { { "ResponseData", body } },
            };
        }

        return taskResponse;
    }

    /// <summary>
    /// Retrieves the request URI based on the provided endpoint and model information.
    /// </summary>
    /// <returns>
    /// A <see cref="Uri"/> object representing the request URI.
    /// </returns>
    private Uri GetRequestUri()
    {
        var baseUrl = HuggingFaceApiEndpoint;

        if (!string.IsNullOrEmpty(this._endpoint))
        {
            return new Uri(this._endpoint);
        }
        else if (this._httpClient.BaseAddress?.AbsoluteUri != null)
        {
            baseUrl = this._httpClient.BaseAddress!.AbsoluteUri;
        }

        return new Uri($"{baseUrl!.TrimEnd('/')}/{this._model}");
    }

    #endregion
}
