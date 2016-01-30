using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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
    }
}
