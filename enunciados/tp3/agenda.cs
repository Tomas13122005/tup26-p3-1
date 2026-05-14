#:package Terminal.Gui@2.0.1
#:property PublishAot=false

using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

// Punto de entrada

using IApplication app = Application.Create().Init();
app.Run(new AgendaWindow());

// Ventana principal
public sealed class AgendaWindow : Runnable {

    public AgendaWindow() {
        Title  = "Agenda - Terminal.Gui";
        Width  = Dim.Fill();
        Height = Dim.Fill();

        Menu.DefaultBorderStyle = LineStyle.Single;
        BuildLayout();
    }

    private void BuildLayout() {
        MenuBar menu = new() {
            Menus = [
                new MenuBarItem("_Archivo", [
                    new MenuItem("_Nuevo contacto", null!, AbrirDialogo),
                    null!, // Separador
                    new MenuItem("_Salir", "Ctrl+Q", SolicitarSalir)
                ])
            ]
        };

        Button openButton = new() {
            Text = "_Abrir diálogo",
            X    = Pos.Center(),
            Y    = Pos.Center()
        };

        openButton.Accepting += (_, e) => {
            AbrirDialogo();
            e.Handled = true;
        };

        Add(menu, openButton);
    }

    private void AbrirDialogo() {
        EjemploDialog dialog = new();
        App!.Run(dialog);
    }

    private void SolicitarSalir() {
        App!.RequestStop();
    }

    protected override bool OnKeyDown(Key key) {
        if (key == Key.Q.WithCtrl) {
            SolicitarSalir();
            return true;
        }

        return base.OnKeyDown(key);
    }
}

// Diálogo de ejemplo
public sealed class EjemploDialog : Dialog {
    public EjemploDialog() {
        Title  = "Diálogo de ejemplo";
        Width  = 50;
        Height = 8;

        Label message = new() {
            Text = "Este es un diálogo modal de ejemplo.",
            X    = Pos.Center(),
            Y    = 1
        };

        Button closeButton = new() {
            Text      = "_Cerrar",
            IsDefault = true
        };

        closeButton.Accepting += (_, e) => {
            App!.RequestStop();
            e.Handled = true;
        };

        Add(message);
        AddButton(closeButton);
    }
}

