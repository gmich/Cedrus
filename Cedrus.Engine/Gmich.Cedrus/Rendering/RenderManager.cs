using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmich.Cedrus.Rendering
{
    public class RenderManager
    {
        private readonly Func<Matrix> matrix;
        private readonly ScreenRenderer screenRenderer;

        public RenderManager(IEnumerable<ARenderer> renderers, IEnumerable<ScreenPriorityRenderer> priorityRenderers, ScreenRenderer screenRenderer, Func<Matrix> matrix)
        {
            this.screenRenderer = screenRenderer;
            this.matrix = matrix;
            Renderers = renderers;
            this.priorityRenderers = priorityRenderers;
        }

        public IEnumerable<ARenderer> Renderers { get; }
        private readonly IEnumerable<ScreenPriorityRenderer> priorityRenderers;
        public void Render()
        {
            var transformationMatrix = matrix();
            foreach (var renderer in Renderers)
            {
                renderer.Render(transformationMatrix);
            }
            foreach (var renderer in priorityRenderers.OrderByDescending(c => c.Priority))
            {
                screenRenderer.Render(transformationMatrix);
            }
        }
    }

    public class ScreenPriorityRenderer
    {
        private readonly Func<int> priority;
        public ScreenPriorityRenderer(Func<int> priority, ScreenRenderer target)
        {
            this.priority = priority;
            Target = target;
        }
        public int Priority => priority();
        public ScreenRenderer Target { get; }
    }
}
