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
            
                if (!File.Exists(arquivoDb)) { File.Create(arquivoDb).Close(); }
                
                if (_sistema==null)
                    return (false, JsonConvert.DeserializeObject<Sistema>(File.ReadAllText(arquivoDb)));
 
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
