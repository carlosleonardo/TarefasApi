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

var tarefasItens = app.MapGroup("/tarefas");

tarefasItens.MapGet("/", ObterTarefas);

static async Task<IResult> ObterTarefas(TarefaDb db)
{
	return TypedResults.Ok(await db.Tarefas.ToListAsync());
}

tarefasItens.MapGet("/complete", ObterTarefasCompletas);

static async Task<IResult> ObterTarefasCompletas(TarefaDb db)
{
	return TypedResults.Ok(await db.Tarefas.Where(t => t.IsComplete).ToListAsync());
}

tarefasItens.MapGet("/{id}", ObterTarefaPorid);

static async Task<IResult> ObterTarefaPorid(int id, TarefaDb db)
{
	return await db.Tarefas.FindAsync(id)
			is Tarefa tarefa
			? TypedResults.Ok(tarefa)
			: TypedResults.NotFound();
}

tarefasItens.MapPost("/", CriarTarefa);

static async Task<IResult> CriarTarefa(Tarefa tarefa, TarefaDb db)
{
	db.Tarefas.Add(tarefa);
	await db.SaveChangesAsync();
	return TypedResults.Created($"/tarefas/{tarefa.Id}", tarefa);
}

tarefasItens.MapPut("/{id}", AtualizarTarefa);

static async Task<IResult> AtualizarTarefa(int id, Tarefa inputTarefa, TarefaDb db)
{
	var tarefa = await db.Tarefas.FindAsync(id);
	if (tarefa is null) return TypedResults.NotFound();
	tarefa.Name = inputTarefa.Name;
	tarefa.IsComplete = inputTarefa.IsComplete;
	await db.SaveChangesAsync();
	return TypedResults.NoContent();
}

tarefasItens.MapDelete("/{id}", ExcluirTarefa);

static async Task<IResult> ExcluirTarefa(int id, TarefaDb db)
{
	if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
	{
		db.Tarefas.Remove(tarefa);
		await db.SaveChangesAsync();
		return TypedResults.NoContent();
	}
	return TypedResults.NotFound();
}

app.Run();
