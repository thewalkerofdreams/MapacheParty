using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MapachePartyServidor;
using MapachePartyServidor.Gestora;
using MapachePartyServidor_Entities;
using Microsoft.AspNet.SignalR;

namespace MapachePartyServidor
{
    public class ClsMapachePartyHub : Hub
    {
        public void seleccionarCasilla(int posicionCasilla)
        {
            //Nos aseguramos que la casilla pulsada no ha sido seleccionada antes
            if (!ClsDatosJuego.tablero[posicionCasilla].Seleccionada)
            {
                //Descubrimos la casilla en los otros clientes
                ClsDatosJuego.tablero[posicionCasilla].Seleccionada = true;
                Clients.All.descubrirCasilla(posicionCasilla);

                Random random = new Random();
                int posicionCasillaParaSeta = 0;
                ICollection<String> keys = ClsDatosJuego.jugadores.Keys;    //Reduciremos las monedas de todos los jugadores enemigos a la mitad
                List<String> listKeys = keys.ToList();
                int turnoJugador = 0;

                switch (ClsDatosJuego.tablero[posicionCasilla].Item.TipoItem)
                {
                    case 1://si se ha activado una caja sorpresa
                        if (ClsDatosJuego.tablero[posicionCasilla].Item.Monedas >= 1 && ClsDatosJuego.tablero[posicionCasilla].Item.Monedas <= 5)//Si son monedas
                        {
                            ClsDatosJuego.tablero[posicionCasilla].Imagen = "ms-appx:///Assets/moneda_casilla.jpg";
                        }
                        else
                        {
                            if (ClsDatosJuego.tablero[posicionCasilla].Item.Monedas == 20)//Si es una estrella
                            {
                                ClsDatosJuego.tablero[posicionCasilla].Imagen = "ms-appx:///Assets/estrella_casilla.jpg";
                            }
                            else//Si es un fantasma
                            {
                                ClsDatosJuego.tablero[posicionCasilla].Imagen = "ms-appx:///Assets/boo_casilla.jpg";
                            }
                        }
                        ClsDatosJuego.jugadores[Context.ConnectionId].Monedas += ClsDatosJuego.tablero[posicionCasilla].Item.Monedas;//Acumulamos el número de monedas ganadas
                        Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);

                        turnoJugador = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 2 : 1;
                        Clients.AllExcept(Context.ConnectionId).cambiarTurno(turnoJugador);//Cambiamos el turno de juego
                        break;
                    case 2://si se ha activado una seta (Para diferenciar una casilla seta de la otra, he jugado con las monedas del tipo Item, de esta manera nos ahorramos una variable y vuelvo útil otra que ya no debía serlo)
                        posicionCasillaParaSeta = ClsDatosJuego.tablero.IndexOf(ClsDatosJuego.tablero[posicionCasilla]);
                        if (posicionCasillaParaSeta > 4)//si existe una casilla superior a la seta
                        {
                            ClsDatosJuego.tablero[posicionCasillaParaSeta - 5].Oculta = false;
                        }
                        if (posicionCasillaParaSeta < 19)//si existe una casilla inferior a la seta
                        {
                            ClsDatosJuego.tablero[posicionCasillaParaSeta + 5].Oculta = false;
                        }
                        if (posicionCasillaParaSeta != 4 && posicionCasillaParaSeta != 9 &&
                            posicionCasillaParaSeta != 14 && posicionCasillaParaSeta != 19 &&
                            posicionCasillaParaSeta != 24)//si existe una casilla a la derecha de la seta
                        {
                            ClsDatosJuego.tablero[posicionCasillaParaSeta + 1].Oculta = false;
                        }
                        if (posicionCasillaParaSeta % 5 != 0)//Si existe una casilla a la izquierda de la seta
                        {
                            ClsDatosJuego.tablero[posicionCasillaParaSeta - 1].Oculta = false;
                        }

                        ClsDatosJuego.jugadores[Context.ConnectionId].Monedas -= 3;//Descontamos las monedas al jugador
                        Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);
                        break;
                    case 3://si se ha activado una bomba
                        if (random.Next(1, 2) == 1)
                        {
                            if (ClsDatosJuego.jugadores[Context.ConnectionId].Monedas > 0)
                            {
                                ClsDatosJuego.jugadores[Context.ConnectionId].Monedas /= 2;//Reducimos el número de monedas
                                Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);
                            }
                        }
                        else
                        {
                            /*if (_monedasJugador2 > 0)
                            {
                                _monedasJugador2 /= 2;
                                NotifyPropertyChanged("MonedasJugador2");
                            }*/
                            for (int i = 0; i < listKeys.Count; i++)//Lo dejaremos así por si en un futuro desamos que haya cuatro jugadores
                            {
                                if (!listKeys.ElementAt(i).Equals(Context.ConnectionId))
                                {
                                    ClsDatosJuego.jugadores[listKeys.ElementAt(i)].Monedas /= 2;
                                    Clients.User(listKeys.ElementAt(i)).updatePersonalCoins(ClsDatosJuego.jugadores[listKeys.ElementAt(i)].Monedas);
                                }
                            }
                        }
                        turnoJugador = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 2 : 1;
                        Clients.AllExcept(Context.ConnectionId).cambiarTurno(turnoJugador);//Cambiamos el turno de juego
                        break;
                    case 4://si se ha activado la casilla de bowser
                        ICollection<String> keys02 = ClsDatosJuego.jugadores.Keys;
                        List<String> listKeys01 = keys02.ToList();
                        int idjugadorActual = listKeys01.ElementAt(0).Equals(Context.ConnectionId) ? 1 : 2;
                        int idJugadorGanador = !listKeys01.ElementAt(0).Equals(Context.ConnectionId) ? 1 : 2;
                        if (random.Next(1, 10) == 1)
                        {
                            idJugadorGanador = idjugadorActual;
                        }
                        Clients.All.endGame(idJugadorGanador);
                        break;
                }

                //Si ya se han encontrado las cuatro estrellas
                if (ClsDatosJuego.estrellasEncontradas == 4)
                {
                    int auxMonedas = -7, posicionJugadorGanador = 0;
                    for (int i = 0; i < ClsDatosJuego.jugadores.Count; i++)
                    {
                        if (ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas > auxMonedas)
                        {
                            auxMonedas = ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas;
                            posicionJugadorGanador = i;
                        }
                    }
                    Clients.All.endGame(posicionJugadorGanador++);//Indicamos el jugador que ha ganado: 1, 2, 3... (Ahora mismo solo tenemos dos jugadores)
                }
            }
        }


        /// <summary>
        /// Comentario: Los clientes llamarán a este método cuando quieran que se cambio de turno.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void cambioTurno(int idJugador)
        {
            int turnoJugador = idJugador == 1 ? 2 : 1;//En este caso como solo tenemos dos jugadores

            // Call the broadcastMessage method to update clients.
            Clients.All.cambiarTurno(turnoJugador);//Existirá la función??????
        }

        //Cuando un jugador se desconecte
        public override Task OnDisconnected(bool stopCalled)
        {
            //Cuando se desconecte
            ClsDatosJuego.jugadores.Remove(Context.ConnectionId);

            //Restamos uno al número de jugadores
            ClsDatosJuego.numeroDeJugadores--;
            Clients.All.updateNumberOfPlayers(ClsDatosJuego.numeroDeJugadores);

            return base.OnDisconnected(stopCalled);
        }

        //Cuando un jugador se conecte
        public override Task OnConnected()
        {
            //Cuando se conecte, agregamos su entrada al diccionario
            if (ClsDatosJuego.jugadores.Count < 2)
            {
                int idJugador = ClsDatosJuego.jugadores.ElementAt(0).Value.Id == 1 ? 2 : 1;//Obtenemos la id del jugador
                ClsDatosJuego.jugadores.Add(Context.ConnectionId, new ClsJugador(idJugador));

                //Le mandamos la información de la partida
                Clients.Caller.cargarTablero(ClsDatosJuego.tablero);
                //Clients.Caller.updateGlobalScore(GameInfo.globalScore);
                //Clients.Caller.updateRanking(GameInfo.jugadores.Values.ToList().OrderByDescending(x => x.puntuacion));

                //Y llamamos al método que indica que todo ha cargado
                Clients.Caller.onConnectedIsDone();
            }

            return base.OnConnected();
        }

        public void obtenerIdJugador()
        {
            ICollection<String> keys = ClsDatosJuego.jugadores.Keys;
            List<String> listKeys = keys.ToList();
            int idjugadorActual = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 1 : 2;
            Clients.Caller.obtenerIdJugador(idjugadorActual);
        }
    }
}