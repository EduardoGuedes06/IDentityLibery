using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Curso.Domain
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        //Propriedade de navega��o
        public List<Produto> Produtos { get; set; }
    }
}