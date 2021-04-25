// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class FileStatResponse
    {
        /**
         * The type of the file, e.g. is a regular file, a directory, or symbolic link
         * to a file/directory.
         *
         * *Note:* This value might be a bitmask, e.g. `FileType.File | FileType.SymbolicLink`.
         */
        public abstract FileType type { get; }

        /**
         * The creation timestamp in milliseconds elapsed since January 1, 1970 00:00:00 UTC.
         */
        public abstract int ctime { get; }

        /**
         * The modification timestamp in milliseconds elapsed since January 1, 1970 00:00:00 UTC.
         *
         * *Note:* If the file changed, it is important to provide an updated `mtime` that advanced
         * from the previous value. Otherwise there may be optimizations in place that will not show
         * the updated file contents in an editor for example.
         */
        public abstract int mtime { get; }

        /**
         * The size in bytes.
         *
         * *Note:* If the file changed, it is important to provide an updated `size`. Otherwise there
         * may be optimizations in place that will not show the updated file contents in an editor for
         * example.
         */
        public abstract int size { get; }
    }
}
