using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MapachePartyServidor;
using Microsoft.AspNet.SignalR;

namespace MapachePartyServidor
{
    public class ClsMapachePartyHub : Hub
    {
        public static IDictionary<string, int> idjugadores { get; set; }

        /// <summary>
        /// Comentario: Los clientes llamarán a este método cuando quieran que se cambio de turno.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(int idJugador)
        {
            int turnoJugador = idJugador == 1 ? 2 : 1;

            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(turnoJugador);//Existirá la función??????
        }

        //Cuando un jugador se desconecte
        public override Task OnDisconnected(bool stopCalled)
        {
            //Cuando se desconecte, quitamos su entrada del diccionario
            idjugadores.Remove(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        //Cuando un jugador se conecte
        public override Task OnConnected()
        {
            //Cuando se conecte un jugador, agregamos su entrada al diccionario
            if (idjugadores.Count < 2)
            {
                idjugadores.Add(Context.ConnectionId, idjugadores.Count +1);
            }

            //Llamamos al método que indica que todo ha cargado
            Clients.Caller.onConnectedIsDone(idjugadores[Context.ConnectionId]);

            return base.OnConnected();
        }
    }
}