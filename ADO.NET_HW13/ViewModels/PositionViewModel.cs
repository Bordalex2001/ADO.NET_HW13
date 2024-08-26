using ADO.NET_HW13.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW13.ViewModels
{
    public class PositionViewModel : ViewModelBase
    {
        private readonly Position _position;
        
        public PositionViewModel(Position position)
        {
            _position = position;
        }
        
        public string Name
        {
            get { return _position.Name!; }
            set
            {
                _position.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}
