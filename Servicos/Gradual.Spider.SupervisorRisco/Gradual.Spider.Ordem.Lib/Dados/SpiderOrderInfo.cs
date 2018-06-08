using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.ComponentModel;

namespace Gradual.Spider.Ordem.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class SpiderOrderInfo
    {
        #region Bloco de Identificacao do Instrumento

        /// <summary>
        /// Bovespa/BMF:
        /// Identificador do instrumento, conforme definido pela BMFBOVESPA. Para a lista 
        /// de instrumentos, consulte a mensagem correspondente (Security List).
        /// </summary>
        [Category("Identificação do Instrumento")]
        [Description("BMF: Identificador do instrumento, conforme definido pela BMFBOVESPA. Para a lista de instrumentos, consulte a mensagem correspondente (Security List).")]
        [ProtoMember(1)]
        public string SecurityID { get; set; }

        #endregion

        #region Bloco NewOrderSingle

        /// <summary>
        /// Id da Ordem no banco de dados na tabela tbOrder da coluna OrderId
        /// </summary>
        [Category("Acompanhamento de Ordens")]
        [Description("(ID da Ordem )Id da ordem no banco de dados na tabela tbOrder da coluna OrderId")]
        [ProtoMember(2,IsRequired=true)]
        public int IdOrdem { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Identificador único da ordem, conforme atribuído pela instituição
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Identificador único da ordem, conforme atribuído pela instituição")]
        [ProtoMember(2, IsRequired = true)]
        public string ClOrdID { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// OrderID atribuído pela BM&FBOVESPA
        /// </summary>
        [Category("New Order Single")]
        [Description("OrderID atribuído pela BM&FBOVESPA")]
        [ProtoMember(2, IsRequired = true)]
        public string OrderID { set; get; }

        /// <summary>
        /// Bovespa:
        /// ClOrdID da oferta que o cliente deseja cancelar
        /// </summary>
        [Category("New Order Single")]
        [Description("Bovespa: ClOrdID da oferta que o cliente deseja cancelar")]
        [ProtoMember(2, IsRequired = true)]
        public string OrigClOrdID { get; set; }

        /// <summary>
        /// Bovespa:
        /// Conta acordada entre a corretora e a instituição. Observação: o campo de código do usuário 
        /// não deve ultrapassar 8 digitos, incluindo o dígito de verificação, sem o hífen.
        /// </summary>
        [Category("New Order Single")]
        [Description("Bovespa: Conta acordada entre a corretora e a instituição. Observação: o campo de código do usuário não deve ultrapassar 8 digitos, incluindo o dígito de verificação, sem o hífen.")]
        [ProtoMember(2, IsRequired = true)]
        public int Account { get; set; }

        /// <summary>
        /// Bovespa/BMF:
        /// Símbolo. A BMFBOVESPA exige que esse campo seja adequadamente preenchido. 
        /// Ele contém a forma inteligível do campo SecurityID, disponível na mensagem de 
        /// lista de instrumentos.
        /// ------------------------------------------------------------------------------
        /// Bovespa:
        /// Símbolo da ação
        /// </summary>
        [Category("Identificação do Instrumento")]
        [Description("BMF: Símbolo. A BMFBOVESPA exige que esse campo seja adequadamente preenchido. Ele contém a forma inteligível do campo SecurityID, disponível na mensagem de lista de instrumentos. | Bovespa: Símbolo da ação")]
        [ProtoMember(2, IsRequired = true)]
        public string Symbol { get; set; }

        /// <summary>
        /// Bovespa/BMF:
        /// Símbolo. A BMFBOVESPA exige que esse campo seja adequadamente preenchido. 
        /// Ele contém a forma inteligível do campo SecurityID, disponível na mensagem de 
        /// lista de instrumentos.
        /// ------------------------------------------------------------------------------
        /// Bovespa:
        /// Símbolo da ação
        /// </summary>
        [Category("Origem do SecurityID")]
        [Description("BMF: Origem do campo FIX SecurityID")]
        [ProtoMember(2, IsRequired = true)]
        public string SecurityIDSource { get; set; }


        /// <summary>
        /// Bovespa/BMF:
        /// Mercado ao qual o instrumento pertence (campo 48). Valor aceito: XBMF=BMFBOVESPA. 
        /// Esse campo é opcional e sua ausência implica a atribuição automática do valor XBMF 
        /// (BMFBOVESPA) para mercado (Market Center)
        /// </summary>
        [Category("Identificação do Instrumento")]
        [Description("BMF: Mercado ao qual o instrumento pertence (campo 48). Valor aceito: XBMF=BMFBOVESPA. Esse campo é opcional e sua ausência implica a atribuição automática do valor XBMF (BMFBOVESPA) para mercado (Market Center)")]
        [ProtoMember(2, IsRequired = true)]
        public string SecurityExchangeID { get; set; }

        /// <summary>
        /// Bovespa/BMF:
        /// Status da ordem
        /// </summary>
        [Category("Ordens")]
        [Description("Status da ordem")]
        [ProtoMember(2, IsRequired = true)]
        public string OrdStatus { get; set; }

        /// <summary>
        /// BMF:
        /// Tipo de ordem. Valores aceitos: 2 = limitada; 4 = stop limitada; K = market with leftover as limit
        /// -------------------------------------------------------------------------------------------------------
        /// Bovespa:
        /// Tipo de ordem. Valores aceitos: 1 = mercado; 2 = limitada; 4 = stop limitada (ordem que se transforma em 
        /// ordem limitada assim que o preço de acionamento é atingido); A = on close (ordem a mercado a ser executada 
        /// pelo preço de fechamento do leilão de pré-abertura); K = market with leftover as limit (ordem a mercado em 
        /// que qualquer quantidade não executada se torna ordem limitada)
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF: Tipo de ordem. Valores aceitos: 2 = limitada; 4 = stop limitada; K = market with leftover as limit | Bovespa: Tipo de ordem. Valores aceitos: 1 = mercado; 2 = limitada; 4 = stop limitada (ordem que se transforma em ordem limitada assim que o preço de acionamento é atingido); A = on close (ordem a mercado a ser executada pelo preço de fechamento do leilão de pré-abertura); K = market with leftover as limit (ordem a mercado em que qualquer quantidade não executada se torna ordem limitada)")]
        [ProtoMember(2, IsRequired = true)]
        public int OrdType { get; set; }

        /// <summary>
        /// Bovespa/BMF
        /// Data/Hora que a ordem foi enviada para o servidor de ordens
        /// </summary>
        [Category("New Order Single")]
        [Description("Data/Hora que a ordem foi enviada para o servidor de ordens")]
        [ProtoMember(2, IsRequired = true)]
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Horário de execução / geração da ordem, expresso em UTC.
        /// Faz referencia à data referencia da mensagem base.
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Horário de execução / geração da ordem, expresso em UTC")]
        [ProtoMember(2, IsRequired = true)]
        public DateTime TransactTime { get; set; }

        /// <summary>
        /// Tempo de validade da ordem. A
        /// ausência desse campo significa que
        /// a ordem é válida para o dia. Valores
        /// aceitos:
        /// 0 = válida para o dia (ou sessão)
        /// 3 = executa integral ou parcialmente
        ///    ou cancela (IOC ou FAK)
        /// 4 = executa integralmente ou
        ///   cancela (FOK)
        /// </summary>
        ///  /// </summary>
        [Category("New Order Single")]
        [Description("Tempo de validade da ordem.")]
        [ProtoMember(2, IsRequired = true)]
        public string TimeInForce { set; get; }

        /// <summary>
        /// Bovespa:
        /// Data de vencimento da ordem. Condicionalmente obrigatório se 59 = GTD.
        /// </summary>
        [Category("New Order Single")]
        [Description("Bovespa: Data de vencimento da ordem. Condicionalmente obrigatório se 59 = GTD.")]
        [ProtoMember(2, IsRequired = true)]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Código do canal em que a ordem sera executada
        /// </summary>
        [Category("Atributos")]
        [Description("Código do canal em que a ordem  sexra executada.")]
        [ProtoMember(2, IsRequired = true)]
        public int ChannelID { get; set; }

        /// <summary>
        /// Bolsa na qual a ordem será executada
        /// </summary>
        [Category("Atributos")]
        [Description("Bolsa na qual a ordem será executada. (BMF/BOVESPA)")]
        [ProtoMember(2, IsRequired = true)]
        public string Exchange { get; set; }

        /// <summary>
        /// Bovespa:
        /// Condicionalmente obrigatório se as ordens forem roteadas pela porta 300. Contém o código da corretora 
        /// a que a oferta pertence.
        /// -----------------------------------------------------
        /// Responsabilidade:
        /// Preenchido pela aplicação
        /// </summary>
        [Category("New Order Single")]
        [Description("Bovespa: Condicionalmente obrigatório se as ordens forem roteadas pela porta 300. Contém o código da corretora a que a oferta pertence. | Responsabilidade: Preenchido pela aplicação")]
        [ProtoMember(2, IsRequired = true)]
        public string ExecBroker { get; set; }


        /// <summary>
        /// BMF / Bovespa:
        /// Ponta da ordem. Valores aceitos: 1 = compra; 2 = venda
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Ponta da ordem. Valores aceitos: 1 = compra; 2 = venda")]
        [ProtoMember(2, IsRequired = true)]
        public string Side { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Número de ações ou contratos da oferta
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Número de ações ou contratos da oferta")]
        [ProtoMember(2, IsRequired = true)]
        public int OrderQty { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Quantidade remanecente de uma ordem
        /// </summary>
        [Category("ExecutionReport")]
        [Description("Quantidade remanecente de uma ordem")]
        [ProtoMember(2, IsRequired = true)]
        public int OrderQtyRemmaining { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Quantidade remanecente de uma ordem
        /// </summary>
        [Category("ExecutionReport")]
        [Description("Quantidade executada total")]
        [ProtoMember(2, IsRequired = true)]
        public int CumQty { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Quantidade mínima para execução da oferta
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Quantidade mínima para execução da oferta")]
        [ProtoMember(2, IsRequired = true)]
        public int MinQty { get; set; }


        /// <summary>
        /// BMF / Bovespa:
        /// Número máximo de ações ou contratos da oferta a ser exibido no núcleo de negociação a qualquer tempo
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Número máximo de ações ou contratos da oferta a ser exibido no núcleo de negociação a qualquer tempo")]
        [ProtoMember(2, IsRequired = true)]
        public int MaxFloor { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Preço por ação ou contrato. Condicionado ao tipo de ordem definir preço-limite (exceto ordem a mercado)
        /// </summary>
        [Category("New Order Single")]
        [Description("BMF / Bovespa: Preço por ação ou contrato. Condicionado ao tipo de ordem definir preço-limite (exceto ordem a mercado)")]
        [ProtoMember(2, IsRequired = true)]
        public double Price { get; set; }

        /// <summary>
        /// BMF / Bovespa:
        /// Preço estipulado para disparar a ordem ao ser atingido
        /// </summary>
        [Category("New Order Single")]
        [Description("Preço estipulado para disparar a ordem ao ser atingido")]
        [ProtoMember(2, IsRequired = true)]
        public double StopPrice { get; set; }



        #endregion

    }
}
