namespace Planazo.Modelos;

/// <summary>Reto 2. Usuario que realiza reservas. Mantiene la lista de sus reservas asociadas.</summary>
public class Usuario
{
    private readonly List<Reserva> _reservas = new();
    private string _nombre = string.Empty;

    public Usuario(int id, string nombre, string correo)
    {
        Id = id;
        Nombre = nombre;
        Correo = correo;
    }

    public int Id { get; }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del usuario no puede estar vacío.");
            _nombre = value.Trim();
        }
    }

    public string Correo { get; set; }

    /// <summary>Vista de solo lectura: nadie fuera del dominio puede alterar la lista.</summary>
    public IReadOnlyList<Reserva> Reservas => _reservas.AsReadOnly();

    internal void AgregarReserva(Reserva reserva) => _reservas.Add(reserva);

    public override string ToString() => $"[{Id}] {Nombre} <{Correo}>";
}
