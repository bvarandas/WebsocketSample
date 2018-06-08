using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.Lib.Util
{
    public class Cloner
    {

        public static PosClientSymbolInfo ClonePosClient(PosClientSymbolInfo pc)
        {
            PosClientSymbolInfo ret = new PosClientSymbolInfo();
            ret.Account = pc.Account;
            ret.Ativo = pc.Ativo;
            ret.Variacao = pc.Variacao;
            ret.UltPreco = pc.UltPreco;
            ret.PrecoFechamento = pc.PrecoFechamento;
            ret.QtdAbertura = pc.QtdAbertura;
            ret.QtdExecC = pc.QtdExecC;
            ret.QtdExecV = pc.QtdExecV;
            ret.NetExec = pc.NetExec;
            ret.QtdAbC = pc.QtdAbC;
            ret.QtdAbV = pc.QtdAbV;
            ret.NetAb = pc.NetAb;
            ret.PcMedC = pc.PcMedC;
            ret.PcMedV = pc.PcMedV;
            ret.FinancNet = pc.FinancNet;
            ret.LucroPrej = pc.LucroPrej;
            ret.DtPosicao = pc.DtPosicao;
            ret.DtMovimento = pc.DtMovimento;
            ret.QtdD1 = pc.QtdD1;
            ret.QtdD2 = pc.QtdD2;
            ret.QtdD3 = pc.QtdD3;
            ret.SegmentoMercado = pc.SegmentoMercado;
            ret.Bolsa = pc.Bolsa;
            ret.CodCarteira = pc.CodCarteira;
            ret.TipoMercado = pc.TipoMercado;
            ret.DtVencimento = pc.DtVencimento;
            ret.CodPapelObjeto = pc.CodPapelObjeto;
            ret.ExecBroker = pc.ExecBroker;
            ret.MsgId = pc.MsgId;
            ret.EventSource = pc.EventSource;
            ret.QtdTotal = pc.QtdTotal;
            ret.QtdDisponivel = pc.QtdDisponivel;
            ret.VolCompra = pc.VolCompra;
            ret.VolVenda = pc.VolVenda;
            ret.VolTotal = pc.VolTotal;
            ret.NomeCliente = pc.NomeCliente;
            ret.LucroPrejuizoSemAbertura = pc.LucroPrejuizoSemAbertura;
            
            ret.TradeDetails = new List<PosClientSymbolDetailInfo>();
            
            for(int i=0; i < pc.TradeDetails.Count; i++)
            {
                var item = pc.TradeDetails[i];

                var info = new PosClientSymbolDetailInfo();

                info.Bolsa          = item.Bolsa;
                info.ClOrdID        = item.ClOrdID;
                info.CodigoCliente  = item.CodigoCliente;
                info.Horario        = item.Horario;
                info.Instrumento    = item.Instrumento;
                info.LP             = item.LP;
                info.PrecoCliente   = item.PrecoCliente;
                info.PrecoMercado   = item.PrecoMercado;
                info.Qtde           = item.Qtde;
                info.Sentido        = item.Sentido;
                info.TotalCliente   = item.TotalCliente;
                info.TotalMercado   = item.TotalMercado;

                ret.TradeDetails.Add(info);
            }

            return ret;
        }
    }
}
