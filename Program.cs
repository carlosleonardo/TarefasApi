using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TarefaDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
	config.DocumentName = "TodoAPI";
	config.Title = "TarefasAPI v1";
	config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseOpenApi();
	app.UseSwaggerUi(config =>
	{
		config.DocumentTitle = "TarefasAPI";
		config.Path = "/swagger";
		config.DocumentPath = "/swagger/{documentName}/swagger.json";
		config.DocExpansion = "list";
	});
}

app.MapGet("/tarefas", async (TarefaDb db) =>
{
	return await db.Tarefas.ToListAsync();
});

app.MapGet("/tarefas/complete", async (TarefaDb db) =>
{
	return await db.Tarefas.Where(t => t.IsComplete).ToListAsync();
});

app.MapGet("/tarefas/{id}", async (int id, TarefaDb db) =>
{
	return await db.Tarefas.FindAsync(id)
		is Tarefa tarefa
		? Results.Ok(tarefa)
		: Results.NotFound();
});

app.MapPost("/tarefas", async (Tarefa tarefa, TarefaDb db) =>
{
	db.Tarefas.Add(tarefa);
	await db.SaveChangesAsync();
	return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});

app.MapPut("/tarefas/{id}", async (int id, Tarefa inputTarefa, TarefaDb db) =>
{
	var tarefa = await db.Tarefas.FindAsync(id);
	if (tarefa is null) return Results.NotFound();
	tarefa.Name = inputTarefa.Name;
	tarefa.IsComplete = inputTarefa.IsComplete;
	await db.SaveChangesAsync();
	return Results.NoContent();
});

app.MapDelete("/tarefas/{id}", async (int id, TarefaDb db) =>
{
	if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
	{
		db.Tarefas.Remove(tarefa);
		await db.SaveChangesAsync();
		return Results.NoContent();
	}
	return Results.NotFound();
});

app.Run();
