using MapacheCliente_Entities;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace MapacheCliente.ViewModels
{
    public class ClsMainPageVM : INotifyPropertyChanged
    {
        private List<ClsCasilla> _tablero;
        private ClsCasilla _casillaSeleccionada;
        private int _numeroJugadoresEnSala;
        private int _turnoJugador;
        private int _monedasRival;
        private int _jugadorGanador;
        private String _mensajeVictoria;
        private bool _isNotDoneLoading;
        private ClsJugador _jugador;
        private HubConnection conn;
        private IHubProxy proxy;


        #region Constructores
        public ClsMainPageVM()
        {
            SignalR();
            _jugador = new ClsJugador();
            _monedasRival = 0;
            _turnoJugador = 1;
            _jugadorGanador = 0;
            _isNotDoneLoading = true;
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
                if (_turnoJugador == _jugador.Id && _casillaSeleccionada != null && _casillaSeleccionada.Seleccionada == false && _numeroJugadoresEnSala == 2)
                {
                    proxy.Invoke("seleccionarCasilla", Tablero.IndexOf(_casillaSeleccionada));
                }
                NotifyPropertyChanged();
            }
        }

        public int MonedasRival
        {
            get
            {
                return _monedasRival;
            }
            set
            {
                _monedasRival = value;
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
                if (_jugadorGanador != 0)
                {
                    switch (_jugadorGanador)
                    {
                        case 1:
                            _mensajeVictoria = "¡Ha ganado el jugador 1!";
                            break;
                        case 2:
                            _mensajeVictoria = "¡Ha ganado el jugador 2!";
                            break;
                        case 3:
                            _mensajeVictoria = "¡Ha sido un empate!";
                            break;
                    }
                    NotifyPropertyChanged("MensajeVictoria");
                }
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

        public bool IsNotDoneLoading
        {
            get
            {
                return _isNotDoneLoading;
            }
            set
            {
                _isNotDoneLoading = value;
                NotifyPropertyChanged();
            }
        }

        public ClsJugador Jugador
        {
            get
            {
                return _jugador;
            }
            set
            {
                _jugador = value;
                NotifyPropertyChanged();
            }
        }

        public int NumeroJugadoresEnSala
        {
            get
            {
                return _numeroJugadoresEnSala;
            }
            set
            {
                _numeroJugadoresEnSala = value;
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

        #region Funciones SignalR
        /// <summary>
        /// Comentario: Este método nos permite instanciar la conexión con el servidor y declarar los métodos de respuesta 
        /// a sus envios.
        /// </summary>
        private void SignalR()
        {
            //conn = new HubConnection("https://raccoonpartyserverui.azurewebsites.net");//Instanciamos la conexión
            conn = new HubConnection("http://localhost:50678/"); 
            proxy = conn.CreateHubProxy("ClsMapacheServer");
            conn.Start();

            proxy.On<int>("endGame", endGame);
            proxy.On<List<Casilla>>("cargarTablero", cargarTablero);
            proxy.On<int>("getRivalCoins", getRivalCoins);
            proxy.On<int>("updatePersonalCoins", updatePersonalCoins);
            proxy.On<int>("updateEnemyCoins", updateEnemyCoins);
            proxy.On("onConnectedIsDone", onConnectedIsDone);
            proxy.On<int>("descubrirCasilla", descubrirCasilla);
            proxy.On<int>("cambiarTurno", cambiarTurno);
            proxy.On<int>("pasarIdJugador", pasarIdJugador);
            proxy.On<int>("desocultarCasilla", desocultarCasilla);
            proxy.On<int>("updateNumberOfPlayers", updateNumberOfPlayers);
        }

        public async void updateEnemyCoins(int monedasRival)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _monedasRival = monedasRival;
                NotifyPropertyChanged("MonedasRival");
            }
            );
        }

        public async void updateNumberOfPlayers(int numeroJugadores)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _numeroJugadoresEnSala = numeroJugadores;
                NotifyPropertyChanged("NumeroJugadoresEnSala");
            }
            );
        }

        public async void desocultarCasilla(int posicionCasilla)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _tablero.ElementAt(posicionCasilla).Oculta = false;
                NotifyPropertyChanged("Tablero");
            }
            );
        }

        public async void pasarIdJugador(int idJugador)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _jugador.Id = idJugador;
                NotifyPropertyChanged("Jugador");
            }
            );
        }

        public async void descubrirCasilla(int posicionCasilla)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _tablero.ElementAt(posicionCasilla).Seleccionada = true;
                _tablero.ElementAt(posicionCasilla).Oculta = false;
                NotifyPropertyChanged("Tablero");
            }
            );
        }

        public async void cambiarTurno(int idJugadorJugar)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _turnoJugador = idJugadorJugar;
                NotifyPropertyChanged("TurnoJugador");
            }
            );
        }

        public async void getRivalCoins(int coins)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                MonedasRival = coins;
            }
            );
        }

        private async void playSelectedBoxSound()
        {
            MediaElement mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("pop.wav");//TODO sacar otro sonido
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Play();
        }

        public async void updatePersonalCoins(int coins)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _jugador.Monedas = coins;
                NotifyPropertyChanged();
            }
            );
        }

        public async void cargarTablero(List<Casilla> tablero)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                List<ClsCasilla> listadoCasillas = new List<ClsCasilla>();
                foreach (Casilla casilla in tablero)
                {
                    listadoCasillas.Add(new ClsCasilla(casilla.Item, casilla.Oculta, casilla.Seleccionada, casilla.Imagen));
                }
                _tablero = listadoCasillas;
                NotifyPropertyChanged("Tablero");
            }
            );
        }

        public async void endGame(int idjugadorGanador)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                JugadorGanador = idjugadorGanador;
                mensajeFinPartida(idjugadorGanador);
            }
            );
            
        }

        public async void onConnectedIsDone()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                IsNotDoneLoading = false;
            }
            );
        }

        #endregion

        /*
         Interfaz
         Nombre: mensajeFinPartida
         Comentario: Este método muestra por pantalla un mensaje de fin de partida.
         Cabecera: public static async void mensajeFinPartida(int idJugador)
         Entrada:
            -int idJugador
         Postcondiciones: El métoso muestra un mensaje por pantalla.
         */
        public static async void mensajeFinPartida(int idJugador)
        {
            ContentDialog confirmar = new ContentDialog();
            confirmar.Title = "Fin de la partida";
            confirmar.Content = "¡Ha ganado el jugador " + idJugador + "!";
            confirmar.PrimaryButtonText = "Aceptar";
            ContentDialogResult resultado = await confirmar.ShowAsync();
        }
    }
}
