using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fill
{
    public class RelayCommand : ICommand
    {
        Action<object> exe;
        Func<object, bool> can;

        public RelayCommand( Action<object> e, Func<object, bool> c )
        {
            exe = e;
            can = c;
        }
        public RelayCommand( Action e, Func<bool> c )
        {
            exe = ( param ) => { e.Invoke(); };
            can = ( param ) => { return c == null ? true : c.Invoke(); };
        }
        public RelayCommand( Action e )
        {
            exe = ( param ) => { e.Invoke(); };
            can = ( param ) => { return true; };
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute( object parameter )
        {
            return can == null ? true : can.Invoke( parameter );
        }
        public void Execute( object parameter )
        {
            exe?.Invoke( parameter );
        }
    }

}
