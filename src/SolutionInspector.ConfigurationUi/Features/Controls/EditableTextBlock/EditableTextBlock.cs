using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SolutionInspector.Commons.Extensions;

namespace SolutionInspector.ConfigurationUi.Features.Controls
{
  /// <summary>
  ///   A <see cref="TextBlock" /> with in-place editing capabilities.
  /// </summary>
  [TemplatePart (Name = c_editButtonPart, Type = typeof(Button))]
  [TemplatePart (Name = c_submitButtonPart, Type = typeof(Button))]
  [TemplatePart (Name = c_cancelButtonPart, Type = typeof(Button))]
  [TemplatePart (Name = c_textBoxPart, Type = typeof(TextBox))]
  public class EditableTextBlock : Control
  {
    private const string c_editButtonPart = "PART_EditButton";
    private const string c_submitButtonPart = "PART_SubmitButton";
    private const string c_cancelButtonPart = "PART_CancelButton";
    private const string c_textBoxPart = "PART_TextBox";

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));

    public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));

    public static readonly DependencyProperty IsInEditModeProperty =
        DependencyProperty.Register(nameof(IsInEditMode), typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EditableTextBlock), new PropertyMetadata(null));

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(EditableTextBlock), new PropertyMetadata(null));

    private Button _editButton;
    private Button _submitButton;
    private Button _cancelButton;
    private TextBox _textBox;
    private string _oldText;

    static EditableTextBlock ()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock), new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
    }

    public string Text
    {
      get { return (string) GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public bool IsEditable
    {
      get => (bool) GetValue(IsEditableProperty).AssertNotNull();
      set
      {
        SetValue(IsEditableProperty, value);

        if (!value)
          SubmitEdit();
      }
    }

    public bool IsInEditMode
    {
      get => IsEditable && (bool) GetValue(IsInEditModeProperty).AssertNotNull();
      set
      {
        if (IsEditable)
          SetValue(IsInEditModeProperty, value);
      }
    }

    public ICommand Command
    {
      get => (ICommand) GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    public override void OnApplyTemplate ()
    {
      base.OnApplyTemplate();

      if (_editButton != null)
        _editButton.Click -= EditButtonOnClick;

      if (_submitButton != null)
        _submitButton.Click -= SubmitButtonOnClick;

      if (_cancelButton != null)
        _cancelButton.Click -= CancelButtonOnClick;

      if (_textBox != null)
      {
        _textBox.KeyDown -= TextBoxOnKeyDown;
        _textBox.LostFocus -= TextBoxOnLostFocus;
      }

      _editButton = (Button) GetTemplateChild(c_editButtonPart);
      _submitButton = (Button) GetTemplateChild(c_submitButtonPart);
      _cancelButton = (Button) GetTemplateChild(c_cancelButtonPart);
      _textBox = (TextBox) GetTemplateChild(c_textBoxPart).AssertNotNull();

      if (_editButton != null)
        _editButton.Click += EditButtonOnClick;

      if (_submitButton != null)
        _submitButton.Click += SubmitButtonOnClick;

      if (_cancelButton != null)
        _cancelButton.Click += CancelButtonOnClick;

      _textBox.KeyDown += TextBoxOnKeyDown;
      _textBox.LostFocus += TextBoxOnLostFocus;
    }

    private void EditButtonOnClick (object sender, RoutedEventArgs routedEventArgs)
    {
      StartEditing();
    }

    private void SubmitButtonOnClick (object sender, RoutedEventArgs routedEventArgs)
    {
      SubmitEdit();
    }

    private void CancelButtonOnClick (object sender, RoutedEventArgs routedEventArgs)
    {
      CancelEdit();
    }

    private void TextBoxOnKeyDown (object sender, KeyEventArgs keyEventArgs)
    {
      if (keyEventArgs.Key == Key.Enter)
      {
        SubmitEdit();
        keyEventArgs.Handled = true;
      }
      else if (keyEventArgs.Key == Key.Escape)
      {
        CancelEdit();
        keyEventArgs.Handled = true;
      }
    }

    private void TextBoxOnLostFocus (object sender, RoutedEventArgs routedEventArgs)
    {
      var newFocus = FocusManager.GetFocusedElement(Window.GetWindow(_textBox).AssertNotNull());

      // ReSharper disable PossibleUnintendedReferenceComparison
      if (newFocus != _cancelButton && newFocus != _submitButton)
        SubmitEdit();
      // ReSharper restore PossibleUnintendedReferenceComparison
    }

    private void StartEditing ()
    {
      _oldText = Text;
      IsInEditMode = true;
      _textBox.Focus();
      _textBox.SelectAll();
    }

    private void SubmitEdit ()
    {
      Text = _textBox.Text;
      IsInEditMode = false;

      if(Command != null)
      {
        var parameter = Tuple.Create(Text, CommandParameter);

        if (Command.CanExecute(parameter))
          Command.Execute(parameter);
      }
    }

    private void CancelEdit ()
    {
      Text = _oldText;
      IsInEditMode = false;
    }
  }
}