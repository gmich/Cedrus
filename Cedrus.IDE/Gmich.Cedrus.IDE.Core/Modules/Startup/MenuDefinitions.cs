using System.ComponentModel.Composition;
using Gmich.Cedrus.IDE.Core.Modules.SceneViewer.Commands;
using Gemini.Framework.Menus;
using Gmich.Cedrus.IDE.Core.Modules.Camera.Commands;

namespace Gmich.Cedrus.IDE.Core.Modules
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewSceneViewerMenuItem = new CommandMenuItemDefinition<ViewSceneViewerCommandDefinition>(
            Startup.Module.DemosMenuGroup, 1);

        [Export]
        public static ExcludeMenuItemDefinition ExcludeOpenMenuItem = new ExcludeMenuItemDefinition(Gemini.Modules.Shell.MenuDefinitions.FileOpenMenuItem);

        [Export]
        public static ExcludeMenuItemDefinition ExcludNewMenuItem = new ExcludeMenuItemDefinition(Gemini.Modules.Shell.MenuDefinitions.FileNewMenuItem);

        [Export]
        public static MenuItemDefinition ViewCameraMenuItem = new CommandMenuItemDefinition<CameraCommandDefinition>(
            Startup.Module.DemosMenuGroup, 2);

    }
}