#:package Terminal.Gui@2.*
#:property PublishAot=false

using System.Net.Http.Json;
using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Input;
using Terminal.Gui.Drawing;     

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

using (IApplication app = Application.Create().Init()){

Window gui = new () { Title = " Catalogo de Productos (ESC para salir) " };


var maestro = new FrameView {
    Title = " Maestro ",
    X = 0,
    Y = 4,
    Width = Dim.Percent(30),
    Height = Dim.Fill(1)
};
    
var panelmaestro = new TextView {
    Text = string.Join("\n\n", productos
    .Select(p => $"""
            - Id     : {p.Id, 1}
            - Código : {p.Codigo, 1}
            - Nombre : {p.Nombre, 1}
            - Precio : {p.Precio, 1}
            - Stock  : {p.Stock, 1}
            -----------------
            """)),
    X = 0, Y = 0, Width = Dim.Fill(), Height= Dim.Fill(1),
    ReadOnly = true, 
    WordWrap = true
    };
maestro.Add(panelmaestro);

var detalle = new FrameView {
    Title = "Detalle",
    X= Pos.Right(maestro), Y = Pos.Top(maestro),
    Width = Dim.Fill(), Height = Dim.Fill()
};
var buscar = new Label { 
    Text = " Buscar:" , 
    X = Pos.Center(),
    Y= Pos.Top(detalle) - 2,  
    };

detalle.Add(buscar);
var input = new TextField() {
    X = Pos.Right(buscar) + 1,
    Y = Pos.Top(buscar),
    Width= 20
};

gui.Add(maestro, detalle, buscar, input);


app.Run(gui);
}
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
// record MovimientoDTO(int Id, int Codigo, TipoMovimiento Tipo, int Cantidad,DateTime Fecha,
// int ProductoId) 
// {
//     public Producto? Producto { get; set; }
// }