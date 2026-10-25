namespace Planazo.Modelos;

/// <summary>
/// Reto 1. Evento deportivo con regla propia: 5% de cargo por seguridad
/// y un 20% adicional (sobre el precio base) si es una final.
/// </summary>
public class EventoDeportivo : Evento
{
    public const decimal CargoSeguridad = 0.05m;
    public const decimal RecargoFinal = 0.20m;

    public EventoDeportivo(int id, string nombre, DateTime fecha, string lugar, decimal precioBase, int cupoTotal,
        string deporte, bool esFinal = false)
        : base(id, nombre, fecha, lugar, precioBase, cupoTotal)
    {
        Deporte = deporte;
        EsFinal = esFinal;
    }

    public string Deporte { get; set; }
    public bool EsFinal { get; set; }

    public override string Tipo => "Deportivo";

    public override decimal CalcularPrecioFinal()
    {
        decimal factor = 1 + CargoSeguridad + (EsFinal ? RecargoFinal : 0m);
        return Math.Round(PrecioBase * factor, 2);
    }
}
