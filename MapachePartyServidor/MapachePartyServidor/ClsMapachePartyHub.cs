using System;
using System.Web;
using MapachePartyServidor;
using Microsoft.AspNet.SignalR;

namespace MapachePartyServidor
{
    public class ClsMapachePartyHub : Hub
    {
        /*public void SendUWP(ClsMensaje msg)   
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(msg);
        }*/
        /// <summary>
        /// Comentario: Los clientes llamarán a este método cuando quieran que su mensaje aparazca 
        /// en el listado de mensajes de todos los clientes (EL TODO).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(String name, String message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);//Existirá la función??????
        }
    }
}