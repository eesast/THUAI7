using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace installer.ViewModel
{
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        ///<summary>
        ///announce notification
        ///</summary>
        ///<param name="propertyName">property name</param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    ///<summary>
    ///BaseCommand
    ///</summary>
    public class BaseCommand : ICommand
    {
        private Func<object?, bool>? _canExecute;
        private Action<object?> _execute;

        public BaseCommand(Func<object?, bool>? canExecute, Action<object?> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public BaseCommand(Action<object?> execute) :
            this(null, execute)
        {
        }


        public event EventHandler? CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    //CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    //CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_execute != null && CanExecute(parameter))
            {
                _execute(parameter);
            }
        }
    }

    public class RadioConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            string checkvalue = value.ToString() ?? "";
            string targetvalue = parameter.ToString() ?? "";
            bool r = checkvalue.Equals(targetvalue);
            return r;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null || parameter is null)
            {
                return null;
            }

            if ((bool)value)
            {
                return parameter.ToString();
            }
            return null;
        }
    }

    /*
    /// <summary>
    /// Password 附加属性，来自https://blog.csdn.net/qq_43562262/article/details/121786337
    /// </summary>
    public class PasswordFileService
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordFileService),
            new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject d, string value)
        {
            d.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(string), typeof(PasswordFileService),
            new PropertyMetadata(new PropertyChangedCallback(OnAttachChanged)));

        public static string GetAttach(DependencyObject d)
        {
            return (string)d.GetValue(AttachProperty);
        }
        public static void SetAttach(DependencyObject d, string value)
        {
            d.SetValue(AttachProperty, value);
        }

        static bool _isUpdating = false;
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = (d as PasswordBox)!;
            pb.PasswordChanged -= Pb_PasswordChanged;
            if (!_isUpdating)
                (d as PasswordBox)!.Password = e.NewValue.ToString();
            pb.PasswordChanged += Pb_PasswordChanged;
        }

        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = (d as PasswordBox)!;
            pb.PasswordChanged += Pb_PasswordChanged;
        }

        private static void Pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (sender as PasswordBox)!;
            _isUpdating = true;
            SetPassword(pb, pb.Password);
            _isUpdating = false;
        }
    }
    */

    public abstract class BaseViewModel : NotificationObject
    {
        private const string constBackgroundColor = "White";
        public string ConstBackgroundColor { get => constBackgroundColor; }

        private const string constFontSize = "18";
        public string ConstFontSize { get => constFontSize; }

        private const string constTextColor = "Blue";
        public string ConstTextColor { get => constTextColor; }
    }
}
