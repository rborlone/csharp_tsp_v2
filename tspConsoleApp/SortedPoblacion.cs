public class SortedPoblacion : IComparable
{
    public int Costo { get; set; }
    public Tour Tour { get; set; }

    public SortedPoblacion(int Costo, Tour tour)
    {
        
        this.Costo = Costo;
        this.Tour = new Tour(tour);
    }

    public int CompareTo(object obj)
    {
        var other = (int)obj;
        return Costo.CompareTo(other);

    }
}