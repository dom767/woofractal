using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    public class OrbitColours
    {
        public enum EBlendType
        {
            Linear=0,
            Chop=1,
        };

        public EBlendType _BlendType = EBlendType.Linear;
        public Material _StartColour = new Material();
        public Material _EndColour = new Material();
        public double _Multiplier = 1.0;
        public double _Offset = 0.0;
        public double _Power = 0.0;
    }
}
