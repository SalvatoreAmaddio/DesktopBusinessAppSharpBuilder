using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    public class PhotoFrame : Control, IDisposable
    {
        private Window? _parentWindow;
        private Image? PART_Picture;

        #region RemovePictureCommand
        public ICommand? RemovePictureCommand
        {
            get => (ICommand?)GetValue(RemovePictureCommandProperty);
            private set => SetValue(RemovePictureCommandProperty, value);
        }

        public static readonly DependencyProperty RemovePictureCommandProperty =
            DependencyProperty.Register(
                nameof(RemovePictureCommand),
                typeof(ICommand),
                typeof(PhotoFrame),
                new FrameworkPropertyMetadata(null)
                );
        #endregion

        #region FileTrasferCMD
        public ICommand? FilePickedCommand
        {
            get => (ICommand?)GetValue(FileTransferCommandProperty);
            set => SetValue(FileTransferCommandProperty, value);
        }

        public static readonly DependencyProperty FileTransferCommandProperty =
            DependencyProperty.Register(
                nameof(FilePickedCommand),
                typeof(ICommand),
                typeof(PhotoFrame),
                new FrameworkPropertyMetadata(null)
                );
        #endregion

        #region Folder
        /// <summary>
        /// Gets and Sets a <see cref="ImageSource"/>
        /// </summary>
        public string Folder
        {
            get => (string)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register(
                nameof(Folder),
                typeof(string),
                typeof(PhotoFrame),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region Source
        /// <summary>
        /// Gets and Sets a <see cref="ImageSource"/>
        /// </summary>
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(string),
                typeof(PhotoFrame), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSourcePropertyChanged)
                );

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PhotoFrame control = (PhotoFrame)d;
            control.SetImageSource(e.NewValue.ToString());
        }
        #endregion

        #region DefaultBannerPath
        /// <summary>
        /// Gets and Sets a <see cref="ImageSource"/>
        /// </summary>
        public string DefaultBannerPath
        {
            get => (string)GetValue(DefaultBannerPathProperty);
            set => SetValue(DefaultBannerPathProperty, value);
        }

        public static readonly DependencyProperty DefaultBannerPathProperty =
            DependencyProperty.Register(
                nameof(DefaultBannerPath),
                typeof(string),
                typeof(PhotoFrame),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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

        public PhotoFrame() => RemovePictureCommand = new CMD(OnRemovePictureButtonClicked);

        private void OnClosed(object? sender, EventArgs e) => Dispose();

        private void SetImageSource(string? path)
        {
            if (PART_Picture == null) return;
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(DefaultBannerPath))
                    PART_Picture.Source = Helper.LoadFromImages("uploadBanner");
                else
                    PART_Picture.Source = Helper.LoadImg(DefaultBannerPath);
                return;
            }

            path = Path.Combine(Sys.AppPath(), Folder, path);

            if (!File.Exists(path) && !path.Equals(DefaultBannerPath) && !path.Equals(Helper.LoadFromStrings("uploadImagePath")))
            {
                SetBrokenImage();
                return;
            }

            PART_Picture.Source = Helper.LoadImg(path);
        }
        
        private void SetBrokenImage() => PART_Picture!.Source = Helper.LoadFromImages("brokenImage");

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Picture = (Image?)GetTemplateChild(nameof(PART_Picture));
            SetImageSource(Source);

            if (PART_Picture != null)
                PART_Picture.MouseUp += OnPictureMouseUp;

            _parentWindow = Window.GetWindow(this);
            if (_parentWindow != null)
                _parentWindow.Closed += OnClosed;
        }

        private void OnRemovePictureButtonClicked() 
        {
            SetImageSource(null);
            FilePickedCommand?.Execute(new FilePickerCatch(Source));
        }

        private void OnPictureMouseUp(object sender, MouseButtonEventArgs e)
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
                    Source = path;
                    FilePickedCommand?.Execute(new FilePickerCatch(path, filePicker?.SelectedFileName, filePicker?.SelectedFileExtension()));
                }
            filePicker?.Dispose();
        }

        public void Dispose()
        {
            if (PART_Picture != null)
            {
                PART_Picture.MouseUp -= OnPictureMouseUp;
                PART_Picture.Source = null;
                PART_Picture = null;
            }

            if (_parentWindow!=null)
                _parentWindow.Closed -= OnClosed;

            ClearValue(SourceProperty);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.SuppressFinalize(this);
        }
    }

    public class FilePickerCatch
    {
        public string FilePath { get; } 
        public string? FileName { get; } 
        public string? Extension { get; }

        public bool FileRemoved { get; } = false;

        public FilePickerCatch(string filePath, string? fileName, string? extension) 
        { 
            FilePath = filePath;
            FileName = fileName;
            Extension = extension;
        }

        public FilePickerCatch(string filePath)
        {
            FilePath = filePath;
            FileRemoved = true;
        }
    }
}