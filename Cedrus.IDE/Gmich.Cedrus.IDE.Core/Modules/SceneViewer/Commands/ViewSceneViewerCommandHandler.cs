using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gmich.Cedrus.IDE.Core.Modules.SceneViewer.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Gmich.Cedrus.IDE.Core.Modules.SceneViewer.Commands
{
    [CommandHandler]
    public class ViewSceneViewerCommandHandler : CommandHandlerBase<ViewSceneViewerCommandDefinition>
    {
        private readonly IShell shell;

        [ImportingConstructor]
        public ViewSceneViewerCommandHandler(IShell shell)
        {
            this.shell = shell;
        }

        public override Task Run(Command command)
        {
            shell.OpenDocument(new SceneViewModel());
            return TaskUtility.Completed;
        }
    }
}