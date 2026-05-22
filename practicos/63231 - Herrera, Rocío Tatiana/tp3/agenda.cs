#!/usr/bin/env dotnet
#:property PublishAot=false

#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@*
#:package Dapper@*
#:package Dapper.Contrib@*


using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Data.Common;
using Dapper.Contrib.Extensions;

#!/usr/bin/env dotnet
#:property PublishAot=false

#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@*
#:package Dapper@*
#:package Dapper.Contrib@*

using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Data.Common;
using Dapper.Contrib.Extensions;

string archivoDb = args.Length > 0 ? args[0] : ":memory:";
using SqliteAgendaStore store = new(archivoDb);
store.CrearTablas();
using IApplication app = Application.Create().Init();
app.Run(new AgendaWindow(store, archivoDb));

[Table("Contactos")]
public sealed class Contacto {
    [Key] public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefonos { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notas { get; set; } = "";
    public bool Favorito { get; set; }
    public Contacto Clone() => new() { Id = Id, Nombre = Nombre, Telefonos = Telefonos, Email = Email, Notas = Notas, Favorito = Favorito };
}

public sealed class SqliteAgendaStore : IDisposable {
    readonly SqliteConnection cn;
    public SqliteAgendaStore(string archivo) { cn = new(new SqliteConnectionStringBuilder { DataSource = archivo }.ConnectionString); cn.Open(); }
    public void CrearTablas() => cn.Execute("""
        CREATE TABLE IF NOT EXISTS Contactos(
            Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL,
            Telefonos TEXT NOT NULL DEFAULT '', Email TEXT NOT NULL DEFAULT '',
            Notas TEXT NOT NULL DEFAULT '', Favorito INTEGER NOT NULL DEFAULT 0);
        """);
    public IEnumerable<Contacto> Listar() => cn.GetAll<Contacto>();
    public Contacto Agregar(Contacto c) { Validar(c); c.Id = 0; c.Id = Convert.ToInt32(cn.Insert(c)); return c; }
    public void Modificar(Contacto c) { Validar(c); cn.Update(c); }
    public void Eliminar(Contacto c) => cn.Delete(c);
    public void Dispose() => cn.Dispose();
    static void Validar(Contacto c) {
        if (string.IsNullOrWhiteSpace(c.Nombre)) throw new InvalidOperationException("El nombre no puede estar vacío.");
        if (!string.IsNullOrWhiteSpace(c.Email) && !c.Email.Contains('@')) throw new InvalidOperationException("El email debe contener @.");
    }
}
