using Planazo.Modelos;

namespace Planazo.Servicios;

/// <summary>
/// Servicio de dominio: concentra TODA la lógica de negocio (registro, reservas,
/// cupos, cancelaciones y estadísticas). La capa de presentación solo lo invoca.
/// </summary>
public class GestorReservas
{
    private readonly List<Evento> _eventos = new();
    private readonly List<Usuario> _usuarios = new();
    private readonly List<Reserva> _reservas = new();
    private int _siguienteIdReserva = 1;
    private int _siguienteIdUsuario = 1;

    // ---------- Eventos ----------
    public void RegistrarEvento(Evento evento)
    {
        if (evento is null) throw new ReservaException("El evento no puede ser nulo.");
        if (_eventos.Any(e => e.Id == evento.Id))
            throw new ReservaException($"Ya existe un evento con id {evento.Id}.");
        _eventos.Add(evento);
    }

    public IReadOnlyList<Evento> ObtenerEventos() => _eventos.AsReadOnly();

    public IEnumerable<Evento> ObtenerEventosPorTipo(string tipo) =>
        _eventos.Where(e => e.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));

    public Evento BuscarEvento(int id) =>
        _eventos.FirstOrDefault(e => e.Id == id)
        ?? throw new ReservaException($"No existe un evento con id {id}.");

    // ---------- Usuarios ----------
    public Usuario RegistrarUsuario(string nombre, string correo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReservaException("El nombre del usuario es obligatorio.");
        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
            throw new ReservaException("El correo del usuario no es válido.");

        var usuario = new Usuario(_siguienteIdUsuario++, nombre, correo.Trim());
        _usuarios.Add(usuario);
        return usuario;
    }

    public IReadOnlyList<Usuario> ObtenerUsuarios() => _usuarios.AsReadOnly();

    public Usuario BuscarUsuario(int id) =>
        _usuarios.FirstOrDefault(u => u.Id == id)
        ?? throw new ReservaException($"No existe un usuario con id {id}.");

    // ---------- Reservas ----------
    public Reserva CrearReserva(int usuarioId, int eventoId, int cantidad)
    {
        if (cantidad <= 0)
            throw new ReservaException("La cantidad de entradas debe ser mayor que cero.");

        var usuario = BuscarUsuario(usuarioId);
        var evento = BuscarEvento(eventoId);

        if (!evento.HayCupoPara(cantidad))
            throw new ReservaException(
                $"Sobreventa: pediste {cantidad} entrada(s) pero solo quedan {evento.CupoDisponible} para '{evento.Nombre}'.");

        try
        {
            evento.ReservarCupo(cantidad);
            var reserva = new Reserva(_siguienteIdReserva++, usuario, evento, cantidad);
            _reservas.Add(reserva);
            usuario.AgregarReserva(reserva);
            return reserva;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            throw new ReservaException(ex.Message, ex);
        }
    }

    public Reserva CancelarReserva(int reservaId)
    {
        var reserva = BuscarReserva(reservaId);
        try
        {
            reserva.Cancelar();                         // lanza si ya estaba cancelada
            reserva.Evento.LiberarCupo(reserva.Cantidad); // devuelve los cupos
            return reserva;
        }
        catch (InvalidOperationException ex)
        {
            throw new ReservaException(ex.Message, ex);
        }
    }

    public Reserva BuscarReserva(int id) =>
        _reservas.FirstOrDefault(r => r.Id == id)
        ?? throw new ReservaException($"No existe una reserva con id {id}.");

    public IReadOnlyList<Reserva> ObtenerTodasLasReservas() => _reservas.AsReadOnly();

    public IEnumerable<Reserva> ObtenerReservasActivas() => _reservas.Where(r => r.Activa);

    public IEnumerable<Reserva> ObtenerReservasDeUsuario(int usuarioId) =>
        BuscarUsuario(usuarioId).Reservas;

    // ---------- Resumen ----------
    public int TotalReservasActivas => _reservas.Count(r => r.Activa);

    public int TotalEntradasActivas => _reservas.Where(r => r.Activa).Sum(r => r.Cantidad);

    public decimal ValorTotalActivo => _reservas.Where(r => r.Activa).Sum(r => r.Total);
}
