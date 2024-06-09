using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrontEnd.Forms
{
    public class PhotoFrame : Control, IDisposable
    {
        private Button? PART_RemovePictureButton;
        private Image? PART_Picture;
        public ICommand? FileTransferCommand
        {
            get => (ICommand?)GetValue(FileTransferCommandProperty);
            set => SetValue(FileTransferCommandProperty, value);
        }

        public static readonly DependencyProperty FileTransferCommandProperty =
            DependencyProperty.Register(
                nameof(FileTransferCommand),
                typeof(ICommand),
                typeof(PhotoFrame),
                new FrameworkPropertyMetadata(null)
                );

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
            DependencyProperty.Register(
                nameof(Source), 
                typeof(ImageSource), 
                typeof(PhotoFrame), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
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
            string path = string.Empty;
            string? uri = Source?.ToString();
            if (!string.IsNullOrEmpty(uri) && uri.StartsWith("file:///"))
            {
                path = uri.Substring(8).Replace('/', '\\');
                Source = null;
                ClearValue(SourceProperty);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                FileTransferCommand?.Execute(null);
            }
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
                {
                    Source = Helper.LoadImg(path);
                    FileTransferCommand?.Execute(new FilePickerCatch(path, filePicker?.SelectedFileName, filePicker?.SelectedFileExtension()));
                }
            filePicker?.Dispose();
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

    public class FilePickerCatch(string filePath, string? fileName, string? extension)
    {
        public string FilePath { get; } = filePath;
        public string? FileName { get; } = fileName;
        public string? Extension { get; } = extension;
    }
}
