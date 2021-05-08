// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.LanguageServer.Common;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion;
using Microsoft.AspNetCore.Razor.LanguageServer.ProjectSystem;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using HoverModel = OmniSharp.Extensions.LanguageServer.Protocol.Models.Hover;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Hover
{
    internal class RazorHoverEndpoint : IHoverHandler
    {
        private HoverCapability _capability;
        private readonly ILogger _logger;
        private readonly ForegroundDispatcher _foregroundDispatcher;
        private readonly DocumentResolver _documentResolver;
        private readonly RazorHoverInfoService _hoverInfoService;
        private readonly IClientLanguageServer _server;
        private readonly RazorDocumentMappingService _documentMappingService;

        public RazorHoverEndpoint(
            ForegroundDispatcher foregroundDispatcher,
            DocumentResolver documentResolver,
            RazorHoverInfoService hoverInfoService,
            IClientLanguageServer server,
            RazorDocumentMappingService documentMappingService,
            ILoggerFactory loggerFactory)
        {
            if (foregroundDispatcher is null)
            {
                throw new ArgumentNullException(nameof(foregroundDispatcher));
            }

            if (documentResolver is null)
            {
                throw new ArgumentNullException(nameof(documentResolver));
            }

            if (hoverInfoService is null)
            {
                throw new ArgumentNullException(nameof(hoverInfoService));
            }

            if (server is null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            if (documentMappingService is null)
            {
                throw new ArgumentNullException(nameof(documentMappingService));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _foregroundDispatcher = foregroundDispatcher;
            _documentResolver = documentResolver;
            _hoverInfoService = hoverInfoService;
            _server = server;
            _documentMappingService = documentMappingService;
            _logger = loggerFactory.CreateLogger<RazorHoverEndpoint>();
        }

        public async Task<HoverModel> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var documentFilePath = request.TextDocument.Uri.GetAbsoluteOrUNCPath();
            var document = await Task.Factory.StartNew(() =>
            {
                _documentResolver.TryResolveDocument(documentFilePath, out var documentSnapshot);

                return documentSnapshot;
            }, cancellationToken, TaskCreationOptions.None, _foregroundDispatcher.ForegroundScheduler);

            if (document is null)
            {
                return null;
            }

            var codeDocument = await document.GetGeneratedOutputAsync();
            if (codeDocument.IsUnsupported())
            {
                return null;
            }

            var sourceText = await document.GetTextAsync();
            var linePosition = new LinePosition((int)request.Position.Line, (int)request.Position.Character);
            var hostDocumentIndex = sourceText.Lines.GetPosition(linePosition);
            var location = new SourceLocation(hostDocumentIndex, (int)request.Position.Line, (int)request.Position.Character);

            var hoverItem = _hoverInfoService.GetHoverInfo(codeDocument, location);

            var languageKind = _documentMappingService.GetLanguageKind(codeDocument, hostDocumentIndex);
            if (languageKind == RazorLanguageKind.CSharp)
            {
                var delegatedHoverParams = request;
                var virtualFilePath = "/" + documentFilePath + RazorServerLSPConstants.VirtualCSharpFileNameSuffix;
                var virtualDocumentUri = new DocumentUri(RazorServerLSPConstants.EmbeddedFileScheme, authority: string.Empty, path: virtualFilePath, query: string.Empty, fragment: string.Empty);
                delegatedHoverParams.TextDocument.Uri = virtualDocumentUri;
                if (_documentMappingService.TryMapToProjectedDocumentPosition(codeDocument, hostDocumentIndex, out var projectedPosition, out var projectedIndex))
                {
                    delegatedHoverParams.Position = projectedPosition;
                }
                delegatedHoverParams.WorkDoneToken = null;
                var delegatedRequest = _server.SendRequest("textDocument/hover", delegatedHoverParams);
                var hoverModels = await delegatedRequest.Returning<HoverModel[]>(cancellationToken).ConfigureAwait(false);
                if (hoverModels != null && hoverModels.Length > 0)
                {
                    hoverItem = hoverModels[0];

                    if (hoverItem.Range != null &&
                        _documentMappingService.TryMapFromProjectedDocumentRange(codeDocument, hoverItem.Range, out var mappedRange))
                    {
                        hoverItem.Range = mappedRange;
                    }
                }
            }
            else if (languageKind == RazorLanguageKind.Html)
            {
                var delegatedHoverParams = request;
                var virtualFilePath = "/" + documentFilePath + RazorServerLSPConstants.VirtualHtmlFileNameSuffix;
                var virtualDocumentUri = new DocumentUri(RazorServerLSPConstants.EmbeddedFileScheme, authority: string.Empty, path: virtualFilePath, query: string.Empty, fragment: string.Empty);
                delegatedHoverParams.TextDocument.Uri = virtualDocumentUri;
                if (_documentMappingService.TryMapToProjectedDocumentPosition(codeDocument, hostDocumentIndex, out var projectedPosition, out var projectedIndex))
                {
                    delegatedHoverParams.Position = projectedPosition;
                }
                delegatedHoverParams.WorkDoneToken = null;
                var delegatedRequest = _server.SendRequest("textDocument/hover", delegatedHoverParams);
                var hoverModels = await delegatedRequest.Returning<HoverModel[]>(cancellationToken).ConfigureAwait(false);
                if (hoverModels != null && hoverModels.Length > 0)
                {
                    hoverItem = hoverModels[0];
                }
            }

            _logger.LogTrace($"Found hover info items.");

            return hoverItem;
        }

        public void SetCapability(HoverCapability capability)
        {
            _capability = capability;
        }


        public HoverRegistrationOptions GetRegistrationOptions()
        {
            return new HoverRegistrationOptions
            {
                DocumentSelector = RazorDefaults.Selector,
            };
        }
    }
}
