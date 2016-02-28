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
    public interface WooFractalIteration
    {
        UserControl GetControl();
        string GetFractalString();
        void CreateElement(XElement parent);
    };


}
