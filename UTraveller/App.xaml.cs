using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Resources;
using System.Collections.Generic;
using UTraveller.Common;
using Ninject;
using GalaSoft.MvvmLight.Messaging;
using LocalRepositoryImpl.UTraveller.LocalRepository;
using System.Globalization;
using System.Threading;
using UTravellerModel.UTraveller.Model;
using Windows.Networking.Connectivity;
using UTraveller.Common.Message;
using Microsoft.Phone.Net.NetworkInformation;
using UTraveller.Service.Model;
using Windows.ApplicationModel.Activation;
using UTraveller.Common.Control;
using System.IO.IsolatedStorage;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Converter;
using System.Windows.Media;
using Windows.ApplicationModel;
using Microsoft.Phone.Data.Linq;
using UTravellerModel.UTraveller.Entity;

namespace UTraveller
{
    public partial class App : Application
    {
        public static int DATABASE_VERSION = 2;
        public static readonly string IS_LAUNCHED_FIRST_TIME = "IS_LAUNCHED_FIRST_TIME";
        public static readonly string VERSION = "VERSION";
        public static readonly string SELECTED_CURRENCY = "SELECTED_CURRENCY";
        public static readonly string SELECTED_EXPENSE_CATEGORY = "SELECTED_EXPENSE_CATEGORY";

        public static IKernel IocContainer { get; private set; }
        private static LocalDatabase localDatabase;

        public static AppProperties AppProperties
        {
            get;
            set;
        }


        static App()
        {
            IocContainer = new StandardKernel(new IocLoader());
            InitializeAppProperties();

            InitializeLocalDatabase();
        }


        private static void InitializeAppProperties()
        {
            AppProperties = new AppProperties();

            if (IsolatedStorageSettings.ApplicationSettings.Contains(BaseViewModel.MAIN_COLOR_KEY))
            {
                var mainColor = HexColorConverter.GetColorFromHex(IsolatedStorageSettings.ApplicationSettings[BaseViewModel.MAIN_COLOR_KEY].ToString());
                AppProperties.MainColor = new SolidColorBrush(mainColor);
            }
        }


        private static void InitializeLocalDatabase()
        {
            localDatabase = IocContainer.Get<LocalDatabase>();

            if (!localDatabase.DatabaseExists())
            {
                localDatabase.CreateDatabase();

                DatabaseSchemaUpdater dbUpdater = localDatabase.CreateDatabaseSchemaUpdater();
                dbUpdater.DatabaseSchemaVersion = DATABASE_VERSION;
                dbUpdater.Execute();
            }
            else
            {
                DatabaseSchemaUpdater dbUpdater = localDatabase.CreateDatabaseSchemaUpdater();

                if (dbUpdater.DatabaseSchemaVersion < DATABASE_VERSION)
                {
                    dbUpdater.AddTable<TripPlanEntity>();
                    dbUpdater.DatabaseSchemaVersion = DATABASE_VERSION;
                    dbUpdater.Execute();
                }
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage("en-US");

            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        public FileOpenPickerContinuationEventArgs FilePickerContinuationArgs
        {
            get;
            set;
        }


        private void Application_ContractActivated(object sender, IActivatedEventArgs e)
        {
            var filePickerContinuationArgs = e as FileOpenPickerContinuationEventArgs;
            if (filePickerContinuationArgs != null)
            {
                this.FilePickerContinuationArgs = filePickerContinuationArgs;

                var currentPage = App.RootFrame.Content as IFilePickerContinuationViewModelPage;
                if (currentPage != null)
                {
                    currentPage.ViewModel.ContinueFileOpenPicker(FilePickerContinuationArgs);
                }
            }
        }


        public static SocialAppData CreateFacebookAppData()
        {
            var socialAppData = new SocialAppData();
            socialAppData.AppKey = "1445350552367748";
            return socialAppData;
        }


        private void Current_Exit(object sender, EventArgs e)
        {
            ReleaseDatabase();
        }


        public static IMessenger Messenger
        {
            get { return GalaSoft.MvvmLight.Messaging.Messenger.Default; }
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
            var currentPage = App.RootFrame.Content as IActivateablePage;
            if (currentPage != null)
            {
                currentPage.Activate();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            var currentPage = App.RootFrame.Content as IActivateablePage;
            if (currentPage != null)
            {
                currentPage.Deactivate();
            }
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        public static void ReleaseDatabase()
        {
            if (localDatabase != null)
            {
                localDatabase.Dispose();
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            ReleaseDatabase();
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            ReleaseDatabase();
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
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

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            NetworkInformation.NetworkStatusChanged += (sender) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var message = new NetworkStatusChangedMessage(NetworkInterface.GetIsNetworkAvailable());
                    Messenger.Send<NetworkStatusChangedMessage>(message);
                });
            };
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

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        public static void InitializeLanguage(string locale)
        {
            try
            {
                CultureInfo newCulture = new CultureInfo(locale);
                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;

                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                App.RootFrame.FlowDirection = flow;

                App.RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.
                ReleaseDatabase();
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }


        public static CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }


        public static string LiveClientId
        {
            get
            {
                return "0000000040112F27";
            }
        }


        public static string Version
        {
            get
            {
                var version = Package.Current.Id.Version;
                return string.Format("{0}.{1}", version.Major, version.Minor);
            }
        }
    }
}