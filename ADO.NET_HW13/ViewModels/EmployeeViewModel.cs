using ADO.NET_HW13.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW13.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private readonly Employee _employee;

        public EmployeeViewModel(Employee employee)
        {
            _employee = employee;
        }

        public string FirstName
        {
            get { return _employee.FirstName!; }
            set
            {
                _employee.FirstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get { return _employee.LastName!; }
            set
            {
                _employee.LastName = value;                 
                OnPropertyChanged(nameof(LastName));
            }
        }

        public DateTime HireDate
        {
            get { return _employee.HireDate!; }
        }
    }
}
