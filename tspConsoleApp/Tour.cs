using System.Reflection;
using System.Runtime.InteropServices;

class Tour
{
    public int largo;
    public Ciudad[] ady;
    public int costo;
    public List<int> ciudades;

    public Tour(int n, Random engine)
    {
        largo = n;
        ady = InitializeArray<Ciudad>(n);


          ciudades = Enumerable.Range(0, n).ToList();
          ciudades.Add(0);

        //ciudades = new List<int>(){0,48,31,44,18,40,7,8,9,42,32,50,10,51,13,12,46,25,26,27,11,24,3,5,14,4,23,47,37,36,39,38,35,34,33,43,45,15,28,49,19,22,29,1,6,41,20,16,2,17,30,21,0};
          Revolver(ciudades, 1, ciudades.Count - 1, engine);

        //  mostrarCiudadesTour(ciudades);

        

        Ciudad anterior = ady[0];
        Ciudad inicio = ady[1];
         
        int posicion = 0;
        
        for (int index = 1; index != n; index++){

            posicion++; 
            if(posicion == n) posicion = 0;

            Ciudad ptr = ady[ciudades[index]];

            ptr.posicion = posicion;
            ptr.idCiudad = ciudades[index];
            ptr.anterior = anterior;

            if (index==n-1){
                ptr.siguiente=ady[0];
                ady[0].anterior = ptr;
                ady[0].siguiente = ady[ciudades[1]];
            } else {
                ptr.siguiente=ady[ciudades[index +1]];
            }
            anterior = ptr;
        }

        Console.WriteLine("Terminado");
    }

    public void Show()
    {
        Console.Write(string.Format("{0},", 0));
        Ciudad inicio = ady[0];
        Ciudad actual=inicio.siguiente;
        
        while(actual != inicio){
            Console.Write(string.Format("{0},", actual.idCiudad));
            actual = actual.siguiente;
        }
        Console.Write("0, \n");
    }

    /***
        Metodo para evaluar la cantidad total.
    */
    public int Evaluar(Mapa m)
    {
        Ciudad inicio = ady[0];
        Ciudad actual=inicio.siguiente;
        
        int total = 0;
        while(actual != inicio){
            total += m.data[actual.anterior.idCiudad][actual.idCiudad];
            actual = actual.siguiente;
        }

        total += m.data[actual.anterior.idCiudad][actual.idCiudad];
        return total;
    }

    /***
        Realiza el movimiento
    */
    private void Mover(Ciudad t0, Ciudad t1, Ciudad t2, Ciudad t3)
    {
        int pos = t3.posicion;
        var ptr = t1;
        while (ptr != t2)
        {
            ptr.posicion = pos--;
            if (pos < 0) pos = largo - 1;
            Swap(ref ptr.siguiente, ref ptr.anterior);
            ptr = ptr.anterior;
        }
        t0.siguiente = t3; t3.anterior = t0;
        t1.siguiente = t2; t2.anterior = t1;
    }


        public bool IsConexa()
    {
        Ciudad inicio = ady[0];
        Ciudad actual=inicio.siguiente;
        
        int cont = 1;
        int pos = inicio.posicion;
        pos++; if (pos == ady.Length) pos = 0;
        while (actual != inicio)
        {
            if (pos != actual.posicion)
            {
                Console.WriteLine("Esperada: " + pos + ", encontrada: " + actual.posicion + ", en nodo " + actual.idCiudad);
                return false;
            }
            pos++; if (pos == ady.Length) pos = 0;
            cont++;
            if (actual.siguiente.anterior != actual)
            {
                Console.WriteLine("Anterior mal configurado en nodo: " + actual.siguiente.idCiudad);
                return false;
            }
            actual = actual.siguiente;
            if (cont > ady.Length) 
            return false;
        }
        return cont == ady.Length;
    }

    public int TwoOpt(Random engine, Mapa mapa)
    {
        int aleatorio = engine.Next(0, largo - 1); // RandomNumberGenerator.Instance.Generate(0, largo -1);

        var t0 = ady[aleatorio];
        var t1 = t0.siguiente;

        foreach (var nodo in mapa.candidatosDeNodo[t1.idCiudad])
        {
            var t2 = ady[nodo];
            if (t1.siguiente == t2 || t0 == t2) continue; 
            var t3 = t2.anterior;
            int ganancia = mapa.data[t0.idCiudad][t1.idCiudad] - mapa.data[t1.idCiudad][t2.idCiudad] + mapa.data[t2.idCiudad][t3.idCiudad] - mapa.data[t3.idCiudad][t0.idCiudad];
            if (ganancia > 0)
            {
                Mover(t0, t1, t2, t3);
                return ganancia;
            }
        }
        return 0;
    }
    
       public int TwoOpt(Random engine, Mapa mapa, Tour mejor = null)
    {
        int aleatorio = engine.Next(0, largo - 1); // RandomNumberGenerator.Instance.Generate(0, largo -1);

        Ciudad inicio = ady[0];
        var t0 = ady[aleatorio];
        Ciudad t1 = null;

        while(true) //optimización aplicada a un mejor tour
        {
            t1 = t0.siguiente;
            if (mejor == null) 
                break;
            if (mejor.ady[t0.idCiudad].siguiente.idCiudad != t1.idCiudad && mejor.ady[t0.idCiudad].anterior.idCiudad != t1.idCiudad) 
                break;
            t0 = t0.siguiente;

            if (t0 == inicio)
                return 0;
        }

        foreach (var nodo in mapa.candidatosDeNodo[t1.idCiudad])
        {
            var t2 = ady[nodo];
            if (t1.siguiente == t2 || t0 == t2) continue; 
            var t3 = t2.anterior;
            int ganancia = mapa.data[t0.idCiudad][t1.idCiudad] - mapa.data[t1.idCiudad][t2.idCiudad] + mapa.data[t2.idCiudad][t3.idCiudad] - mapa.data[t3.idCiudad][t0.idCiudad];
            if (ganancia > 0)
            {
                int t3t1 = t3.posicion - t1.posicion;
                int t0t2 = t0.posicion - t2.posicion;
                if (t3t1 < 0) 
                    t3t1 += ady.Length;

                if (t0t2 < 0)
                    t0t2 += ady.Length;

                if (t3t1<t0t2)
                    Mover(t0, t1, t2, t3); //Movemos en el sentido horario.
                else
                    Mover(t3, t2, t1, t0); //Movemos en el sentido horario.
                return ganancia;
            }
        }
        return 0;
    }

        public int ThreeOpt(Random engine, Mapa mapa, Tour mejor = null)
    {
        //int aleatorio = engine.Next(0, largo - 1); // RandomNumberGenerator.Instance.Generate(0, largo -1);

        int aleatorio = (int)RandomNumberGenerator.Instance.Generate(0, largo -1); 

        Ciudad inicio = ady[0];
        var t0 = ady[aleatorio];
        Ciudad t1 = null;

        while(true) //optimización aplicada a un mejor tour
        {
            t1 = t0.siguiente;
            if (mejor == null) 
                break;
            if (mejor.ady[t0.idCiudad].siguiente.idCiudad != t1.idCiudad && mejor.ady[t0.idCiudad].anterior.idCiudad != t1.idCiudad) 
                break;
            t0 = t0.siguiente;

            if (t0 == inicio)
                return 0;
        }

         int g0=mapa.data[t0.idCiudad][t1.idCiudad];

        foreach (var nodo in mapa.candidatosDeNodo[t1.idCiudad])
        {
            var t2 = ady[nodo];
            int G0=g0-mapa.data[t1.idCiudad][t2.idCiudad];
            if (G0 <= 0 || t1.siguiente == t2 || t1.anterior == t2) 
                continue; 
            var t3 = t2.anterior;
            int g1 = G0+mapa.data[t2.idCiudad][t3.idCiudad]-mapa.data[t3.idCiudad][t0.idCiudad];
            if (g1 > 0){
                int t3t1 = t3.posicion - t1.posicion;
                int t0t2 = t0.posicion - t2.posicion;
                if (t3t1 < 0) 
                    t3t1 += ady.Length;

                if (t0t2 < 0)
                    t0t2 += ady.Length;

                if (t3t1<t0t2)
                    Mover(t0, t1, t2, t3); //Movemos en el sentido horario.
                else
                    Mover(t3, t2, t1, t0); //Movemos en el sentido contra sentido horario.
                return g1;    
            } else {
                //3opt
                foreach (var opt3 in mapa.candidatosDeNodo[t3.idCiudad]){
                    var t4 = ady[opt3];

                    int G1=G0+mapa.data[t2.idCiudad][t3.idCiudad]-mapa.data[t3.idCiudad][t4.idCiudad];
                    
                    if (G1<=0 || t4==t3.siguiente || t4==t3.siguiente)
                        continue;

                    bool post4=Entre(t1, t3, t4);
                        Ciudad t5=post4?t4.siguiente:t4.anterior;

                        int g2=G1+mapa.data[t4.idCiudad][t5.idCiudad]-mapa.data[t5.idCiudad][t0.idCiudad];
                        if (g2>0){
                            Mover(t0,t1, t2, t3);
                            Mover(t0,t3, t4, t5);
                            return g2;
                        }
                }
            }   
        }
        return 0;
    }

    /***
        Metodo que realizar un swap de ciudades por referencias.
    */
    private void Swap(ref Ciudad a, ref Ciudad b)
    {
        Ciudad temp = a;
        a = b;
        b = temp;
    }

    /***
        Metodo para barajar las ciudades.
    */
    private static void Revolver<T>(List<T> list, int start, int end, Random rng)
    {
        int n = end - start;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = list[start + k];
            list[start + k] = list[start + n];
            list[start + n] = temp;
        }
    }

    T[] InitializeArray<T>(int length) where T : new()
    {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
            array[i] = new T();
        }

        return array;
    }
}