using System;
using System.Collections.Generic;

public class Program
{
    public const int HILOS = 6;
    public const int ITERACIONES = 3;                // CREACIÓN POBLACIÓN INICIAL
    public const int PROBMUT = 10;                   // PROBABILIDAD DE MUTACIÓN
    public const double MORTALIDAD_INFANTIL = 0.02;  // PEORES HIJOS ELIMINADOS EN CADA GENERACIÓN
    public const int SELECCIONADOS = 6;              // TAMAÑO TORNEO DE SELECCIÓN
    public const int MAXSINMEJORA = 35;              // GENERACIONES SIN MEJORA
    public const int TAMANOPOBLACION = 1000;
    public const int ITER = 30;                      // REINICIOS DE POBLACIÓN
    public const int CARACTERISTICAS_AFINIDAD = 10;
    public const int UMBRAL_MAX_AFINIDAD = 8;
    public const int UMBRAL_MEDIA_AFINIDAD = 5;

    public static void Main(string[] args)
    {
        Mapa m = new Mapa("../data/att532.dat");
        m.ComputarCandidatos(25);

        int sumaEstadistica = 0;
        int optimoCount = 0;

        var inicio = DateTime.Now;
        int it = 0;
        Tour mejorEver = null;
        while (it < ITER)
        {
            Poblacion p = new Poblacion(TAMANOPOBLACION, m);
            int mejor = p.mejorValor;
            int cont = 0, i = 0;
            Console.Write($"{i++}\tBEST: {(mejorEver != null ? mejorEver.costo.ToString() : mejor.ToString())}, ");
            p.Reporte();
            while (cont < MAXSINMEJORA)
            {
                if (i % 10 == 0)
                {
                    int valMejor = (mejorEver == null || mejorEver.costo > mejor) ? mejor : mejorEver.costo;
                    Console.Write($"{i++}\tBEST: {valMejor}, ");
                    p.Reporte();
                }
                p.NuevaGeneracion(m);
                if (p.mejorValor < mejor)
                {
                    mejor = p.mejorValor;
                    int valMejor = (mejorEver == null || mejorEver.costo > mejor) ? mejor : mejorEver.costo;
                    if (mejor < mejorEver.costo)
                        Console.Write($"{i}\033[31m\tBEST: {valMejor}, ");
                    else
                        Console.Write($"{i++}\tBEST: {valMejor}, ");
                    p.Reporte();
                    Console.Write("\033[0m");
                    cont = 0;
                }
                else
                {
                    cont++;
                }
                i++;
            }

            if (mejorEver == null || p.GetMejorSolucion().costo < mejorEver.costo)
                mejorEver = p.GetMejorSolucion();
            sumaEstadistica += p.mejorValor;
            if (p.mejorValor == 86756)
                optimoCount++;
            it++;
        }
        mejorEver.Show();
        // Console.WriteLine((mejorEver.EsConexa() ? "CONEXA" : "NO CONEXA"));
        Console.WriteLine($"COSTO: {mejorEver.Evaluar(m)}");
        Console.WriteLine($"PROMEDIO: {sumaEstadistica / ITER}");
        Console.WriteLine($"OPTIMOS: {optimoCount}");

        var fin = DateTime.Now;
        var duracion = fin - inicio;
        Console.WriteLine($"Duración Total: {duracion.TotalMilliseconds}");

        // Tour t = new Tour(new List<int>{0, 21, 30, 17, 2, 16, 20, 41, 6, 1, 29, 22, 19, 49, 28, 15, 45, 43, 33, 34, 35, 38, 39, 36, 37, 47, 23, 4, 14, 5, 3, 24, 11, 27, 26, 25, 46, 12, 13, 51, 10, 50, 32, 42, 9, 8, 7, 40, 18, 44, 31, 48, 0});
    }
}
