using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapacheCliente_Entities
{
    public class ClsCasilla : INotifyPropertyChanged
    {
        private ClsItem _item;
        private bool _oculta;
        private bool _seleccionada;
        private String _imagen;

        #region Constructores

        public ClsCasilla()
        {
            _item = null;
            _oculta = false;
            _seleccionada = false;
            _imagen = null;
        }


        public ClsCasilla(ClsItem item, bool oculta, bool seleccionada, String imagen)
        {
            _item = item;
            _oculta = oculta;
            _seleccionada = seleccionada;
            _imagen = imagen;
        }

        #endregion

        #region Propiedades públicas
        public ClsItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                NotifyPropertyChanged();
            }
        }

        public bool Oculta
        {
            get
            {
                return _oculta;
            }
            set
            {
                _oculta = value;
                if (_item != null && _item.TipoItem == 1)
                {//Si el item es una caja sorpresa
                    if (_item.Monedas >= 1 && _item.Monedas <= 5)//Si son monedas
                    {
                        _imagen = "ms-appx:///Assets/moneda_casilla.jpg";
                    }
                    else
                    {
                        if (_item.Monedas == 20)//Si es una estrella
                        {
                            _imagen = "ms-appx:///Assets/estrella_casilla.jpg";
                        }
                        else//Si es un fantasma
                        {
                            _imagen = "ms-appx:///Assets/boo_casilla.jpg";
                        }
                    }
                }
                NotifyPropertyChanged();
                NotifyPropertyChanged("Imagen");
            }
        }

        public bool Seleccionada
        {
            get
            {
                return _seleccionada;
            }
            set
            {
                _seleccionada = value;
                NotifyPropertyChanged();
            }
        }

        public String Imagen
        {
            get
            {
                return _imagen;
            }
            set
            {
                _imagen = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
