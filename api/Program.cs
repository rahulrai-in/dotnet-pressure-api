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

        try
        {
            while (GC.GetTotalMemory(false) <= numMegaBytes * 1000 * 1000)
            {
                XmlDocument doc = new();
                for (var i = 0; i < 1000000; i++)
                {
                    memList.Add(doc.CreateNode(XmlNodeType.Element, "node", string.Empty));
                }
            }
        }
        // Don't fail if memory is not available
        catch (OutOfMemoryException ex)
        {
            Console.WriteLine(ex);
        }

        Thread.Sleep(TimeSpan.FromSeconds(durationSec));
        memList.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        return Results.Ok();
    })
    .WithName("LoadMemory");

app.MapPost("/cpu/{threads}/duration/{durationSec}", (int threads, int durationSec) =>
    {
        CancellationTokenSource cts = new();
        for (var counter = 0; counter < threads; counter++)
        {
            ThreadPool.QueueUserWorkItem(tokenIn =>
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                var token = (CancellationToken)tokenIn;
#pragma warning restore CS8605 // Unboxing a possibly null value.
                while (!token.IsCancellationRequested)
                {
                }
            }, cts.Token);
        }

        Thread.Sleep(TimeSpan.FromSeconds(durationSec));
        cts.Cancel();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        cts.Dispose();
        return Results.Ok();
    })
    .WithName("LoadCPU");


app.Run();