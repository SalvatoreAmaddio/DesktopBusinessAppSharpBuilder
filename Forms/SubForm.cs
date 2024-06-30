using FrontEnd.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a sub-form that can contain another <see cref="AbstractForm"/> within a <see cref="ContentControl"/>. It implements <see cref="IAbstractControl"/>
    /// </summary>
    public class SubForm : ContentControl, IAbstractControl
    {
        /// <summary>
        /// The content host in this <see cref="SubForm"/>. This variable is set in <see cref="OnContentChanged"/>
        /// </summary>
        private AbstractForm? subFormContent;

        /// <summary>
        /// Gets the controller for this <see cref="SubForm"/>.
        /// </summary>
        private ISubFormController? Controller => (ISubFormController?)subFormContent?.DataContext;

        private event ParentRecordChangedEventHandler? ParentRecordChangedEvent;
        public Window? ParentWindow { get; private set; }

        #region ParentRecord
        /// <summary>
        /// Gets or sets the parent record, which is the <see cref="IAbstractFormController.CurrentRecord"/> property of the parent <see cref="AbstractForm"/>.
        /// This property is used to filter the records displayed in this <see cref="SubForm"/>.
        /// </summary>
        public IAbstractModel ParentRecord
        {
            get => (IAbstractModel)GetValue(ParentRecordProperty);
            set => SetValue(ParentRecordProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ParentRecord"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentRecordProperty = DependencyProperty.Register(nameof(ParentRecord), typeof(IAbstractModel), typeof(SubForm), new PropertyMetadata(OnParentRecordPropertyChanged));
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="SubForm"/> class.
        /// Overrides the default style key property metadata for the <see cref="SubForm"/> class.
        /// </summary>
        static SubForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SubForm), new FrameworkPropertyMetadata(typeof(SubForm)));

        /// <summary>
        /// Initializes a new instance of the <see cref="SubForm"/> class.
        /// Subscribes to the <see cref="ParentRecordChangedEvent"/>.
        /// </summary>
        public SubForm() => ParentRecordChangedEvent += OnParentRecordChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            subFormContent = (AbstractForm?)newContent;
            if (subFormContent == null) throw new NullReferenceException($"A {nameof(SubForm)} can only contain an AbstractForm object.");
            subFormContent.DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// Handles changes to the <see cref="ParentRecord"/> property.
        /// </summary>
        private static void OnParentRecordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SubForm)d).OnParentRecordChanged(d, new(e.OldValue, e.NewValue));

        /// <summary>
        /// Handles the <see cref="ParentRecordChangedEvent"/> event.
        /// </summary>
        private void OnParentRecordChanged(object? sender, ParentRecordChangedArgs e) => NotifySubFormContent(e.OldValue, e.NewValue);

        /// <summary>
        /// Handles changes to the <see cref="ParentRecord"/>'s dirty state.
        /// </summary>
        private void OnParentRecordDirtyChanged(object? sender, OnDirtyChangedEventArgs e) => IsEnabled = !e.Model.IsNewRecord();

        /// <summary>
        /// Notifies the <see cref="subFormContent"/> of changes to the parent record. 
        /// </summary>
        /// <remarks>
        /// This method is called when the <see cref="ParentRecord"/> property changes or when the <see cref="subFormContent"/>'s DataContext property changes.
        /// </remarks>
        private void NotifySubFormContent(IAbstractModel? oldRecord, IAbstractModel? parentRecord)
        {
            Controller?.SetParentRecord(parentRecord);
            if (oldRecord != null)
                oldRecord.OnDirtyChanged -= OnParentRecordDirtyChanged;
            if (parentRecord != null)
                parentRecord.OnDirtyChanged += OnParentRecordDirtyChanged;
            IsEnabled = (parentRecord == null) ? false : !parentRecord.IsNewRecord();
        }

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e) { }
        public void OnClosed(object? sender, EventArgs e) => Dispose();
        public void DisposeEvents()
        {
            if (subFormContent != null)
                subFormContent.DataContextChanged -= OnDataContextChanged;
            ParentRecord.OnDirtyChanged -= OnParentRecordDirtyChanged;
            ParentRecordChangedEvent -= OnParentRecordChanged;
            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }
        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => NotifySubFormContent(ParentRecord, ParentRecord);
        #endregion

        public void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }
}