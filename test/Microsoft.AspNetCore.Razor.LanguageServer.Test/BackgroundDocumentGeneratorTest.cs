﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.LanguageServer.StrongNamed;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Test
{
    // These tests are really integration tests. There isn't a good way to unit test this functionality since
    // the only thing in here is threading.
    public class BackgroundDocumentGeneratorTest : TestBase
    {
        public BackgroundDocumentGeneratorTest()
        {
            Documents = new HostDocumentShim[]
            {
                HostDocumentShim.Create("c:\\Test1\\Index.cshtml", "Index.cshtml"),
                HostDocumentShim.Create("c:\\Test1\\Components\\Counter.cshtml", "Components\\Counter.cshtml"),
            };

            HostProject1 = HostProjectShim.Create("c:\\Test1\\Test1.csproj", RazorConfiguration.Default);
            HostProject2 = HostProjectShim.Create("c:\\Test2\\Test2.csproj", RazorConfiguration.Default);

            var projectId1 = ProjectId.CreateNewId("Test1");
            var projectId2 = ProjectId.CreateNewId("Test2");
        }

        private HostDocumentShim[] Documents { get; }

        private HostProjectShim HostProject1 { get; }

        private HostProjectShim HostProject2 { get; }
        [ForegroundFact]
        public async Task Queue_ProcessesNotifications_AndGoesBackToSleep()
        {
            // Arrange
            var projectManager = TestProjectSnapshotManager.Create(Dispatcher);
            projectManager.HostProjectAdded(HostProject1);
            projectManager.HostProjectAdded(HostProject2);
            projectManager.DocumentAdded(HostProject1, Documents[0], null);
            projectManager.DocumentAdded(HostProject1, Documents[1], null);

            var project = projectManager.GetLoadedProject(HostProject1.FilePath);

            var queue = new BackgroundDocumentGenerator(Dispatcher, Logger)
            {
                Delay = TimeSpan.FromMilliseconds(1),
                BlockBackgroundWorkStart = new ManualResetEventSlim(initialState: false),
                NotifyBackgroundWorkStarting = new ManualResetEventSlim(initialState: false),
                BlockBackgroundWorkCompleting = new ManualResetEventSlim(initialState: false),
                NotifyBackgroundWorkCompleted = new ManualResetEventSlim(initialState: false),
            };

            // Act & Assert
            queue.Enqueue(project.GetDocument(Documents[0].FilePath));

            Assert.True(queue.IsScheduledOrRunning, "Queue should be scheduled during Enqueue");
            Assert.True(queue.HasPendingNotifications, "Queue should have a notification created during Enqueue");

            // Allow the background work to proceed.
            queue.BlockBackgroundWorkStart.Set();
            queue.BlockBackgroundWorkCompleting.Set();

            await Task.Run(() => queue.NotifyBackgroundWorkCompleted.Wait(TimeSpan.FromSeconds(3)));

            Assert.False(queue.IsScheduledOrRunning, "Queue should not have restarted");
            Assert.False(queue.HasPendingNotifications, "Queue should have processed all notifications");
        }

        [ForegroundFact]
        public async Task Queue_ProcessesNotifications_AndRestarts()
        {
            // Arrange
            var projectManager = TestProjectSnapshotManager.Create(Dispatcher);
            projectManager.HostProjectAdded(HostProject1);
            projectManager.HostProjectAdded(HostProject2);
            projectManager.DocumentAdded(HostProject1, Documents[0], null);
            projectManager.DocumentAdded(HostProject1, Documents[1], null);

            var project = projectManager.GetLoadedProject(HostProject1.FilePath);

            var queue = new BackgroundDocumentGenerator(Dispatcher, Logger)
            {
                Delay = TimeSpan.FromMilliseconds(1),
                BlockBackgroundWorkStart = new ManualResetEventSlim(initialState: false),
                NotifyBackgroundWorkStarting = new ManualResetEventSlim(initialState: false),
                NotifyBackgroundCapturedWorkload = new ManualResetEventSlim(initialState: false),
                BlockBackgroundWorkCompleting = new ManualResetEventSlim(initialState: false),
                NotifyBackgroundWorkCompleted = new ManualResetEventSlim(initialState: false),
            };

            // Act & Assert
            queue.Enqueue(project.GetDocument(Documents[0].FilePath));

            Assert.True(queue.IsScheduledOrRunning, "Queue should be scheduled during Enqueue");
            Assert.True(queue.HasPendingNotifications, "Queue should have a notification created during Enqueue");

            // Allow the background work to start.
            queue.BlockBackgroundWorkStart.Set();

            await Task.Run(() => queue.NotifyBackgroundWorkStarting.Wait(TimeSpan.FromSeconds(1)));

            Assert.True(queue.IsScheduledOrRunning, "Worker should be processing now");

            await Task.Run(() => queue.NotifyBackgroundCapturedWorkload.Wait(TimeSpan.FromSeconds(1)));
            Assert.False(queue.HasPendingNotifications, "Worker should have taken all notifications");

            queue.Enqueue(project.GetDocument(Documents[1].FilePath));
            Assert.True(queue.HasPendingNotifications); // Now we should see the worker restart when it finishes.

            // Allow work to complete, which should restart the timer.
            queue.BlockBackgroundWorkCompleting.Set();

            await Task.Run(() => queue.NotifyBackgroundWorkCompleted.Wait(TimeSpan.FromSeconds(3)));
            queue.NotifyBackgroundWorkCompleted.Reset();

            // It should start running again right away.
            Assert.True(queue.IsScheduledOrRunning, "Queue should be scheduled during Enqueue");
            Assert.True(queue.HasPendingNotifications, "Queue should have a notification created during Enqueue");

            // Allow the background work to proceed.
            queue.BlockBackgroundWorkStart.Set();

            queue.BlockBackgroundWorkCompleting.Set();
            await Task.Run(() => queue.NotifyBackgroundWorkCompleted.Wait(TimeSpan.FromSeconds(3)));

            Assert.False(queue.IsScheduledOrRunning, "Queue should not have restarted");
            Assert.False(queue.HasPendingNotifications, "Queue should have processed all notifications");
        }
    }
}
