using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaccoonPartyServer.Models
{
    public class ClsJugador
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
    }
}