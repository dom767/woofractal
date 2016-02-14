using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class RenderOptions
    {
        public double _DistanceMinimum = 2;
        public double _DistanceIterations = 40;
        public double _StepSize = 0.7;
        public double _DistanceExtents = 1;
        public int _FractalIterationCount = 15;
        public int _ColourIterationCount = 15;

        public UserControl GetControl()
        {
            return new RenderControls(this);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("RENDEROPTIONS",
                new XAttribute("distanceMinimum", _DistanceMinimum),
                new XAttribute("distanceIterations", _DistanceIterations),
                new XAttribute("stepSize", _StepSize),
                new XAttribute("distanceExtents", _DistanceExtents),
                new XAttribute("fractalIterationCount", _FractalIterationCount),
                new XAttribute("colourIterationCount", _ColourIterationCount));
            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            _DistanceMinimum = double.Parse(reader.GetAttribute("distanceMinimum"));
            _DistanceIterations = double.Parse(reader.GetAttribute("distanceIterations"));
            _StepSize = double.Parse(reader.GetAttribute("stepSize"));
            _DistanceExtents = double.Parse(reader.GetAttribute("distanceExtents"));
            _FractalIterationCount = int.Parse(reader.GetAttribute("fractalIterationCount"));
            _ColourIterationCount = int.Parse(reader.GetAttribute("colourIterationCount"));
        }
    }
}
