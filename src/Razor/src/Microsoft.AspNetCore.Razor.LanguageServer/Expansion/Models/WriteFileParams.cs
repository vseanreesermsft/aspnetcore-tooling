// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MediatR;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class WriteFileParams : IRequest
    {
        /**
         * The uri of the file to write
         */
        public abstract Uri Uri { get; }

        /**
         * The new content of the file `base64` encoded.
         */
        public abstract string content { get; }

        /**
         * Options to define if missing files should or must be created.
         */
        public abstract WriteFileOptions options { get; }
    }
}
