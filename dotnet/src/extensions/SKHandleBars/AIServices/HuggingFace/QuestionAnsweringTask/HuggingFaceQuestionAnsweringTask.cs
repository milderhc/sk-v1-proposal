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
public sealed class HuggingFaceQuestionAnsweringTask : AIService
{
    private const string HuggingFaceApiEndpoint = "https://api-inference.huggingface.co/models";
    private readonly ModelRequestXmlConverter modelRequestXmlConverter = new();

    private readonly string _model;
    private readonly string? _endpoint;
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public HuggingFaceQuestionAnsweringTask(string model, string? apiKey = null, HttpClient? httpClient = null, string? endpoint = null): base(model)
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
        if (modelRequest.Context is null || !modelRequest.Context.ContainsKey("context"))
        {
            throw new SKException("HuggingFaceQuestionAnsweringTask requires a context to be specified");
        }

        var results = await this.ExecuteGetCompletionsAsync(
            userMessages[0].Content.ToString()!,
            modelRequest.Context["context"]?.ToString() ?? string.Empty
        ).ConfigureAwait(false);
        
        var result = new FunctionResult(name, pluginName, results.Answer);
        result.Metadata.Add(AIFunctionResultExtensions.ModelResultsMetadataKey, results);

        return result;
    }

    public override Task<FunctionResult> GetModelStreamingResultAsync(string pluginName, string name, string prompt)
    {
        throw new NotImplementedException();
    }

    #region private ================================================================================

    private async Task<QuestionAnsweringTaskResponse> ExecuteGetCompletionsAsync(string text, string context, CancellationToken cancellationToken = default)
    {
        var completionRequest = new QuestionAnsweringTaskRequest
        {
            Inputs = new() {
                Question = text,
                Context = context
            }
        };

        var test = JsonSerializer.Serialize(completionRequest);

        using var httpRequestMessage = HttpRequest.CreatePostRequest(this.GetRequestUri(), JsonSerializer.Serialize(completionRequest));

        httpRequestMessage.Headers.Add("User-Agent", Telemetry.HttpUserAgent);
        if (!string.IsNullOrEmpty(this._apiKey))
        {
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {this._apiKey}");
        }

        using var response = await this._httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        QuestionAnsweringTaskResponse? taskResponse = JsonSerializer.Deserialize<QuestionAnsweringTaskResponse>(body);

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
