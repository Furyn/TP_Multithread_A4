using System;
using System.Threading;

namespace RPG2022
{
    class Joueur : Personnage
    {
        static Random rand = new Random();
        public bool addedToGame = false;
        public bool inQueue = false;

        public Joueur(string name): base(name, Config.PV_JOUEUR)
        { 
        }

        public void JoueurThread(Object obj)
        {
            Table table = obj as Table;
            bool end = false;
            int time = 0;

            while (!end)
            {
                if (!addedToGame || inQueue)
                {
                    lock (table)
                    {
                        if (!inQueue && table.PartieEnAttente() && table.PlaceDisponible() && !table.PartieEnCours())
                        {
                            time = rand.Next(5, 15);

                            if (!table.PartieEnCours())
                            {
                                table.AjouterJoueur(this);
                                addedToGame = true;
                                inQueue = false;
                                Console.WriteLine("Add joueur : " + this.Name + " / wait time = " + time);
                            }

                        }
                        else if (!inQueue && table.PartieEnCours() && table.PlaceDisponible() && table.PartieEnCours())
                        {
                            table.AddPlayerInQueue(this);
                            inQueue = true;
                            Console.WriteLine("Add joueur in queue : " + this.Name);
                        }

                    }
                }

                if (addedToGame && !inQueue)
                {
                    Thread.Sleep(time * 1000);
                    lock (table)
                    {
                        table.DepartJoueur(this);
                    }
                    Console.WriteLine("Kick joueur : " + this.Name);

                    end = true;
                }
            }

        }
    }
}
