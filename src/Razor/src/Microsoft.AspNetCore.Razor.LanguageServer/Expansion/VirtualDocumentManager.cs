// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion
{
    internal abstract class VirtualDocumentManager
    {
        public abstract Task<string> GetContentAsync(Uri virtualDocumentUri, CancellationToken cancellationToken);

        public abstract Task InitializeVirtualDocumentsAsync(string hostDocumentFilePath, CancellationToken cancellationToken);
    }
}
