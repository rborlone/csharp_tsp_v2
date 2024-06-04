using System;
using System.Collections.Generic;
using static Globals;

public class Program
{
    public static void Main(string[] args)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        Mapa m = new Mapa("../data/att532.dat");
        m.ComputarCandidatos(25);

        int sumaEstadistica = 0;

        var inicio = DateTime.Now;
        int it = 0;
        Tour mejorTour = null;
        while (it < ITERACIONES)
        {
            Poblacion p = new Poblacion(TAMANOPOBLACION, m);
            int mejor = p.mejorValor;
            int cont = 0, i = 0;

            Console.Write(string.Format("{0} MEJOR: {1}, ", i++, (mejorTour != null ? mejorTour.costo.ToString() : mejor.ToString())));

            p.Reporte();

            while (cont < MAXSINMEJORA)
            {

                if (i % IMPRIMIR_MEJOR == 0)
                {
                    int valorMejor = (mejorTour == null || mejorTour.costo > mejor) ? mejor : mejorTour.costo;

                    Console.Write(string.Format("{0} MEJOR: {1}, ", i++, valorMejor));
                    p.Reporte();
                }

                p.nuevaGeneracion(m);

                //si tenemos mejora imprimimos
                if (p.mejorValor < mejor)
                {
                    mejor = p.mejorValor;
                    int valorMejor = (mejorTour == null || mejorTour.costo > mejor) ? mejor : mejorTour.costo;
                    if (mejorTour != null && mejor < mejorTour.costo)
                        Console.Write(string.Format("{0} MEJOR: {1}, ", i, valorMejor));
                    else
                        Console.Write(string.Format("{0} MEJOR: {1}, ", i, valorMejor));
                    p.Reporte();
                    cont = 0;
                }
                else
                {
                    cont++;
                }
                i++;
            }

            if (mejorTour == null || p.GetMejorSolucion().costo < mejorTour.costo)
                mejorTour = p.GetMejorSolucion();
            sumaEstadistica += p.mejorValor;
            if (p.mejorValor == 86756)
            {
                Console.WriteLine("LLEGAMOS");

                mejorTour.Show();
                Console.WriteLine(string.Format("COSTO FINAL: {0}", mejorTour.Evaluar(m)));
                Console.WriteLine(string.Format("PROMEDIO FINAL: {0}", sumaEstadistica / ITERACIONES));

                watch.Stop();

                Console.WriteLine(string.Format("TIEMPO TOTAL: {0} ms", watch.ElapsedMilliseconds));

                Environment.Exit(0);
            }

            it++;
        }
        mejorTour.Show();

        Console.WriteLine(string.Format("COSTO FINAL: {0}", mejorTour.Evaluar(m)));
        Console.WriteLine(string.Format("PROMEDIO FINAL: {0}", (int)sumaEstadistica / ITERACIONES));

        watch.Stop();

        Console.WriteLine(string.Format("TIEMPO TOTAL: {0} ms", watch.ElapsedMilliseconds));
    }
}
