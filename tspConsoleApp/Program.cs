using System;
using System.Diagnostics;
internal class Program
{
    public static int cantidadCiclos = 1000;
    public static int cantidadCandidatos = 25;
    static void Main(string[] args)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start(); //iniciamos tiempo

        Random seeder = new Random();
        int seed = seeder.Next();
        Random engine = new Random(seed);

        Mapa mapa = new Mapa("../data/att532.dat"); //att532.dat
        mapa.ComputarCandidatos(cantidadCandidatos);

        Tour mejor = null;
        for (int i = 0; i < cantidadCiclos; i++)
        {

            Tour tour = new Tour(mapa.data.Count, engine);
            tour.costo = tour.Evaluar(mapa);

            // tour.Show();

            int nSinMejora = 0;
            while (nSinMejora < cantidadCiclos)
            {
                int ganancia = tour.ThreeOpt(engine, mapa, mejor);
                if (ganancia > 0)
                {
                    nSinMejora = 0;
                    tour.costo -= ganancia;
                }
                else
                {
                    nSinMejora++;
                }
            }

            if (tour.costo == tour.Evaluar(mapa) && tour.IsConexa())
            {
                if (mejor == null || tour.costo < mejor.costo)
                {
                    mejor = tour;
                    Console.WriteLine("Mejor tour: " + mejor.costo);
                }
            }
            else
            {
                Console.WriteLine("Error en el tour");
            }
        }
        mejor.Show();
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }
}
