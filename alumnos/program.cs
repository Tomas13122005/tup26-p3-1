using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Tup26.AlumnosApp;

class Program {
    static void Main(string[] args) {
        Alumnos alumnos = AlumnosManager.Cargar(AppPaths.ArchivoAlumnos);
        

        // AlumnosManager.CrearCarpetas(alumnos);
        // alumnos = alumnos.ConPractico(1, Estado.Desaprobado);
        // AlumnosManager.Guardar(alumnos, "alumnos-p1-sin-presentar.md");

        // AlumnosManager.Listar(alumnos.ConFotos(false), "Alumnos sin foto");
        // AlumnosManager.Listar(alumnos.ConTelefono(false), "Alumnos sin telefono");
        // AlumnosManager.Listar(alumnos.ConGithub(false).ConTelefono(true), "Alumnos sin GitHub");
        // AlumnosManager.ActualizarDesdePerfiles(alumnos, "../material-docente/perfil/perfiles");
        // AlumnosManager.Guardar(alumnos, AppPaths.ArchivoAlumnos);
        // AlumnosManager.GuardarJSON(alumnos, "alumnos.json");
        // AlumnosManager.GuardarVCard(alumnos, AppPaths.ArchivoVcf);
        // AlumnosManager.CrearCarpetas(alumnos);
        // AlumnosManager.CopiarFotoPerfil(alumnos, rutaPerfiles);

        // var sinFoto = alumnos.SinFotos();
        // AlumnosManager.Guardar(sinFoto, "alumnos-sin-foto.md");

        // var sinTelefono = alumnos.SinTelefono();
        // AlumnosManager.Guardar(sinTelefono, "alumnos-sin-telefono.md");

        // var sinGitHub = alumnos.FiltrarSinGithub();
        // AlumnosManager.Guardar(sinGitHub, "alumnos-sin-github.md");

        // Console.WriteLine("Enviando mensaje al grupo de WhatsApp...");

        // Alumnos alumnosConTelefonoComision7 = alumnos.EnComision("C7").SinTelefono();
        // AlumnosManager.Listar(alumnosConTelefonoComision7);

        // Alumnos alumnosConTelefonoComision9 = alumnos.EnComision("C9").SinTelefono();

        // AlumnosManager.Listar(alumnos.ParaAgregar());

        // AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C7"), "alumnos-agregar-c7.vcf");
        // AlumnosManager.GuardarVCard(alumnos.ParaAgregar().EnComision("C9"), "alumnos-agregar-c9.vcf");
        // AlumnosManager.CopiarEnunciadoPracticos(alumnos, "tp1");

        GitHub gh = new GitHub();
        if (gh.PRSinLegajo() == 0) { 
            gh.NormalizarTitulos(alumnos, simular: false); 
        } 

        // // AlumnosManager.CopiarEnunciadoPracticos(alumnos, "tp2", forzar: false);
        foreach(var pr in gh.PullRequests()) {
            var commits   = gh.Commits(pr.Numero);
            var detallePr = gh.ObtenerEstado(pr.Numero);
            var estado    = detallePr.Estado == "open" ? "abierto" : detallePr.Estado == "closed" ? "cerrado" : "sin dato";
            var mergeable = detallePr.EsMergeable ? "mergeable" : "con conflictos";
            var legajo    = GitHub.ExtraerLegajo(pr.Titulo);
            var tp        = GitHub.ExtraerTP(pr.Titulo);
            // var archivos = gh.ListarArchivos(pr.Numero);
            // if(archivos.Count < 10) {
            //     continue;
            // }
            var a = alumnos.BuscarPorLegajo(legajo);
            if(a is null) {
                Console.WriteLine($"Alumno con legajo {legajo} no encontrado en la lista de alumnos.");
                continue;
            }
            var cantidadArchivos = gh.ListarArchivos(pr.Numero).Count;
            var cantidadCommits = commits.Count;
            Console.ForegroundColor = mergeable == "mergeable" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.BackgroundColor = cantidadArchivos < 10 ? ConsoleColor.Black : ConsoleColor.DarkRed;
            Console.WriteLine($"PR #{pr.Numero:000} | {legajo} | {a.NombreCompleto, -40} | A:{cantidadArchivos,4} | C{cantidadCommits,2} | {estado} | {mergeable:-12} | {tp}");
            Console.ResetColor();
            // gh.BajarArchivo(pr.Numero, $"practicos/{legajo}*/tp2/*.cs", $"../practicos/{a!.CarpetaNombre}/tp2");
            // gh.CerrarPR(pr.Numero);
        }
        // List<string> colaboradores = gh.ListarColaboradores();
        // List<string> invitaciones  = gh.ListarInvitacionesPendientes();

        // Console.WriteLine($"Colaboradores: {string.Join(" ", colaboradores)}");
        // Console.WriteLine($"Pendientes: {string.Join(" ", invitaciones)}");
        // Console.WriteLine();

        // AlumnosManager.Guardar(alumnos, AppPaths.ArchivoAlumnos);

        // MensajesService.MensajeSinGithub();
        // MensajesService.MensajeGithubErroneo();

        // var wapp = new WAppService();
        // // wapp.Enviar("3815343456", "Hola desde la aplicación de alumnos!");
        // Console.WriteLine("\n= Participantes del grupo C7 =");
        // foreach(var c in wapp.Participantes("C7")) {
        //     Console.WriteLine($"- {c.Name,-30} | {c.PhoneNumber} | {c.Jid}");
        // }
        // Console.WriteLine("\n= Participantes del grupo C9 =");
        // foreach(var c in wapp.Participantes("C9")) {
        //     Console.WriteLine($"- {c.Name,-30} | {c.PhoneNumber} | {c.Jid}");
        // }
        // Console.WriteLine("\n= Grupos ==");
        // foreach(var g in wapp.Grupos()) {
        //     Console.WriteLine($"- {g.Group,-30} | {g.Jid}");
        // }

    }
}

// = PR Sin legajo válido =
// - #96: trabajo practico 2                                63776 x 6
// - #113: Tp 02/paz naim federico Paz, Naim Federico       61581 x 1130 (QUE???)
// - #116: Update Nodos.cs                                  63420 x 1
// - #117: Update Programa.cs                               63208 x 1
// - #120: Tp 02/bajre-martinez-julian                      63222 x 6
// - #129: tp2-Cortez-Josias                                63241 x 1 (Tambien modifica otro alumno)
// - #135: Tp 02/lazarte-sergio-fabricio                    63219 x 2
// - #138: Tp 02/gonzalo miranda                            63418 x 6 (tambien modifica configuracion)
// - #141: Tp02/alvarez hernan fabricio                     63300 x 93 (que???)
// Es 61161 o 61611 ?
// Es 63637 o 63737 ?
// Total de PRs sin legajo válido: 9