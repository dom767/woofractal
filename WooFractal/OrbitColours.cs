using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class OrbitColours
    {
        public enum EBlendType
        {
            Linear=0,
            Chop=1,
        };

        public XElement CreateElement(string name)
        {
            XElement ret;

            ret = new XElement(name,
                new XAttribute("blendType", _BlendType),
                _StartColour.CreateElement("STARTCOLOUR", false),
                _EndColour.CreateElement("ENDCOLOUR", false),
                new XAttribute("multiplier", _Multiplier),
                new XAttribute("offset", _Offset),
                new XAttribute("power", _Power));

            return ret;
        }

        public void LoadXML(XmlReader reader)
        {
            _BlendType = (EBlendType)Enum.Parse(typeof(EBlendType), reader.GetAttribute("blendType"));
            _Multiplier = double.Parse(reader.GetAttribute("multiplier"));
            _Offset = double.Parse(reader.GetAttribute("offset"));
            _Power = double.Parse(reader.GetAttribute("power"));

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "STARTCOLOUR")
                {
                    _StartColour.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ENDCOLOUR")
                {
                    _EndColour.LoadXML(reader);
                }
            }
            reader.Read();
        }

        public EBlendType _BlendType = EBlendType.Linear;
        public Material _StartColour = new Material();
        public Material _EndColour = new Material();
        public double _Multiplier = 1.0;
        public double _Offset = 0.0;
        public double _Power = 0.0;
    }
}
