using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public enum EFractalType
    {
        Tetra = 0,
        Menger = 1,
        Cube = 2
    }
    public class WooFractalIteration
    {
        public Vector3 _PreRotation = new Vector3(0,0,0);
        public Vector3 _PostRotation = new Vector3(0, 0, 0);
        public double _Scale = 1.9;
        public Vector3 _Offset = new Vector3(1, 1, 1);
        public EFractalType _FractalType = EFractalType.Cube;
        public int _Repeats = 1;

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("KIFSFRACTAL",
                new XAttribute("preRotation", _PreRotation),
                new XAttribute("postRotation", _PostRotation),
                new XAttribute("scale", _Scale),
                new XAttribute("offset", _Offset),
                new XAttribute("fractalType", _FractalType),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            _PreRotation.LoadString(reader.GetAttribute("preRotation"));
            _PostRotation.LoadString(reader.GetAttribute("postRotation"));
            _Scale = double.Parse(reader.GetAttribute("scale"));
            _Offset.LoadString(reader.GetAttribute("offset"));
            _FractalType = (EFractalType)Enum.Parse(typeof(EFractalType), reader.GetAttribute("fractalType"));
            _Repeats = int.Parse(reader.GetAttribute("repeats"));
            reader.Read();
        }

        public WooFractalIteration()
        {
        }

        public WooFractalIteration(EFractalType type, Vector3 preRotation, Vector3 PostRotation, double scale, Vector3 offset, int repeats)
        {
            _FractalType = type;
            _PreRotation = preRotation;
            _PostRotation = PostRotation;
            _Scale = scale;
            _Offset = offset;
            _Repeats = repeats;
        }

        public UserControl GetControl()
        {
            return new FractalControl(this);
        }

        public string GetFractalString()
        {
            string retstring = "";
            switch (_FractalType)
            {
                case EFractalType.Cube:
                    retstring = "fractal_cuboid(vec(" + _PreRotation.ToString() + "), vec(" + _PostRotation.ToString() + "), " + _Scale.ToString() + ", vec(" + _Offset.ToString() + "))\r\n";
                    break;
                case EFractalType.Menger:
                    retstring = "fractal_menger(vec(" + _PreRotation.ToString() + "), vec(" + _PostRotation.ToString() + "), " + _Scale.ToString() + ", vec(" + _Offset.ToString() + "))\r\n";
                    break;
                case EFractalType.Tetra:
                    retstring = "fractal_tetra(vec(" + _PreRotation.ToString() + "), vec(" + _PostRotation.ToString() + "), " + _Scale.ToString() + ", vec(" + _Offset.ToString() + "))\r\n";
                    break;
            }

            string repstring = "";
            for (int i = 0; i < _Repeats; i++)
            {
                repstring += retstring;
            }

            return repstring;
        }
    };

}
