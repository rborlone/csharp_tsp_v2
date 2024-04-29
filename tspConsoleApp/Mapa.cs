using System;
using System.Collections.Generic;
using System.IO;

public class Mapa
{
    public List<List<int>> data;
    public List<List<int>> candidatosDeNodo;

    public Mapa(string filename)
    {
        try
        {
            string[] lineas = File.ReadAllLines(filename);   
            data = new List<List<int>>();

            // Lee cada línea del archivo
            foreach (string line in lineas)
            {
                // Divide la línea en campos separados por comas
                string[] fields = line.Split(',');
                List<int> row = new List<int>();

                // Convierte los campos a números y los agrega a la fila
                foreach (string field in fields)
                {
                    row.Add(int.Parse(field));
                }

                // Agrega la fila a la lista de datos
                data.Add(row);
            }
        }
        catch (System.Exception)
        {
            throw new IOException("Error al abrir el archivo");
        }        
    }


    public int Prim(List<int> padres, List<int> penalidades = null)
    {
        int ciudades = data.Count;
        List<bool> visited = new List<bool>(new bool[ciudades]);
        List<int> menorDistancia = new List<int>(new int[ciudades]);
        var pq = new SortedSet<(int, int)>();
        int largo = 0;

        pq.Add((0, 1));

        while (pq.Count > 0)
        {
            var (peso, u) = pq.Min;
            pq.Remove((peso, u));

            if (!visited[u])
            {
                largo += peso;
            }
            visited[u] = true;

            var datau = data[u];

            if (penalidades != null)
            {
                int penu = penalidades[u];
                for (int v = 1; v < ciudades; v++)
                {
                    if (v == u)
                    {
                        continue;
                    }
                    int weight = datau[v] + penu + penalidades[v];
                    if (!visited[v] && weight < menorDistancia[v])
                    {
                        menorDistancia[v] = weight;
                        pq.Add((weight, v));
                        padres[v] = u;
                    }
                }
            }
            else
            {
                for (int v = 1; v < ciudades; v++)
                {
                    if (v == u)
                    {
                        continue;
                    }
                    int weight = datau[v];
                    if (!visited[v] && weight < menorDistancia[v])
                    {
                        menorDistancia[v] = weight;
                        pq.Add((weight, v));
                        padres[v] = u;
                    }
                }
            }
        }
        return largo;
    }

    public int GetSize()
    {
        return data.Count;
    }

    public void ComputarCandidatos(int TAMVEC)
    {
        int n = data.Count;
        candidatosDeNodo = new List<List<int>>();

        for (int i = 0; i < n; i++)
        {
            SortedSet<(int, int)> cola = new SortedSet<(int, int)>();
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    continue;
                }
                cola.Add((data[i][j], j));
            }
            candidatosDeNodo.Add(new List<int>());
            for (int k = 0; k < TAMVEC; k++)
            {
                var aux = cola.Min;
                cola.Remove(aux);
                candidatosDeNodo[i].Add(aux.Item2);
            }
        }
    }
}