namespace Planazo.Modelos;

/// <summary>Concierto: recargo del 15% sobre el precio base.</summary>
public class Concierto : Evento
{
    public const decimal Recargo = 0.15m;

    public Concierto(int id, string nombre, DateTime fecha, string lugar, decimal precioBase, int cupoTotal, string artista)
        : base(id, nombre, fecha, lugar, precioBase, cupoTotal)
    {
        Artista = artista;
    }

    public string Artista { get; set; }

    public override decimal CalcularPrecioFinal() => Math.Round(PrecioBase * (1 + Recargo), 2);
}
