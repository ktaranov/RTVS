﻿using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using Microsoft.R.Host.Client;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.R.Package.DataInspect.Commands;
using Microsoft.VisualStudio.R.Package.Options.R.Tools;
using Microsoft.VisualStudio.R.Package.Plots.Commands;
using Microsoft.VisualStudio.R.Package.Repl.Data;
using Microsoft.VisualStudio.R.Package.Repl.Workspace;
using Microsoft.VisualStudio.R.Package.RPackages.Commands;

namespace Microsoft.VisualStudio.R.Packages.R
{
    internal static class PackageCommands
    {
        public static IEnumerable<MenuCommand> GetCommands(ExportProvider exportProvider) {
            var rSessionProvider = exportProvider.GetExportedValue<IRSessionProvider>();
            var projectServiceAccessor = exportProvider.GetExportedValue<IProjectServiceAccessor>();

            return new List<MenuCommand> {
                new GoToOptionsCommand(),

                new LoadWorkspaceCommand(rSessionProvider, projectServiceAccessor),
                new SaveWorkspaceCommand(rSessionProvider, projectServiceAccessor),
                new AttachDebuggerCommand(rSessionProvider),
                new RestartRCommand(),
                new InterruptRCommand(),

                new ImportDataSetTextFileCommand(),
                new ImportDataSetUrlCommand(),

                new InstallPackagesCommand(),
                new CheckForPackageUpdatesCommand(),

                new ShowPlotWindowsCommand(),
                new ShowRInteractiveWindowsCommand(),

                new ShowVariableWindowCommand()
            };
        }
    }
}