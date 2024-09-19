using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Configurações Swagger - padrão 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Configuração Entity Framework em memória
// builder.Services.AddDbContext<AppDbContext>(
//     options => options.UseInMemoryDatabase("Tarefas")
// );
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//----------------------------

//Configuração Entity Framework para uso do MySQL
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

//Configurações Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Planner API");   //quando acessado o localhost, este retornará com a string escrita ao lado. 

//1a OPÇÃO
//app.MapGet("/tarefas", ()=> "Retornar as tarefas");

//2a OPÇÃO
// string FuncTarefas() {
//     return "Retornar as tarefas função convencional";
// }
// app.MapGet("/tarefas", FuncTarefas);

//3a OPÇÃO
// app.MapGet("/tarefas", ()=> {
//     return Banco.getTarefas();
// });

//4a OPÇÃO
// app.MapGet("/tarefas", ()=> Banco.getTarefas());

//5a OPÇÃO
//app.MapGet("/tarefas", Banco.getTarefas);

//GET no endpoint /tarefas -> async = assíncrono, quando uma tarefa nao necessita esperar outra finalizar para começar  
app.MapGet("/tarefas", async (AppDbContext db) =>   //conectando ao db
    await db.Tarefas.ToListAsync());    //await - assincrono tb, utiliza-se para aguardar o resultado de uma assincrona, mas o resto
    //do programa continua executando. 


//app.MapGet("/tarefas/{id}", (int id) => 
//    Banco.getTarefas().FirstOrDefault(t => t.Id == id)

//get do endpoint /tarefas{id}, de forma assincrona, trazendo como parâmetro o id - conexao com db
app.MapGet("/tarefas/{id}", async (int id, AppDbContext db) => 
    await db.Tarefas.FindAsync(id)  //apos a conexao com o db - realizando a busca da tarefa por id
      is Tarefa tarefa  
        ? Results.Ok(tarefa)    //se for encontrado a tarefa, ok 
          : Results.NotFound());    //caso contrário, retorna error 404 not found
    
// app.MapPost("/tarefas", (Tarefa tarefa) => {
//     List<Tarefa> tarefas = Banco.getTarefas();
//     tarefa.Id = tarefas.Count + 1;
//     tarefas.Add(tarefa);
//     return tarefa;
// });

//add nova tarefa
app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) => {
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();    //enq conecta com o db - adiciona uma nova tarefa ao db 
    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);    //retorna um ok 
});

// app.MapPut("tarefas/{id}", (int id, Tarefa tarefa) =>
// {
//     List<Tarefa> tarefas = Banco.getTarefas();
//     var tarefaExiste = tarefas.FirstOrDefault(t =>
//         t.Id == id
//     );
//     if (tarefaExiste == null){
//         return Results.NotFound();
//     }
//     tarefaExiste.Descricao = tarefa.Descricao;
//     tarefaExiste.Concluida = tarefa.Concluida;
//     return Results.Ok(tarefaExiste);
// });

//alterando tarefa existente buscando por id 
app.MapPut("tarefas/{id}", async (int id, Tarefa tarefaAlterada, AppDbContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);    //procurando tarefa pelo id informado pelo usuário
    if (tarefa is null) return Results.NotFound();  //se nao houve, error 404 not found

    tarefa.Descricao = tarefaAlterada.Descricao;
    tarefa.Concluida = tarefaAlterada.Concluida;

    await db.SaveChangesAsync();    //de forma assincrona, salva no db

    return Results.NoContent();
});

// app.MapDelete("tarefas/{id}", (int id) =>
// {
//     List<Tarefa> tarefas = Banco.getTarefas();
//     var tarefaExiste = tarefas.FirstOrDefault(t =>
//         t.Id == id
//     );
//     if (tarefaExiste == null){
//         return Results.NotFound();
//     }
//     tarefas.Remove(tarefaExiste);
//     return Results.NoContent();
// });

//deletando tarefa por id informado 
app.MapDelete("tarefas/{id}", async (int id, AppDbContext db) =>
{
    if(await db.Tarefas.FindAsync(id) is Tarefa tarefa){    //se o id for encontrado na db

        db.Tarefas.Remove(tarefa);  //remove tarefa 
        await db.SaveChangesAsync();    //salva os resultados apos remover, de forma assincrona
        return Results.NoContent(); 
    }
    return Results.NotFound();  //se nao existir, retorna error 404 not found 
 
});




//exemplos de get com endpoints diferentes - get para buscar a informação
app.MapGet("/produtos", () => "Produtos");
app.MapGet("/pessoas", () => "Pessoas");
app.MapGet("/pessoas/{id}", () => "Pessoa 1");  /*aqui realizando um get com parâmetro de id sendo informado*/

//post com endpoints diferentes - post para incluir a informação
app.MapPost("/pessoas", () => "POST pessoa");
app.MapPost("/produtos", () => "POST produto");

//put com endpoints diferentes -  put para alterar a informação
//o put necessita de um parâmetro id para ser executado!!
app.MapPut("/pessoas/{id}", () => "PUT pessoa");
app.MapPut("/produtos/{id}", () => "PUT produto");

//delete com endpoints diferentes - delete para deletar
//o delete necessita de um parâmetro id para ser executado!!
app.MapDelete("/pessoas/{id}", () => "DELETE pessoa");
app.MapDelete("/produtos/{id}", () => "DELETE produto");

app.Run();  //coloca a web application em execução
