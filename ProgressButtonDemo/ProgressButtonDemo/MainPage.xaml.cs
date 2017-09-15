using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private void OnProgressChanged(object sender, double e)
        {
            ProgressButton.Progress = e;
        }

        private TestService _testService;

        private async void OnStateChanged(object sender, EventArgs e)
        {
            switch (ProgressButton.State)
            {
                case ProgressState.Normal:
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
    }


    public class TestService
    {
        public event EventHandler<double> ProgressChanged;



        public bool IsFaulted { get; private set; }

        public bool IsCompleted { get; private set; }



        public async Task Start(bool throwException = false)
        {
            double progress = 0;
            while (progress < 1)
            {
                await Task.Delay(500);
                progress += 0.1;
                ProgressChanged?.Invoke(this, progress);
            }
            if (throwException)
                throw new Exception("test");

            IsCompleted = true;
        }

    }
}
