using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace ProgressButtonDemo
{
    //todo ProgressState,调用API,各状态之间变换

    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = NormalStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = StartedStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = CompletedStateName)]
    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = FaultedStateName)]
    public partial class ProgressButton : Button
    {
        public ProgressButton()
        {
            this.DefaultStyleKey = typeof(ProgressButton);
            this.Click += OnClick;
        }

        public event EventHandler StateChanged;

        protected virtual void OnStateChanged(ProgressState oldValue, ProgressState newValue)
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(double oldValue, double newValue)
        {
            if (newValue < 0)
                newValue = 0;

            if (newValue > 1)
                newValue = 1;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case ProgressState.Normal:
                    this.State = ProgressState.Started;
                    break;
                case ProgressState.Started:
                    break;
                case ProgressState.Completed:
                    this.State = ProgressState.Normal;
                    break;
                case ProgressState.Faulted:
                    this.State = ProgressState.Normal;
                    break;
                default:
                    break;
            }
        }

    }
}
