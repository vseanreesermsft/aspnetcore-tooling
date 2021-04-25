// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class FileSystemProvider
    {
        public abstract void Watch(Uri uri, string subsriptionId, WatchFileOptions options);

        public abstract void StopWatching(string subscriptionId);

        public abstract FileStatResponse Stat(Uri uri);

        public abstract ReadDirectoryResponse ReadDirectory(Uri uri);

        public abstract void CreateDirectory(Uri uri);

        public abstract ReadFileResponse ReadFile(Uri uri);

        public abstract void WriteFile(Uri uri, string content, WriteFileOptions options);

        public abstract void Delete(Uri uri, DeleteFileOptions options);

        public abstract void Rename(Uri oldUri, Uri newUri, RenameFileOptions options);

    }
}
