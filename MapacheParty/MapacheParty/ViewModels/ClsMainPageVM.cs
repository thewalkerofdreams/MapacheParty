using MapacheParty.Utilidades;
using MapacheParty_Entidades;
using MapacheParty_Entities;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace MapacheParty.ViewModels
{
    public class ClsMainPageVM : INotifyPropertyChanged
    {
        private List<ClsCasilla> _tablero;
        private ClsCasilla _casillaSeleccionada;
        private int _turnoJugador;
        private int _estrellasEncontradas;
        private int _monedasRival;
        private int _jugadorGanador;
        private String _mensajeVictoria;
        private HubConnection conn;
        private IHubProxy proxy;
        private bool _isNotDoneLoading;
        private ClsJugador _jugador;


        #region Constructores
        public ClsMainPageVM()
        {
            SignalR();
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
                    //ejecutarCasilla();
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
            conn = new HubConnection("https://mapachepartyservidor.azurewebsites.net");//Instanciamos la conexión
            proxy = conn.CreateHubProxy("MapachePartyServidor");
            conn.Start();

            proxy.On<int>("endGame", endGame);
            proxy.On<List<ClsCasilla>>("cargarTablero", cargarTablero);
            proxy.On<int>("getRivalCoins", getRivalCoins);
            proxy.On<int>("updatePersonalCoins", updatePersonalCoins);
            proxy.On<int>("seleccionarCasilla", seleccionarCasilla);
            proxy.On("onConnectedIsDone", onConnectedIsDone);
            proxy.On<int>("cambiarTurno", cambiarTurno);
            proxy.On<int>("obtenerIdJugador", obtenerIdJugador)
        }

        public async void obtenerIdJugador(int idjugador)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                _jugador.Id = idjugador;
            }
            );
        }

        public async void cambiarTurno(int idJugadorJugar)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                TurnoJugador = idJugadorJugar;
                NotifyPropertyChanged();
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

        public async void seleccionarCasilla(int posicionCasilla)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Tablero[posicionCasilla].Seleccionada = true;
                Tablero[posicionCasilla].Oculta = false;
                playSelectedBoxSound();
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

        public async void cargarTablero(List<ClsCasilla> tablero)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Tablero = tablero;
            }
            );
        }

        public async void endGame(int idjugadorGanador)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                JugadorGanador = idjugadorGanador;
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
    }
}
