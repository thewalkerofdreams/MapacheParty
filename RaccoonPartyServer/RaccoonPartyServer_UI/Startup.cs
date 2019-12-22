using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using RaccoonPartyServer_UI.Gestoras;
using RaccoonPartyServer_UI.Models;

[assembly: OwinStartup(typeof(RaccoonPartyServer_UI.Startup))]

namespace RaccoonPartyServer_UI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ClsMetodosJuego.reset();
        }
    }
}
