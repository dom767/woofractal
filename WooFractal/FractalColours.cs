using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class FractalColours
    {
        public bool _XOrbitEnabled = true;
        public bool _YOrbitEnabled = true;
        public bool _ZOrbitEnabled = true;
        public OrbitColours _OrbitColoursX = new OrbitColours();
        public OrbitColours _OrbitColoursY = new OrbitColours();
        public OrbitColours _OrbitColoursZ = new OrbitColours();

        public UserControl GetControl()
        {
            return new FractalColourControl(this);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("FRACTALCOLOURS",
                new XAttribute("xOrbitEnabled", _XOrbitEnabled),
                new XAttribute("yOrbitEnabled", _YOrbitEnabled),
                new XAttribute("zOrbitEnabled", _ZOrbitEnabled),
                _OrbitColoursX.CreateElement("ORBITCOLOURSX"),
                _OrbitColoursY.CreateElement("ORBITCOLOURSY"),
                _OrbitColoursZ.CreateElement("ORBITCOLOURSZ"));
            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            _XOrbitEnabled = bool.Parse(reader.GetAttribute("xOrbitEnabled"));
            _YOrbitEnabled = bool.Parse(reader.GetAttribute("yOrbitEnabled"));
            _ZOrbitEnabled = bool.Parse(reader.GetAttribute("zOrbitEnabled"));

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSX")
                {
                    _OrbitColoursX.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSY")
                {
                    _OrbitColoursY.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSZ")
                {
                    _OrbitColoursZ.LoadXML(reader);
                }
            }
            reader.Read();
        }

        public string GenerateScript(bool complexMaterials)
        {
            string script = "";

            Colour totalDiffuse = Colour.max(_OrbitColoursX._StartColour._DiffuseColour, _OrbitColoursX._EndColour._DiffuseColour)
                + Colour.max(_OrbitColoursY._StartColour._DiffuseColour, _OrbitColoursY._EndColour._DiffuseColour)
                + Colour.max(_OrbitColoursZ._StartColour._DiffuseColour, _OrbitColoursZ._EndColour._DiffuseColour);
            double max = totalDiffuse.GetMaxComponent();
            if (max < 0.001) max = 0.001;

            string roundstartX = (_OrbitColoursX._BlendType == OrbitColours.EBlendType.Chop) ? "round(" : "";
            string roundendX = (_OrbitColoursX._BlendType == OrbitColours.EBlendType.Chop) ? ")" : "";
            string roundstartY = (_OrbitColoursY._BlendType == OrbitColours.EBlendType.Chop) ? "round(" : "";
            string roundendY = (_OrbitColoursY._BlendType == OrbitColours.EBlendType.Chop) ? ")" : "";
            string roundstartZ = (_OrbitColoursZ._BlendType == OrbitColours.EBlendType.Chop) ? "round(" : "";
            string roundendZ = (_OrbitColoursZ._BlendType == OrbitColours.EBlendType.Chop) ? ")" : "";

            script += "shader fracColours {\r\n";
            script += "trappos = vec("+roundstartX+"pow(mod((diff.x*" + _OrbitColoursX._Multiplier.ToString() + ")+" + _OrbitColoursX._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursX._Power).ToString() + ")"+roundendX+",";
            script += roundstartY + "pow(mod((diff.y*" + _OrbitColoursY._Multiplier.ToString() + ")+" + _OrbitColoursY._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursY._Power).ToString() + ")" + roundendY + ",";
            script += roundstartZ + "pow(mod((diff.z*" + _OrbitColoursZ._Multiplier.ToString() + ")+" + _OrbitColoursZ._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursZ._Power).ToString() + ")" + roundendZ + ")\r\n";
            script += "diff=lerp(vec(" + _OrbitColoursX._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._DiffuseColour.ToString() + "), trappos.x)\r\n";
            script += "diff+=lerp(vec(" + _OrbitColoursY._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._DiffuseColour.ToString() + "), trappos.y)\r\n";
            script += "diff+=lerp(vec(" + _OrbitColoursZ._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._DiffuseColour.ToString() + "), trappos.z)\r\n";
            script += "diff/=" + max.ToString()+"\r\n";
            script += "spec=lerp(vec(" + _OrbitColoursX._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._SpecularColour.ToString() + "), trappos.x)\r\n";
            script += "spec+=lerp(vec(" + _OrbitColoursY._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._SpecularColour.ToString() + "), trappos.y)\r\n";
            script += "spec+=lerp(vec(" + _OrbitColoursZ._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._SpecularColour.ToString() + "), trappos.z)\r\n";
            script += "spec/=3\r\n";
            if (complexMaterials)
            {
                script += "refl=lerp(vec(" + _OrbitColoursX._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursX._EndColour._Reflectivity.ToString() + "), trappos.x)\r\n";
                script += "refl+=lerp(vec(" + _OrbitColoursY._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursY._EndColour._Reflectivity.ToString() + "), trappos.y)\r\n";
                script += "refl+=lerp(vec(" + _OrbitColoursZ._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursZ._EndColour._Reflectivity.ToString() + "), trappos.z)\r\n";
                script += "refl/=3\r\n";
            }
            script += "emi=lerp(vec(" + _OrbitColoursX._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._EmissiveColour.ToString() + "), trappos.x)\r\n";
            script += "emi+=lerp(vec(" + _OrbitColoursY._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._EmissiveColour.ToString() + "), trappos.y)\r\n";
            script += "emi+=lerp(vec(" + _OrbitColoursZ._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._EmissiveColour.ToString() + "), trappos.z)\r\n";
            script += "emi/=3\r\n";
            script += "power=lerp(" + _OrbitColoursX._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursX._EndColour._SpecularPower.ToString() + ", trappos.x)\r\n";
            script += "power+=lerp(" + _OrbitColoursY._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursY._EndColour._SpecularPower.ToString() + ", trappos.y)\r\n";
            script += "power+=lerp(" + _OrbitColoursZ._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursZ._EndColour._SpecularPower.ToString() + ", trappos.z)\r\n";
            script += "power/=3\r\n";
            script += "gloss=lerp(" + _OrbitColoursX._StartColour._Shininess.ToString() + ", " + _OrbitColoursX._EndColour._Shininess.ToString() + ", trappos.x)\r\n";
            script += "gloss+=lerp(" + _OrbitColoursY._StartColour._Shininess.ToString() + ", " + _OrbitColoursY._EndColour._Shininess.ToString() + ", trappos.y)\r\n";
            script += "gloss+=lerp(" + _OrbitColoursZ._StartColour._Shininess.ToString() + ", " + _OrbitColoursZ._EndColour._Shininess.ToString() + ", trappos.z)\r\n";
            script += "gloss/=3\r\n";
            script += "}\r\n";
            return script;
        }
    }
}
