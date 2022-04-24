using System;
using System.Collections.Concurrent;

namespace RPG2022
{
    class MJ
    {
        public Table table;

        public MJ(Table table)
        {
            this.table = table;
            table.ChangerMJ(this);
        }

        public void MJThread(object obj)
        {
            bool end = false;
            while (!end)
            {
                lock (table)
                {
                    if (table.GetCountPlayer() == Config.MAX_JOUEURS && !table.PartieEnCours())
                    {
                        table.Demarrer();
                    }

                    if (table.PartieTerminer() )
                    {
                        table.ResetPartie();
                    }
                }
            }
        }
    }
}
