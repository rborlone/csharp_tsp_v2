using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Poblacion
{
    private const int HILOS = 6;
    private const int ITERACIONES = 3;
    private const int PROBMUT = 10;
    private const double MORTALIDAD_INFANTIL = 0.02;
    private const int SELECCIONADOS = 6;
    private const int MAXSINMEJORA = 35;
    private const int TAMANOPOBLACION = 1000;
    private const int ITER = 30;
    private const int CARACTERISTICAS_AFINIDAD = 10;
    private const int UMBRAL_MAX_AFINIDAD = 8;
    private const int UMBRAL_MEDIA_AFINIDAD = 5;

    public List<Tour> soluciones;
    public int mejorValor;
    public double promedio;
    public int peorValor;

    public Poblacion(int n, Mapa m)
    {
        soluciones = new List<Tour>(n);
        var inicio = DateTime.Now;

        int ind = 0;
        Mutex mut = new Mutex();
        var hilos = new List<Thread>();
        int p = n / HILOS;
        int q = n % HILOS;
        for (int i = 0; i < HILOS; i++)
        {
            int k = (i < q) ? p + 1 : p;
            var t = new Thread(() =>
            {
                var seeder = new Random();
                var seed = seeder.Next();
                var engine = new Random(seed);

                for (int j = 0; j < k; j++)
                {
                    int indice;

                    mut.WaitOne();
                    indice = ind++;
                    mut.ReleaseMutex();

                    Tour mejor = null;

                    Tour tour = new Tour(m.data.Count, engine);
                    tour.costo = tour.Evaluar(m);

                    // tour.Show();
                    for (int i = 0; i < MAXSINMEJORA; i++)
                    {
                        int nSinMejora = 0;
                        while (nSinMejora < 1 * m.data.Count)
                        {
                            int ganancia = tour.ThreeOpt(engine, m, mejor);
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
                        if (mejor == null || tour.costo < mejor.costo)
                        {
                            mejor = tour;
                        }
                    }

                    soluciones.Add(mejor);
                }
            });
            hilos.Add(t);
        }

        foreach (Thread thread in hilos){
            thread.Start();
        }

        foreach (var hilo in hilos)
        {
            hilo.Join();
        }
        
        Estadisticas();

        var fin = DateTime.Now;
        var duracion = fin - inicio;
        Console.WriteLine($"Gen0 duracion: {duracion.TotalMilliseconds}");
        Console.WriteLine($"individuos generados: {ind}");
    }

    public void Reporte()
    {
        Console.WriteLine($"POB mejor: {mejorValor}, promedio: {promedio}, peor: {peorValor}");
    }

    private (Tour, Tour) ElegirPadres(Random generador)
    {
        var dist = new Random();
        var seleccionados = new HashSet<Tour>();
        for (int i = 0; i < SELECCIONADOS; i++)
        {
            var elegido = soluciones[dist.Next(soluciones.Count)];
            seleccionados.Add(elegido);
        }
        var p = seleccionados.First();
        seleccionados.Remove(p);
        var m = seleccionados.First();
        return (p, m);
    }

    public void NuevaGeneracion(Mapa mapa)
    {
        double suma = 0.0;
        int n = soluciones.Count;
        Mutex mut = new Mutex();
        var hilos = new List<Thread>();
        int nHijos = n + (int)(MORTALIDAD_INFANTIL * n);
        int nPadres = n / 200;
        int ind = 0;
        var nuevaGeneracion = new List<Tour?>(nHijos);
        
               var seeder = new Random();
                var seed = seeder.Next();
                var engine = new Random(seed);
                var probMut = new Random();

                    var padres = ElegirPadres(engine);
                    int comp = padres.Item1.GetCompatibilidad(engine, padres.Item2);
                    int nH = 1;
                    if (comp > UMBRAL_MAX_AFINIDAD)
                        nH = 3;
                    else if (comp > UMBRAL_MEDIA_AFINIDAD)
                        nH = 2;

                    for (int h = 0; h < nH; h++)
                    {
                        int indice;
                        
                       mut.WaitOne();
                            indice = ind++;
                     mut.ReleaseMutex();
                        if (indice >= nHijos)
                        {
                            return;
                        }
                        if (probMut.Next(100) < 50)
                        {
                            Tour temp = padres.Item1;
                            padres.Item1 = padres.Item2;
                            padres.Item2 = temp;
                        }
                        var hijo = new Tour(mapa, padres.Item1, padres.Item2, engine, soluciones[0]);
                        hijo.costo = hijo.Evaluar(mapa);
                        if (hijo.costo < mejorValor)
                        {

                        mut.WaitOne();
                        mejorValor = hijo.costo;
                        mut.ReleaseMutex();
                        
                        }
                        else if (probMut.NextDouble() < PROBMUT)
                        {
                            hijo.costo -= hijo.Mutar(engine, mapa);
                        }
                        lock (nuevaGeneracion)
                        {
                            nuevaGeneracion[indice] = hijo;
                        }
                    }
          
         
        soluciones = new List<Tour?>(nHijos + nPadres);
        int j = nPadres;
        for (int i = 0; i < nHijos; i++)
        {
            soluciones.Add(nuevaGeneracion[i]);
        }
        soluciones.Sort();
        soluciones = soluciones.Take(n).ToList();
        Estadisticas();
    }


    private void Estadisticas()
    {
        double suma = 0.0;
        foreach (var sol in soluciones)
        {
            suma += sol.costo;
        }
        mejorValor = soluciones[0].costo;
        peorValor = soluciones[soluciones.Count - 1].costo;
        promedio = suma / soluciones.Count;
    }

    public Tour GetMejorSolucion()
{
    return soluciones[0];
}

    // public Tour GetMejorSolucion()
    // {
    //     return soluciones[0].costo;
    // }

}