// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MediatR;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class CreateDirectoryParams : IRequest
    {
        /**
         * The uri of the folder
         */
        public abstract Uri Uri { get; }
    }
}
