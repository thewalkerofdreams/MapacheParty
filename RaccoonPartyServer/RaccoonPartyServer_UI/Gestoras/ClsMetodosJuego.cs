using RaccoonPartyServer_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaccoonPartyServer_UI.Gestoras
{
    public class ClsMetodosJuego
    {
        /// <summary>
        /// Interfaz
        /// Nombre: reset
        /// Comentario: Este método nos permite resetear el juego, pasando todos sus valores a por
        /// defecto.
        /// Cabecera: public static void reset()
        /// Postcondiciones: El método modifica los valores del juego a por defecto.
        /// </summary>
        public static void reset()
        {
            ClsGestoraTablero.tableroAleatorio();//Generamos el tablero aleatorio
            ClsDatosJuego.jugadores = new Dictionary<string, ClsJugador>();
            ClsDatosJuego.numeroDeJugadores = 0;
            ClsDatosJuego.estrellasEncontradas = 0;
            ClsDatosJuego.turnoJugador = 1;
        }
    }
}