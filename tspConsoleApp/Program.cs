using System;
using System.Diagnostics;
using System.Globalization;
internal class Program
{

   
    public static int cantidadCiclos = 10;
    public static int cantidadCandidatos = 25;
    public static int cantidadPoblacion = 100;
    public static HashSet<SortedPoblacion> poblacion = new HashSet<SortedPoblacion>();
    static void Main(string[] args)
    {
        string millisecondFormat = $"{NumberFormatInfo.CurrentInfo.NumberDecimalSeparator}fff";
        Stopwatch sw = new Stopwatch();
        sw.Start(); //iniciamos tiempo

        Random seeder = new Random();
        int seed = seeder.Next();
        Random engine = new Random(seed);

        Mapa mapa = new Mapa("../data/att532.dat"); //att532.dat
        mapa.ComputarCandidatos(cantidadCandidatos);

        Tour mejor = null;
        int individuo = 0;
        while(individuo < cantidadPoblacion){
            for (int i = 0; i < cantidadCiclos; i++){

                Tour tour = new Tour(mapa.data.Count, engine);
                tour.costo = tour.Evaluar(mapa);

                // tour.Show();

                int nSinMejora = 0;
                while (nSinMejora < 8 * mapa.data.Count)
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
                    }
                }
                else
                {
                    Console.WriteLine("Error en el tour");
                }
            }


            poblacion.Add(new SortedPoblacion(mejor.costo, mejor));
            individuo++;

            Console.WriteLine(mejor.costo.ToString());    

        }
        
        mejor.Show();
        sw.Stop();


        Console.WriteLine(sw.Elapsed);
    }
}
