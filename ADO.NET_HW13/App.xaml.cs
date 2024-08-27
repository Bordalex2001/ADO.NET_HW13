using ADO.NET_HW13.Models;
using ADO.NET_HW13.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ADO.NET_HW13
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    IQueryable<Position>? position = from p in db.Positions
                                   select p;
                    IQueryable<Employee>? employee = from e in db.Employees
                                   select e;
                    MainWindow mainWindow = new MainWindow();
                    MainViewModel viewModel = new MainViewModel(employee, position);
                    mainWindow.DataContext = viewModel;
                    mainWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
