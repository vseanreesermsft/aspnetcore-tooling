// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion.EndpointContracts;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion
{
    internal class RazorFileSystemEndpoint : IFileSystemHandler, IRegistrationExtension
    {
        private readonly FileSystemProvider _fileSystemProvider;

        public RazorFileSystemEndpoint(FileSystemProvider fileSystemProvider)
        {
            if (fileSystemProvider is null)
            {
                throw new ArgumentNullException(nameof(fileSystemProvider));
            }

            _fileSystemProvider = fileSystemProvider;
        }

        public RegistrationExtensionResult GetRegistration()
        {
            var options = new FileSystemProviderOptions()
            {
                Scheme = RazorServerLSPConstants.EmbeddedFileScheme,
                IsCaseSensitive = true,
                IsReadonly = true,
            };
            var registrationResult = new RegistrationExtensionResult("fileSystem", options);
            return registrationResult;
        }

        public async Task<ReadFileResponse> Handle(ReadFileParams request, CancellationToken cancellationToken)
        {
            var uri = request.Uri.ToUri();
            var content = await _fileSystemProvider.ReadFileAsync(uri, cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(content);
            var base64Content = Convert.ToBase64String(bytes);
            var response = new ReadFileResponse()
            {
                Content = base64Content,
            };

            return response;
        }

        public Task<Unit> Handle(WatchParams request, CancellationToken cancellationToken)
        {
            _fileSystemProvider.Watch(request.Uri, request.SubscriptionId, request.Options);
            return Unit.Task;
        }

        public Task<Unit> Handle(StopWatchingParams request, CancellationToken cancellationToken)
        {
            _fileSystemProvider.StopWatching(request.SubscriptionId);
            return Unit.Task;
        }

        public Task<FileStatResponse> Handle(FileStatParams request, CancellationToken cancellationToken)
        {// TODO: NEED TO IMPLEMENT
            var uri = request.Uri.ToUri();
            var fileStat = _fileSystemProvider.Stat(uri);
            var response = new FileStatResponse(fileStat);
            return Task.FromResult(response);
        }

        public Task<ReadDirectoryResponse> Handle(ReadDirectoryParams request, CancellationToken cancellationToken)
        {
            var children = _fileSystemProvider.ReadDirectory(request.Uri);
            var response = new ReadDirectoryResponse()
            {
                Children = children,
            };
            return Task.FromResult(response);
        }

        public Task<Unit> Handle(CreateDirectoryParams request, CancellationToken cancellationToken)
        {
            _fileSystemProvider.CreateDirectory(request.Uri);
            return Unit.Task;
        }

        public Task<Unit> Handle(WriteFileParams request, CancellationToken cancellationToken)
        {
            var base64Content = request.Content;
            var data = Convert.FromBase64String(base64Content);
            var content = Encoding.UTF8.GetString(data);
            _fileSystemProvider.WriteFile(request.Uri, content, request.Options);
            return Unit.Task;
        }

        public Task<Unit> Handle(DeleteFileParams request, CancellationToken cancellationToken)
        {
            _fileSystemProvider.Delete(request.Uri, request.Options);
            return Unit.Task;
        }

        public Task<Unit> Handle(RenameFileParams request, CancellationToken cancellationToken)
        {
            _fileSystemProvider.Rename(request.OldUri, request.NewUri, request.Options);
            return Unit.Task;
        }
    }
}
