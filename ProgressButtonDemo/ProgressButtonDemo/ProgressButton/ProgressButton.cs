﻿using System;
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

    [TemplateVisualState(GroupName = ProgressStatesGroupName, Name = ReadyStateName)]
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
        public event EventHandler<ProgressStateChangingEventArgs> StateChanging;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualStates(false);
        }

        protected virtual void OnStateChanged(ProgressState oldValue, ProgressState newValue)
        {
            if (newValue == ProgressState.Ready)
                Progress = 0;

            //this.IsHitTestVisible = this.State != ProgressState.Started;
            UpdateVisualStates(true);

            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(double oldValue, double newValue)
        {
            if (newValue < 0)
                Progress = 0;

            if (newValue > 1)
                Progress = 1;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case ProgressState.Ready:
                    ChangeStateCore(ProgressState.Started);
                    break;
                case ProgressState.Started:
                    break;
                case ProgressState.Completed:
                    ChangeStateCore(ProgressState.Ready);
                    break;
                case ProgressState.Faulted:
                    ChangeStateCore(ProgressState.Ready);
                    break;
                default:
                    break;
            }
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            string progressState;
            switch (State)
            {
                case ProgressState.Ready:
                    progressState = ReadyStateName;
                    break;
                case ProgressState.Started:
                    progressState = StartedStateName;
                    break;
                case ProgressState.Completed:
                    progressState = CompletedStateName;
                    break;
                case ProgressState.Faulted:
                    progressState = FaultedStateName;
                    break;
                default:
                    progressState = ReadyStateName;
                    break;
            }
            VisualStateManager.GoToState(this, progressState, useTransitions);
            //if (IsHitTestVisible == false && IsEnabled)
            //    VisualStateManager.GoToState(this, "Normal", false);
        }

        private void ChangeStateCore(ProgressState newstate)
        {
            if (StateChanging != null)
            {
                var args = new ProgressStateChangingEventArgs(this.State,newstate);
                StateChanging(this, args);
                if (args.Cancel)
                    return;
            }

            State = newstate;
        }

    }
}
