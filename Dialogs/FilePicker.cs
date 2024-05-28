using Microsoft.Win32;
using System.ComponentModel;
using System.Text;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// This class hols a collection of constants of common FilePicker filters.
    /// </summary>
    public static class FilePickerFilters
    {
        public const string ALL_FILES = "All Files (*.*)|*.*";
        public const string TEXT_FILES = "Text Files (*.txt)|*.txt";
        public const string IMG_FILES = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
        public const string WORD_FILES = "Word Documents (*.doc;*.docx)|*.doc;*.docx";
        public const string PDF_FILES = "PDF Documents (*.pdf)|*.pdf";
        public const string AUDIO_FILES = "Audio Files (*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma";
        public const string VIDEO_FILES = "Video Files (*.mp4;*.avi;*.mkv)|*.mp4;*.avi;*.mkv";
        public const string XL_FILES = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx";
        public const string PP_FILES = "PowerPoint Files (*.ppt;*.pptx)|*.ppt;*.pptx";
    }

    /// <summary>
    /// This class wrap the <see cref="OpenFileDialog"/> class.
    /// </summary>
    public class FilePicker(string title = "Select a file")
    {
        public delegate void FileOK(object? sender, CancelEventArgs e);

        protected OpenFileDialog? openFileDialog;
        public bool Multiselect { get; set; } = false;
        public bool ShowHiddenItems { get; set; } = false;
        public string[]? SelectedFileNames => openFileDialog?.SafeFileNames;
        public string? SelectedFileName => openFileDialog?.SafeFileName;
        public string? SelectedFile => openFileDialog?.FileName;
        public string[]? SelectedFiles => openFileDialog?.FileNames;
        public string Title { get; set; } = title;
        public int FilterIndex { get; set; } = 1;

        public event FileOK? OnFileOK;
        public string Filter { get; set; } = FilePickerFilters.ALL_FILES;
        public bool ShowDialog()
        {
            openFileDialog = new()
            {
                FilterIndex = this.FilterIndex,
                Title = this.Title,
                Multiselect = this.Multiselect,
                ShowHiddenItems = this.ShowHiddenItems,
                AddToRecent = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = this.Filter,
            };

            openFileDialog.FileOk += OnOpenFileDialogFileOk;
            bool? result = openFileDialog.ShowDialog();

            return result ?? false;
        }

        protected virtual void OnOpenFileDialogFileOk(object? sender, CancelEventArgs e) => OnFileOK?.Invoke(sender, e);

        public void Reset() => openFileDialog?.Reset();
        public void OpenSelectedFile() => openFileDialog?.OpenFile();

        public void OpenSelectedFiles() => openFileDialog?.OpenFiles();

        ~FilePicker()
        {
            OnFileOK = null;
        }
    }

    /// <summary>
    /// This class works together with <see cref="FilePicker"/>. It helps building a Filter String that the <see cref="FilePicker"/> can use.
    /// </summary>
    /// <param name="description"></param>
    public class CreateFilter(string descritpion)
    {
        /// <summary>
        /// Gets and sets the Filter's description. For Example:
        /// <code>
        ///     Description = "Image Files";
        /// </code>
        /// </summary>
        public string Description { get; set; } = descritpion;

        //(*.bmp;*.jpg;*.jpeg;*.png;*.gif)
        private string Extension = string.Empty;

        //*.bmp;*.jpg;*.jpeg;*.png;*.gif
        private string Pattern = string.Empty;

        private readonly HashSet<string> Extensions = [];

        /// <summary>
        /// Adds the file's extentions the file picker should filter. For Example:
        /// <code>
        /// AddExtension(bmp);
        /// AddExtension(jpg);
        /// AddExtension(jpeg);
        /// AddExtension(png);
        /// AddExtension(gif);
        /// </code>
        /// </summary>
        /// <param name="str"></param>
        public void AddExtension(string str) => Extensions.Add($"*.{str}");

        /// <summary>
        /// It uses a <see cref="StringBuilder"/> object to set the <see cref="Extensions"/> property based on the values provided by calling the <see cref="AddExtension(string)"/>
        /// This method is called in <see cref="Create"/>
        /// </summary>
        private void GetExtensions()
        {
            StringBuilder sb = new();
            foreach (string str in Extensions)
            {
                sb.Append($"{str};");
            }

            sb.Remove(sb.Length - 1, 1);
            Extension = sb.ToString();
        }

        /// <summary>
        /// It uses a <see cref="StringBuilder"/> object to set the <see cref="Pattern"/> property based on the values provided by calling the <see cref="AddExtension(string)"/>
        /// This method is called in <see cref="Create"/>
        /// </summary>
        private void GetPatterns()
        {
            StringBuilder sb = new();
            sb.Append("(");
            foreach (string str in Extensions)
            {
                sb.Append($"{str};");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            Pattern = sb.ToString();
        }

        /// <summary>
        /// Creates the full filter string to be used by the <see cref="FilePicker"/>. <para/>
        /// If no extension and/or <see cref="Description"/> was provided, this method will throw an Exception.<para/>
        /// You can use the <see cref="AddExtension(string)"/> to add extensions.
        /// </summary>
        /// <returns>A string.</returns>
        /// <exception cref="Exception"></exception>
        public string Create()
        {
            if (Extensions.Count == 0) throw new Exception("No Extension provided");
            if (string.IsNullOrEmpty(Description)) throw new Exception("No Description was provided");
            GetExtensions();
            GetPatterns();
            return $"{Description} {Pattern}|{Extension}";
        }

        /// <summary>
        /// Reset all properties to their default value.
        /// </summary>
        public void Clear()
        {
            Extensions.Clear();
            Description = string.Empty;
            Extension = string.Empty;
            Pattern = string.Empty;
        }

        public override string? ToString() => $"{Description} {Extension}|{Pattern}";
    }
}
