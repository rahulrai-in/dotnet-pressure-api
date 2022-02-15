using System.Xml;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/memory/{numMegaBytes}/duration/{durationSec}", (long numMegaBytes, int durationSec) =>
{
    List<XmlNode> memList = new List<XmlNode>();
    while (GC.GetTotalMemory(false) <= numMegaBytes * 1000 * 1000)
    {
        XmlDocument doc = new XmlDocument();
        for (int i = 0; i < 1000000; i++)
        {
            memList.Add(doc.CreateNode(XmlNodeType.Element, "node", string.Empty));
        }
    }

    Thread.Sleep(TimeSpan.FromSeconds(durationSec));
    return Results.Ok();
})
.WithName("LoadMemory");

app.MapPost("/cpu/{threads}/duration/{durationSec}", (int threads, int durationSec) =>
{
    ThreadStart ts = new ThreadStart(() => { while (true) { } });
    for (int counter = 0; counter < threads; counter++)
    {
        var burnThread = new Thread(ts);
        burnThread.Start();
    }

    Thread.Sleep(TimeSpan.FromSeconds(durationSec));
    return Results.Ok();
})
.WithName("LoadCPU");


app.Run();