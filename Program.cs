using TCPA.Application;
using TCPA.Presentation;

var app = new Application();
var cli = new ConsoleInterface();
app.AddUpdateListener(cli.Update);
app.Run();