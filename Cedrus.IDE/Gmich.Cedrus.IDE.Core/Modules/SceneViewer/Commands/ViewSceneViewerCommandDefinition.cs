﻿using Gemini.Framework.Commands;

namespace Gmich.Cedrus.IDE.Core.Modules.SceneViewer.Commands
{
    [CommandDefinition]
    public class ViewSceneViewerCommandDefinition : CommandDefinition
    {
        public const string CommandName = "Demos.SceneViewer";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Free Painting"; }
        }

        public override string ToolTip
        {
            get { return "Free Painting"; }
        }
    }
}