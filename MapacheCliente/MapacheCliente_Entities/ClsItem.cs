using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapacheCliente_Entities
{
    public class ClsItem
    {
        private int _tipoItem;
        private int _monedas;

        #region Constructores

        public ClsItem()
        {
            _tipoItem = 0;
            _monedas = 0;
        }

        public ClsItem(int tipoItem, int monedas)
        {
            _tipoItem = tipoItem;
            _monedas = monedas;
        }

        #endregion
        #region Propiedades públicas
        public int TipoItem
        {
            get
            {
                return _tipoItem;
            }
            set
            {
                _tipoItem = value;
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
            }
        }

        #endregion
    }
}
