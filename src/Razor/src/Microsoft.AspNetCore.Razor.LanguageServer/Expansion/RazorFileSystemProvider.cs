// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion
{
    internal class RazorFileSystemProvider : FileSystemProvider
    {
        private readonly VirtualDocumentManager _virtualDocumentManager;

        public RazorFileSystemProvider(VirtualDocumentManager virtualDocumentManager)
        {
            if (virtualDocumentManager is null)
            {
                throw new ArgumentNullException(nameof(virtualDocumentManager));
            }

            _virtualDocumentManager = virtualDocumentManager;
        }

        public override async Task<string> ReadFileAsync(Uri uri, CancellationToken cancellationToken)
        {
            var content = await _virtualDocumentManager.GetContentAsync(uri, cancellationToken);
            return content;
        }

        public override void CreateDirectory(Uri uri) => throw new NotImplementedException();

        public override void Delete(Uri uri, DeleteFileOptions options) => throw new NotImplementedException();

        public override IReadOnlyList<DirectoryChild> ReadDirectory(Uri uri) => throw new NotImplementedException();

        public override void Rename(Uri oldUri, Uri newUri, RenameFileOptions options) => throw new NotImplementedException();

        public override FileStat Stat(Uri uri) => throw new NotImplementedException();

        public override void StopWatching(string subscriptionId) => throw new NotImplementedException();

        public override void Watch(Uri uri, string subsriptionId, WatchFileOptions options) => throw new NotImplementedException();

        public override void WriteFile(Uri uri, string content, WriteFileOptions options) => throw new NotImplementedException();
    }
}
