// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MediatR;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class FileStatParams : IRequest<FileStatResponse>
    {
        /**
         * The uri to retrieve metadata about.
         */
        public abstract Uri Uri { get; }
    }
}
