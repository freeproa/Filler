using Fill.Model;
using Fill.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Fill
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            
        }

        Manager Model;
        MainVM Vm;
        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            Model = new Manager();
            Model.Load();

            Vm = new MainVM( Model );

            MainWindow win = new MainWindow();
            win.DataContext = Vm;

            Current.MainWindow = win;
            win.Show();
        }

        protected override void OnExit( ExitEventArgs e )
        {
            base.OnExit( e );
            Vm.Close();
            Model.Close();
        }
    }
}
