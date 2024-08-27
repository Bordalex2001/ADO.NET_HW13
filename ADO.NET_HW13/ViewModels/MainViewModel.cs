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

        private int _indexSelectedEmployee = -1;

        public int IndexSelectedEmployee
        {
            get { return _indexSelectedEmployee; }
            set
            {
                _indexSelectedEmployee = value;
                OnPropertyChanged(nameof(IndexSelectedEmployee));
            }
        }

        private DelegateCommand _showAllEmployeesCommand;
        public ICommand ShowAllEmployeesCommand
        {
            get
            {
                if (_showAllEmployeesCommand == null)
                {
                    _showAllEmployeesCommand = new DelegateCommand(param => ShowAllEmployees(), param => CanShowAllEmployees());
                }
                return _showAllEmployeesCommand;
            }
        }

        private void ShowAllEmployees()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    var employees = db.Employees.Include(e => e.Position).ToList();

                    EmployeesList.Clear();
                    foreach (var employee in employees)
                    {
                        EmployeesList.Add(new EmployeeViewModel(employee));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanShowAllEmployees()
        {
            return true;
        }

        private DelegateCommand _searchEmployeesCommand;

        public ICommand SearchEmployeesCommand
        {
            get
            {
                if (_searchEmployeesCommand == null)
                {
                    _searchEmployeesCommand = new DelegateCommand(param => SearchEmployees(), param => CanSearchEmployees());
                }
                return _searchEmployeesCommand;
            }
        }

        private void SearchEmployees()
        {
            try
            {
                string firstName = EmployeeFirstName;
                string lastName = EmployeeLastName;
                string position = PositionName;

                using (EmployeesContext db = new())
                {
                    //Жадібне завантаження (Eager loading)
                    /*IQueryable<Book> query = db.Book
                        .Include(b => b.Authors)
                        .Include(b => b.Categories)
                        .Include(b => b.Publishers)
                        .AsQueryable();*/

                    //Ліниве завантаження (Lazy Loading)
                    IQueryable<Employee> query = db.Employees.AsQueryable();

                    if (!string.IsNullOrEmpty(firstName))
                        query = query.Where(e => e.FirstName.Contains(firstName));

                    if (!string.IsNullOrEmpty(lastName))
                        query = query.Where(e => e.LastName.Contains(lastName));

                    if (!string.IsNullOrEmpty(position))
                        query = query.Where(b => b.Position.Name.Contains(position));

                    List<Employee> employees = query.Include(e => e.Position).ToList();

                    EmployeesList.Clear();
                    foreach (Employee employee in employees)
                    {
                        EmployeesList.Add(new EmployeeViewModel(employee));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSearchEmployees()
        {
            return !EmployeeFirstName.IsNullOrEmpty() || !EmployeeLastName.IsNullOrEmpty() || !PositionName.IsNullOrEmpty();
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
                    var employeeToDelete = db.Employees.SingleOrDefault(e => e.Id == IndexSelectedEmployee);

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
            return IndexSelectedEmployee != -1;
        }

        private void UpdateEmployee()
        {
            try
            {
                using (EmployeesContext db = new())
                {
                    //Ліниве завантаження (Lazy loading)
                    Employee? employeeToUpdate = db.Employees.SingleOrDefault(e => e.Id == IndexSelectedEmployee);

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
            return (!EmployeeFirstName.IsNullOrEmpty() || !EmployeeLastName.IsNullOrEmpty()) && IndexSelectedEmployee != -1;
        }

    }
}