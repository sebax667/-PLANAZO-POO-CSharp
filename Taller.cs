namespace Planazo.Modelos;

/// <summary>Taller: descuento del 10% sobre el precio base.</summary>
public class Taller : Evento
{
    public const decimal Descuento = 0.10m;

    public Taller(int id, string nombre, DateTime fecha, string lugar, decimal precioBase, int cupoTotal, string instructor)
        : base(id, nombre, fecha, lugar, precioBase, cupoTotal)
    {
        Instructor = instructor;
    }

    public string Instructor { get; set; }

    public override decimal CalcularPrecioFinal() => Math.Round(PrecioBase * (1 - Descuento), 2);
}
