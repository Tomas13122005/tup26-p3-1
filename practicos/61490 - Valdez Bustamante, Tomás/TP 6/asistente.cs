
#:package DotNetEnv@*
#:package Microsoft.Extensions.AI@10.4.0
#:package Microsoft.Extensions.AI.OpenAI@10.4.0
#:package Terminal.Gui@2.4.3
#:property PublishAot=false


using System.ClientModel;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.AI;
using OpenAI;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

//inicio de la app

DotNetEnv.Env.Load();

// el proveedor se elige por argumento (por defecto "openai").
// dotnet run asistente.cs            -> usa OPENAI
// dotnet run asistente.cs -- groq    -> usa GROQ
var proveedor = (args.Length > 0 ? args[0] : "openai").ToUpperInvariant();
var url    = Environment.GetEnvironmentVariable($"{proveedor}_API_URL");
var apiKey = Environment.GetEnvironmentVariable($"{proveedor}_API_KEY");
var modelo = Environment.GetEnvironmentVariable($"{proveedor}_MODEL") ?? "gpt-4o-mini";

if (string.IsNullOrWhiteSpace(url))
{
    Console.WriteLine($"  Falta la variable {proveedor}_API_URL.");
    Console.WriteLine("   Copiá '.env.ejemplo' a '.env' y completá la URL y la clave del proveedor.");
    return;
}


var urlBase = url.EndsWith("/chat/completions") ? url[..^"/chat/completions".Length] : url;

IChatClient chat = new ChatClientBuilder(
        new OpenAIClient(
                new ApiKeyCredential(string.IsNullOrWhiteSpace(apiKey) ? "no-requiere-key" : apiKey),
                new OpenAIClientOptions { Endpoint = new Uri(urlBase) })
            .GetChatClient(modelo)
            .AsIChatClient())
    .UseFunctionInvocation()
    .Build();

// mensaje cargado desde AGENTS.md
var sistema = File.Exists("AGENTS.md")
    ? File.ReadAllText("AGENTS.md")
    : "Sos un asistente de programación. Respondé en español, directo y técnico.";



var opciones = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(Herramientas.LeerArchivo,    "leer-archivo"),
        AIFunctionFactory.Create(Herramientas.EscribirArchivo, "escribir-archivo"),
        AIFunctionFactory.Create(Herramientas.ListarArchivos,  "listar-archivos"),
    ]
};


List<ChatMessage> mensajes = [new(ChatRole.System, sistema)];

// Cada turno visible: un rol y un texto que puede ir creciendo (streaming).
List<(string Rol, StringBuilder Texto)> turnos =
[
    ("Asistente", new StringBuilder(
        "Soy tu asistente de programación.\n\n" +
        "Escribí tu mensaje abajo y presioná **Enter** para enviar.\n" +
        "Puedo operar sobre los archivos de esta carpeta: probá con " +
        "*\"listá los archivos de esta carpeta\"* o *\"leé AGENTS.md\"*.\n\n" +
        "Presioná **Esc** para salir."))
];

var respondiendo = false; // evita envíos superpuestos mientras el modelo responde

//terminal GUI

using IApplication app = Application.Create().Init();

using var ventana = new Window
{
    Title = $" Asistente IA · {modelo}  (Esc para salir) ",
    Width = Dim.Fill(),
    Height = Dim.Fill()
};

// conversacion con scroll
var panelConversacion = new FrameView
{
    Title = " Conversación ",
    X = 0, Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill(5) 
};

var conversacion = new Markdown
{
    Width = Dim.Fill(),
    Height = Dim.Fill(),
    CanFocus = true        // scroll con teclado o mouse
};
panelConversacion.Add(conversacion);

// bloque de entrada
var panelEntrada = new FrameView
{
    Title = " Mensaje ",
    X = 0,
    Y = Pos.Bottom(panelConversacion),
    Width = Dim.Fill(),
    Height = Dim.Fill()   
};

var entrada = new TextField
{
    X = 0, Y = 0,
    Width = Dim.Fill(12), 
    Height = 1
};

var enviar = new Button
{
    X = Pos.Right(entrada) + 1,
    Y = 0,
    Text = "Enviar"
};

panelEntrada.Add(entrada, enviar);
ventana.Add(panelConversacion, panelEntrada);

//logica de envio

// Enter en el campo de texto o click en el botón enviar.
entrada.Accepting += (_, e) => { e.Handled = true; Enviar(); };
enviar.Accepting   += (_, e) => { e.Handled = true; Enviar(); };

Pintar();
entrada.SetFocus();
app.Run(ventana);


// captura el texto del campo y hace el envío.
void Enviar()
{
    if (respondiendo) return;
    var texto = (entrada.Text ?? "").ToString()!.Trim();
    if (texto.Length == 0) return;
    _ = EnviarAsync(texto);
}

// agrega el mensaje del usuario, pide la respuesta al modelo en streaming
async Task EnviarAsync(string texto)
{
    respondiendo    = true;
    entrada.Enabled = false;
    enviar.Enabled  = false;
    entrada.Text    = "";

    //turno del usuario
    turnos.Add(("Vos", new StringBuilder(texto)));
    mensajes.Add(new ChatMessage(ChatRole.User, texto));

    // turno del asistente
    var burbuja = new StringBuilder();
    turnos.Add(("Asistente", burbuja));
    Pintar();

    try
    {
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in chat.GetStreamingResponseAsync(mensajes, opciones))
        {
            updates.Add(update);
            if (!string.IsNullOrEmpty(update.Text))
            {
                burbuja.Append(update.Text);
                Pintar();   
            }
        }
       //conserva el historial
        mensajes.AddMessages(updates);
    }
    catch (Exception ex)
    {
        burbuja.Append($"\n\n> Error: {ex.Message}");
        Pintar();
    }
    finally
    {
        respondiendo    = false;
        entrada.Enabled = true;
        enviar.Enabled  = true;
        entrada.SetFocus();
    }
}

// construye el markdown
void Pintar()
{
    var sb = new StringBuilder();
    foreach (var (rol, txt) in turnos)
    {
        sb.Append("# ").Append(rol == "Vos" ? " Vos" : " Asistente").Append("\n\n");
        sb.Append(txt).Append("\n\n");
    }

    var texto = sb.ToString();
    conversacion.Text = texto;
    conversacion.SetNeedsDraw();

   //autoscroll
    try { conversacion.ScrollVertical(texto.AsSpan().Count('\n') + 50); }
    catch { }
}

//herramientas de archivo
static class Herramientas
{
    [Description("Devuelve el contenido de un archivo de texto.")]
    public static string LeerArchivo(
        [Description("Ruta del archivo a leer")] string ruta)
        => File.Exists(ruta)
            ? File.ReadAllText(ruta)
            : $"No se encontró el archivo: {ruta}";

    [Description("Crea o sobrescribe un archivo de texto con el contenido indicado.")]
    public static string EscribirArchivo(
        [Description("Ruta del archivo a escribir")] string ruta,
        [Description("Contenido que se guardará en el archivo")] string contenido)
    {
        File.WriteAllText(ruta, contenido);
        return $"Archivo guardado: {ruta} ({contenido.Length} caracteres).";
    }

    [Description("Lista los archivos y carpetas de un directorio.")]
    public static string ListarArchivos(
        [Description("Ruta del directorio (vacío = carpeta actual)")] string ruta)
    {
        var dir = string.IsNullOrWhiteSpace(ruta) ? "." : ruta;
        if (!Directory.Exists(dir)) return $"No se encontró el directorio: {dir}";

        var entradas = Directory.EnumerateFileSystemEntries(dir)
            .Select(p => Directory.Exists(p) ? $"[carpeta] {Path.GetFileName(p)}"
                                             : $"          {Path.GetFileName(p)}")
            .OrderBy(x => x);

        var lista = string.Join("\n", entradas);
        return lista.Length == 0 ? $"(El directorio '{dir}' está vacío)" : lista;
    }
}
