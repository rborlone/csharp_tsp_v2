public class SortedPoblacion : IComparable
{
    public int Costo { get; set; }
    public Tour Tour { get; set; }

    public SortedPoblacion(int Costo, Tour  Tour)
    {
        Tour = Tour;
        Costo = Costo;
    }

    public int CompareTo(object obj)
    {
        var other = (int)obj;
        return Costo.CompareTo(other);

    }
}