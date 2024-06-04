using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Globals;

public class Poblacion
{
    private Utils util = new Utils();
    public List<Tour> soluciones;
    public int mejorValor;
    public int promedio;
    public int peorValor;
    T[] InitializeArray<T>(int length) where T : new()
    {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
            array[i] = new T();
        }

        return array;
    }

    public Poblacion(int n, Mapa m)
    {
        int MAXITERSINMEJORA=m.data.Count;
        soluciones = new List<Tour>(n);
        var inicio = DateTime.Now;
        var threads = new List<Thread>();
        int p = n / THREADS;
        int q = n % THREADS;
        for (int i = 0; i < THREADS; i++)
        {
            int k = (i < q) ? p + 1 : p;
            var t = new Thread(() =>
            {
                var seeder = new Random();
                var seed = seeder.Next();
                var engine = new Random(seed);

                for (int j = 0; j < k; j++)
                {
                    Tour mejor = null;

                    Tour tour = new Tour(m.data.Count, engine);
                    tour.costo = tour.Evaluar(m);

                    // tour.Show();
                    for (int i = 0; i < ITER; i++)
                    {
                        int nSinMejora = 0;
                        while (nSinMejora < MAXITERSINMEJORA)
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
            threads.Add(t);
        }

        foreach (Thread thread in threads)
        {
            thread.Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        var fin = DateTime.Now;
        var duracion = fin - inicio;

        soluciones.Sort((x, y) => x.costo.CompareTo(y.costo));
        Calcular();
        Console.WriteLine("NUEVA GENERACIÓN");
        //Console.WriteLine($"individuos generados: {ind}");
    }

    public void Reporte()
    {
        Console.WriteLine(string.Format("POB MEJOR: {0}, PROMEDIO: {1}, PEOR: {2}", mejorValor, promedio, peorValor));
    }

    private (Tour, Tour) ElegirPadres(Random generador)
    {
        var dist = new Random();
        var competidores = new HashSet<Tour>();
        for (int i = 0; i < TORNEO_DE_PODER; i++)
        {
            var elegido = soluciones[dist.Next(soluciones.Count)];
            competidores.Add(elegido);
        }
        var p = competidores.First();
        competidores.Remove(p);
        var m = competidores.First();
        return (p, m);
    }

    private void Swap(ref Tour a, ref Tour b)
    {
        Tour temp = a;
        a = b;
        b = temp;
    }

    public void nuevaGeneracion(Mapa mapa, Tour mejorTour){
        soluciones.Insert(0, mejorTour);
        soluciones.RemoveAt(soluciones.Count -1);
        
        this.nuevaGeneracion(mapa);
    }

    public void nuevaGeneracion(Mapa mapa)
    {
        int n = soluciones.Count;
        List<Thread> threads = new List<Thread>();
        int nHijos = n + (int)(MORTALIDAD_INFANTIL * n);
        int nPadres = n / 200;
        int ind = 0;
        Mutex mut = new Mutex();


        Tour[] nuevaGeneracion = InitializeArray<Tour>(nHijos);

        for (int i = 0; i < THREADS; i++)
        {
            Thread t = new Thread(() =>
            {
                Random seeder = new Random();
                int seed = seeder.Next();
                Random engine = new Random(seed);
                bool breakFlag = false;
                while (!breakFlag)
                {
                    var padres = ElegirPadres(engine);
                    int compatibilidad = padres.Item1.GetCompatibilidad(engine, padres.Item2); //EN BASE A LA COMPATIBILIDAD DE LOS PADRES GENERAMOS 1, 2, o 3 Hijos Segun el umbral

                    int nH = 1;
                    if (compatibilidad > UMBRAL_MAX_AFINIDAD)
                        nH = 3;
                    else if (compatibilidad > UMBRAL_MEDIA_AFINIDAD)
                        nH = 2;

                    for (int h = 0; h < nH; h++)
                    {
                        mut.WaitOne();
                        int indice = ind++;
                        int mejorV = mejorValor;
                        mut.ReleaseMutex();

                        if (indice >= nHijos)
                        {
                            breakFlag = true;
                            break;
                        }
                        
                        if (engine.Next(0, 100) < 50) // 50%
                            Swap(ref padres.Item1, ref padres.Item2);

                            //Constructor PMX
                        Tour h1 = new Tour(mapa, padres.Item1, padres.Item2, engine, soluciones[0]);
                        h1.costo = h1.Evaluar(mapa);
                        if (h1.costo >= mejorV)
                        {
                            mut.WaitOne();
                            mejorValor = h1.costo;
                            mut.ReleaseMutex();
                        }
                        else if (engine.Next(100) < PROBMUT)
                        {
                            h1.costo -= h1.Mutar(engine, mapa);
                        }

                        mut.WaitOne();
                        nuevaGeneracion[indice] = h1;
                          mut.ReleaseMutex();
                    }
                }
            });
            threads.Add(t);
        }

        foreach (Thread t in threads)
        {
            t.Start();
        }

        foreach (Thread t in threads)
        {
            t.Join();
        }

soluciones.Sort((x, y) => x.costo.CompareTo(y.costo));
        // los nuevos pisan los viejos mas malos
        soluciones.AddRange(nuevaGeneracion.Take(nHijos).Select(t => t));

        // Reordena y luego corta los sobrantes.
        soluciones.Sort((x, y) => x.costo.CompareTo(y.costo));
        soluciones.RemoveRange(n, soluciones.Count - n);
        Calcular();
    }

    private void Calcular()
    {
        int suma = 0;
        foreach (var sol in soluciones)
        {
            suma += sol.costo;
        }
        mejorValor = soluciones[0].costo;
        peorValor = soluciones[soluciones.Count - 1].costo;
        promedio = (int)(suma / soluciones.Count);
    }

    public Tour GetMejorSolucion()
    {
        return soluciones[0];
    }
}