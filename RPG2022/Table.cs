using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RPG2022
{

    class Table
    {
        private enum StatusPartie
        {
            Attente,
            EnCours,
            Finalisation,
            Terminee
        };

        StatusPartie status = StatusPartie.Attente;

        Monstre monstreCourant;
        List<Joueur> joueurs = new List<Joueur>();
        MJ mj;

        ConcurrentQueue<Joueur> queue_waiting = new ConcurrentQueue<Joueur>();

        List<string> journal = new List<string>();

        public int GetCountPlayer()
        {
            return joueurs.Count;
        }

        public void AddPlayerInQueue(Joueur joueur)
        {
            queue_waiting.Enqueue(joueur);
        }

        public bool AllPlayerNotStarted()
        {
            bool notStarted = true;
            foreach (Joueur j in joueurs)
            {
                if (j.startGame)
                {
                    notStarted = false;
                }
            }

            return notStarted;
        }

        public void StartAllPlayerToPlay()
        {
            foreach (Joueur j in joueurs)
            {
                j.startGame = true;
            }
        }

        public bool QueueIsEmpty()
        {
            return queue_waiting.IsEmpty;
        }

        public Joueur GetFirstPlayerInQueue()
        {
            queue_waiting.TryDequeue(out Joueur j);

            return j;
        }

        public void Demarrer()
        {
            if (status != StatusPartie.Attente)
                throw new Exception("Partie déjà commencée");

            Console.WriteLine("");
            Console.WriteLine("START");
            status = StatusPartie.EnCours;
        }
        public void Finaliser()
        {
            if (status != StatusPartie.EnCours)
                throw new Exception("Partie pas en cours");

            status = StatusPartie.Finalisation;
        }

        public void Terminer()
        {
            if (!PartieEnCours())
                throw new Exception("Partie pas en cours");

            Console.WriteLine("END");
            status = StatusPartie.Terminee;
        }

        public void ResetPartie()
        {
            Console.WriteLine("RESET GAME");
            status = StatusPartie.Attente;
            StartAllPlayerToPlay();
        }

        public bool PartieEnCours()
        {
            return status == StatusPartie.EnCours;
        }

        public bool PartieEnAttente()
        {
            return status == StatusPartie.Attente;
        }

        public bool PartieTerminer()
        {
            return status == StatusPartie.Terminee;
        }

        public bool PartieFinaliser()
        {
            return status == StatusPartie.Finalisation;
        }

        public bool PlaceDisponible()
        {
            return joueurs.Count < Config.MAX_JOUEURS;
        }

        public void PlacerMonstre(Monstre monstre)
        {
            if (status != StatusPartie.EnCours)
                throw new Exception("Ce n'est pas le bon moment pour placer un monstre.");

            if (monstreCourant != null)
            {
                if (monstreCourant.Pv > 0)
                    throw new Exception("Un monstre est déjà présent et vivant");
            }
            monstreCourant = monstre;
            monstre.RejoindreTable(this);
            PublierDansJournal(monstre.Name, "entre en scène");
        }

        public void ChangerMJ(MJ mj)
        {
            if(PartieEnCours())
                throw new Exception("Il y a déjà un MJ en action.");

            this.mj = mj;
        }

        public void AjouterJoueur(Joueur joueur)
        {
            if (status != StatusPartie.Attente && status != StatusPartie.EnCours && status != StatusPartie.Finalisation)
                throw new Exception("La partie n'est pas en cours.");

            if(joueurs.Count >= Config.MAX_JOUEURS)
                throw new Exception("La partie est pleine.");
            joueurs.Add(joueur);
            joueur.RejoindreTable(this);
            PublierDansJournal(joueur.Name, "rejoint la partie");
        }

        public void DepartJoueur(Joueur joueur)
        {
            if(!joueurs.Contains(joueur))
                throw new Exception("Le joueur n'était pas sur cette table.");
            joueurs.Remove(joueur);
            if (joueurs.Count == 0 && status != StatusPartie.Terminee)
                Terminer();
        }

        public void PublierDansJournal(string auteur, string message)
        {
            if(Config.ACTIVER_JOURNAL)
            { 
                if (journal.Count > 100)
                    journal.Clear();
                journal.Add("[" + auteur + "] " + message);
            }
        }

        public string ConsulterJournal()
        {
            if (!Config.ACTIVER_JOURNAL || journal.Count == 0)
                return "";
            return journal[journal.Count - 1];
        }
    }
}
