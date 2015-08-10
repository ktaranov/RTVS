﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Languages.Editor.ContentType;
using Microsoft.Languages.Editor.Controller;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.R.Package.Commands
{
    [Export(typeof(ICommandFactory))]
    [ContentType(RContentTypeDefinition.ContentType)]
    internal class VsCommandFactory : ICommandFactory
    {
        public IEnumerable<ICommand> GetCommands(ITextView textView, ITextBuffer textBuffer)
        {
            var commands = new List<ICommand>();

            commands.Add(new ShowContextMenuCommand(textView, GuidList.PackageGuid, GuidList.CmdSetGuid, ContextMenuId.R));

            return commands;
        }
    }
}