using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DropNet;
using DropboxSync.CQRS;
using DropboxSync.Commands.Handlers;
using DropboxSync.Core;
using DropboxSync.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DropboxSync
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }
        private static SettingsResolver<AppSettings> _settings = new SettingsResolver<AppSettings>();
        public static AppSettings Settings
        {
            get
            {

                return _settings.Settings;
            }
        }



        public static void SaveSettings()
        {
            _settings.Save();
        }

        private static DropNetClient _globalClient;
        public static DropNetClient GlobalClient
        {
            get
            {
                if (_globalClient != null) return _globalClient;

                if (!String.IsNullOrEmpty(Settings.DropboxUserSecret))
                    _globalClient = new DropNetClient(Constants.DropboxAPIKey, Constants.DropboxSecret, Settings.DropboxUserToken, Settings.DropboxUserSecret);
                else
                    _globalClient = new DropNetClient(Constants.DropboxAPIKey, Constants.DropboxSecret);

                return _globalClient;
            }
        }


        private static CommandExecutor commandExecutor;
        public static CommandExecutor Executor
        {
            get
            {
                if (commandExecutor == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    commandExecutor = new CommandExecutor(assembly);
                }
                return commandExecutor;
            }
        }



        private static volatile MasterViewModel _instance;
        private static object syncRoot = new Object();



        public static MasterViewModel ViewModel
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MasterViewModel();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            RootFrame.Navigated += new NavigatedEventHandler(RootFrame_Navigated);
        }

        void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            _instance = new MasterViewModel();
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            var currentPage = ((App)Application.Current).RootFrame.Content as PhoneApplicationPage;
            if (currentPage != null)
            {
                currentPage.Dispatcher.BeginInvoke(() => MessageBox.Show("Something went unexpectably wrong. Would you like us to report this to the developers?", "Aw, snap!", MessageBoxButton.OKCancel));
                e.Handled = true;
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        public static void SetCurrentViewModel(ViewModel viewModel)
        {
            _instance.SetCurrentViewModel(viewModel);
        }

        private static readonly List<object> ExecutingList = new List<object>();
        public static void RegisterExecutor<T>(ICommandHandler<T> command)
        {
            ExecutingList.Add(command);
        }

        public static void UnregisterExecutor<T>(ICommandHandler<T> command)
        {
            ExecutingList.Remove(command);
        }

        public static bool IsExecuting<T>(ICommandHandler<T> command)
        {
            return ExecutingList.Contains(command);
        }
        public static bool IsExecuting(Type t)
        {
            return ExecutingList.Any(c => c.GetType() == t);
        }

        private static Dictionary<Type, List<Action<IDomainEvent>>> _eventHandlers;
        private static Dictionary<Type, List<Action<IDomainEvent>>> EventHandlers
        {
            get
            {
                if(_eventHandlers == null) _eventHandlers = new Dictionary<Type, List<Action<IDomainEvent>>>();
                return _eventHandlers;
            }
        }
            

        public static void On<T>(Action<IDomainEvent> handler)
            where T : IDomainEvent
        {
            if (!EventHandlers.ContainsKey(typeof(T)))
            {
                EventHandlers.Add(typeof(T), new List<Action<IDomainEvent>>());
            }
            if (!EventHandlers[typeof(T)].Contains(handler))
                EventHandlers[typeof(T)].Add(handler);
        }

        public static void Raise<T>(T ev) where T : IDomainEvent
        {
            if (!EventHandlers.ContainsKey(typeof(T))) return;
            EventHandlers[typeof(T)].ForEach(c => c(ev));
        }
    }

    public interface IDomainEvent
    {
    }
}