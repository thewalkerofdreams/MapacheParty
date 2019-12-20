using System;
using System.Web;
using MapachePartyServidor;
using Microsoft.AspNet.SignalR;

namespace MapachePartyServidor
{
    public class ClsMapachePartyHub : Hub
    {
        private int _turnoJugador = 0;

        /*public void SendUWP(ClsMensaje msg)   
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(msg);
        }*/
        /// <summary>
        /// Comentario: Los clientes llamarán a este método cuando quieran que se cambio de turno.
        /// </summary>
        /// <param name="idJugador"></param>
        public void Send(int idJugador)
        {
            int turnoJugador = idJugador == 1 ? 2 : 1;

            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(turnoJugador);//Existirá la función??????
        }

        /// <summary>
        /// Comentario: Los clientes llamarán a este método para obtener una id.
        /// </summary>
        /// <returns></returns>
        public void GetConnectionId()
        {
            Clients.All.getPlayerID(Context.ConnectionId);
        }
    }
}