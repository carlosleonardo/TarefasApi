public class TarefaDTO
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public bool IsComplete { get; set; }
	public TarefaDTO()
	{
	}
	public TarefaDTO(Tarefa tarefa)
	{
		(Id, Name, IsComplete) = (tarefa.Id, tarefa.Name, tarefa.IsComplete);
	}
}