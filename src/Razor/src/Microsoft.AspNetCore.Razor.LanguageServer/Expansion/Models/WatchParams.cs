// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using MediatR;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Expansion.Models
{
    internal abstract class WatchParams : IRequest
    {
        /**
         * The uri of the file or folder to be watched.
         */
        public abstract Uri Uri { get; }

        /**
         * The subscription ID to be used in order to stop watching the provided file or folder uri via the [StopWatching](#stopWatching) notification.
         */
        public abstract string subscriptionId { get; }

        /**
         * Configures the watch
         */
        public abstract WatchFileOptions options { get; }
    }
}
