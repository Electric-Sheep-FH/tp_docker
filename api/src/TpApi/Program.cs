using System.Net;
using Npgsql;
using TpApi;

var builder = WebApplication.CreateBuilder(args);

// Aucune chaîne de connexion en dur : elle vient de l'environnement,
// via la variable ConnectionStrings__Default (double underscore).
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings__Default n'est pas définie");

builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

var app = builder.Build();

// Sonde de santé utilisée par le HEALTHCHECK du Dockerfile.
app.MapGet("/health", () => Results.Ok(new { status = "UP" }));

app.MapGet("/api/taches", async (NpgsqlDataSource db) =>
{
    var taches = new List<Tache>();
    await using var cmd = db.CreateCommand("SELECT id, titre, faite FROM tache ORDER BY id");
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
        taches.Add(new Tache(reader.GetInt64(0), reader.GetString(1), reader.GetBoolean(2)));

    return Results.Ok(taches);
});

app.MapPost("/api/taches", async (NouvelleTache nouvelle, NpgsqlDataSource db) =>
{
    string titre;
    try
    {
        titre = TitreUtils.Normaliser(nouvelle.Titre);
    }
    catch (ArgumentException e)
    {
        return Results.BadRequest(new { erreur = e.Message });
    }

    await using var cmd = db.CreateCommand(
        "INSERT INTO tache (titre, faite) VALUES ($1, $2) RETURNING id");
    cmd.Parameters.AddWithValue(titre);
    cmd.Parameters.AddWithValue(nouvelle.Faite);
    var id = (long)(await cmd.ExecuteScalarAsync())!;

    return Results.Created($"/api/taches/{id}", new Tache(id, titre, nouvelle.Faite));
});

// Renvoie le hostname du conteneur : permet de vérifier le load-balancing.
app.MapGet("/api/qui", () => Results.Text(Dns.GetHostName()));

app.Run();

public record Tache(long Id, string Titre, bool Faite);
public record NouvelleTache(string? Titre, bool Faite);
