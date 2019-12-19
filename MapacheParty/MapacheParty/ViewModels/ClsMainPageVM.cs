using MapacheParty.Utilidades;
using MapacheParty_Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapacheParty.ViewModels
{
    public class ClsMainPageVM : INotifyPropertyChanged
    {
        private List<ClsCasilla> _tablero;
        private ClsCasilla _casillaSeleccionada;
        private int _turnoJugador;
        private int _estrellasEncontradas;
        private int _monedasJugador1;
        private int _monedasJugador2;
        private int _jugadorGanador;
        DelegateCommand _ejecutarCasilla;//No lo utilizo, preguntar antes de quitar a Fernando
        private String _mensajeVictoria;

        #region Constructores
        public ClsMainPageVM()
        {
            tableroAleatorio();//Generamos aleatoriamente un tablero de juego
        }
        #endregion

        #region Propiedades públicas 
        public List<ClsCasilla> Tablero
        {
            get
            {
                return _tablero;
            }
            set
            {
                _tablero = value;
                NotifyPropertyChanged();
            }
        }

        public int TurnoJugador
        {
            get
            {
                return _turnoJugador;
            }
            set
            {
                _turnoJugador = value;
                NotifyPropertyChanged();
            }
        }

        public ClsCasilla CasillaSeleccionada
        {
            get
            {
                return _casillaSeleccionada;
            }
            set
            {
                _casillaSeleccionada = value;
                if (_casillaSeleccionada != null && _casillaSeleccionada.Seleccionada == false)
                {
                    ejecutarCasilla();
                    _casillaSeleccionada.Seleccionada = true;//Indicamos que la casilla ya ha sido seleccionada

                    if (_casillaSeleccionada.Item.TipoItem == 1 && _casillaSeleccionada.Item.Monedas == 20)//Si se ha seleccionado una estrella
                    {
                        _estrellasEncontradas++;
                        if (_estrellasEncontradas == 4)//Fin del juego
                        {
                            if (_monedasJugador1 > _monedasJugador2)
                            {
                                _jugadorGanador = 1;//Gana el jugador1
                            }
                            else
                            {
                                if (_monedasJugador2 > _monedasJugador1)
                                {
                                    _jugadorGanador = 2;//Gana el jugador2
                                }
                                else
                                {
                                    _jugadorGanador = 3;//Empate
                                }
                            }
                            NotifyPropertyChanged("JugadorGanador");
                        }
                        NotifyPropertyChanged("EstrellasEncontradas");
                    }
                }
                NotifyPropertyChanged();
            }
        }

        public int MonedasJugador1
        {
            get
            {
                return _monedasJugador1;
            }
            set
            {
                _monedasJugador1 = value;
                NotifyPropertyChanged();
            }
        }

        public int MonedasJugador2
        {
            get
            {
                return _monedasJugador2;
            }
            set
            {
                _monedasJugador2 = value;
                NotifyPropertyChanged();
            }
        }

        public int JugadorGanador
        {
            get
            {
                return _jugadorGanador;
            }
            set
            {
                _jugadorGanador = value;
                NotifyPropertyChanged();
            }
        }

        public DelegateCommand EjecutarCasilla
        {
            get
            {
                return _ejecutarCasilla;
            }
        }

        public int EstrellasEncontradas
        {
            get
            {
                return _estrellasEncontradas;
            }
            set
            {
                _estrellasEncontradas = value;
                NotifyPropertyChanged();
            }
        }

        public String MensajeVictoria
        {
            get
            {
                return _mensajeVictoria;
            }
            set
            {
                _mensajeVictoria = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Métodos Tablero
        /// <summary>
        /// Comentario: Este método nos permite generar un tablero aleatorio para
        /// la aplicación MapacheParty. El tablero constará de 5x5 casillas.
        /// Tendrá los siguientes items:
        /// -2 setas
        /// -1 bowser
        /// -4 bombas
        /// -18 cajas sorpresa
        /// </summary>
        public void tableroAleatorio()
        {
            List<ClsCasilla> listadoCasillas = new List<ClsCasilla>();
            Random random = new Random();
            int posicionCasilla = 0;
            bool bowserInsertado = false;

            for (int i = 0; i < 25; i++)//Rellenamos todo el tablero con cajas sorpresa
            {
                switch (random.Next(1, 2))
                {
                    case 1://Caja sorpresa con monedas
                        listadoCasillas.Add(new ClsCasilla(new ClsItem(1, random.Next(1, 5)), true, false, "ms-appx:///Assets/caja_sorpresa_casilla.jpg"));
                        break;
                    case 2://Caja sorpresa con fantasma
                        listadoCasillas.Add(new ClsCasilla(new ClsItem(1, 0), true, false, "ms-appx:///Assets/caja_sorpresa_casilla.jpg"));
                        break;
                }     
            }

            for (int i = 0; i < 4; i++)//Insertamos las cuatro bombas en el tablero
            {
                posicionCasilla = random.Next(0, 24);
                if (listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 3)//Si la posición no contiene una bomba
                {
                    listadoCasillas[posicionCasilla] = new ClsCasilla(new ClsItem(3, 0), true, false, "ms-appx:///Assets/bomb_casilla.jpg");
                }
                else
                {
                    i--;
                }
            }
            
            for (int i = 0; i < 2; i++)//Insertamos las dos setas en el tablero
            {
                posicionCasilla = random.Next(0, 24);
                if (listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 3 && 
                    listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 2)//Si la posición no contiene una bomba o una seta
                {
                    listadoCasillas[posicionCasilla] = new ClsCasilla(new ClsItem(2, i), true, false, "ms-appx:///Assets/seta_casilla.jpg");
                }
                else
                {
                    i--;
                }
            }

            do//Insertamos la casilla de bowser
            {
                posicionCasilla = random.Next(0, 24);
                if (listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 3 &&
                    listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 2)//Si la posición no contiene una bomba o una seta
                {
                    listadoCasillas[posicionCasilla] = new ClsCasilla(new ClsItem(4, 0), true, false, "ms-appx:///Assets/bowser_casilla.jpg");
                    bowserInsertado = true;
                }
            } while (!bowserInsertado);//Si no se ha conseguido insertar la casilla bowser

            for (int i = 0; i < 4; i++)//Insertamos las cuatro estrellas en el tablero
            {
                posicionCasilla = random.Next(0, 24);
                if (listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 3 &&
                    listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 2 &&
                    listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 4)//Si la posición no contiene una bomba, una seta o una casilla bowser
                {
                    listadoCasillas[posicionCasilla] = new ClsCasilla(new ClsItem(1, 20), true, false, "ms-appx:///Assets/caja_sorpresa_casilla.jpg");
                }
                else
                {
                    i--;
                }
            }

            Tablero = listadoCasillas;//Insertamos el nuevo tablero
        }

        #endregion

        #region Comandos
        /// <summary>
        /// Nombre: ejecutarCasilla
        /// Comentario: Este comando nos permite ejecutar un tipo de casilla, según el tipo seleccionado
        /// ocurrirá lo siguiente:
        /// -Caja sorpresa:
        ///     -Podrá aparecer un fantasma no consiguiendo ninguna moneda el jugador
        ///     -Podrá aparecer una moneda donde el jugador obtener entre 1 y 5 monedas.
        ///     -Podrá aparecer una estrella, obteniendo el jugador 20 monedas.
        /// -Seta: El jugador perderá 3 monedas y se mostrará el contenido de las cajas sorpresas adyacentes a la seta.
        /// -Bomba: El jugador o el enemigo perderá lamitad de sus monedas. Si el perdedor no tiene una cantidad positiva de monedas, no perderá nada.
        /// -Bowser: El jugador tendrá un 10% de posibilidades de ganar o perder el juego automáticamente.
        /// Cabecera: public void ejecutarCasilla()
        /// Precondiciones:
        ///     -CasillaSeleccionada debe ser diferente de null y tampoco el item que contiene.
        /// Postcondiciones: El método ejecuta una casilla del teblero.    
        /// </summary>
        public void ejecutarCasilla()
        {
            Random random = new Random();
            int posicionCasillaParaSeta = 0;

            switch (_casillaSeleccionada.Item.TipoItem)
            {                                                   //Preguntar: No se si tengo que tener dos marcadores de jugadores, es decir, monedasJugador1 y monedasjugador2
                case 1://si se ha activado una caja sorpresa
                    if (_casillaSeleccionada.Item.Monedas >= 1 && _casillaSeleccionada.Item.Monedas <= 5)//Si son monedas
                    {
                        _casillaSeleccionada.Imagen = "ms-appx:///Assets/moneda_casilla.jpg";
                    }
                    else
                    {
                        if (_casillaSeleccionada.Item.Monedas == 20)//Si es una estrella
                        {
                            _casillaSeleccionada.Imagen = "ms-appx:///Assets/estrella_casilla.jpg";
                        }
                        else//Si es un fantasma
                        {
                            _casillaSeleccionada.Imagen = "ms-appx:///Assets/boo_casilla.jpg";
                        }
                    }
                    _monedasJugador1 += _casillaSeleccionada.Item.Monedas;//Acumulamos el número de monedas ganadas
                    NotifyPropertyChanged("MonedasJugador1");
                    break;
                case 2://si se ha activado una seta (Para diferenciar una casilla seta de la otra, he jugado con las monedas del tipo Item, de esta manera nos ahorramos una variable y vuelvo útil otra que ya no debía serlo)
                    posicionCasillaParaSeta = _tablero.IndexOf(CasillaSeleccionada);
                    if (posicionCasillaParaSeta > 4)//si existe una casilla superior a la seta
                    {
                        _tablero.ElementAt(posicionCasillaParaSeta - 5).Oculta = false;
                    }
                    if (posicionCasillaParaSeta < 19)//si existe una casilla inferior a la seta
                    {
                        _tablero.ElementAt(posicionCasillaParaSeta + 5).Oculta = false;
                    }
                    if (posicionCasillaParaSeta != 4 && posicionCasillaParaSeta != 9 && 
                        posicionCasillaParaSeta != 14 && posicionCasillaParaSeta != 19 &&
                        posicionCasillaParaSeta != 24)//si existe una casilla a la derecha de la seta
                    {
                        _tablero.ElementAt(posicionCasillaParaSeta + 1).Oculta = false;
                    }
                    if (posicionCasillaParaSeta % 5 != 0)//Si existe una casilla a la izquierda de la seta
                    {
                        _tablero.ElementAt(posicionCasillaParaSeta - 1).Oculta = false;
                    }
                    _monedasJugador1 -= 3;//Descontamos 3 monedas
                    NotifyPropertyChanged("MonedasJugador1");
                    break;
                case 3://si se ha activado una bomba
                    if (random.Next(1, 2) == 1)
                    {
                        if (_monedasJugador1 > 0)
                            _monedasJugador1 /= 2; 
                    }
                    else
                    {
                        if (_monedasJugador2 > 0)
                            _monedasJugador2 /= 2;
                    }
                    NotifyPropertyChanged("MonedasJugador1");
                    NotifyPropertyChanged("MonedasJugador2");
                    break;
                case 4://si se ha activado la casilla de bowser
                    if (random.Next(1, 10) == 1)
                    {
                        _jugadorGanador = 1;
                    }
                    else
                    {
                        _jugadorGanador = 2;
                    }
                    NotifyPropertyChanged("JugadorGanador");
                    break;
            }
            NotifyPropertyChanged("Tablero");
        }

        #endregion
    }
}
