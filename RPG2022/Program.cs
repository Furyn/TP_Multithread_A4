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
        static void Main(string[] args)
        {

            Table table = new Table();
            MJ mj = new MJ(table);

            Thread childThreadMJ = new Thread(mj.MJThread);
            childThreadMJ.Start();

            for (int i = 0; i < Config.QUEUE_INITIALE; i++)
            {
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(new Joueur("J" + (i + 1)).JoueurThread);
                Thread childThread = new Thread(threadStart);
                childThread.Start( table );
            }

            Console.ReadKey();
        }
    }
}
