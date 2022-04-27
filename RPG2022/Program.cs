using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Concurrent;

namespace RPG2022
{

    class Program
    {
        static Table table;
        static int nb_joueurs = 0;

        static void Main(string[] args)
        {
            table = new Table();
            MJ mj = new MJ(table);

            Thread childThreadMJ = new Thread(mj.MJThread);
            childThreadMJ.Start();

            Thread switchMJ = new Thread(SwitchMJ);
            switchMJ.Start();

            Thread vagues = new Thread(VagueJoueurs);
            vagues.Start();

            Console.ReadKey();
        }

        static void VagueJoueurs()
        {
            for (int i = 0; i < Config.QUEUE_INITIALE; i++)
            {
                nb_joueurs++;
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(new Joueur("J" + (nb_joueurs)).JoueurThread);
                Thread childThread = new Thread(threadStart);
                childThread.Start(table);
            }

            Thread.Sleep(30 * 1000);

            for (int i = 0; i < Config.QUEUE_INITIALE; i++)
            {
                nb_joueurs++;
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(new Joueur("J" + (nb_joueurs)).JoueurThread);
                Thread childThread = new Thread(threadStart);
                childThread.Start(table);
            }

            Thread.Sleep(10 * 1000);

            for (int i = 0; i < Config.QUEUE_INITIALE; i++)
            {
                nb_joueurs++;
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(new Joueur("J" + (nb_joueurs)).JoueurThread);
                Thread childThread = new Thread(threadStart);
                childThread.Start(table);
            }
        }

        static void SwitchMJ()
        {
            bool end = false;
            while (!end)
            {
                if (!table.PartieFinaliser())
                {
                    Thread.Sleep(60 * 1000);
                    Console.WriteLine("Switch MJ NEXT GAME");
                    table.Finaliser();
                }
            }
        }

    }
}
