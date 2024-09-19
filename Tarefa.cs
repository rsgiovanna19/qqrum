
//entidade/objeto tarefa - que quando iniciada irá para o DB "Tarefas"
public class Tarefa{

    public int Id { get; set; } //p identificacao do objeto utilizamos um id (padrão get; set;)
    public string? Descricao { get; set; }
    public bool Concluida { get; set; }

}