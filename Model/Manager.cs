using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fill.Model
{
    internal class Manager
    {
        public Filler Filler { get; set; }

        internal void Run()
        {
            Filler.Run();
        }   

        internal void Load()
        {
            XmlSerializer xml = new XmlSerializer( typeof( Filler ) );
            try
            {
                using (FileStream fs = File.OpenRead( "data.xml" ))
                {
                    Filler = xml.Deserialize( fs ) as Filler;
                }
            }
            catch { Filler = new Filler(); }
        }
        internal void Close()
        {
            XmlSerializer xml = new XmlSerializer( typeof( Filler ) );
            try
            {
                File.Delete( "data.xml" );
                using (FileStream fs = File.Create( "data.xml" ))
                {
                    xml.Serialize( fs, Filler );
                }
            }
            catch { }
        }
    }
}
