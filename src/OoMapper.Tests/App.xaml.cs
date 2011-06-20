using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Automation;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;
using XunitContrib.Runner.Silverlight.Toolkit;

namespace SilverlightTest1
{
    public partial class App : Application
    {
        public App()
        {
            Startup += ApplicationStartup;
            Exit += ApplicationExit;
            UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var unitTestProvider = new UnitTestProvider();
            UnitTestSystem.RegisterUnitTestProvider(unitTestProvider);
            UnitTestSettings unitTestSettings = UnitTestSystem.CreateDefaultSettings();
            unitTestSettings.StartRunImmediately = true;
            RootVisual = UnitTestSystem.CreateTestPage();
        }

        private static void ApplicationExit(object sender, EventArgs e)
        {
        }

        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
                return;

            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
        }

        private static void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                HtmlPage.Window.Eval(string.Format("throw new Error(\"Unhandled Error in Silverlight Application {0}\");", errorMsg));
            }
            catch (Exception)
            {
            }
        }
    }
}