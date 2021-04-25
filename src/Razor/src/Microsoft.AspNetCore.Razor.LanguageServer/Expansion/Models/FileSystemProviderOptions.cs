// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class FileSystemProviderOptions
    {
        /**
         * The uri-scheme the provider registers for
         */
        public abstract string Scheme { get; }

        /**
         * Whether or not the file system is case sensitive.
         */
        public abstract bool? IsCaseSensitive { get; }

        /**
         * Whether or not the file system is readonly.
         */
        public abstract bool? IsReadonly { get; }
    }
}
