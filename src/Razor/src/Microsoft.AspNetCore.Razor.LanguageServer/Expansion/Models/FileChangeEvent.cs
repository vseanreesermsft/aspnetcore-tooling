// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    /**
     * The event filesystem providers must use to signal a file change.
     */
    internal abstract class FileChangeEvent
    {
        /**
         * The type of change.
         */
        public abstract Uri Uri { get; }

        /**
         * The uri of the file that has changed.
         */
        public abstract FileChangeType type { get; }
    }
}
