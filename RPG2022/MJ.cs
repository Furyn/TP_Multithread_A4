using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RPG2022
{
    class MJ
    {
        public Table table;
        private bool wantToStart = false;
        private int nbJoueur = 0;
        bool end = false;

        public MJ(Table table)
        {
            this.table = table;
            table.ChangerMJ(this);
        }

        public void MJThread(object obj)
        {
            Thread switchMJ = new Thread(SwitchMJThread);
            switchMJ.Start();

            while (!end)
            {
                lock (table)
                {
                    if (table.GetCountPlayer() == Config.MAX_JOUEURS && (!table.PartieEnCours() || (table.QueueIsEmpty() && table.GetCountPlayer() > 0)))
                    {
                        wantToStart = true;
                        nbJoueur = table.GetCountPlayer();
                    }

                    if ( (table.PartieEnCours() || table.PartieFinaliser())  && table.PlaceDisponible())
                    {
                        Joueur j = table.GetFirstPlayerInQueue();
                        if (j != null)
                        {
                            j.addedToGame = true;
                            j.inQueue = false;
                            table.AjouterJoueur(j);
                            Console.WriteLine("Add joueur : " + j.Name + " / wait time = " + j.timeEnd);
                        }
                    }

                    if ( (table.PartieTerminer() && !table.QueueIsEmpty()) || ( !table.PartieEnAttente() && !table.PartieFinaliser() && table.AllPlayerNotStarted() && (!table.PlaceDisponible() || (table.QueueIsEmpty() && table.GetCountPlayer() > 0)) ) )
                    {
                        table.ResetPartie();
                        table.Demarrer();
                        wantToStart = false;
                    }else if (table.PartieTerminer())
                    {
                        table.ResetPartie();
                    }
                }

                if (wantToStart && !table.PartieFinaliser())
                {
                    wantToStart = false;
                    Thread.Sleep(1000);
                    lock (table)
                    {
                        if (table.PartieEnAttente() && nbJoueur == table.GetCountPlayer())
                        {
                            table.Demarrer();
                            table.StartAllPlayerToPlay();
                        }
                    }
                }

                lock (table)
                {
                    if (table.PartieFinaliser() && table.AllPlayerNotStarted())
                    {
                        SwitchMJ();
                    }
                }
            }

            Console.WriteLine("MJ ENDED");
        }

        public void SwitchMJ()
        {
            Console.WriteLine("SWITCH MJ");
            MJ mj = new MJ(table);
            Thread childThreadMJ = new Thread(mj.MJThread);
            childThreadMJ.Start();
            table.ResetPartie();
            if (!table.QueueIsEmpty())
            {
                table.Demarrer();
            }
            end = true;
        }

        public void SwitchMJThread()
        {
            while (!end)
            {
                if (!table.PartieFinaliser())
                {
                    Thread.Sleep(60 * 1000);
                    if (!table.PartieEnCours())
                    {
                        SwitchMJ();
                    }
                    else
                    {
                        table.Finaliser();
                    }
                }
            }
        }
    }
}
