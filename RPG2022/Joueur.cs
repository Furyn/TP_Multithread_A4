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
        public bool startToPlay = false;
        bool end = false;

        [ThreadStatic]
        public int timeEnd = 0;

        public Joueur(string name): base(name, Config.PV_JOUEUR)
        { 
        }

        public void JoueurThread(Object obj)
        {
            Table table = obj as Table;
            
            while (!end)
            {
                lock (this)
                {
                    if (!addedToGame && !inQueue && !startGame)
                    {
                        lock (table)
                        {
                            timeEnd = rand.Next(5, 15);
                            if (table.PlaceDisponible())
                            {
                                if (table.PartieEnAttente())
                                {
                                    table.AjouterJoueur(this);
                                    addedToGame = true;
                                    Console.WriteLine("Add joueur : " + this.Name + " / wait time = " + timeEnd);
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

                if (startGame && !startToPlay && !end)
                {
                    startToPlay = true;
                    Thread attaqueThread = new Thread(new ThreadStart(AttaqueThread));
                    attaqueThread.Start();

                    Thread.Sleep(timeEnd * 1000);
                    this.startToPlay = false;
                    lock (table)
                    {
                        table.DepartJoueur(this);
                    }
                    end = true;
                }

                if (Pv > 0)
                {
                    startToPlay = false;
                    end = true;
                }
                
            }
            Console.WriteLine("End thread joueur : " + this.Name);
        }

        public void AttaqueThread()
        {
            bool attaque = false;
            while (startToPlay)
            {
                if (attaque)
                {
                    if (Config.TEMPS_ATTAQUE > 0)
                        Thread.Sleep(Config.TEMPS_ATTAQUE);
                    attaque = false;
                }
                else
                {
                    lock (table)
                    {
                        if (!end && table.MonstreIsAlive() && !attaque && table.PartieEnCours() && Pv > 0)
                        {
                            attaque = true;
                            table.JoueurAttaque(this);
                        }
                    }
                }
            }
        }
    }
}
