using ADO.NET_HW13.Commands;
using ADO.NET_HW13.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ADO.NET_HW13.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<EmployeeViewModel> EmployeesList { get; set; }
        public ObservableCollection<PositionViewModel> PositionsList { get; set; }

        public MainViewModel(IQueryable<Employee> employees, IQueryable<Position> positions)
        {
            EmployeesList = new ObservableCollection<EmployeeViewModel>(employees.Select(e => new EmployeeViewModel(e)));
            PositionsList = new ObservableCollection<PositionViewModel>(positions.Select(p => new PositionViewModel(p)));
        }

        private string _employeeFirstName;

        public string EmployeeFirstName
        {
            get
            {
                return _employeeFirstName;
            }
            set
            {
                _employeeFirstName = value;
                OnPropertyChanged(nameof(EmployeeFirstName));
            }
        }

        private string _employeeLastName;

        public string EmployeeLastName
        {
            get
            {
                return _employeeLastName;
            }
            set
            {
                _employeeLastName = value;
                OnPropertyChanged(nameof(EmployeeLastName));
            }
        }

        private int _employeePositionId;
        public int EmployeePositionId
        {
            get { return _employeePositionId; }
            set
            {
                _employeePositionId = value;
                OnPropertyChanged(nameof(EmployeePositionId));
            }
        }

        private string _positionName;
        public string PositionName
        {
            get { return _positionName; }
            set
            {
                _positionName = value;
                OnPropertyChanged(nameof(PositionName));
            }
        }

        private DelegateCommand _showAllEmployees;
        public ICommand ShowAllEmployees
        {
            get
            {
                if (_showAllEmployees == null)
                {
                    _showAllEmployees = new DelegateCommand(p => ShowAllEmployees(), p => CanShowAllEmployees());
                }
                return _showAllEmployees;
            }
        }

        public void ShowAllEmployees()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    EmployeesList = new ObservableCollection<Employee>(db.Employees.Include(e => e.Position).ToList());
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanShowAllEmployees()
        {
            return true;
        }

        private DelegateCommand _addEmployeeCommand;

        public ICommand AddEmployeeCommand
        {
            get
            {
                if (_addEmployeeCommand == null)
                {
                    _addEmployeeCommand = new DelegateCommand(param => AddEmployee(), param => CanAddEmployee());
                }
                return _addEmployeeCommand;
            }
        }

        private DelegateCommand _deleteEmployeeCommand;

        public ICommand DeleteEmployeeCommand
        {
            get
            {
                if (_deleteEmployeeCommand == null)
                {
                    _deleteEmployeeCommand = new DelegateCommand(param => DeleteEmployee(), param => CanDeleteEmployee());
                }
                return _deleteEmployeeCommand;
            }
        }

        private DelegateCommand _updateEmployeeCommand;

        public ICommand UpdateEmployeeCommand
        {
            get
            {
                if (_updateEmployeeCommand == null)
                {
                    _updateEmployeeCommand = new DelegateCommand(param => UpdateEmployee(), param => CanUpdateEmployee());
                }
                return _updateEmployeeCommand;
            }
        }

        private DelegateCommand _updateEmployeeCommand;

        public ICommand UpdateEmployeeCommand
        {
            get
            {
                if (_updateEmployeeCommand == null)
                {
                    _updateEmployeeCommand = new DelegateCommand(param => UpdateEmployee(), param => CanUpdateEmployee());
                }
                return _updateEmployeeCommand;
            }
        }

        private void AddEmployee()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    var position = db.Positions.SingleOrDefault(p => p.Name == PositionName);
                    if (position == null)
                    {
                        position = new Position { Name = PositionName };
                        db.Positions.Add(position);
                    }

                    var employee = new Employee
                    {
                        FirstName = EmployeeFirstName,
                        LastName = EmployeeLastName,
                        Position = position,
                        HireDate = DateTime.Now.Date
                    };

                    db.Employees.Add(employee);
                    db.SaveChanges();
                }

                System.Windows.MessageBox.Show("Дані додано успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddEmployee()
        {
            return !EmployeeFirstName.IsNullOrEmpty() && !EmployeeLastName.IsNullOrEmpty() && !PositionName.IsNullOrEmpty();
        }

        private void DeleteEmployee()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    //Ліниве завантаження (Lazy loading)
                    var employeeToDelete = db.Employees.SingleOrDefault(e => e.Id == selectedBook.Id);

                    db.Employees.Remove(employeeToDelete);
                    db.SaveChanges();
                }
                System.Windows.MessageBox.Show("Дані видалено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    

        private bool CanDeleteEmployee()
        {
            return IndexSelectedAuthors != -1;
        }

        private void UpdateEmployee()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    //Ліниве завантаження (Lazy loading)
                    Employee employeeToUpdate = db.Employees.SingleOrDefault(e => e.Id == selectedBook.Id);

                    employeeToUpdate.FirstName = EmployeeFirstName;
                    employeeToUpdate.LastName = EmployeeLastName;

                    var position = db.Positions.SingleOrDefault(p => p.Name == PositionName);
                    if (position == null)
                    {
                        position = new Position { Name = PositionName };
                        db.Positions.Add(position);
                    }
                    employeeToUpdate.Position = position;

                    System.Windows.MessageBox.Show("Автора оновлено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private bool CanUpdateEmployee()
        {
            return (!EmployeeFirstName.IsNullOrEmpty() || !EmployeeLastName.IsNullOrEmpty()) && IndexSelectedAuthors != -1;
        }
    }
}