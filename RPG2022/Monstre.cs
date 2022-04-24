namespace RPG2022
{
    class Monstre : Personnage
    {
        static int cptMonstres = 1;
        public Monstre(): base ("Monstre " + (cptMonstres++), Config.PV_MONSTRE)
        { }
    }
}
