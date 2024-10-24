﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.LanguageServer.CodeActions.Models;
using Microsoft.AspNetCore.Razor.Test.Common.LanguageServer;
using Microsoft.AspNetCore.Razor.Threading;
using Microsoft.CodeAnalysis.Razor.Formatting;
using Microsoft.CodeAnalysis.Razor.ProjectSystem;
using Microsoft.CodeAnalysis.Razor.Protocol;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Razor.LanguageServer.CodeActions;

public class CodeActionResolutionEndpointTest(ITestOutputHelper testOutput) : LanguageServerTestBase(testOutput)
{
    [Fact]
    public async Task Handle_Valid_RazorCodeAction_WithResolver()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [new MockRazorCodeActionResolver("Test")],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.Razor,
            Data = new AddUsingsCodeActionParams()
            {
                Namespace = "Test",
            }
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

        // Act
        var razorCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.NotNull(razorCodeAction.Edit);
    }

    [Fact]
    public async Task Handle_Valid_CSharpCodeAction_WithResolver()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            [new MockCSharpCodeActionResolver("Test")],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.CSharp,
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

        // Act
        var razorCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.NotNull(razorCodeAction.Edit);
    }

    [Fact]
    public async Task Handle_Valid_CSharpCodeAction_WithMultipleLanguageResolvers()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [new MockRazorCodeActionResolver("TestRazor")],
            csharpCodeActionResolvers: [new MockCSharpCodeActionResolver("TestCSharp")],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "TestCSharp",
            Language = RazorLanguageKind.CSharp,
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

        // Act
        var razorCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.NotNull(razorCodeAction.Edit);
    }

    [Fact(Skip = "Debug.Fail fails in CI")]
    public async Task Handle_Valid_RazorCodeAction_WithoutResolver()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.Razor,
            Data = new AddUsingsCodeActionParams()
            {
                Namespace = "Test",
            }
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

#if DEBUG
        // Act & Assert (Throws due to debug assert on no Razor.Test resolver)
        await Assert.ThrowsAnyAsync<Exception>(async () => await codeActionEndpoint.HandleRequestAsync(request, requestContext, default));
#else
        // Act
        var resolvedCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.Null(resolvedCodeAction.Edit);
#endif
    }

    [Fact(Skip = "Debug.Fail fails in CI")]
    public async Task Handle_Valid_CSharpCodeAction_WithoutResolver()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.CSharp,
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

#if DEBUG
        // Act & Assert (Throws due to debug assert on no resolver registered for CSharp.Test)
        await Assert.ThrowsAnyAsync<Exception>(async () => await codeActionEndpoint.HandleRequestAsync(request, requestContext, default));
#else
        // Act
        var resolvedCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.Null(resolvedCodeAction.Edit);
#endif
    }

    [Fact(Skip = "Debug.Fail fails in CI")]
    public async Task Handle_Valid_RazorCodeAction_WithCSharpResolver_ResolvesNull()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [new MockCSharpCodeActionResolver("Test")],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.Razor,
            Data = new AddUsingsCodeActionParams()
            {
                Namespace = "Test",
            }
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

#if DEBUG
        // Act & Assert (Throws due to debug assert on no resolver registered for Razor.Test)
        await Assert.ThrowsAnyAsync<Exception>(async () => await codeActionEndpoint.HandleRequestAsync(request, requestContext, default));
#else
        // Act
        var resolvedCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.Null(resolvedCodeAction.Edit);
#endif
    }

    [Fact(Skip = "Debug.Fail fails in CI")]
    public async Task Handle_Valid_CSharpCodeAction_WithRazorResolver_ResolvesNull()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [new MockRazorCodeActionResolver("Test")],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "Test",
            Language = RazorLanguageKind.CSharp,
        };
        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

#if DEBUG
        // Act & Assert (Throws due to debug asserts)
        await Assert.ThrowsAnyAsync<Exception>(async () => await codeActionEndpoint.HandleRequestAsync(request, requestContext, default));
#else
        // Act
        var resolvedCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.Null(resolvedCodeAction.Edit);
#endif
    }

    [Fact]
    public async Task ResolveRazorCodeAction_ResolveMultipleRazorProviders_FirstMatches()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [
                new MockRazorCodeActionResolver("A"),
                new MockRazorNullCodeActionResolver("B"),
            ],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var codeAction = new CodeAction();
        var request = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "A",
            Language = RazorLanguageKind.Razor,
            Data = JsonSerializer.SerializeToElement(new AddUsingsCodeActionParams()
            {
                Namespace = "Test",
            }),
        };

        // Act
        var resolvedCodeAction = await codeActionEndpoint.GetTestAccessor().ResolveRazorCodeActionAsync(documentContext, codeAction, request, new RazorFormattingOptions(), DisposalToken);

        // Assert
        Assert.NotNull(resolvedCodeAction.Edit);
    }

    [Fact]
    public async Task ResolveRazorCodeAction_ResolveMultipleRazorProviders_SecondMatches()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [
                new MockRazorNullCodeActionResolver("A"),
                new MockRazorCodeActionResolver("B"),
            ],
            csharpCodeActionResolvers: [],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var codeAction = new CodeAction();
        var request = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "B",
            Language = RazorLanguageKind.Razor,
            Data = JsonSerializer.SerializeToElement(new AddUsingsCodeActionParams()
            {
                Namespace = "Test",
            })
        };

        // Act
        var resolvedCodeAction = await codeActionEndpoint.GetTestAccessor().ResolveRazorCodeActionAsync(documentContext, codeAction, request, new RazorFormattingOptions(), DisposalToken);

        // Assert
        Assert.NotNull(resolvedCodeAction.Edit);
    }

    [Fact]
    public async Task ResolveCSharpCodeAction_ResolveMultipleCSharpProviders_FirstMatches()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [
                new MockCSharpCodeActionResolver("A"),
                new MockCSharpNullCodeActionResolver("B"),
            ],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var codeAction = new CodeAction();
        var request = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "A",
            Language = RazorLanguageKind.CSharp,
        };

        // Act
        var resolvedCodeAction = await codeActionEndpoint.GetTestAccessor().ResolveCSharpCodeActionAsync(documentContext, codeAction, request, DisposalToken);

        // Assert
        Assert.NotNull(resolvedCodeAction.Edit);
    }

    [Fact]
    public async Task ResolveCSharpCodeAction_ResolveMultipleCSharpProviders_SecondMatches()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [
                new MockCSharpNullCodeActionResolver("A"),
                new MockCSharpCodeActionResolver("B"),
            ],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var codeAction = new CodeAction();
        var request = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "B",
            Language = RazorLanguageKind.Razor,
        };

        // Act
        var resolvedCodeAction = await codeActionEndpoint.GetTestAccessor().ResolveCSharpCodeActionAsync(documentContext, codeAction, request, DisposalToken);

        // Assert
        Assert.NotNull(resolvedCodeAction.Edit);
    }

    [Fact]
    public async Task ResolveCSharpCodeAction_ResolveMultipleLanguageProviders()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [
                new MockRazorNullCodeActionResolver("A"),
                new MockRazorCodeActionResolver("B"),
            ],
            csharpCodeActionResolvers: [
                new MockCSharpNullCodeActionResolver("C"),
                new MockCSharpCodeActionResolver("D"),
            ],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var codeAction = new CodeAction();
        var request = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = "D",
            Language = RazorLanguageKind.CSharp,
        };

        // Act
        var resolvedCodeAction = await codeActionEndpoint.GetTestAccessor().ResolveCSharpCodeActionAsync(documentContext, codeAction, request, DisposalToken);

        // Assert
        Assert.NotNull(resolvedCodeAction.Edit);
    }

    [Fact]
    public async Task Handle_ResolveEditBasedCodeActionCommand()
    {
        // Arrange
        var documentContext = TestDocumentContext.Create(new Uri("C:/path/to/Page.razor"));
        var codeActionEndpoint = new CodeActionResolveEndpoint(
            razorCodeActionResolvers: [],
            csharpCodeActionResolvers: [new MockCSharpCodeActionResolver("Test")],
            htmlCodeActionResolvers: [],
            TestRazorLSPOptionsMonitor.Create(),
            LoggerFactory);
        var requestParams = new RazorCodeActionResolutionParams()
        {
            TextDocument = (VSTextDocumentIdentifier)documentContext.GetTextDocumentIdentifier(),
            Action = LanguageServerConstants.CodeActions.EditBasedCodeActionCommand,
            Language = RazorLanguageKind.Razor,
            Data = JsonSerializer.SerializeToElement(new WorkspaceEdit())
        };

        var request = new CodeAction()
        {
            Title = "Valid request",
            Data = JsonSerializer.SerializeToElement(requestParams)
        };
        var requestContext = CreateRazorRequestContext(documentContext);

        // Act
        var razorCodeAction = await codeActionEndpoint.HandleRequestAsync(request, requestContext, DisposalToken);

        // Assert
        Assert.NotNull(razorCodeAction.Edit);
    }

    private class MockRazorCodeActionResolver : IRazorCodeActionResolver
    {
        public string Action { get; }

        internal MockRazorCodeActionResolver(string action)
        {
            Action = action;
        }

        public Task<WorkspaceEdit?> ResolveAsync(DocumentContext documentContext, JsonElement data, RazorFormattingOptions options, CancellationToken cancellationToken)
        {
            return Task.FromResult<WorkspaceEdit?>(new WorkspaceEdit());
        }
    }

    private class MockRazorNullCodeActionResolver : IRazorCodeActionResolver
    {
        public string Action { get; }

        internal MockRazorNullCodeActionResolver(string action)
        {
            Action = action;
        }

        public Task<WorkspaceEdit?> ResolveAsync(DocumentContext documentContext, JsonElement data, RazorFormattingOptions options, CancellationToken cancellationToken)
        {
            return SpecializedTasks.Null<WorkspaceEdit>();
        }
    }

    private class MockCSharpCodeActionResolver : ICSharpCodeActionResolver
    {
        public string Action { get; }

        internal MockCSharpCodeActionResolver(string action)
        {
            Action = action;
        }

        public Task<CodeAction> ResolveAsync(DocumentContext documentContext, CodeAction codeAction, CancellationToken cancellationToken)
        {
            codeAction.Edit = new WorkspaceEdit();
            return Task.FromResult(codeAction);
        }
    }

    private class MockCSharpNullCodeActionResolver : ICSharpCodeActionResolver
    {
        public string Action { get; }

        internal MockCSharpNullCodeActionResolver(string action)
        {
            Action = action;
        }

        public Task<CodeAction> ResolveAsync(DocumentContext documentContext, CodeAction codeAction, CancellationToken cancellationToken)
        {
            // This is deliberately returning null when it's not supposed to, so that if this code action
            // is ever returned by a method, the test will fail
            return Task.FromResult<CodeAction>(null!);
        }
    }
}
