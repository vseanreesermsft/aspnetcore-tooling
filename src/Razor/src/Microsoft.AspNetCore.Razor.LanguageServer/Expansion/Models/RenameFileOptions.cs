// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class RenameFileOptions
    {
        /**
         * If existing files should be overwritten.
         */
        public abstract bool overwrite { get; }
    }
}
