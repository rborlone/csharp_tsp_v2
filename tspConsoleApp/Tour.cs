class Tour
{
    public int largo;
    public Ciudad[] ady;
    public int costo;
    public List<int> ciudades;
    Ciudad INICIO;

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
        INICIO = ady[1];
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
    }

    private void showCityTour(){
        Console.Write("Tour");
        foreach (int item in ciudades)
        {
            Console.Write(string.Format("{0},",item));
        }

        Console.WriteLine();
    }

    public void Show()
    {
        Console.Write("0,");

        var actual = INICIO.siguiente;
        while(actual != INICIO){
            Console.Write(string.Format("{0},", actual.idCiudad));
            actual = actual.siguiente;
        }
        Console.Write("0");
    }

    /***
        Metodo para evaluar la cantidad total.
    */
    public int Evaluar(Mapa m)
    {
        var actual = INICIO.siguiente;
        int total = 0;
        while(actual != INICIO){
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
        var actual = INICIO.siguiente;
        int cont = 1;
        int pos = INICIO.posicion;
        pos++; if (pos == ady.Length) pos = 0;
        while (actual != INICIO)
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