using System.Collections;

public class Poblacion
{
    public PriorityQueue<Tour, int> listaPoblacion = new PriorityQueue<Tour, int>();
    public List<Tour> listaMejores = new List<Tour>();

    public List<Tour> elegirPadres(int cantidad){
        
        List<Tour> retorno = new List<Tour>();
        //obtenemos un total de padres.
        for(int i = 0; i<cantidad; i++){
            var t = listaPoblacion.Dequeue();

            listaMejores.Add(t);
        }

        Tour t1, t2;
        while(true){
            int aleatorio1, aleatorio2;
            aleatorio1 = (int)RandomNumberGenerator.Instance.Generate(0,cantidad--);
            aleatorio2 = (int)RandomNumberGenerator.Instance.Generate(0,cantidad--);
            t1 = listaMejores[aleatorio1];
            t2 = listaMejores[aleatorio2];

            if (aleatorio1 != aleatorio2){
                break;
            }  //verificamos que no sea el mismo que se selecciono.
         }
        
        //sacamos a los 2 mejores candidatos.
        retorno.Add(t1);
        retorno.Add(t2);
       
        return retorno;
    }

    public Tour CrearTour(Mapa mapa, int cantidadCiclos){

         Tour mejor = null;

        Random seeder = new Random();
        int seed = seeder.Next();
        Random engine = new Random(seed);

        Tour tour = new Tour(mapa.data.Count, engine);
                tour.costo = tour.Evaluar(mapa);

                // tour.Show();
                for (int i = 0; i < cantidadCiclos; i++){
                    int nSinMejora = 0;
                                while (nSinMejora < 1 * mapa.data.Count)
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
                if (mejor == null || tour.costo < mejor.costo)
                    {
                        mejor = tour;
                    }
                }

                return mejor;         
    }
}