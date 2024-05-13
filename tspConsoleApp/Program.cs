using System;
using System.Diagnostics;
using System.Globalization;
internal class Program
{

   
    public static int cantidadCiclos = 10;
    public static int cantidadCandidatos = 25;
    public static int cantidadPoblacion = 1000;
    public static PriorityQueue<Tour, int> poblacion = new PriorityQueue<Tour, int>();
    static int Main(string[] args)
    {
        string millisecondFormat = $"{NumberFormatInfo.CurrentInfo.NumberDecimalSeparator}fff";
        Stopwatch sw = new Stopwatch();
        sw.Start(); //iniciamos tiempo

     
        Random seeder = new Random();
        int seed = seeder.Next();
        Random engine = new Random(seed);

        Poblacion pob = new Poblacion();


        Mapa mapa = new Mapa("../data/att532.dat"); //att532.dat
        mapa.ComputarCandidatos(cantidadCandidatos);

        Tour mejor = null;
        int individuo = 0;
        while(individuo < cantidadPoblacion){

      

            // for (int i = 0; i < cantidadCiclos; i++){

      

            //     Tour tour = new Tour(mapa.data.Count, engine);
            //     tour.costo = tour.Evaluar(mapa);

                // tour.Show();

                // int nSinMejora = 0;
                // while (nSinMejora < 8 * mapa.data.Count)
                // {
                //     int ganancia = tour.ThreeOpt(engine, mapa, mejor);
                //     if (ganancia > 0)
                //     {
                //         nSinMejora = 0;
                //         tour.costo -= ganancia;
                //     }
                //     else
                //     {
                //         nSinMejora++;
                //     }
                // }
            
                // if (tour.costo == tour.Evaluar(mapa) && tour.IsConexa())
                // {
                //     if (mejor == null || tour.costo < mejor.costo)
                //     {
                //         mejor = tour;
                //         pob.listaPoblacion.Enqueue(new Tour(mejor), mejor.costo);
                //         individuo++;
                //     }
                // }
                // else
                // {
                //     Console.WriteLine("Error en el tour");
                // }
            // }

       

            

            var t = pob.CrearTour(mapa, cantidadCiclos);

            Console.WriteLine(t.costo.ToString());  
            pob.listaPoblacion.Enqueue(t, t.costo);
            individuo++;

        }

        var fathers = pob.elegirPadres(10);


        Console.WriteLine(sw.Elapsed);

        return 0;





//        mejor.Show();
//        sw.Stop();



    }
}
