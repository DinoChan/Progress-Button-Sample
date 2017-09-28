using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ProgressButtonDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            _testService = new TestService();
            _testService.ProgressChanged += OnProgressChanged;
        }


        public static async Task ShowDialogAsync(string contents, string title = null)
        {
            var dialog = title == null ?
                new MessageDialog(contents) { CancelCommandIndex = 0 } :
                new MessageDialog(contents, title) { CancelCommandIndex = 0 };
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private void OnProgressChanged(object sender, double e)
        {
            ProgressButton.Progress = e;
        }

        private TestService _testService;

        private async void OnStateChanged(object sender, EventArgs e)
        {
            switch (ProgressButton.State)
            {
                case ProgressState.Ready:
                    break;
                case ProgressState.Started:
                    try
                    {
                        await _testService.Start(ThrowExceptionElement.IsOn);
                        ProgressButton.State = ProgressState.Completed;
                    }
                    catch (Exception ex)
                    {
                        ProgressButton.State = ProgressState.Faulted;
                    }
                    break;
                //case ProgressState.Paused:
                //    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
                    break;
                default:
                    break;
            }
        }

        private async void OnCase1StateChanged(object sender, EventArgs e)
        {
            var progressButton = sender as ProgressButton;
            var item = progressButton.DataContext.ToString();
            
            switch (progressButton.State)
            {
                case ProgressState.Ready:
                    break;
                case ProgressState.Started:
                    try
                    {
                        var testService = new TestService();
                        testService.ProgressChanged += (s, args) => {
                            progressButton.Progress = args;
                        };
                        await testService.Start();
                        progressButton.State = ProgressState.Completed;
                    }
                    catch (Exception ex)
                    {
                        progressButton.State = ProgressState.Faulted;
                    }
                    break;
                //case ProgressState.Paused:
                //    break;
                case ProgressState.Completed:
                   await ShowDialogAsync(string.Format("The file({0}) has been downloaded", item), "File Downloaded");
                    break;
                case ProgressState.Faulted:
                    break;
                default:
                    break;
            }
        }



        private void OnCase1StateChanging(object sender, ProgressStateChangingEventArgs e)
        {
            if (e.OldValue == ProgressState.Completed && e.NewValue == ProgressState.Ready)
                e.Cancel = true;
        }

      

        private async void OnCase2StateChanged(object sender, EventArgs e)
        {
            var progressButton = sender as ProgressButton;
        

            switch (progressButton.State)
            {
                case ProgressState.Ready:
                    break;
                case ProgressState.Started:
                    try
                    {
                        var testService = new TestService();
                        testService.ProgressChanged += (s, args) => {
                            progressButton.Progress = args;
                        };
                        await testService.Start();
                        progressButton.State = ProgressState.Completed;
                    }
                    catch (Exception ex)
                    {
                        progressButton.State = ProgressState.Faulted;
                    }
                    break;
                //case ProgressState.Paused:
                //    break;
                case ProgressState.Completed:
                    progressButton.Content = "Open";
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    progressButton.State = ProgressState.Ready;
                    break;
                case ProgressState.Faulted:
                    break;
                default:
                    break;
            }
        }

        private async void OnCase2StateChanging(object sender, ProgressStateChangingEventArgs e)
        {
            var progressButton = sender as ProgressButton;
            var item = progressButton.DataContext.ToString();
            if (e.OldValue == ProgressState.Ready && e.NewValue == ProgressState.Started &&
                    progressButton.Content == "Open")
            {
                e.Cancel = true;
                await ShowDialogAsync(string.Format("The file({0}) has been opened", item), "File Opened");
            }
        }
    }


    public class TestService
    {
        public event EventHandler<double> ProgressChanged;

        public bool IsFaulted { get; private set; }

        public bool IsCompleted { get; private set; }

        public async Task Start(bool throwException = false)
        {
            double progress = 0;
            await Task.Delay(1000);
            while (progress < 1)
            {
                await Task.Delay(100);
                progress += 0.03;
                ProgressChanged?.Invoke(this, progress);
                if (progress > 0.7 && throwException)
                    throw new Exception("test");
            }

            IsCompleted = true;
        }
    }
}
