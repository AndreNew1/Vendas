using Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Core.Util
{
    public static class file
    {
        public static string arquivoDb = AppDomain.CurrentDomain.BaseDirectory + "db.json";
        public static (bool gravacao, Sistema sistema) ManipulacaoDeArquivos(Sistema _sistema)
        {
            try
            {
                //Caso Arquivo não existe
                if (!File.Exists(arquivoDb)) { File.Create(arquivoDb).Close(); }

                //Caso sistema ainda não exista
                //Ele lê o arquivo
                if (_sistema==null)
                    return (false, JsonConvert.DeserializeObject<Sistema>(File.ReadAllText(arquivoDb)));
                //Caso o sistema ja exista 
                //Escreve as mudanças
                File.WriteAllText(arquivoDb, JsonConvert.SerializeObject(_sistema,Formatting.Indented));

                return (true, null);

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
