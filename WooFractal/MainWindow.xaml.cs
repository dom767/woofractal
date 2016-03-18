using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WooFractal.Objects.WooScript;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using WooFractal.Objects;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Camera _Camera;
        WooScript _BackgroundScript;
        WooScript _SceneScript;
        WooScript _LightingScript;
        Scene _Scene;
        PostProcess _PostProcess;

        public double _CamPosX
        {
            get { return (double)GetValue(_CamPosXProperty); }
            set { SetValue(_CamPosXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamPosXProperty =
            DependencyProperty.Register("_CamPosX", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _CamPosY
        {
            get { return (double)GetValue(_CamPosYProperty); }
            set { SetValue(_CamPosYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamPosYProperty =
            DependencyProperty.Register("_CamPosY", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _CamPosZ
        {
            get { return (double)GetValue(_CamPosZProperty); }
            set { SetValue(_CamPosZProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamPosZProperty =
            DependencyProperty.Register("_CamPosZ", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _CamTagX
        {
            get { return (double)GetValue(_CamTagXProperty); }
            set { SetValue(_CamTagXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamTagXProperty =
            DependencyProperty.Register("_CamTagX", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _CamTagY
        {
            get { return (double)GetValue(_CamTagYProperty); }
            set { SetValue(_CamTagYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamTagYProperty =
            DependencyProperty.Register("_CamTagY", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _CamTagZ
        {
            get { return (double)GetValue(_CamTagZProperty); }
            set { SetValue(_CamTagZProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _CamTagZProperty =
            DependencyProperty.Register("_CamTagZ", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));
            
        private void InitialiseCamera()
        {
            _Camera = new Camera(_AppSettings._CameraFrom, _AppSettings._CameraTo, _AppSettings._FOV, _AppSettings._Spherical, _AppSettings._Stereographic);
            _WootracerOptions._FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();
            _WootracerOptions._ApertureSize = _AppSettings._ApertureSize;
            _WootracerOptions._FieldOfView = _AppSettings._FOV;
            _WootracerOptions._Spherical = _AppSettings._Spherical;
            _WootracerOptions._Stereographic = _AppSettings._Stereographic;
        }

        private void InitialiseScene()
        {
            _Scene = new Scene(_Camera);
        }

        public void InitialiseScript()
        {
            _BackgroundScript = new WooScript();
            _SceneScript = new WooScript();
            _LightingScript = new WooScript();
            _BackgroundScript.Load("background", "scratch");
            _SceneScript.Load("scene", "scratch");
            _LightingScript.Load("lighting", "scratch");
            _BackgroundScript._Program = "rule main {\r\npos.y -= 0\r\ndiff = vec(1.0, 1.0, 1.0)\r\nrefl = vec(0.4, 0.4, 0.4)\r\ngloss = 0.97\r\nscale = vec(460, 460, 460)\r\npos.y-=1\r\ncylinder\r\n}";
            _SceneScript._Program = "rule main {box}";
            _LightingScript._Program = "rule main {directionalLight(vec(1.0, 1.0, 1.0), vec(-0.7, 1.0, -0.6), 0.02, 1) \r\n background(vec(0.8,0.8,0.8))}";
            LoadFractal("scratch");
//            backgroundDesc.Text = _BackgroundScript._Program;
//            sceneDesc.Text = _SceneScript._Program;
//            lightingDesc.Text = _LightingScript._Program;
        }

        private string BuildXML(bool preview)
        {
            bool pt = _Scene._PathTracer;

            _Scene._PathTracer = false;

            string XML = @"
<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";

            Camera previewCamera = new Camera(_Camera._Position, _Camera._Target, _Camera._FOV, _Camera._Spherical, _Camera._Stereographic);
            previewCamera._AAEnabled = true;// false;
            previewCamera._DOFEnabled = false;

            XML += previewCamera.CreateElement().ToString();
            XML += _Scene.CreateElement(preview, false).ToString();

            _Scene._PathTracer = pt;

            _CameraDirty = false;

            return XML;
        }
        
        public void InitialiseTestScene()
        {
            Matrix3 identity = new Matrix3();
            identity.MakeIdentity();

            _Scene.AddRenderObject(_BackgroundScript);
            _Scene.AddRenderObject(_SceneScript);
            _Scene.AddRenderObject(_LightingScript);
        }

        string _SettingsLocation;
        AppSettings _AppSettings;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _SettingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Settings.xml";
            _AppSettings = AppSettings.Load(_SettingsLocation);
            _WootracerOptions = _AppSettings._WootracerOptions;

            // initialise post process settings
            _PostProcess = new PostProcess();

            // starting camera settings
            _WootracerOptions = new WootracerOptions(); 
            InitialiseCamera();

            // initialise the scene
            InitialiseScene();

            // initialise the script objects
            InitialiseScript();

            InitialiseTestScene();

            ShaderScript.ReadDistanceSchema();

            BuildFractalList();

            BuildOptionsList();

            BuildColourList();

//            FractalSettings fractalSettings = LoadFractal("scratch");
//            _FractalIterations = fractalSettings._FractalIterations;
  //          _FractalColours = fractalSettings._FractalColours;
    //        _RenderOptions = fractalSettings._RenderOptions;

        }

        private List<WooFractalIteration> _FractalIterations = new List<WooFractalIteration>();

        public void AddCuboid()
        {
            _FractalIterations.Add(new KIFSIteration(EFractalType.Cube, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2.1, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddMenger()
        {
            _FractalIterations.Add(new KIFSIteration(EFractalType.Menger, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 3.0, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddTetra()
        {
            _FractalIterations.Add(new KIFSIteration(EFractalType.Tetra, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2.0, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddMandelbulb()
        {
            _FractalIterations.Add(new MandelbulbIteration(new Vector3(0, 0, 0), 8, 1));
            BuildFractalList();
        }

        public void AddMandelbox()
        {
            _FractalIterations.Add(new MandelboxIteration(new Vector3(0, 0, 0), 2.1, 1));
            BuildFractalList();
        }

        private void BuildFractalList()
        {
            stackPanel1.Children.Clear();

            for (int i = 0; i < _FractalIterations.Count(); i++)
            {
                stackPanel1.Children.Add(_FractalIterations[i].GetControl());
            }

            stackPanel1.Children.Add(new AddFractal());
        }

        RenderOptions _RenderOptions = new RenderOptions();
        WootracerOptions _WootracerOptions = new WootracerOptions();

        private void BuildOptionsList()
        {
            stackPanel2.Children.Clear();

            stackPanel2.Children.Add(_RenderOptions.GetControl());
            stackPanel2.Children.Add(_WootracerOptions.GetControl());
        }

        FractalColours _FractalColours = new FractalColours();

        private void BuildColourList()
        {
            stackPanel3.Children.Clear();

            stackPanel3.Children.Add(_FractalColours.GetControl());
        }

        ImageRenderer _ImageRenderer;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Preview(true);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Compile();
        }

        private void Preview(bool preview)
        {
//            _ImageRenderer.Stop();
            _ImageRenderer.SetFixedExposure(true);//!_WootracerOptions._AutoExposure);
            _ImageRenderer.SetExposureValue((float)_WootracerOptions._Exposure);
            if (!preview)
            {
                _ImageRenderer = new ImageRenderer(image1, BuildXML(false), 480, 270, false);   
                _ImageRenderer.Render();
                _ImageRenderer = new ImageRenderer(image1, BuildXML(false), (int)((float)480 * _Scale), (int)((float)270 * _Scale), false);
            }
            else
            {
                _ImageRenderer.Render();
            }
            if (_WootracerOptions._AutoExposure)
            {
//                _WootracerOptions._Exposure = _ImageRenderer._MaxValue;
            }
        }

        private bool CompileSingle(ref WooScript script)
        {
            string log = "";
            string error = "";
            bool success = script.Parse(ref log, ref error);
            if (!success)
            {
                MessageBox.Show(error);
                return false;
            }
            return true;
        }

        bool _ComplexMaterials = false;

        public void Compile()
        {
//            _BackgroundScript._Program = backgroundDesc.Text;
//            _SceneScript._Program = sceneDesc.Text;
//            _LightingScript._Program = lightingDesc.Text;

            SaveStatus();

            _BackgroundScript._Program = _RenderOptions._Backgrounds[_RenderOptions._Background]._Description;
            _LightingScript._Program = _RenderOptions._LightingEnvironments[_RenderOptions._Lighting]._Description;

            _SceneScript._Program = "rule main {\r\n";
            _SceneScript._Program += "fractal_reset()\r\n";
            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                _SceneScript._Program += _FractalIterations[i].GetFractalString();
            }
            _SceneScript._Program += "distanceminimum=" + Math.Pow(10, -_RenderOptions._DistanceMinimum).ToString("0.#######") + "\r\n";
            _SceneScript._Program += "distanceiterations=" + _RenderOptions._DistanceIterations + "\r\n";
            _SceneScript._Program += "distanceextents = vec(" + _RenderOptions._DistanceExtents + "," + _RenderOptions._DistanceExtents + "," + _RenderOptions._DistanceExtents + ")\r\n";
            _SceneScript._Program += "fractaliterationcount = " + _RenderOptions._FractalIterationCount + "\r\n";
            _SceneScript._Program += "fractalcolouriterationcount = " + _RenderOptions._ColourIterationCount + "\r\n";
            _SceneScript._Program += "demode = " + _RenderOptions._DEMode + "\r\n";
            _SceneScript._Program += "stepsize = " + _RenderOptions._StepSize + "\r\n";
            _SceneScript._Program += "materialfunction(fracColours)\r\n";
            _SceneScript._Program += "fractal\r\n";
            _SceneScript._Program += "}\r\n";

            _SceneScript._Program += _FractalColours.GenerateScript(_ComplexMaterials);

            bool success = CompileSingle(ref _BackgroundScript);
            success &= CompileSingle(ref _SceneScript);
            success &= CompileSingle(ref _LightingScript);
            
//            if (success)
            {
                TriggerPreview();
            }

            _Dirty = false;
        }

        private void TriggerPreview()
        {
//            _Scale = getPreviewResolution();
            _Scene._Shadows = getShadowsEnabled();
            _SimpleLighting = getSimpleLighting();
            _Scale = 1.0f;

            // reset the renderer
            if (_ImageRenderer != null)
            {
                _ImageRenderer.TransferLatest(false);
                _ImageRenderer.Stop();
            }

            _ImageRenderer = new ImageRenderer(image1, BuildXML(true), (int)image1.Width, (int)image1.Height, true);
            _ImageRenderer.SetFixedExposure(true);//!_WootracerOptions._AutoExposure);
            _ImageRenderer.SetExposureValue((float)_WootracerOptions._Exposure);
            _ImageRenderer.Render();
        }

        Vector3 _Velocity = new Vector3(0, 0, 0);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Button source = e.Source as Button;
            if (source != null)
            {
                double Multiplier = 0.1;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    Multiplier = 0.01;
                }

                if (e.Key == Key.Left)
                {
                    _Velocity.x -= Multiplier * _WootracerOptions._FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    _Velocity.x += Multiplier * _WootracerOptions._FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z += Multiplier * _WootracerOptions._FocusDistance;
                        e.Handled = true;
                    }
                    else
                    {
                        _Velocity.y += Multiplier * _WootracerOptions._FocusDistance;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Down)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z -= Multiplier * _WootracerOptions._FocusDistance;
                        e.Handled = true;
                    }
                    else
                    {
                        _Velocity.y -= Multiplier * _WootracerOptions._FocusDistance;
                        e.Handled = true;
                    }
                }

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        bool _Dirty = true;
        bool _CameraDirty = false;
        DispatcherTimer _Timer;

        public void SetDirty()
        {
            _Dirty = true;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Vector3 to = _Camera._Target - _Camera._Position;
            to.Normalise();
            Vector3 up = new Vector3(0, 1, 0);
            Vector3 right = up.Cross(to);
            right.Normalise();

            Vector3 newup = to.Cross(right);
            newup.Normalise();

            right.Mul(_Velocity.x);
            to.Mul(_Velocity.z);
            newup.Mul(_Velocity.y);

//            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(to);
                _Camera._Position.Add(newup);
                _Camera._Target.Add(right);
                _Camera._Target.Add(to);
                _Camera._Target.Add(newup);
            }
  /*          else
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(newup);
            }
            */
            _WootracerOptions.UpdateGUI();
            _WootracerOptions._FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();
            _Camera._FOV = _WootracerOptions._FieldOfView;
            _Camera._Spherical = _WootracerOptions._Spherical;
            _Camera._Stereographic = _WootracerOptions._Stereographic;

            _Velocity *= 0.6;

            _ImageRenderer._RampValue = 1;// _ImageRenderer._MaxValue;
            _ImageRenderer.TransferLatest(false);

            if (_Dirty || _CameraDirty || _Velocity.MagnitudeSquared() > 0.0001 && _ImageRenderer != null)
            {
                _ImageRenderer.Stop();
                if (_Dirty)
                {
                    Compile();
                }
                else
                {
                    Compile();
//                    _ImageRenderer.UpdateCamera(_Camera.CreateElement().ToString());
                }
                _ImageRenderer.Render();
            }

//            if (_Velocity.MagnitudeSquared() < 0.0001)
  //              _Timer.Stop();
        }

        private void image1_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("GotFocus");
        }

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDepth(ref float depth, int x, int y);

        bool _ImageDrag = false;
        double _Pitch;
        double _Yaw;
        Point _DragStart;
        float _Scale = 0.25f;
        bool _SimpleLighting = false;

        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            Debug.WriteLine("LMBDown");
            float depth = 0;
            Point mousePos = e.GetPosition(image1);

            GetDepth(ref depth, (int)(_Scale * mousePos.X), (int)(_Scale * mousePos.Y));
            if (depth>0)
                _WootracerOptions._FocusDistance = (double)depth;

            Vector3 dir = (_Camera._Target - _Camera._Position);
            dir.Normalise();

            _Pitch = Math.Asin(dir.y);
            dir.y = 0;
            dir.Normalise();
            _Yaw = Math.Asin(dir.x);
            if (dir.z < 0)
                _Yaw = (Math.PI) - _Yaw;

            _DragStart = e.GetPosition(this);
            Debug.WriteLine("dragstart (x = " + _DragStart.X + ", y=" + _DragStart.Y + ")");

            dir = (_Camera._Target - _Camera._Position);
            dir.Normalise();
            dir *= _WootracerOptions._FocusDistance;
            _Camera._Target = _Camera._Position + dir;
            _WootracerOptions.UpdateGUI();

            _ImageDrag = true;

        //    CaptureMouse();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ImageDrag)
            {
                Point dragPos = e.GetPosition(this);
                dragPos.X -= _DragStart.X;
                dragPos.Y -= _DragStart.Y;
                Debug.WriteLine("dragpos (x = " + dragPos.X + ", y=" + dragPos.Y + ")");

                double newyaw = _Yaw - 0.01 * dragPos.X;
                double newpitch = _Pitch + 0.01 * dragPos.Y;
                if (newpitch > Math.PI * 0.49f) newpitch = Math.PI * 0.49f;
                if (newpitch < -Math.PI * 0.49f) newpitch = -Math.PI * 0.49f;
                Vector3 dir = _Camera._Target - _Camera._Position;
                double length = dir.Magnitude();

                Vector3 newdir = new Vector3();
                newdir.y = Math.Sin(newpitch);
                newdir.x = Math.Cos(newpitch) * Math.Sin(newyaw);
                newdir.z = Math.Cos(newpitch) * Math.Cos(newyaw);

                double mag = newdir.Magnitude();
                newdir *= length;

                _Camera._Target = _Camera._Position + newdir;
                _WootracerOptions.UpdateGUI();
                _CameraDirty = true;

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            TriggerPreview();
        }

        public void StopPreview()
        {
            _Timer.Stop();
            _ImageRenderer.Stop();
            _ImageRenderer = null;
        }

        public void StartPreview()
        {
            Compile();
            _Timer.Start();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            SaveStatus(); 

            _Velocity = new Vector3(0, 0, 0);
            _Camera._FocusDepth = (float)_WootracerOptions._FocusDistance;
            _Camera._ApertureSize = (float)_WootracerOptions._ApertureSize;
            _Camera._FOV = (float)_WootracerOptions._FieldOfView;
            _Camera._Spherical = (float)_WootracerOptions._Spherical;
            _Camera._Stereographic = (float)_WootracerOptions._Stereographic;

            StopPreview();

            FinalRender ownedWindow = new FinalRender(ref _Scene, ref _Camera, ref _PostProcess);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();

            StartPreview();
        }

        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ButtonLMBUp");
            _ImageDrag = false;
            Mouse.Capture(null);

        }

        private void saveBackground_Click(object sender, RoutedEventArgs e)
        {
            _BackgroundScript.SaveUserInput("background");
        }

        private void loadBackground_Click(object sender, RoutedEventArgs e)
        {
            _BackgroundScript.LoadUserInput("background");
            //backgroundDesc.Text = _BackgroundScript._Program;
        }

        private void saveScene_Click(object sender, RoutedEventArgs e)
        {
            _SceneScript.SaveUserInput("scene");
        }

        private void loadScene_Click(object sender, RoutedEventArgs e)
        {
            _SceneScript.LoadUserInput("scene");
//            sceneDesc.Text = _SceneScript._Program;
        }

        private void saveLighting_Click(object sender, RoutedEventArgs e)
        {
            _LightingScript.SaveUserInput("lighting");
        }

        private void loadLighting_Click(object sender, RoutedEventArgs e)
        {
            _LightingScript.LoadUserInput("lighting");
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            string XML = BuildXML(false);

            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\XML";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "Scene XML (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                // Save document
                string filename = saveFileDialog1.FileName;
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(XML);
                sw.Close();
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();

            helpWindow.Owner = Window.GetWindow(this);
            helpWindow.Show();            
        }

        private bool getShadowsEnabled()
        {
            return _WootracerOptions._ShadowsEnabled;
        }

        private bool getSimpleLighting()
        {
            return true;// (simpleLighting.IsChecked.HasValue && simpleLighting.IsChecked.Value);
        }

        private float getPreviewResolution()
        {
/*            if (radioButton1.IsChecked.HasValue && radioButton1.IsChecked.Value)
            {
                return 1.0f;
            }
            else if (radioButton2.IsChecked.HasValue && radioButton2.IsChecked.Value)
            {
                return 0.5f;
            }
            else if (radioButton3.IsChecked.HasValue && radioButton3.IsChecked.Value)
            {
                return 0.33333f;
            }
            else if (radioButton4.IsChecked.HasValue && radioButton4.IsChecked.Value)
            {
                return 0.1f;
            }
 */           return 0.33f;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
                TriggerPreview();
        }

        private void SaveStatus()
        {
            _BackgroundScript.Save("background", "scratch");
            _SceneScript.Save("scene", "scratch");
            _LightingScript.Save("lighting", "scratch");
            _AppSettings.Save(_SettingsLocation, _Camera, _WootracerOptions);
            SaveFractal("scratch");
        }

        public class FractalSettings
        {
            public FractalSettings()
            {
            }

            public void Set(RenderOptions renderOptions, FractalColours fractalColours, List<WooFractalIteration> fractalIterations)
            {
                _RenderOptions = renderOptions;
                _FractalColours = fractalColours;
                _FractalIterations = fractalIterations;
            }
            public string BuildXML()
            {
                XElement parent = new XElement("FRACTAL");
                _RenderOptions.CreateElement(parent);
                _FractalColours.CreateElement(parent);
                for (int i=0; i<_FractalIterations.Count; i++)
                    _FractalIterations[i].CreateElement(parent);
                return parent.ToString();
            }
            public void Load(string xml)
            {
                _FractalIterations = new List<WooFractalIteration>();
                using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
                {
                    while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "RENDEROPTIONS")
                        {
                            _RenderOptions = new RenderOptions();
                            _RenderOptions.LoadXML(reader);
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "FRACTALCOLOURS")
                        {
                            _FractalColours = new FractalColours();
                            _FractalColours.LoadXML(reader);
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "KIFSFRACTAL")
                        {
                            KIFSIteration fractalIteration = new KIFSIteration();
                            fractalIteration.LoadXML(reader);
                            _FractalIterations.Add(fractalIteration);
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "BULBFRACTAL")
                        {
                            MandelbulbIteration fractalIteration = new MandelbulbIteration();
                            fractalIteration.LoadXML(reader);
                            _FractalIterations.Add(fractalIteration);
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "BOXFRACTAL")
                        {
                            MandelboxIteration fractalIteration = new MandelboxIteration();
                            fractalIteration.LoadXML(reader);
                            _FractalIterations.Add(fractalIteration);
                        }
                    }
                }
            }
            public RenderOptions _RenderOptions;
            public FractalColours _FractalColours;
            public List<WooFractalIteration> _FractalIterations;
        }
        private FractalSettings LoadFractal(string name)
        {
            FractalSettings fractalSettings = new FractalSettings();
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scripts\\fractal\\" + name + ".wfd";
            if (System.IO.File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                string fractal = sr.ReadToEnd();
                fractalSettings.Load(fractal);
                sr.Close();
                _RenderOptions = fractalSettings._RenderOptions;
                _FractalColours = fractalSettings._FractalColours;
                _FractalIterations = fractalSettings._FractalIterations;
            }
            return fractalSettings;
        }
        private void SaveFractal(string name)
        {
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scripts";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            store = store + "\\" + "fractal";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            string filename = store + "\\" + name + ".wfd";

            using (StreamWriter sw = new StreamWriter(filename))
            {
                try
                {
                    FractalSettings fractalSettings = new FractalSettings();
                    fractalSettings.Set(_RenderOptions, _FractalColours, _FractalIterations);
                    sw.Write(fractalSettings.BuildXML());
                    sw.Close();
                }
                catch (Exception /*e*/)
                {
                    // lets not get overexcited...
                }
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveStatus();
        }

        private void RefreshRender(object sender, TextChangedEventArgs e)
        {
            if (_Timer != null && !_Timer.IsEnabled)
                _Timer.Start();
        }

        private void RefreshRenderRouted(object sender, RoutedEventArgs e)
        {
            if (_Timer!=null && !_Timer.IsEnabled)
                _Timer.Start();
            if (this.IsLoaded)
                TriggerPreview();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image1.Height = imagebutton.ActualHeight;
            image1.Width = imagebutton.ActualWidth;

            Compile();

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(17);
            _Timer.Tick += this.timer_Tick;
            _Timer.Start();
        }

        private void imagebutton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            image1.Height = imagebutton.ActualHeight;
            image1.Width = imagebutton.ActualWidth;

            if (imagebutton.IsLoaded)
               TriggerPreview();
        }

        public void RemoveIteration(WooFractalIteration iteration)
        {
            _FractalIterations.Remove(iteration);

            Compile();

            BuildFractalList();
        }

        public void PromoteIteration(WooFractalIteration iteration)
        {
            int index = -1;
            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                if (_FractalIterations[i] == iteration)
                {
                    index = i;
                }
            }
            if (index != -1 && index>0)
            {
                _FractalIterations[index] = _FractalIterations[index - 1];
                _FractalIterations[index - 1] = iteration;
            }

            Compile();

            BuildFractalList();
        }

        public void DemoteIteration(WooFractalIteration iteration)
        {
            int index = -1;
            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                if (_FractalIterations[i] == iteration)
                {
                    index = i;
                }
            }
            if (index != -1 && index < _FractalIterations.Count-1)
            {
                _FractalIterations[index] = _FractalIterations[index + 1];
                _FractalIterations[index + 1] = iteration;
            }

            Compile();

            BuildFractalList();
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            FractalSettings fractalSettings = new FractalSettings();
            
            // load fractal
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Scripts\\fractal";

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "fractal"; // Default file name
            dlg.DefaultExt = ".wfd"; // Default file extension
            dlg.Filter = "Fractal Descriptor|*.wfd"; // Filter files by extension
            dlg.InitialDirectory = store;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // get name of file
            if (result == true)
            {
                string filename = dlg.FileName;
                StreamReader sr = new StreamReader(filename);
                string fractal = sr.ReadToEnd();
                fractalSettings.Load(fractal);
                sr.Close();
                _RenderOptions = fractalSettings._RenderOptions;
                _FractalColours = fractalSettings._FractalColours;
                _FractalIterations = fractalSettings._FractalIterations;
            }
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            // save fractal
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scripts";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            store = store + "\\" + "fractal";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "Fractal Descriptor (*.wfd)|*.wfd";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                // Save document
                string filename = saveFileDialog1.FileName;
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    try
                    {
                        FractalSettings fractalSettings = new FractalSettings();
                        fractalSettings.Set(_RenderOptions, _FractalColours, _FractalIterations);
                        sw.Write(fractalSettings.BuildXML());
                        sw.Close();
                    }
                    catch (Exception /*e*/)
                    {
                        // lets not get overexcited...
                    }
                }
            }
        }
    }
}
