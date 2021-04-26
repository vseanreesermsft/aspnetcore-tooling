// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion
{
    internal abstract class FileSystemProvider
    {
        public abstract void Watch(Uri uri, string subsriptionId, WatchFileOptions options);

        public abstract void StopWatching(string subscriptionId);

        public abstract FileStat Stat(Uri uri);

        public abstract IReadOnlyList<DirectoryChild> ReadDirectory(Uri uri);

        public abstract void CreateDirectory(Uri uri);

        public abstract Task<string> ReadFileAsync(Uri uri, CancellationToken cancellationToken);

        public abstract void WriteFile(Uri uri, string content, WriteFileOptions options);

        public abstract void Delete(Uri uri, DeleteFileOptions options);

        public abstract void Rename(Uri oldUri, Uri newUri, RenameFileOptions options);
    }
}
