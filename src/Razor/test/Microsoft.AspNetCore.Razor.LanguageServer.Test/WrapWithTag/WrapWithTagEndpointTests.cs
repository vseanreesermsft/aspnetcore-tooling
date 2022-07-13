﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.LanguageServer.Common;
using Microsoft.AspNetCore.Razor.LanguageServer.Common.Extensions;
using Microsoft.AspNetCore.Razor.LanguageServer.EndpointContracts.WrapWithTag;
using Microsoft.AspNetCore.Razor.LanguageServer.Protocol;
using Microsoft.AspNetCore.Razor.Test.Common;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using Xunit;
using Range = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace Microsoft.AspNetCore.Razor.LanguageServer.WrapWithTag
{
    public class WrapWithTagEndpointTest : LanguageServerTestBase
    {
        [Fact]
        public async Task Handle_Html_ReturnsResult()
        {
            // Arrange
            var codeDocument = TestRazorCodeDocument.Create("<div></div>");
            var uri = new Uri("file://path/test.razor");
            var documentContextFactory = CreateDocumentContextFactory(uri, codeDocument);
            var responseRouterReturns = new Mock<IResponseRouterReturns>(MockBehavior.Strict);
            responseRouterReturns
                .Setup(l => l.Returning<WrapWithTagResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WrapWithTagResponse());

            var languageServer = new Mock<ClientNotifierServiceBase>(MockBehavior.Strict);
            languageServer
                .Setup(l => l.SendRequestAsync(LanguageServerConstants.RazorWrapWithTagEndpoint, It.IsAny<WrapWithTagParamsBridge>()))
                .ReturnsAsync(responseRouterReturns.Object);

            var documentMappingService = Mock.Of<RazorDocumentMappingService>(
                s => s.GetLanguageKind(codeDocument, It.IsAny<int>(), It.IsAny<bool>()) == RazorLanguageKind.Html, MockBehavior.Strict);
            var endpoint = new WrapWithTagEndpoint(
                languageServer.Object,
                documentContextFactory,
                documentMappingService,
                LoggerFactory);

            var wrapWithDivParams = new WrapWithTagParamsBridge(new TextDocumentIdentifier { Uri = uri })
            {
                Range = new Range { Start = new Position(0, 0), End = new Position(0, 2) },
            };

            // Act
            var result = await endpoint.Handle(wrapWithDivParams, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            languageServer.Verify();
        }

        [Fact]
        public async Task Handle_CSharp_ReturnsNull()
        {
            // Arrange
            var codeDocument = TestRazorCodeDocument.Create("@counter");
            var uri = new Uri("file://path/test.razor");
            var documentContextFactory = CreateDocumentContextFactory(uri, codeDocument);
            var responseRouterReturns = new Mock<IResponseRouterReturns>(MockBehavior.Strict);
            responseRouterReturns
                .Setup(l => l.Returning<WrapWithTagResponse>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WrapWithTagResponse());

            var languageServer = new Mock<ClientNotifierServiceBase>(MockBehavior.Strict);
            languageServer
                .Setup(l => l.SendRequestAsync(LanguageServerConstants.RazorWrapWithTagEndpoint, It.IsAny<WrapWithTagParamsBridge>()))
                .ReturnsAsync(responseRouterReturns.Object);

            var documentMappingService = Mock.Of<RazorDocumentMappingService>(
                s => s.GetLanguageKind(codeDocument, It.IsAny<int>(), It.IsAny<bool>()) == RazorLanguageKind.CSharp, MockBehavior.Strict);
            var endpoint = new WrapWithTagEndpoint(
                languageServer.Object,
                documentContextFactory,
                documentMappingService,
                LoggerFactory);

            var wrapWithDivParams = new WrapWithTagParamsBridge(new TextDocumentIdentifier { Uri = uri })
            {
                Range = new Range { Start = new Position(0, 0), End = new Position(0, 2) },
            };

            // Act
            var result = await endpoint.Handle(wrapWithDivParams, CancellationToken.None);

            // Assert
            Assert.Null(result);
            languageServer.Verify();
        }

        [Fact]
        public async Task Handle_DocumentNotFound_ReturnsNull()
        {
            // Arrange
            var codeDocument = TestRazorCodeDocument.Create("<div></div>");
            var realUri = new Uri("file://path/test.razor");
            var missingUri = new Uri("file://path/nottest.razor");
            var documentContextFactory = CreateDocumentContextFactory(missingUri, codeDocument, documentFound: false);

            var languageServer = new Mock<ClientNotifierServiceBase>(MockBehavior.Strict);

            var documentMappingService = Mock.Of<RazorDocumentMappingService>(
                s => s.GetLanguageKind(codeDocument, It.IsAny<int>(), It.IsAny<bool>()) == RazorLanguageKind.Html, MockBehavior.Strict);
            var endpoint = new WrapWithTagEndpoint(
                languageServer.Object,
                documentContextFactory,
                documentMappingService,
                LoggerFactory);

            var wrapWithDivParams = new WrapWithTagParamsBridge(new TextDocumentIdentifier { Uri = missingUri })
            {
                Range = new Range { Start = new Position(0, 0), End = new Position(0, 2) },
            };

            // Act
            var result = await endpoint.Handle(wrapWithDivParams, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_UnsupportedCodeDocument_ReturnsNull()
        {
            // Arrange
            var codeDocument = TestRazorCodeDocument.Create("<div></div>");
            codeDocument.SetUnsupported();
            var uri = new Uri("file://path/test.razor");
            var documentContextFactory = CreateDocumentContextFactory(uri, codeDocument);

            var languageServer = new Mock<ClientNotifierServiceBase>(MockBehavior.Strict);

            var documentMappingService = Mock.Of<RazorDocumentMappingService>(
                s => s.GetLanguageKind(codeDocument, It.IsAny<int>(), It.IsAny<bool>()) == RazorLanguageKind.Html, MockBehavior.Strict);
            var endpoint = new WrapWithTagEndpoint(
                languageServer.Object,
                documentContextFactory,
                documentMappingService,
                LoggerFactory);

            var wrapWithDivParams = new WrapWithTagParamsBridge(new TextDocumentIdentifier { Uri = uri })
            {
                Range = new Range { Start = new Position(0, 0), End = new Position(0, 2) },
            };

            // Act
            var result = await endpoint.Handle(wrapWithDivParams, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}