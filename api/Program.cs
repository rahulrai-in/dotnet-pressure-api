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
        // ReSharper disable once CollectionNeverQueried.Local
        List<XmlNode> memList = new();
        while (GC.GetTotalMemory(false) <= numMegaBytes * 1000 * 1000)
        {
            XmlDocument doc = new();
            for (var i = 0; i < 1000000; i++)
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
        for (var counter = 0; counter < threads; counter++)
        {
            var burnThread = new Thread(() =>
            {
                while (true)
                {
                }
                // ReSharper disable once FunctionNeverReturns
            });
            burnThread.Start();
        }

        Thread.Sleep(TimeSpan.FromSeconds(durationSec));
        return Results.Ok();
    })
    .WithName("LoadCPU");


app.Run();