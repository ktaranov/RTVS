﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Common.Core;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.R.Host.Client.Host;
using Microsoft.R.Interpreters;
using Microsoft.R.Support.Settings;
using Microsoft.UnitTests.Core.Mef;

namespace Microsoft.R.Host.Client.Test.Script {
    [ExcludeFromCodeCoverage]
    public class RHostScript : IDisposable {
        private bool _disposed = false;

        public IRSessionProvider SessionProvider { get; private set; }
        public IRSession Session { get; }

        public static Version RVersion => new RInstallation().GetInstallationData(RToolsSettings.Current.LastActiveConnection.Path, new SupportedRVersionRange()).Version;

        public RHostScript(IExportProvider exportProvider, IRSessionCallback clientApp = null)
            : this(exportProvider.GetExportedValue<IRSessionProvider>(), exportProvider.GetExportedValue<IRInteractiveWorkflowProvider>().GetOrCreate().BrokerConnector, clientApp) { 
        }

        public RHostScript(IRSessionProvider sessionProvider, IRHostBrokerConnector brokerConnector, IRSessionCallback clientApp = null) {
            SessionProvider = sessionProvider;

            Session = SessionProvider.GetOrCreate(GuidList.InteractiveWindowRSessionGuid, brokerConnector);
            if (Session.IsHostRunning) {
                Session.StopHostAsync().Wait();
            }

            Session.StartHostAsync(new RHostStartupInfo {
                Name = "RHostScript",
                CranMirrorName = RToolsSettings.Current.CranMirror,
                CodePage = RToolsSettings.Current.RCodePage,
                RHostCommandLineArguments = RToolsSettings.Current.LastActiveConnection.RCommandLineArguments
            }, clientApp ?? new RHostClientTestApp(), 50000).Wait();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                if (Session != null) {
                    Session.StopHostAsync().Wait(15000);
                    Debug.Assert(!Session.IsHostRunning);
                }

                if (SessionProvider != null) {
                    SessionProvider = null;
                }
            }

            _disposed = true;
        }
    }
}
