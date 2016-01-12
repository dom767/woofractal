using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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
        public Vector3 _PreRotation;
        public Vector3 _PostRotation;
        public double _Scale;
        public Vector3 _Offset;
        public EFractalType _FractalType;
        public int _Repeats;

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
