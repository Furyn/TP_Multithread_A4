using System;
using System.Threading;

namespace RPG2022
{
    class Joueur : Personnage
    {
        static Random rand = new Random();
        public bool addedToGame = false;
        public bool inQueue = false;
        public bool startGame = false;

        [ThreadStatic]
        public int time = 0;

        public Joueur(string name): base(name, Config.PV_JOUEUR)
        { 
        }

        public void JoueurThread(Object obj)
        {
            Table table = obj as Table;
            bool end = false;

            while (!end)
            {
                lock (this)
                {
                    if (!addedToGame && !inQueue && !startGame)
                    {
                        lock (table)
                        {
                            time = rand.Next(5, 15);
                            if (table.PlaceDisponible())
                            {
                                if (table.PartieEnAttente())
                                {
                                    table.AjouterJoueur(this);
                                    addedToGame = true;
                                    Console.WriteLine("Add joueur : " + this.Name + " / wait time = " + time);
                                }
                            }
                            else
                            {
                                table.AddPlayerInQueue(this);
                                inQueue = true;
                                Console.WriteLine("Add joueur in queue : " + this.Name);
                            }

                        }
                    }
                }

                if (startGame)
                {
                    Thread.Sleep(time * 1000);
                    lock (table)
                    {
                        table.DepartJoueur(this);
                    }

                    end = true;
                }
                
            }
            Console.WriteLine("End thread joueur : " + this.Name);
        }
    }
}
