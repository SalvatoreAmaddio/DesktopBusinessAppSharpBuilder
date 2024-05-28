using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using System.Windows.Controls;
using System.Windows;
using System.Security.Policy;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class instantiate a SubForm which can contain another <see cref="AbstractForm"/> in a <see cref="Form"/>.
    /// </summary>
    public class SubForm : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        private AbstractForm? abstractForm;

        private event ParentRecordChangedEventHandler? ParentRecordChangedEvent;
        static SubForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SubForm), new FrameworkPropertyMetadata(typeof(SubForm)));

        public SubForm() => ParentRecordChangedEvent += OnParentRecordChanged;

        private void OnParentRecordChanged(object? sender, ParentRecordChangedArgs e) => NotifyAbstractForm(e.OldValue, e.NewValue);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            abstractForm = (AbstractForm?)newContent;
            if (abstractForm == null) throw new Exception("A SubForm can only contain an AbstractForm object.");
            abstractForm.DataContextChanged += OnAbstractFormDataContextChanged;
        }
        private void OnAbstractFormDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => NotifyAbstractForm(null, ParentRecord);
        private ISubFormController? GetController() => (ISubFormController?)abstractForm?.DataContext;

        private void NotifyAbstractForm(AbstractModel? oldRecord, AbstractModel? parentRecord)
        {
            GetController()?.SetParentRecord(parentRecord);
            if (oldRecord != null)
                oldRecord.OnDirtyChanged -= OnParentRecordDirtyChanged;
            if (parentRecord != null)
                parentRecord.OnDirtyChanged += OnParentRecordDirtyChanged;
            IsEnabled = (parentRecord == null) ? false : !parentRecord.IsNewRecord();
        }

        private void OnParentRecordDirtyChanged(object? sender, OnDirtyChangedEventArgs e) => IsEnabled = !e.Model.IsNewRecord();

        #region ParentRecord
        /// <summary>
        /// Gets and Sets the <see cref="Form"/>'s <see cref="IAbstractFormController"/> CurrentRecord property which filter the records of the this SubForm.
        /// </summary>
        public AbstractModel ParentRecord
        {
            get => (AbstractModel)GetValue(ParentRecordProperty);
            set => SetValue(ParentRecordProperty, value);
        }

        public static readonly DependencyProperty ParentRecordProperty = DependencyProperty.Register(nameof(ParentRecord), typeof(AbstractModel), typeof(SubForm), new PropertyMetadata(OnParentRecordPropertyChanged));
        private static void OnParentRecordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SubForm)d).OnParentRecordChanged(d, new(e.OldValue, e.NewValue));
        #endregion

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
                // Unsubscribe from events
                ParentRecord.OnDirtyChanged -= OnParentRecordDirtyChanged;
                ParentRecordChangedEvent -= OnParentRecordChanged;
            }

            _disposed = true;
        }

        ~SubForm() => Dispose(false);
    }
}
