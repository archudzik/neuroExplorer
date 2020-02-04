using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using NeuroExplorerViewer.Handlers;

namespace NeuroExplorerViewer
{
    public partial class ViewerForm : Form
    {

        public ChromiumWebBrowser browser;
        private GlobalKeyboardHook _globalKeyboardHook;
        private bool isInFullScreen = false;
        Dictionary<string, string> arguments;


        public ViewerForm()
        {
            InitializeComponent();
        }

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += KeyPressHandler;
        }

        public void ParseCommandLine()
        {
            arguments = new Dictionary<string, string>();
            foreach (var item in Environment.GetCommandLineArgs())
            {
                try
                {
                    var parts = item.Split('=');
                    arguments.Add(parts[0], parts[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void InitializeBrowser()
        {
            string[] args = Environment.GetCommandLineArgs();
            string url = arguments.ContainsKey("--url") ? arguments["--url"] : "about:blank";
            browser = new ChromiumWebBrowser(url);
            Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.MenuHandler = new MenuHandler();
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion, bitness);
        }

        private void ViewerForm_Load(object sender, EventArgs e)
        {
            ParseCommandLine();
            InitializeBrowser();
            if (arguments.ContainsKey("--fs"))
            {
                SetupKeyboardHooks();
            }
        }

        private void KeyPressHandler(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardData.VirtualCode == 122) // F11
            {
                if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
                {
                    if (isInFullScreen)
                    {
                        LeaveFullScreenMode();
                    }
                    else
                    {
                        EnterFullScreenMode();
                    }
                    isInFullScreen = !isInFullScreen;
                    e.Handled = true;
                }
            }
        }

        public void EnterFullScreenMode()
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        public void LeaveFullScreenMode()
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            if (e.IsBrowserInitialized)
            {
                var b = ((ChromiumWebBrowser)sender);
                this.InvokeOnUiThreadIfRequired(() => b.Focus());
            }
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            Console.WriteLine(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
        }

        private void SetCanGoBack(bool canGoBack)
        {
        }

        private void SetCanGoForward(bool canGoForward)
        {
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }
    }
}
