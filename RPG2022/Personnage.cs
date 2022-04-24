using System;
using System.Threading;

namespace RPG2022
{
    class Personnage
    {
        protected Table table;

        public int Pv { get;  private set; }
        public string Name { get; private set; }

        protected Personnage(string name, int pv)
        {
            Name = name;
            Pv = pv;
        }


        public void RejoindreTable(Table table)
        {
            this.table = table;
        }

        public bool Attaquer(Personnage attaquant)
        {
            if(!table.PartieEnCours())
                throw new Exception("La partie n'est pas en cours.");

            if (Pv <= 0)
            {
                throw new Exception("Personnage sans PV attaqué : " + Name);
            }
            if(attaquant.Pv <= 0)
            {
                throw new Exception("Attaqué par un personnage sans PV : " + attaquant.Name);
            }
            --Pv;
            table.PublierDansJournal(attaquant.Name, " Attaque " + Name + " (" + Pv +" pv)" );
            if (Config.TEMPS_ATTAQUE > 0)
                Thread.Sleep(Config.TEMPS_ATTAQUE);
            return Pv <= 0;
        }

    }
}
