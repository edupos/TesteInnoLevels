using SistemaCompra.Domain.Core;
using SistemaCompra.Domain.Core.Model;
using SistemaCompra.Domain.ProdutoAggregate;
using SistemaCompra.Domain.SolicitacaoCompraAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaCompra.Domain.SolicitacaoCompraAggregate
{
    public class SolicitacaoCompra : Entity
    {
        public UsuarioSolicitante UsuarioSolicitante { get; private set; }
        public NomeFornecedor NomeFornecedor { get; private set; }
        public IList<Item> Itens { get; private set; }
        public DateTime Data { get; private set; }
        public Money TotalGeral { get; private set; }
        public Situacao Situacao { get; private set; }
        public CondicaoPagamento CondicaoPagamento { get; private set; }

        private SolicitacaoCompra() { }

        public SolicitacaoCompra(string usuarioSolicitante, string nomeFornecedor)
        {
            Id = Guid.NewGuid();
            UsuarioSolicitante = new UsuarioSolicitante(usuarioSolicitante);
            NomeFornecedor = new NomeFornecedor(nomeFornecedor);
            Data = DateTime.Now;
            Situacao = Situacao.Solicitado;
        }

        public void AdicionarItem(Produto produto, int qtde)
        {
            Itens.Add(new Item(produto, qtde));
        }

        public bool ValidaCondicaoPagamentoQuandoTotalMaior50000()
        { 
            if(TotalGeral.Value > 50000)
            {
                if(CondicaoPagamento.Valor != 30)
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidaTotalItens()
        {
            if ( Itens.Count() <= 0)
            {
                return false;
            }
            return true;
        }

        public void RegistrarCompra(IEnumerable<Item> itens)
        {
            if (ValidaCondicaoPagamentoQuandoTotalMaior50000() == false)
            {
                throw new Exception("Quando valor maior que 50000, Condição de pagamento deve ser de 30 dias.");
            }

            if(ValidaTotalItens())
            {
                throw new Exception("A solicitação de compra deve possuir itens!");
            }

            foreach(var item in itens)
            {
                AdicionarItem(item.Produto, item.Qtde);
            }
        }
    }
}
