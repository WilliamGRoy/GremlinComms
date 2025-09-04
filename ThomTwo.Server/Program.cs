using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using ThomTwo.Infrasctructure.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);

var gremlinHostname = "localhost";
var gremlinPort = 8901;
var gremlinUsername = "/dbs/db1/colls/coll1";
var gremlinPassword = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
var gremlinWsUri = new Uri($"ws://{gremlinHostname}:{gremlinPort}");

builder.Services.AddSingleton(_ =>
{
    var server = new GremlinServer(
        hostname: gremlinHostname,
        port: gremlinPort,
        username: gremlinUsername,
        password: gremlinPassword,
        enableSsl: false 
    );

    return new GremlinClient(
        gremlinServer: server,
        new GraphSON2Reader(),
        new GraphSON2Writer(),
        mimeType: GremlinClient.GraphSON2MimeType
    );
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ThomTwo.Application.Features.Officers.Commands.CreateOfficerCommand).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


