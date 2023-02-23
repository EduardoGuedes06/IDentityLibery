using Curso.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace DominandoEFCore
{
    class Program 
    {
        public static void Main(string[] args)
        {
            //CreateAndDelete();
            //GapDoEnsureCreated();
            //GerenciarEstadoDaConexao(false);
            //GerenciarEstadoDaConexao(true);
            //ExecuteSQL();
            //HealthCheckBancoDeDados();
            //SqlInjection();

            //MigracoesPendentes();
            //TodasMigracoes();

            //MigracoesJaAplicadas();
            //ScriptGeralDoBancoDeDados();

            //CarregamentoAdiantado();
            //CarregamentoExplicito();
            //SetupTiposCarregamentos();

            //AplicarMigracaoEmTempodeExecucao();

            //FiltroGlobal();
            //SemFiltroGlobal();
            //ConsultaProjetada();
            //ConsultaComTAG();
            //Consulta1Para1();
            //DivisaoDeConsulta();
        }

        //Cria e dropa
        static void CreateAndDelete()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureCreated();//Cria banco
            //db.Database.EnsureDeleted();//Dropa banco

        }

        //Cria com 1 ou mais contextos
        static void GapDoEnsureCreated()
        {
            using var db1 = new Curso.Data.ApplicationContext();
            using var db2 = new Curso.Data.ApplicationContextCidade();

            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        //Checa se ha um erro
        static void HealthCheckBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
            {

                Console.WriteLine("Posso me conectar");
            }
            else
            {
                Console.WriteLine("Não posso me conectar");
            }
        }

        //Demonstra a diferença de realizar a mesma operações de forma simultanea ou não

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Curso.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (var i = 0; i < 200; i++)
            {
                db.Categorias.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        //3 opções diferentes de realizar um select
        static void ExecuteSQL()
        {
            using var db = new Curso.Data.ApplicationContext();

            // Primeira Opcao
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // Segunda Opcao
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //Terceira Opcao
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }

        //Testando o create 
        static void SqlInjection()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Categorias.AddRange(
                new Curso.Domain.Categoria
                {
                    Descricao = "Departamento 01"
                },
                new Curso.Domain.Categoria
                {
                    Descricao = "Departamento 02"
                });
            db.SaveChanges();

            //var descricao = "Teste ' or 1='1";
            //db.Database.ExecuteSqlRaw("update departamentos set descricao='AtaqueSqlInjection' where descricao={0}",descricao);
            //db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSqlInjection' where descricao='{descricao}'");
            foreach (var departamento in db.Categorias.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descricao: {departamento.Descricao}");
            }
        }

        //Lista Migrações não aplicadas
        static void MigracoesPendentes()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        //Cria a migração e aplica no banco em seguida
        static void AplicarMigracaoEmTempodeExecucao()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.Migrate();
        }

        //Lista todas as Migrações
        static void TodasMigracoes()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        //lista todas as migrações aplicadas no banco
        static void MigracoesJaAplicadas()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        //Lista o script do banco
        static void ScriptGeralDoBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        //Extensao do CarregamentoAdiantado
        static void SetupCategorias(Curso.Data.ApplicationContext db)
        {
            if (!db.Categorias.Any())
            {
                db.Categorias.AddRange(
                    new Curso.Domain.Categoria
                    {
                        Descricao = "Celular",
                        Produtos = new System.Collections.Generic.List<Curso.Domain.Produto>
                        {
                            new Curso.Domain.Produto
                            {
                                Nome = "iphone 11",
                                Descricao = "Iphone 11 Descricao",
                                Preco = 3000,
                                Fornecedor = "Apple"
                            },                            
                            new Curso.Domain.Produto
                            {
                                Nome = "Galaxy S22",
                                Descricao = "SnapDraggon",
                                Preco = 2599,
                                Fornecedor = "Sansung"
                            }
                        }
                    },
                    new Curso.Domain.Categoria
                    {
                        Descricao = "Carro",
                        Produtos = new System.Collections.Generic.List<Curso.Domain.Produto>
                        {
                                new Curso.Domain.Produto
                            {
                                Nome = "Gol G3",
                                Descricao = "2004",
                                Preco = 20000,
                                Fornecedor = "volkswagen"
                            },
                            new Curso.Domain.Produto
                            {
                                Nome = "Voyage",
                                Descricao = "2011",
                                Preco = 45000,
                                Fornecedor = "volkswagen"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        //Fixa o objeto objeto forcando o relarionamento e ecutando uma unica vez e fechando a conexão
        static void CarregamentoAdiantado()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db
                .Categorias
                .Include(p => p.Produtos);

            foreach (var categoria in categorias)
            {

                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Categoria: {categoria.Descricao}");

                if (categoria.Produtos?.Any() ?? false)
                {
                    foreach (var funcionario in categoria.Produtos)
                    {
                        Console.WriteLine($"\tProduto: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        //fixa o ojeto de forma que posterior, só carregando quando for solicitado
        static void CarregamentoExplicito()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db
                .Categorias
                .ToList();

            foreach (var categoria in categorias)
            {
                if (categoria.Id == 2)
                {
                    //db.Entry(departamento).Collection(p=>p.Funcionarios).Load();
                    db.Entry(categoria).Collection(p => p.Produtos).Query().Where(p => p.Id > 2).ToList();
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Categoria: {categoria.Descricao}");

                if (categoria.Produtos?.Any() ?? false)
                {
                    foreach (var funcionario in categoria.Produtos)
                    {
                        Console.WriteLine($"\tProduto: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum produto encontrado!");
                }
            }
        }

        //fixa o ojeto de forma que os dados são carregados sob demanda
        static void CarregamentoLento()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var categorias = db
                .Categorias
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Categoria: {categoria.Descricao}");

                if (categoria.Produtos?.Any() ?? false)
                {
                    foreach (var produto in categoria.Produtos)
                    {
                        Console.WriteLine($"\tProduto: {produto.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum produto encontrado!");
                }
            }
        }


        /////////////////////////////Consultas/////////////////////////////////
        
        //Filtra pesquisa pelo campo (Excluido) usando o padrão do Entity
        static void FiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db.Categorias.Where(p => p.Id > 0).ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao} \t Excluido: {categoria.Excluido}");
            }
        }
        //Filtra pesquisa pelo campo (Excluido) usando o filtro global
        static void SemFiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db.Categorias.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao} \t Excluido: {categoria.Excluido}");
            }
        }

        //Consulta limpa focando em otimização
        static void ConsultaProjetada()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db.Categorias
                .Where(p => p.Id > 0)
                .Select(p => new { p.Descricao, Produtos = p.Produtos.Select(f => f.Nome) })
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao}");

                foreach (var produto in categoria.Produtos)
                {
                    Console.WriteLine($"\t Nome: {produto}");
                }
            }
        }

        //Consulta limpa focando em otimização e cusomização
        static void ConsultaParametrizada()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var id = new SqlParameter
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };
            var categorias = db.Categorias
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id>{0}", id)
                .Where(p => !p.Excluido)
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao}");
            }
        }

        //Consulta limpa focando em otimização e cusomização usando Interpolação de string para proteção de ataque de injeção SQL
        static void ConsultaInterpolada()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var id = 1;
            var categorias = db.Categorias
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id>{id}")
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao}");
            }
        }


        //Realizando consulta e enviando um comentario para o servidor
        static void ConsultaComTAG()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db.Categorias
                .TagWith(@"Estou enviando um comentario para o servidor
                Teste ---ddd
                Segundo comentario
                Terceiro comentario")
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao}");
            }
        }

        //Realiza uma consulta de 1 para 1
        static void Consulta1Para1()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var produtos = db.Produtos
                .Include(p => p.Categoria)
                .ToList();


            foreach (var produto in produtos)
            {
                Console.WriteLine($"Nome: {produto.Nome} / Categoria : {produto.Categoria.Descricao}");
            }

            /*var departamentos = db.Departamentos
                .Include(p=>p.Funcionarios)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {funcionario.Nome}");
                }
            }*/
        }

        //Consulta Sem duplicidade de campos em ambas as tabelas, assim criando uma tabelas auxiliares
        static void DivisaoDeConsulta()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupCategorias(db);

            var categorias = db.Categorias
                .Include(p => p.Produtos)
                .Where(p => p.Id < 3)
                .AsSplitQuery()
                .AsSingleQuery()
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Descrição: {categoria.Descricao}");
                foreach (var produto in categoria.Produtos)
                {
                    Console.WriteLine($"\tNome: {produto.Nome} : Preço {produto.Preco}");
                }
            }
        }





    }
}