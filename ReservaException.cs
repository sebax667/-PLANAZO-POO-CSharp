namespace Planazo.Servicios;

/// <summary>Error de negocio del sistema de reservas (validaciones, no encontrados, sobreventa, etc.).</summary>
public class ReservaException : Exception
{
    public ReservaException(string mensaje) : base(mensaje) { }

    public ReservaException(string mensaje, Exception inner) : base(mensaje, inner) { }
}
