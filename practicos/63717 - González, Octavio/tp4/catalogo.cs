#:package Terminal.Gui@2.*
#:property PublishAot=false

using System.Net.Http.Json;
using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Input;
using Terminal.Gui.Drawing;     
using Terminal.Gui.Configuration;     
using System.Collections.ObjectModel;
using System.Xml.Serialization;

#pragma warning disable CS0618


// ── Consulta inicial al servidor ──────────────────────────────────────────


ProductoDto[] productos;
try {
    using var http = new HttpClient();
    productos = await ObtenerProductos(http);
} catch (Exception ex) {
    Console.Error.WriteLine($"{ex.Message}");
    return;
}

// ── Interfaz TUI ──────────────────────────────────────────────────────────

ConfigurationManager.Enable(ConfigLocations.All);
ConfigurationManager.Apply();

using (IApplication app = Application.Create().Init()){

Window gui = new () {  SchemeName = "Esquemaestro"};

//Menu 

var menu = new MenuBar {
    Menus = [
        new MenuBarItem("_Archivo", [
            new MenuItem("_Agregar", "", () => {}),
            new MenuItem("Salir", "", () => app.RequestStop())
        ]),
        new MenuBarItem("_Movimientos", [
            new MenuItem("_Compra", "", () => {}),
        ])
    ]
};
//SCHEMES 

Color fondo = new Color (30, 30, 46);

var esquemaestro = new Scheme
{
    Normal = new Terminal.Gui.Drawing.Attribute(Color.Green, fondo),
    Focus = new Terminal.Gui.Drawing.Attribute(Color.White, Color.Green)
};

var esquedetalle = new Scheme
{
    Normal = new Terminal.Gui.Drawing.Attribute(Color.Green, Color.Black),
    Focus = new Terminal.Gui.Drawing.Attribute(Color.White, Color.Green)
};

SchemeManager.AddScheme("Esquemaestro", esquemaestro);
SchemeManager.AddScheme("esquedetalle", esquedetalle);



//maestro

var maestro = new FrameView {
    Title = " Maestro ",
    X = 0,
    Y = 4,
    Width = 25,
    Height = Dim.Fill(1),
    SchemeName = "Esquemaestro"
};

var panelmaestro = new ListView {
    X = 0, 
    Y = 0, 
    Width = Dim.Fill(), 
    Height = Dim.Fill(1),
    
};

panelmaestro.SetSource(new ObservableCollection<string>(productos
.Select(p => "ID " + p.Id + " - "+ p.Nombre)
.ToList()
));

maestro.Add(panelmaestro);

//detalle

var detalle = new FrameView {
    Title = "Detalle",
    X= Pos.Right(maestro), Y = Pos.Top(maestro),
    Width = Dim.Fill(), Height = Dim.Fill(),
    SchemeName = "esquedetalle"
};

var buscar = new Label { 
    Text = " Buscar:" , 
    X = Pos.Center(),
    Y= Pos.Top(detalle) - 2,  
    };

var detalles = new TextView {
    Text = string.Join("\n\n", productos.Select(p => $"""
    ID:    {p.Id}
    Cod:   {p.Codigo}
    Nombre: {p.Nombre}
    Precio: ${p.Precio}
    Stock: {p.Stock} u
    """)),
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
    ReadOnly=true,
    SchemeName = "esquedetalle"
   
};
detalle.Add(detalles, buscar);

var input = new TextField() {
    X = Pos.Right(buscar) + 1,
    Y = Pos.Top(buscar),
    Width= 20
};
//Añadir las views
gui.Add(menu, maestro, detalle, buscar, input);
gui.SchemeName = "Dialog";

app.Run(gui);
}

//----------------------------------------------------



static async Task<ProductoDto[]> ObtenerProductos (HttpClient http) {
    const string url = "http://localhost:3000/productos";
    return await http.GetFromJsonAsync<ProductoDto[]>(url) ?? throw new HttpRequestException("No hay productos");
}

// static async Task<ProductoDto> CargarProducto (HttpClient http, int id) {
//     string url = $"http://localhost:3000/productos/{id}";
//     return await http.GetFromJsonAsync<ProductoDto>(url) ?? throw new HttpRequestException("No existe un producto con este ID");
// }
// ── DTO ───────────────────────────────────────────────────────────────────

record ProductoDto(int Id, string Codigo, string Nombre, decimal Precio, int Stock);
record MovimientoDto(int Id, int ProductoId, string Tipo, int Cantidad, DateTime Fecha);