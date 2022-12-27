﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.AspNetCore.Razor.LanguageServer.EndpointContracts;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.AspNetCore.Razor.Telemetry;

namespace Microsoft.AspNetCore.Razor.LanguageServer;

internal class RazorRequestContextFactory : IRequestContextFactory<RazorRequestContext>
{
    private readonly ILspServices _lspServices;

    public RazorRequestContextFactory(ILspServices lspServices)
    {
        _lspServices = lspServices;
    }

    public async Task<RazorRequestContext> CreateRequestContextAsync<TRequestParams>(IQueueItem<RazorRequestContext> queueItem, TRequestParams @params, CancellationToken cancellationToken)
    {
        DocumentContext? documentContext = null;
        var textDocumentHandler = queueItem.MethodHandler as ITextDocumentIdentifierHandler;

        Uri? uri = null;
        if (textDocumentHandler is not null)
        {
            if (textDocumentHandler is ITextDocumentIdentifierHandler<TRequestParams, TextDocumentIdentifier> tdiHandler)
            {
                var textDocumentIdentifier = tdiHandler.GetTextDocumentIdentifier(@params);
                uri = textDocumentIdentifier.Uri;
            }
            else if (textDocumentHandler is ITextDocumentIdentifierHandler<TRequestParams, Uri> uriHandler)
            {
                uri = uriHandler.GetTextDocumentIdentifier(@params);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        if (uri is not null)
        {
            var documentContextFactory = _lspServices.GetRequiredService<DocumentContextFactory>();
            documentContext = await documentContextFactory.TryCreateAsync(uri, cancellationToken);
        }

        var loggerAdapter = (LoggerAdapter?)_lspServices.TryGetService(typeof(LoggerAdapter));
        if (loggerAdapter is null)
        {
            var loggerFactory = _lspServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(queueItem.MethodName);
            loggerAdapter = new LoggerAdapter(new[] { logger }, _lspServices.GetRequiredService<ITelemetryReporter>());
        }

        var requestContext = new RazorRequestContext(documentContext, loggerAdapter, _lspServices);

        return requestContext;
    }
}
