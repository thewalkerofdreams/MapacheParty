using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapacheCliente_Entities
{
    public class ClsJugador : INotifyPropertyChanged
    {
        private int _id;
        private int _monedas;

        public ClsJugador()
        {
            _id = 0;
            _monedas = 0;
        }

        public ClsJugador(int id)
        {
            _id = id;
            _monedas = 0;
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        public int Monedas
        {
            get
            {
                return _monedas;
            }
            set
            {
                _monedas = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
