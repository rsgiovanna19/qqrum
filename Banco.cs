

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext {     //herança - AppDbContext utilizando p conexão do Db  

    // public AppDbContext(
    //     DbContextOptions<AppDbContext> options)
    //      : base(options)
    // {
    // }

    protected override void OnConfiguring(
        DbContextOptionsBuilder builder)
    {
        string con = "server=localhost;port=3306;" +           //parâmetros 
                     "database=planner;user=root;password=positivo";

        builder.UseMySQL(con)   //con = string de conexão c MySQL
        .LogTo(Console.WriteLine, LogLevel.Information);
        
    }

    //Tabelas
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();  //iniciando a tabela no DB relacional "Tarefa" com as "tarefas" -> objetos

}




public class Banco
{
    private static List<Tarefa> tarefas = new List<Tarefa>  //iniciando lista estática Tarefa, sem persistência de dados - temporário
    {
        new Tarefa { Id = 1, Descricao = "Estudar C#", Concluida = false },
        new Tarefa { Id = 2, Descricao = "Estudar ASP.NET Core", Concluida = false }
    };

    //GET p retornar as tarefas em memória criadas ali em cima
    public static List<Tarefa> getTarefas()
    {
        return tarefas;
    }

    //retornando as tarefas por ID 
    public static Tarefa getTarefa(int id)
    {
        return tarefas.FirstOrDefault(t => t.Id == id);
    }

    //Adicionando tarefa
    public static Tarefa addTarefa(Tarefa tarefa)
    {
        tarefa.Id = tarefas.Count + 1;
        tarefas.Add(tarefa);
        return tarefa;
    }

    //atualizando tarefa por id 
    public static Tarefa updateTarefa(int id, Tarefa tarefa)
    {
        var tarefaExistente = tarefas.FirstOrDefault(t => t.Id == id);  //se ela existe retorna
        if (tarefaExistente == null)    //se nao existe, volta nulo
        {
            return null;
        }

        tarefaExistente.Descricao = tarefa.Descricao;   //da a mensagem de ok
        tarefaExistente.Concluida = tarefa.Concluida;
        return tarefaExistente;
    }


    //deletando tarefa por id 
    public static bool deleteTarefa(int id)     
    {
        var tarefaExistente = tarefas.FirstOrDefault(t => t.Id == id);
        if (tarefaExistente == null)
        {
            return false;
        }

        tarefas.Remove(tarefaExistente);
        return true;
    }

}