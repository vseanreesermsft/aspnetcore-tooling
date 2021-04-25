// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MediatR;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class RenameFileParams : IRequest
    {
        /**
         * The existing file.
         */
        public abstract Uri OldUri { get; }

        /**
         * The new location.
         */
        public abstract Uri NewUri { get; }

        /**
         * Defines if existing files should be overwritten.
         */
        public abstract RenameFileOptions options { get; }
    }
}
