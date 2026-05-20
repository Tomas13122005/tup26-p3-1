#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://localhost:5001");
var app = builder.Build();

int contador = 0;

app.MapGet("/contador", () => {
    return $$"""{ "contador": {{contador}} }""";
});

app.MapPost("/contador", () => {
    contador++;
    return Results.Ok();
});

app.MapDelete("/contador", () => {
    contador = 0;
    return Results.Ok();
});

Console.Clear();
Console.WriteLine("=== Servidor de Contador (C#) ===\n");
app.Run();
