using Planazo.Modelos;
using Planazo.Servicios;

namespace Planazo;

/// <summary>Capa de presentación: solo muestra el menú, lee datos y delega en GestorReservas.</summary>
public static class Program
{
    private static readonly GestorReservas Gestor = new();

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CargarDatosDeEjemplo();

        bool salir = false;
        while (!salir)
        {
            MostrarMenu();
            string opcion = LeerTexto("Elige una opción: ");

            try
            {
                switch (opcion)
                {
                    case "1": ConsultarEventos(); break;
                    case "2": ConsultarPorTipo(); break;
                    case "3": RegistrarUsuario(); break;
                    case "4": CrearReserva(); break;
                    case "5": CancelarReserva(); break;
                    case "6": VerReservasDeUsuario(); break;
                    case "7": MostrarResumen(); break;
                    case "0": salir = true; Console.WriteLine("¡Hasta pronto, gracias por usar PLANAZO!"); break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
            catch (ReservaException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error de negocio] {ex.Message}");
                Console.ResetColor();
            }
            catch (FormatException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Entrada inválida] {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error inesperado] {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine();
        Console.WriteLine("========== PLANAZO ==========");
        Console.WriteLine("1. Consultar todos los eventos");
        Console.WriteLine("2. Consultar eventos por tipo");
        Console.WriteLine("3. Registrar usuario");
        Console.WriteLine("4. Crear reserva");
        Console.WriteLine("5. Cancelar reserva");
        Console.WriteLine("6. Ver reservas de un usuario");
        Console.WriteLine("7. Resumen (reservas activas, entradas y valor total)");
        Console.WriteLine("0. Salir");
        Console.WriteLine("=============================");
    }

    private static void ConsultarEventos()
    {
        Console.WriteLine("\n--- Eventos disponibles ---");
        foreach (var e in Gestor.ObtenerEventos())
            Console.WriteLine(e);
    }

    private static void ConsultarPorTipo()
    {
        string tipo = LeerTexto("Tipo (Cine, Concierto, Taller, Deportivo): ");
        var lista = Gestor.ObtenerEventosPorTipo(tipo).ToList();
        if (lista.Count == 0)
        {
            Console.WriteLine("No hay eventos de ese tipo.");
            return;
        }
        lista.ForEach(Console.WriteLine);
    }

    private static void RegistrarUsuario()
    {
        string nombre = LeerTexto("Nombre: ");
        string correo = LeerTexto("Correo: ");
        var usuario = Gestor.RegistrarUsuario(nombre, correo);
        Console.WriteLine($"Usuario registrado: {usuario}");
    }

    private static void CrearReserva()
    {
        MostrarUsuarios();
        int usuarioId = LeerEntero("Id del usuario: ");
        ConsultarEventos();
        int eventoId = LeerEntero("Id del evento: ");
        int cantidad = LeerEntero("Cantidad de entradas: ");

        var reserva = Gestor.CrearReserva(usuarioId, eventoId, cantidad);
        Console.WriteLine($"Reserva creada con éxito -> {reserva}");
    }

    private static void CancelarReserva()
    {
        int id = LeerEntero("Id de la reserva a cancelar: ");
        var reserva = Gestor.CancelarReserva(id);
        Console.WriteLine($"Reserva cancelada -> {reserva}");
    }

    private static void VerReservasDeUsuario()
    {
        MostrarUsuarios();
        int id = LeerEntero("Id del usuario: ");
        var reservas = Gestor.ObtenerReservasDeUsuario(id).ToList();
        if (reservas.Count == 0)
        {
            Console.WriteLine("El usuario no tiene reservas.");
            return;
        }
        reservas.ForEach(Console.WriteLine);
    }

    private static void MostrarResumen()
    {
        Console.WriteLine("\n--- Resumen de reservas activas ---");
        foreach (var r in Gestor.ObtenerReservasActivas())
            Console.WriteLine(r);

        Console.WriteLine($"Reservas activas : {Gestor.TotalReservasActivas}");
        Console.WriteLine($"Entradas totales : {Gestor.TotalEntradasActivas}");
        Console.WriteLine($"Valor total      : ${Gestor.ValorTotalActivo:N2}");
    }

    private static void MostrarUsuarios()
    {
        Console.WriteLine("\n--- Usuarios ---");
        foreach (var u in Gestor.ObtenerUsuarios())
            Console.WriteLine(u);
    }

    // ---------- Utilidades de entrada ----------
    private static string LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static int LeerEntero(string mensaje)
    {
        string texto = LeerTexto(mensaje);
        if (!int.TryParse(texto, out int valor))
            throw new FormatException($"'{texto}' no es un número entero válido.");
        return valor;
    }

    // ---------- Datos iniciales ----------
    private static void CargarDatosDeEjemplo()
    {
        Gestor.RegistrarEvento(new Cine(1, "Dune: Parte Tres", new DateTime(2026, 11, 5, 19, 30, 0), "Cine Royal", 20000m, 50, "IMAX"));
        Gestor.RegistrarEvento(new Concierto(2, "Gira Vallenata 2026", new DateTime(2026, 12, 12, 20, 0, 0), "Estadio Metropolitano", 100000m, 5, "Silvestre Dangond"));
        Gestor.RegistrarEvento(new Taller(3, "Taller de C# y POO", new DateTime(2026, 11, 20, 9, 0, 0), "Aula Magna", 80000m, 20, "Ing. Pérez"));
        Gestor.RegistrarEvento(new EventoDeportivo(4, "Final Liga Costera", new DateTime(2026, 12, 20, 16, 0, 0), "Estadio Romelio Martínez", 60000m, 100, "Fútbol", esFinal: true));

        Gestor.RegistrarUsuario("Jorge Demo", "jorge@example.com");
        Gestor.RegistrarUsuario("Ana Gómez", "ana@example.com");
    }
}
