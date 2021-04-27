// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models;
using Microsoft.AspNetCore.Razor.LanguageServer.ProjectSystem;
using Microsoft.CodeAnalysis.Razor;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion
{
    internal class DefaultVirtualDocumentManager : VirtualDocumentManager
    {
        private readonly ForegroundDispatcher _foregroundDispatcher;
        private readonly IClientLanguageServer _server;
        private readonly DocumentResolver _documentResolver;
        private readonly WorkspaceDirectoryPathResolver _workspaceDirectoryPathResolver;

        public DefaultVirtualDocumentManager(
            ForegroundDispatcher foregroundDispatcher,
            IClientLanguageServer server,
            DocumentResolver documentResolver,
            WorkspaceDirectoryPathResolver workspaceDirectoryPathResolver)
        {
            if (foregroundDispatcher is null)
            {
                throw new ArgumentNullException(nameof(foregroundDispatcher));
            }

            if (server is null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            if (documentResolver is null)
            {
                throw new ArgumentNullException(nameof(documentResolver));
            }

            if (workspaceDirectoryPathResolver is null)
            {
                throw new ArgumentNullException(nameof(workspaceDirectoryPathResolver));
            }

            _foregroundDispatcher = foregroundDispatcher;
            _server = server;
            _documentResolver = documentResolver;
            _workspaceDirectoryPathResolver = workspaceDirectoryPathResolver;
        }

        public override async Task<string> GetContentAsync(Uri virtualDocumentUri, CancellationToken cancellationToken)
        {
            var document = await Task.Factory.StartNew(() =>
            {
                var razorDocumentUri = RazorServerLSPConventions.GetRazorDocumentUri(virtualDocumentUri);

                // TODO: HACK We have to de-relativeize the razor document
                var workspaceDirectory = _workspaceDirectoryPathResolver.Resolve();
                var documentFileNamePath = Path.Combine(workspaceDirectory, razorDocumentUri.GetAbsoluteOrUNCPath().Trim('/'));
                _documentResolver.TryResolveDocument(documentFileNamePath, out var documentSnapshot);

                Debug.Assert(documentSnapshot != null, "Failed to get the document snapshot, could not map to document ranges.");

                return documentSnapshot;
            }, cancellationToken, TaskCreationOptions.None, _foregroundDispatcher.ForegroundScheduler);

            var codeDocument = await document.GetGeneratedOutputAsync();

            if (RazorServerLSPConventions.IsVirtualCSharpFile(virtualDocumentUri))
            {
                var csharpDocument = codeDocument.GetCSharpDocument();
                return csharpDocument.GeneratedCode;
            }
            else if (RazorServerLSPConventions.IsVirtualHtmlFile(virtualDocumentUri))
            {
                var htmlDocument = codeDocument.GetHtmlDocument();
                return htmlDocument.GeneratedHtml;
            }
            else
            {
                // Unknown document
                throw new InvalidOperationException("Unknown virtual document");
            }
        }

        public override async Task InitializeVirtualDocumentsAsync(string hostDocumentFilePath, CancellationToken cancellationToken)
        {
            var openCSharpTask = SendOpenTextDocumentAsync(hostDocumentFilePath, RazorServerLSPConstants.VirtualCSharpFileNameSuffix, cancellationToken);
            var openHtmlTask = SendOpenTextDocumentAsync(hostDocumentFilePath, RazorServerLSPConstants.VirtualHtmlFileNameSuffix, cancellationToken);
            await Task.WhenAll(openCSharpTask, openHtmlTask).ConfigureAwait(false);
        }

        private async Task SendOpenTextDocumentAsync(string hostDocumentFilePath, string virtualFileSuffix, CancellationToken cancellationToken)
        {
            //var workspaceDirectory = _workspaceDirectoryPathResolver.Resolve();
            //var relativeHostDocumentFilePath = hostDocumentFilePath.Substring(workspaceDirectory.Length);
            //var fileName = Path.GetFileName(relativeHostDocumentFilePath);
            var csharpFilePath = "/" + hostDocumentFilePath + virtualFileSuffix;
            var csharpDocumentUri = new DocumentUri(RazorServerLSPConstants.EmbeddedFileScheme, authority: string.Empty, path: csharpFilePath, query: string.Empty, fragment: string.Empty);
            var openDocumentParams = new OpenTextDocumentParams()
            {
                TextDocument = new TextDocumentIdentifier(csharpDocumentUri)
            };
            var request = this._server.SendRequest("textDocument/open", openDocumentParams);
            await request.ReturningVoid(cancellationToken);
        }
    }
}
