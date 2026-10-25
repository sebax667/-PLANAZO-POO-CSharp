namespace Planazo.Modelos;

/// <summary>Función de cine: se cobra el precio base sin recargos ni descuentos.</summary>
public class Cine : Evento
{
    public Cine(int id, string nombre, DateTime fecha, string lugar, decimal precioBase, int cupoTotal, string formato = "2D")
        : base(id, nombre, fecha, lugar, precioBase, cupoTotal)
    {
        Formato = formato;
    }

    public string Formato { get; set; }

    public override decimal CalcularPrecioFinal() => Math.Round(PrecioBase, 2);
}
