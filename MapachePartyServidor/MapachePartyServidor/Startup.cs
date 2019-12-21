using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapachePartyServidor.Gestora;
using MapachePartyServidor.Gestora_Tablero;
using MapachePartyServidor_Entities;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MapachePartyServidor.Startup))]

namespace MapachePartyServidor
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
            ClsGestoraTablero.tableroAleatorio();//Generamos el tablero aleatorio
            ClsDatosJuego.jugadores = new Dictionary<string, ClsJugador>();
            ClsDatosJuego.numeroDeJugadores = 0;
            ClsDatosJuego.estrellasEncontradas = 0;
        }
    }
}