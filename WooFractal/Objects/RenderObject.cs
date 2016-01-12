using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WooFractal
{
    public interface RenderObject
    {
        void CreateElement(bool preview, XElement parent);
    }
}
