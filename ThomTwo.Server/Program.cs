using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

var builder = WebApplication.CreateBuilder(args);

var gremlinHostname = "localhost";
var gremlinPort = 8901;
var gremlinUsername = "/dbs/db1/colls/coll1";
var gremlinPassword = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
var gremlinWsUri = new Uri($"ws://{gremlinHostname}:{gremlinPort}");

// Register a Gremlin.Net GremlinClient and let Gremlinq use the GremlinServer provider
builder.Services.AddSingleton(_ =>
{
    var server = new GremlinServer(
        hostname: gremlinHostname,
        port: gremlinPort,
        username: gremlinUsername,
        password: gremlinPassword,
        enableSsl: false // emulator typically runs without SSL
    );

    return new GremlinClient(
        gremlinServer: server,
        new GraphSON2Reader(),
        new GraphSON2Writer(),
        mimeType: GremlinClient.GraphSON2MimeType
    );
});

//builder.Services
//    .AddGremlinq(setup => setup
//        // Tell Gremlinq to use the GremlinServer provider and resolve the GremlinClient from DI.
//        // The provider will obtain the GremlinClient we registered above.
//        .UseGremlinServer<Vertex, Edge>()
//        .UseNewtonsoftJson());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


