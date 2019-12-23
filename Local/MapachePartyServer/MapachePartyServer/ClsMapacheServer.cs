using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using RaccoonPartyServer.Gestoras;
using RaccoonPartyServer.Models;

namespace MapachePartyServer
{
    public class ClsMapacheServer : Hub
    {
        /// <summary>
        /// Interfaz
        /// Nombre: seleccionarCasilla
        /// Comentario: Este método nos permite seleccionar una casilla del tablero, 
        /// realizando los cambios oportunos en la partida según la casilla seleccionada.
        /// Posbles acciones al presionar una casilla:
        /// Caja sorpresa:
        ///     -Puedes conseguir de 1 a 5 monedas.
        ///     -Puedes conseguir una estrella (20 monedas).
        ///     -Puede que la casilla contenga un fantasma (El jugador no gana monedas).
        /// Bomba:
        ///     -El jugador o el oponente pierde la mitad de sus monedas, si el jugador que sufre el penalizador tiene más de 0 monedas.
        /// Seta:
        ///     -El jugador pierde 3 monedas (Puede quedarse con un valor negativo de monedas) y descubre las cajas sorpresa adyacentes a la casilla de la seta.
        /// Bowser:
        ///     -El jugador tendrá un 10% de probabilidades de ganar el juego automaticamente o en caso contrario de perder la partida.
        /// Cabecera: public void seleccionarCasilla(int posicionCasilla)
        /// Entrada:
        ///     -int posicionCasilla
        /// Precondiciones:
        ///     -La aplicación cliente se encargará de verificar que la casilla seleccionada, no fue seleccionada anteriormente en esa partida. //Nos ahorramos código en el servidor
        /// Postcondiciones: Se modifica el estado actual de la partida, según la casilla seleccionada.
        /// </summary>
        /// <param name="posicionCasilla"></param>
        public void seleccionarCasilla(int posicionCasilla)
        {
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
                        Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/moneda2_casilla.jpg");//Descubrimos esa casilla en los clientes
                    }
                    else
                    {
                        if (ClsDatosJuego.tablero[posicionCasilla].Item.Monedas == 20)//Si es una estrella
                        {
                            Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/estrella2_casilla.jpg");//Descubrimos esa casilla en los clientes
                            ClsDatosJuego.estrellasEncontradas++;
                        }
                        else//Si es un fantasma
                        {
                            Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/boo2_casilla.jpg");//Descubrimos esa casilla en los clientes
                        }
                    }
                    ClsDatosJuego.jugadores[Context.ConnectionId].Monedas += ClsDatosJuego.tablero[posicionCasilla].Item.Monedas;//Acumulamos el número de monedas ganadas
                    Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);
                    Clients.AllExcept(Context.ConnectionId).updateEnemyCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);//Le indicamos al rival las monedas

                    turnoJugador = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 2 : 1;
                    Clients.All.cambiarTurno(turnoJugador);//Cambiamos el turno de juego
                    break;
                case 2://si se ha activado una seta (Para diferenciar una casilla seta de la otra, he jugado con las monedas del tipo Item, de esta manera nos ahorramos una variable y vuelvo útil otra que ya no debía serlo)
                    posicionCasillaParaSeta = ClsDatosJuego.tablero.IndexOf(ClsDatosJuego.tablero[posicionCasilla]);
                    if (posicionCasillaParaSeta > 4)//si existe una casilla superior a la seta
                    {
                        Clients.All.desocultarCasilla(posicionCasillaParaSeta - 5);
                    }
                    if (posicionCasillaParaSeta < 19)//si existe una casilla inferior a la seta
                    {
                        Clients.All.desocultarCasilla(posicionCasillaParaSeta + 5);
                    }
                    if (posicionCasillaParaSeta != 4 && posicionCasillaParaSeta != 9 &&
                        posicionCasillaParaSeta != 14 && posicionCasillaParaSeta != 19 &&
                        posicionCasillaParaSeta != 24)//si existe una casilla a la derecha de la seta
                    {
                        Clients.All.desocultarCasilla(posicionCasillaParaSeta + 1);
                    }
                    if (posicionCasillaParaSeta % 5 != 0)//Si existe una casilla a la izquierda de la seta
                    {
                        Clients.All.desocultarCasilla(posicionCasillaParaSeta - 1);
                    }

                    ClsDatosJuego.jugadores[Context.ConnectionId].Monedas -= 3;//Descontamos las monedas al jugador
                    Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);
                    Clients.AllExcept(Context.ConnectionId).updateEnemyCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);//Le indicamos al rival las monedas
                    Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/seta2_casilla.jpg");//Descubrimos esa casilla en los clientes
                    break;
                case 3://si se ha activado una bomba
                    if (random.Next(2) == 1)
                    {
                        if (ClsDatosJuego.jugadores[Context.ConnectionId].Monedas > 0)
                        {
                            ClsDatosJuego.jugadores[Context.ConnectionId].Monedas /= 2;//Reducimos el número de monedas
                            Clients.Caller.updatePersonalCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);
                            Clients.AllExcept(Context.ConnectionId).updateEnemyCoins(ClsDatosJuego.jugadores[Context.ConnectionId].Monedas);//Le indicamos al rival las monedas
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listKeys.Count; i++)//Lo dejaremos así por si en un futuro desamos que haya cuatro jugadores, para dos jugadores sería una línea de código
                        {
                            if (!listKeys.ElementAt(i).Equals(Context.ConnectionId))
                            {
                                ClsDatosJuego.jugadores[listKeys.ElementAt(i)].Monedas /= 2;
                                Clients.AllExcept(Context.ConnectionId).updatePersonalCoins(ClsDatosJuego.jugadores[listKeys.ElementAt(i)].Monedas);
                                Clients.Caller.updateEnemyCoins(ClsDatosJuego.jugadores[listKeys.ElementAt(i)].Monedas);
                            }
                        }
                    }
                    turnoJugador = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 2 : 1;
                    Clients.All.cambiarTurno(turnoJugador);//Cambiamos el turno de juego
                    Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/bomb2_casilla.jpg");//Descubrimos esa casilla en los clientes
                    break;
                case 4://si se ha activado la casilla de bowser    
                    int idjugadorActual = listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 1 : 2;
                    int idJugadorGanador = !listKeys.ElementAt(0).Equals(Context.ConnectionId) ? 1 : 2;
                    if (random.Next(1, 10) == 2)
                    {
                        idJugadorGanador = idjugadorActual;
                    }
                    Clients.All.endGame(idJugadorGanador);
                    ClsMetodosJuego.reset();
                    Clients.All.cargarTablero(ClsDatosJuego.tablero);
                    Clients.All.updatePersonalCoins(0);
                    Clients.All.updateEnemyCoins(0);
                    Clients.All.descubrirCasilla(posicionCasilla, "ms-appx:///Assets/bowser_casilla.jpg");//Descubrimos esa casilla en los clientes
                    for (int i = 0; i < ClsDatosJuego.jugadores.Count; i++)
                    {
                        ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas = 0;
                    }
                    break;
            }

            //Si ya se han encontrado las cuatro estrellas
            if (ClsDatosJuego.tablero[posicionCasilla].Item.TipoItem != 4 && ClsDatosJuego.estrellasEncontradas == 4)//Si no se pulso en la casilla bowser y ya se han encontrado todas las estrellas
            {
                int auxMonedas = -7, posicionJugadorGanador = 0;
                for (int i = 0; i < ClsDatosJuego.jugadores.Count; i++)//TODO Obtenemos el ganador del juego, cambiar solo para dos jugadores, es una línea
                {
                    if (ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas > auxMonedas)
                    {
                        auxMonedas = ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas;
                        posicionJugadorGanador = i;
                    }
                }
                Clients.All.endGame(++posicionJugadorGanador);//Indicamos el jugador que ha ganado: 1, 2, 3... (Ahora mismo solo tenemos dos jugadores)
                ClsMetodosJuego.reset();
                Clients.All.cargarTablero(ClsDatosJuego.tablero);
                Clients.All.updatePersonalCoins(0);
                Clients.All.updateEnemyCoins(0);
                for (int i = 0; i < ClsDatosJuego.jugadores.Count; i++)
                {
                    ClsDatosJuego.jugadores.ElementAt(i).Value.Monedas = 0;
                }
            }
        }

        /// <summary>
        /// Comentario: Los clientes llamarán a este método cuando quieran saber que jugador debe jugar el turno.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void getTurnoActual()
        {
            Clients.All.cambiarTurno(ClsDatosJuego.turnoJugador);
        }

        //Cuando un jugador se desconecte
        public override Task OnDisconnected(bool stopCalled)
        {
            //Cuando se desconecte, lo eliminamos del diccionario de jugadores
            ClsDatosJuego.jugadores.Remove(Context.ConnectionId);

            //Informamos al resto de jugadores
            Clients.All.updateNumberOfPlayers(ClsDatosJuego.jugadores.Count);

            ClsMetodosJuego.reset();//Cuando un jugador se desconecta, se resetea el juego

            return base.OnDisconnected(stopCalled);
        }

        //Cuando un jugador se conecte
        public override Task OnConnected()
        {
            //Cuando se conecte, agregamos su entrada al diccionario
            if (ClsDatosJuego.jugadores.Count < 2)
            {
                if (ClsDatosJuego.jugadores.Count == 0)//Si aún no hay jugadores
                {
                    ClsDatosJuego.jugadores.Add(Context.ConnectionId, new ClsJugador(1));
                }
                else
                {
                    int idJugador = ClsDatosJuego.jugadores.ElementAt(0).Value.Id == 1 ? 2 : 1;//Obtenemos la id del jugador
                    ClsDatosJuego.jugadores.Add(Context.ConnectionId, new ClsJugador(idJugador));
                }

                //Mandamos el número de jugadores actuales a todos los clientes
                Clients.All.updateNumberOfPlayers(ClsDatosJuego.jugadores.Count);

                //Le mandamos la información de la partida
                Clients.Caller.cargarTablero(ClsDatosJuego.tablero);
                Clients.Caller.pasarIdJugador(ClsDatosJuego.jugadores[Context.ConnectionId].Id);
                Clients.All.cambiarTurno(1);//Indicamos que el primero en jugar será el jugador 1

                //Llamamos al método que indica que todo ha cargado correctamente
                Clients.Caller.onConnectedIsDone();
            }

            return base.OnConnected();
        }
    }
}