using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.BlazorUISyncManager
{
    public class UISyncComponent : ComponentBase, IObserver<UISyncEvent>, IDisposable
    {
        [Inject]
        public IUISyncManager? UISyncManager { get; set; }

        private IDisposable? uiSyncDisposer;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                uiSyncDisposer = UISyncManager?.Subscribe(this);
            }

            base.OnAfterRender(firstRender);
        }

        public void Dispose() => uiSyncDisposer?.Dispose();

        public virtual void OnCompleted() => throw new NotImplementedException();

        public virtual void OnError(Exception error) => throw new NotImplementedException();

        public virtual async void OnNext(UISyncEvent value) => await InvokeAsync(StateHasChanged);
    }
}
