using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.IO;

namespace Gradual.Spider.Communications.Lib.Mensagens
{
    [ProtoContract]
    [Serializable]
    public class AssinaturaCommServerRequest
    {

        [ProtoContract]
        [Serializable]
        private struct ObjectContainer 
        {
            [ProtoMember(1)]
            public byte[] Data { get; set; }

            [ProtoMember(2)]
            public Type ObjectType { get; set; }
        }

        /// <summary>
        /// Session ID conforme recebido apos o login junto ao communicationserver
        /// </summary>
        [ProtoMember(1)]
        public string SessionID { get; set; }

        /// <summary>
        /// Informa uma lista de tipos de Classes que esta aguardando serem recebidos como sinal
        /// ou publicados como resposta desta solicitacao de assinatura
        /// </summary>
        [ProtoMember(2)]
        public List<Type> TiposAssinados { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        public string Instrumento { get; set; }


        [ProtoMember(4)]
        private List<ObjectContainer> Objetos;


        public AssinaturaCommServerRequest()
        {
            Objetos = new List<ObjectContainer>();
            TiposAssinados = new List<Type>();
        }

        /// <summary>
        /// Adiciona um objeto/instancia como parametro para a solicitacao de assinatura
        /// </summary>
        /// <param name="request"></param>
        /// <param name="objeto"></param>
        public static void AddObject(AssinaturaCommServerRequest request, Object objeto)
        {
            MemoryStream xxx = new MemoryStream();

            Serializer.NonGeneric.Serialize(xxx, objeto);

            ObjectContainer container = new ObjectContainer();

            container.Data = xxx.ToArray();
            container.ObjectType = objeto.GetType();

            request.Objetos.Add(container);
        }

        /// <summary>
        /// Retorna os objetos informados como parametro de assinatura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<Object> GetObjects(AssinaturaCommServerRequest request)
        {
            List<Object> retorno = new List<object>();

            foreach (ObjectContainer container in request.Objetos)
            {
                MemoryStream xxx = new MemoryStream(container.Data);

                Object objeto = Serializer.NonGeneric.Deserialize(container.ObjectType, xxx);

                retorno.Add(objeto);
            }

            return retorno;
        }
    }
}
