namespace Planazo.Modelos;

/// <summary>
/// Reserva de un usuario para un evento. Guarda el precio unitario del momento
/// de la compra, de modo que un cambio posterior de precio no altere reservas existentes.
/// </summary>
public class Reserva
{
    public Reserva(int id, Usuario usuario, Evento evento, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad de entradas debe ser mayor que cero.");

        Id = id;
        Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        Evento = evento ?? throw new ArgumentNullException(nameof(evento));
        Cantidad = cantidad;
        PrecioUnitario = evento.CalcularPrecioFinal();
        FechaCreacion = DateTime.Now;
        Activa = true;
    }

    public int Id { get; }
    public Usuario Usuario { get; }
    public Evento Evento { get; }
    public int Cantidad { get; }
    public decimal PrecioUnitario { get; }
    public DateTime FechaCreacion { get; }
    public bool Activa { get; private set; }

    public decimal Total => PrecioUnitario * Cantidad;

    /// <summary>Marca la reserva como cancelada. Una reserva no puede cancelarse dos veces.</summary>
    internal void Cancelar()
    {
        if (!Activa)
            throw new InvalidOperationException($"La reserva #{Id} ya fue cancelada anteriormente.");
        Activa = false;
    }

    public override string ToString() =>
        $"Reserva #{Id} | {Usuario.Nombre} | {Evento.Nombre} | {Cantidad} entrada(s) | " +
        $"Total ${Total:N2} | {(Activa ? "ACTIVA" : "CANCELADA")}";
}
