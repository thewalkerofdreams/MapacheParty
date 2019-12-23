
using RaccoonPartyServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaccoonPartyServer.Gestoras
{
    public class ClsGestoraTablero
    {
        /// <summary>
        /// Comentario: Este método nos permite generar un tablero aleatorio para
        /// la aplicación MapacheParty. El tablero constará de 5x5 casillas.
        /// Tendrá los siguientes items:
        /// -2 setas
        /// -1 bowser
        /// -4 bombas
        /// -18 cajas sorpresa
        /// </summary>
        public static void tableroAleatorio()
        {
            List<ClsCasilla> listadoCasillas = new List<ClsCasilla>();
            Random random = new Random();
            int posicionCasilla = 0;
            bool bowserInsertado = false;

            for (int i = 0; i < 25; i++)//Rellenamos todo el tablero con cajas sorpresa
            {
                switch (random.Next(1, 3))
                {
                    case 1://Caja sorpresa con monedas
                        listadoCasillas.Add(new ClsCasilla(new ClsItem(1, random.Next(1, 5)), true, false, "ms-appx:///Assets/caja_sorpresa_casilla.jpg"));
                        break;
                    default://Caja sorpresa con fantasma
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
                    listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem != 4 &&
                    (listadoCasillas.ElementAt(posicionCasilla).Item.TipoItem == 1 &&
                    listadoCasillas.ElementAt(posicionCasilla).Item.Monedas != 20))//Si la posición no contiene una bomba, una seta, una casilla bowser u otra estrella
                {
                    listadoCasillas[posicionCasilla] = new ClsCasilla(new ClsItem(1, 20), true, false, "ms-appx:///Assets/caja_sorpresa_casilla.jpg");
                }
                else
                {
                    i--;
                }
            }

            ClsDatosJuego.tablero = listadoCasillas;
        }
    }
}