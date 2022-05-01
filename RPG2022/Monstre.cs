using System;
using System.Threading;

namespace RPG2022
{
    class Monstre : Personnage
    {
        static int cptMonstres = 1;
        public Monstre(): base ("Monstre " + (cptMonstres++), Config.PV_MONSTRE)
        { }

        internal void MonstreThread()
        {
            bool attaque = false;
            while (Pv > 0)
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
                        if (table.PartieEnCours())
                        {
                            Joueur j = table.GetRandPlayer();
                            if (j != null && j.Pv > 0)
                            {
                                attaque = true;
                                j.Attaquer(this);
                            }
                        }
                    }
                }
            }
        }
    }
}
