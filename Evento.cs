namespace Planazo.Modelos;

/// <summary>
/// Clase base abstracta de todos los eventos de PLANAZO.
/// Define los datos comunes, protege el estado interno (cupos) y obliga a cada
/// tipo de evento a definir su propia regla de precio mediante polimorfismo.
/// </summary>
public abstract class Evento
{
    private string _nombre = string.Empty;
    private string _lugar = string.Empty;
    private decimal _precioBase;
    private int _cupoTotal;

    protected Evento(int id, string nombre, DateTime fecha, string lugar, decimal precioBase, int cupoTotal)
    {
        Id = id;
        Nombre = nombre;
        Fecha = fecha;
        Lugar = lugar;
        PrecioBase = precioBase;
        CupoTotal = cupoTotal;
        CupoDisponible = cupoTotal;
    }

    public int Id { get; }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del evento no puede estar vacío.");
            _nombre = value.Trim();
        }
    }

    public DateTime Fecha { get; set; }

    public string Lugar
    {
        get => _lugar;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El lugar del evento no puede estar vacío.");
            _lugar = value.Trim();
        }
    }

    public decimal PrecioBase
    {
        get => _precioBase;
        set
        {
            if (value < 0)
                throw new ArgumentException("El precio base no puede ser negativo.");
            _precioBase = value;
        }
    }

    public int CupoTotal
    {
        get => _cupoTotal;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El cupo total debe ser mayor que cero.");
            _cupoTotal = value;
        }
    }

    /// <summary>Cupos libres. Solo el propio evento puede modificarlos.</summary>
    public int CupoDisponible { get; private set; }

    /// <summary>Nombre legible del tipo de evento (Cine, Concierto, ...).</summary>
    public virtual string Tipo => GetType().Name;

    /// <summary>Precio final por entrada. Cada subclase define su regla.</summary>
    public abstract decimal CalcularPrecioFinal();

    public bool HayCupoPara(int cantidad) => cantidad > 0 && cantidad <= CupoDisponible;

    // internal: solo el servicio de dominio (mismo ensamblado) puede alterar cupos.
    internal void ReservarCupo(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        if (cantidad > CupoDisponible)
            throw new InvalidOperationException(
                $"Sobreventa: se piden {cantidad} entradas y solo quedan {CupoDisponible}.");
        CupoDisponible -= cantidad;
    }

    internal void LiberarCupo(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        if (CupoDisponible + cantidad > CupoTotal)
            throw new InvalidOperationException("No se pueden liberar más cupos que el cupo total.");
        CupoDisponible += cantidad;
    }

    public override string ToString() =>
        $"[{Id}] {Tipo,-10} | {Nombre,-28} | {Fecha:dd/MM/yyyy HH:mm} | {Lugar,-20} | " +
        $"${CalcularPrecioFinal():N2} | Cupos: {CupoDisponible}/{CupoTotal}";
}
