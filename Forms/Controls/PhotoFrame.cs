using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    public class PhotoFrame : Control, IDisposable
    {
        private Button? PART_RemovePictureButton;
        private Image? PART_Picture;

        #region Source
        /// <summary>
        /// Gets and Sets a <see cref="ImageSource"/>
        /// </summary>
        public ImageSource? Source
        {
            get => (ImageSource?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(PhotoFrame), new PropertyMetadata(null));
        #endregion

        #region CornerRadius
        /// <summary>
        /// Gets and Sets a <see cref="System.Windows.CornerRadius"/>
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(PhotoFrame), new PropertyMetadata(new CornerRadius()));
        #endregion

        static PhotoFrame() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PhotoFrame), new FrameworkPropertyMetadata(typeof(PhotoFrame)));

        public PhotoFrame() 
        {
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_RemovePictureButton = (Button?)GetTemplateChild(nameof(PART_RemovePictureButton));
            PART_Picture = (Image)GetTemplateChild(nameof(PART_Picture));

            if (PART_Picture != null)
                PART_Picture.MouseUp += OnPictureMouseUp;

            if (PART_RemovePictureButton != null)
                PART_RemovePictureButton.Click += OnRemovePictureButtonClicked;
        }

        private void OnRemovePictureButtonClicked(object sender, RoutedEventArgs e)
        {
            Source = null;
        }

        private void OnPictureMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FilePicker filePicker = new("Select a Picture")
            {
                Filter = FilePickerFilters.IMG_FILES
            };

            filePicker.ShowDialog();
            string? path = filePicker.SelectedFile;
            if (path!=null) 
                if (path.Length > 0)
                    Source = Helper.LoadImg(path);
        }

        public void Dispose()
        {
            if (PART_Picture != null)
                PART_Picture.MouseUp -= OnPictureMouseUp;

            if (PART_RemovePictureButton != null)
                PART_RemovePictureButton.Click -= OnRemovePictureButtonClicked;

            Unloaded -= OnUnloaded;
            GC.SuppressFinalize(this);
        }
    }
}
