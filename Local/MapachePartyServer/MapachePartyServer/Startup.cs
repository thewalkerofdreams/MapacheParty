using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using RaccoonPartyServer.Gestoras;
using RaccoonPartyServer.Models;

[assembly: OwinStartup(typeof(MapachePartyServer.Startup))]

namespace MapachePartyServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ClsDatosJuego.jugadores = new Dictionary<string, ClsJugador>();
            ClsDatosJuego.numeroDeJugadores = 0;
            ClsMetodosJuego.reset();
        }
    }
}
