using MapachePartyServidor_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapachePartyServidor.Gestora
{
    public class ClsDatosJuego
    {  
        public static List<ClsCasilla> tablero { get; set; }
        public static IDictionary<string, ClsJugador> jugadores { get; set; }
        public static int numeroDeJugadores { get; set; }
        public static int estrellasEncontradas { get; set; }
    }
}