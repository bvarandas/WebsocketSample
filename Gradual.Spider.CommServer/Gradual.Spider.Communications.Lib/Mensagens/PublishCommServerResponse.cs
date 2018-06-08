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
    public class PublishCommServerResponse
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

        [ProtoMember(1)]
        public string SessionID { get; set; }

        [ProtoMember(2)]
        public bool IsSnapshot { get; set; }

        [ProtoMember(3)]
        public string Instrumento { get; set; }

        /// <summary>
        /// </summary>
        [ProtoMember(4)]
        public Type Tipo { get; set; }

        [ProtoMember(5)]
        private List<ObjectContainer> Objetos;

        public PublishCommServerResponse()
        {
            Objetos = new List<ObjectContainer>();
        }

        /// <summary>
        /// Adiciona um objeto como parametro de resposta desta mensagem de publicacao
        /// </summary>
        /// <param name="response">Mensagem de publicacao</param>
        /// <param name="objeto">Instancia do objeto</param>
        public static void AddObject(PublishCommServerResponse response, Object objeto)
        {
            MemoryStream xxx = new MemoryStream();

            Serializer.NonGeneric.Serialize(xxx, objeto);

            ObjectContainer container = new ObjectContainer();

            container.Data = xxx.ToArray();
            container.ObjectType = objeto.GetType();

            response.Objetos.Add(container);
        }

        /// <summary>
        /// Retorna uma lista de objetos que compoe essa mensagem de publicacao
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<Object> GetObjects(PublishCommServerResponse request)
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
