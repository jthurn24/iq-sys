using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace IQI.Intuition.Infrastructure.Services.Protection
{
    [DataContract]
    public class ChangeData
    {
        [DataMember(Name = "Description")]
        public string Description { get; set; }
        [DataMember(Name = "Fields")]
        public List<Field> Fields { get; set; }


        public static ChangeData Load(string data)
        {
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(Encoding.Unicode.GetBytes(data), new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof(ChangeData));

            var model = (ChangeData)ser.ReadObject(reader);
            return model;
        }

        public string Serialize()
        {
            var data = new StringBuilder();
            var ser = new DataContractSerializer(typeof(ChangeData));
            using (XmlWriter xw = XmlWriter.Create(data))
            {
                ser.WriteObject(xw, this);
                xw.Flush();
                return data.ToString();
            }
        }

        [DataContract]
        public class Field
        {
            [DataMember(Name = "Name")]
            public string Name { get; set; }
            [DataMember(Name = "Change")]
            public string Change { get; set; }
        }
    }
}
