﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Common.Core;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.R.Host.Client;
using Microsoft.R.Host.Client.Session;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.FileSystemMirroring.Project;
using Microsoft.VisualStudio.R.Package.Commands;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.R.Packages.R;

namespace Microsoft.VisualStudio.R.Package.Repl.Workspace {
    internal sealed class SaveWorkspaceCommand : PackageCommand {
        private readonly IApplicationShell _appShell;
        private readonly IRInteractiveWorkflow _interactiveWorkflow;
        private readonly IRSession _rSession;
        private readonly IProjectServiceAccessor _projectServiceAccessor;

        public SaveWorkspaceCommand(IApplicationShell appShell, IRInteractiveWorkflow interactiveWorkflow, IProjectServiceAccessor projectServiceAccessor) :
            base(RGuidList.RCmdSetGuid, RPackageCommandId.icmdSaveWorkspace) {
            _appShell = appShell;
            _rSession = interactiveWorkflow.RSession;
            _projectServiceAccessor = projectServiceAccessor;
            _interactiveWorkflow = interactiveWorkflow;
        }

        protected override void SetStatus() {
            var window = _interactiveWorkflow.ActiveWindow;
            if (window != null && window.Container.IsOnScreen) {
                Visible = true;
                Enabled = _rSession.IsHostRunning;
            } else {
                Visible = false;
            }
        }

        protected override void Handle() {
            var projectService = _projectServiceAccessor.GetProjectService();
            var lastLoadedProject = projectService.LoadedUnconfiguredProjects.LastOrDefault();

            var initialPath = lastLoadedProject != null ? lastLoadedProject.GetProjectDirectory() : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = _appShell.BrowseForFileSave(IntPtr.Zero, Resources.WorkspaceFileFilter, initialPath, Resources.SaveWorkspaceAsTitle);
            if (file == null) {
                return;
            }

            SaveWorkspace(file).DoNotWait();
        }

        private async Task SaveWorkspace(string file) {
            using (var evaluation = await _rSession.BeginEvaluationAsync()) {
                try {
                    await evaluation.SaveWorkspaceAsync(file);
                } catch (RException ex) {
                    var message = string.Format(CultureInfo.CurrentCulture, Resources.SaveWorkspaceFailedMessageFormat, file, ex.Message);
                    _appShell.ShowErrorMessage(message);
                } catch (OperationCanceledException) {
                }
            }
        }
    }
}
