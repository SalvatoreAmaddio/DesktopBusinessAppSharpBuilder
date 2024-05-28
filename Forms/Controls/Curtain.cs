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
    /// This object works with a instance of <see cref="Backend.Utils.SoftwareInfo"/>.
    /// <para/>
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
    /// </summary>
    public class Curtain : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        Button? PART_CloseButton;
        Hyperlink? PART_WebLink;
        static Curtain()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Curtain), new FrameworkPropertyMetadata(typeof(Curtain)));
        }

        #region SoftwareInfo
        public SoftwareInfo SoftwareInfo
        {
            get => (SoftwareInfo)GetValue(SoftwareInfoProperty);
            set => SetValue(SoftwareInfoProperty, value);
        }

        public static readonly DependencyProperty SoftwareInfoProperty = DependencyProperty.Register(nameof(SoftwareInfo), typeof(SoftwareInfo), typeof(Curtain), new PropertyMetadata(OnSoftwareInfoPropertyChanged));

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

        #endregion

        #region DeveloperName
        public string DeveloperName
        {
            get => (string)GetValue(DeveloperNameProperty);
            set => SetValue(DeveloperNameProperty, value);
        }

        public static readonly DependencyProperty DeveloperNameProperty = DependencyProperty.Register(nameof(DeveloperName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareYear
        public string SoftwareYear
        {
            get => (string)GetValue(SoftwareYearProperty);
            set => SetValue(SoftwareYearProperty, value);
        }

        public static readonly DependencyProperty SoftwareYearProperty = DependencyProperty.Register(nameof(SoftwareYear), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region DeveloperWebsite
        public Uri DeveloperWebsite
        {
            get => (Uri)GetValue(DeveloperWebsiteProperty);
            set => SetValue(DeveloperWebsiteProperty, value);
        }

        public static readonly DependencyProperty DeveloperWebsiteProperty = DependencyProperty.Register(nameof(DeveloperWebsite), typeof(Uri), typeof(Curtain), new PropertyMetadata());
        #endregion

        #region HeaderTitle
        public string HeaderTitle
        {
            get => (string)GetValue(HeaderTitleProperty);
            set => SetValue(HeaderTitleProperty, value);
        }

        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register(nameof(HeaderTitle), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareName
        public string SoftwareName
        {
            get => (string)GetValue(SoftwareNameProperty);
            set => SetValue(SoftwareNameProperty, value);
        }

        public static readonly DependencyProperty SoftwareNameProperty = DependencyProperty.Register(nameof(SoftwareName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region ClientName
        public string ClientName
        {
            get => (string)GetValue(ClientNameProperty);
            set => SetValue(ClientNameProperty, value);
        }

        public static readonly DependencyProperty ClientNameProperty = DependencyProperty.Register(nameof(ClientName), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

        #region SoftwareVersion
        public string SoftwareVersion
        {
            get => (string)GetValue(SoftwareVersionProperty);
            set => SetValue(SoftwareVersionProperty, value);
        }

        public static readonly DependencyProperty SoftwareVersionProperty = DependencyProperty.Register(nameof(SoftwareVersion), typeof(string), typeof(Curtain), new PropertyMetadata(string.Empty));
        #endregion

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
        }
        
        /// <summary>
        /// Open the Curtain.
        /// </summary>
        public void Open() => Visibility = Visibility.Visible;

        /// <summary>
        /// Event to open the Browser and navigate to the <see cref="DeveloperWebsite"/>
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

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (PART_WebLink != null)
                    PART_WebLink.RequestNavigate -= OnHyperlinkClicked;
            }

            _disposed = true;
        }

        ~Curtain() => Dispose(false);
    }
}