using Backend.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace FrontEnd.Forms
{

    /// <summary>
    /// A Curtain object is a drop-side GUI Content which displays some information about the developer, the application and the client.
    /// By default, this object is hidden.
    /// This object works with an instance of <see cref="Backend.Utils.SoftwareInfo"/>.
    /// </summary>
    /// <remarks>    
    /// For Example in your xaml:
    /// <code>
    /// &lt;!--Create a MenuItem to open the Curtain-->
    /// &lt;Menu VerticalAlignment="Top">
    ///     &lt;fr:OpenCurtain Click="Open"/>
    ///     ...
    /// &lt;/Menu>
    /// ...
    /// &lt;fr:Curtain x:Name="Curtain"/> &lt;!--Place your Curtain object-->
    /// ...
    /// </code>
    /// Then in your code behind:
    /// <code>
    /// //Click event to open the curtain
    /// private void OpenCurtain(object sender, RoutedEventArgs e) => Curtain.Open();
    /// </code>
    /// See also <seealso cref="OpenCurtain"/>, <seealso cref="Backend.Utils.SoftwareInfo"/>
    /// </remarks>
    public class Curtain : ContentControl, IAbstractControl
    {
        private Button? PART_CloseButton;
        private Hyperlink? PART_WebLink;
        public Window? ParentWindow { get; private set; }

        #region SoftwareInfo
        /// <summary>
        /// Gets or sets the software information for the Curtain.
        /// </summary>
        /// <remarks>
        /// This property will also set: <see cref="DeveloperName"/>, <see cref="DeveloperWebsite"/>, 
        /// <see cref="SoftwareName"/>, <see cref="SoftwareYear"/>, <see cref="SoftwareVersion"/> and <see cref="ClientName"/>
        /// </remarks>
        public SoftwareInfo SoftwareInfo
        {
            get => (SoftwareInfo)GetValue(SoftwareInfoProperty);
            set => SetValue(SoftwareInfoProperty, value);
        }

        public static readonly DependencyProperty SoftwareInfoProperty = DependencyProperty.Register(nameof(SoftwareInfo), typeof(SoftwareInfo), typeof(Curtain), new PropertyMetadata(OnSoftwareInfoPropertyChanged));
        #endregion

        #region DeveloperName
        /// <summary>
        /// Gets the developer's name.
        /// </summary>
        public string DeveloperName
        {
            get => (string)GetValue(DeveloperNameProperty);
            private set => SetValue(DeveloperNameProperty, value);
        }

        public static readonly DependencyProperty DeveloperNameProperty = DependencyProperty.Register(nameof(DeveloperName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region DeveloperWebsite
        /// <summary>
        /// Gets the developer's website URI.
        /// </summary>
        public Uri DeveloperWebsite
        {
            get => (Uri)GetValue(DeveloperWebsiteProperty);
            private set => SetValue(DeveloperWebsiteProperty, value);
        }

        public static readonly DependencyProperty DeveloperWebsiteProperty = DependencyProperty.Register(nameof(DeveloperWebsite), typeof(Uri), typeof(Curtain), new PropertyMetadata());
        #endregion

        #region SoftwareYear
        /// <summary>
        /// Gets the year of the software release.
        /// </summary>
        public string SoftwareYear
        {
            get => (string)GetValue(SoftwareYearProperty);
            private set => SetValue(SoftwareYearProperty, value);
        }

        public static readonly DependencyProperty SoftwareYearProperty = DependencyProperty.Register(nameof(SoftwareYear), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareName
        /// <summary>
        /// Gets the software name.
        /// </summary>
        public string SoftwareName
        {
            get => (string)GetValue(SoftwareNameProperty);
            private set => SetValue(SoftwareNameProperty, value);
        }

        public static readonly DependencyProperty SoftwareNameProperty = DependencyProperty.Register(nameof(SoftwareName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareVersion
        /// <summary>
        /// Gets the software version.
        /// </summary>
        public string SoftwareVersion
        {
            get => (string)GetValue(SoftwareVersionProperty);
            private set => SetValue(SoftwareVersionProperty, value);
        }

        public static readonly DependencyProperty SoftwareVersionProperty = DependencyProperty.Register(nameof(SoftwareVersion), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region ClientName
        /// <summary>
        /// Gets the client name.
        /// </summary>
        public string ClientName
        {
            get => (string)GetValue(ClientNameProperty);
            private set => SetValue(ClientNameProperty, value);
        }

        public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register(nameof(ClientName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region HeaderTitle
        /// <summary>
        /// Gets the header title.
        /// </summary>
        public string HeaderTitle
        {
            get => (string)GetValue(HeaderTitleProperty);
            private set => SetValue(HeaderTitleProperty, value);
        }

        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register(nameof(HeaderTitle), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="Curtain"/> class.
        /// Overrides the default style key property metadata for the <see cref="Curtain"/> class.
        /// </summary>
        static Curtain()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Curtain), new FrameworkPropertyMetadata(typeof(Curtain)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Curtain"/> class.
        /// </summary>
        public Curtain()
        {
            try
            {
                HeaderTitle = $"Ciao {CurrentUser.UserName}!";
            }
            catch
            {
                HeaderTitle = $"Ciao!";
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_CloseButton = (Button?)GetTemplateChild(nameof(PART_CloseButton)); //get the close button to attach a click event to close the curtain.
            if (PART_CloseButton == null) throw new Exception($"Failed to get {nameof(PART_CloseButton)}");
            PART_CloseButton.Click += (s,e) => { Visibility = Visibility.Hidden; };

            PART_WebLink = (Hyperlink?)GetTemplateChild(nameof(PART_WebLink)); //get the Hyperlink object to attach a RequestNavigate event.
            if (PART_WebLink == null) throw new Exception($"Failed to get {nameof(PART_WebLink)}");

            PART_WebLink.RequestNavigate += OnHyperlinkClicked;

            //clean the URI to display the website name only without https:// 
            string url = DeveloperWebsite.AbsoluteUri;

            try 
            {
                url = url.Split("//")[1];
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);
            }
            catch 
            {
                throw new Exception();           
            }

            PART_WebLink.Inlines.Add(url); //add the clean URI which is visibile to the User.

            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        /// <summary>
        /// Opens the Curtain.
        /// </summary>
        public void Open() => Visibility = Visibility.Visible;

        /// <summary>
        /// Event handler to open the browser and navigate to the <see cref="DeveloperWebsite"/>.
        /// </summary>
        private void OnHyperlinkClicked(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo info = new(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            };
            Process.Start(info);
            e.Handled = true;
        }

        /// <summary>
        /// Handles changes to the <see cref="SoftwareInfo"/> property.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnSoftwareInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Curtain curtain = (Curtain)d;
            curtain.SetBinding(DeveloperNameProperty, new Binding(nameof(DeveloperName)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareYearProperty, new Binding(nameof(SoftwareYear)) { Source = e.NewValue });
            curtain.SetBinding(DeveloperWebsiteProperty, new Binding(nameof(DeveloperWebsite)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareNameProperty, new Binding(nameof(SoftwareName)) { Source = e.NewValue });
            curtain.SetBinding(SoftwareVersionProperty, new Binding(nameof(SoftwareVersion)) { Source = e.NewValue });
            curtain.SetBinding(ClientNameProperty, new Binding(nameof(ClientName)) { Source = e.NewValue });
        }

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e) { }

        public void OnClosed(object? sender, EventArgs e) => Dispose();

        public void DisposeEvents()
        {
            if (PART_WebLink != null)
                PART_WebLink.RequestNavigate -= OnHyperlinkClicked;

            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }

        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { }
        #endregion

        public virtual void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }
}