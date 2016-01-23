using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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

        public string GenerateScript(bool complexMaterials)
        {
            string script = "";

            Colour totalDiffuse = Colour.max(_OrbitColoursX._StartColour._DiffuseColour, _OrbitColoursX._EndColour._DiffuseColour)
                + Colour.max(_OrbitColoursY._StartColour._DiffuseColour, _OrbitColoursY._EndColour._DiffuseColour)
                + Colour.max(_OrbitColoursZ._StartColour._DiffuseColour, _OrbitColoursZ._EndColour._DiffuseColour);
            double max = totalDiffuse.GetMaxComponent();
            if (max < 0.001) max = 0.001;

            script += "shader fracColours {\r\n";
            script += "trappos = vec(mod(diff.x*" + _OrbitColoursX._Multiplier.ToString() + ",1.0),";
            script += "mod(diff.y*" + _OrbitColoursY._Multiplier.ToString() + ",1.0),";
            script += "mod(diff.z*" + _OrbitColoursZ._Multiplier.ToString() + ",1.0))\r\n";
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
