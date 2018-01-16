using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BRClib
{
    public interface IBrcRenderManager
    {
        bool InProgress { get; }
        bool WasAborted { get; }
        AfterRenderAction Action { get; }
        Renderer Renderer { get; }

        void Setup(Project project, AfterRenderAction action, Renderer renderer);
        void Setup(Project project);
        void StartAsync();
        void Abort();

        event EventHandler<RenderProgressInfo> ProgressChanged;
        event EventHandler<AfterRenderAction> AfterRenderStarted;
        event EventHandler<BrcRenderResult> Finished;
    }
}
