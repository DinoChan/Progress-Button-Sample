using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ProgressButtonDemo
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Files = new List<string>
            {
                "File 1",
                "File 2",
                "File 3",
                "File 4"
            };
        }

        public IEnumerable<string> Files { get; }

        public static async Task ShowDialogAsync(string contents, string title = null)
        {
            var dialog = title == null
                ? new MessageDialog(contents) { CancelCommandIndex = 0 }
                : new MessageDialog(contents, title) { CancelCommandIndex = 0 };
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private async void OnStateChanged(object sender, EventArgs e)
        {
            switch (ProgressButton.State)
            {
                case ProgressState.Ready:
                    break;
                case ProgressState.Started:
                    try
                    {
                        var testService = new TestService();
                        testService.ProgressChanged += (s, args) => { ProgressButton.Progress = args; };
                        await testService.Start(ThrowExceptionElement.IsOn);
                        ProgressButton.State = ProgressState.Completed;
                    }
                    catch (Exception)
                    {
                        ProgressButton.State = ProgressState.Faulted;
                    }
                    break;
                case ProgressState.Completed:
                    break;
                case ProgressState.Faulted:
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
                        testService.ProgressChanged += (s, args) => { progressButton.Progress = args; };
                        await testService.Start();
                        progressButton.State = ProgressState.Completed;
                    }
                    catch (Exception)
                    {
                        progressButton.State = ProgressState.Faulted;
                    }
                    break;
                case ProgressState.Completed:
                    await ShowDialogAsync(string.Format("The file({0}) has been downloaded", item), "File Downloaded");
                    break;
                case ProgressState.Faulted:
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
                        testService.ProgressChanged += (s, args) => { progressButton.Progress = args; };
                        await testService.Start();
                        progressButton.State = ProgressState.Completed;
                    }
                    catch (Exception)
                    {
                        progressButton.State = ProgressState.Faulted;
                    }
                    break;
                case ProgressState.Completed:
                    progressButton.Content = "Open";
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    progressButton.State = ProgressState.Ready;
                    break;
                case ProgressState.Faulted:
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

        private async void OnCase3StateChanged(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    var progressButton = sender as ProgressButton;
                    switch (progressButton.State)
                    {
                        case ProgressState.Ready:
                            break;
                        case ProgressState.Started:
                            try
                            {
                                var testService = progressButton.Tag as TestService;
                                if (testService == null)
                                {
                                    testService = new TestService();
                                    testService.ProgressChanged += (s, args) => { progressButton.Progress = args; };
                                    progressButton.Tag = testService;
                                }
                                testService.IsPaused = false;
                                await testService.Start();
                                if (testService.IsCompleted)
                                    progressButton.State = ProgressState.Completed;
                            }
                            catch (Exception)
                            {
                                progressButton.State = ProgressState.Faulted;
                            }
                            break;
                        case ProgressState.Completed:
                            break;
                        case ProgressState.Faulted:
                            break;
                    }
                });
        }

        private void OnCase3StateChanging(object sender, ProgressStateChangingEventArgs e)
        {
            if (e.OldValue == ProgressState.Completed && e.NewValue == ProgressState.Ready)
                e.Cancel = true;
        }

        private void OnCase3Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var progressButton = sender as ProgressButton;
            var testService = progressButton.Tag as TestService;
            if (testService != null&& testService.IsStarted==true && progressButton.State == ProgressState.Started)
            {

                testService.IsPaused = true;
                progressButton.State = ProgressState.Ready;
                progressButton.Content = "Restart";
            }
        }
    }


    public class TestService
    {
        public TestService()
        {
            _progress = 0;
        }

        private double _progress;

        public bool IsCompleted { get; private set; }

        public event EventHandler<double> ProgressChanged;

        public bool IsPaused { get; set; }
        public bool IsStarted { get; private set; }

        public async Task Start(bool throwException = false)
        {
            IsStarted = true;
            try
            {
                await Task.Delay(1000);
                while (_progress < 1)
                {
                    await Task.Delay(100);
                    _progress += 0.03;
                    ProgressChanged?.Invoke(this, _progress);
                    if (_progress > 0.7 && throwException)
                        throw new Exception("test");

                    if (IsPaused)
                        return;
                }

                IsCompleted = true;
            }
            finally
            {
                IsStarted = false;
            }
        }
    }
}