using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using Cairo;
using static Program;
using static Constants;
using Structures;
namespace StartupScreen {
    public class Menu : Window {
        public VBox containerbox {get; private set;}
        public VBox radiobox {get; private set;}
        public ScrolledWindow systemscrollbox {get; private set;}
        public VBox systembox {get; private set;}
        public VBox donebox {get; private set;}
        public Scale TimestepScale {get; private set;}
        public Scale RScale {get; private set;}
        public Scale LineScale {get; private set;}
        public RadioButton radio0;
        public RadioButton radio1;
        public RadioButton radio2;
        public RadioButton radio3;
        public RadioButton radio4;
        private ComboBoxText bCombo;
        public Button loadButton;
        public Entry filename;
        public static List<Structures.Body> std_bodies = Examples.solar_system_bodies;
        internal static List<BodyBox> new_bodies = new List<BodyBox>();
        public SaveData temp_savedata = null;
        public List<bool> centers = new List<bool>();
        
        public Menu(Gtk.WindowType s = Gtk.WindowType.Toplevel) : base(s) { // weird inheritancy stuff, don't change
            this.SetDefaultSize(300,400);
			this.DeleteEvent += delegate { Application.Quit (); };
            containerbox = new VBox(homogeneous: false, spacing: 3);
            radiobox = new VBox(homogeneous: false, spacing: 3);
            systemscrollbox = new ScrolledWindow();
            systembox = new VBox(homogeneous: false, spacing: 3);
            donebox = new VBox(homogeneous: false, spacing: 3);

            
            var l1 = new Label("Mechanics Timestep");
            var l2 = new Label("Planetary Radii Multiplier");
            var l3 = new Label("Orbit Trail Length");
            TimestepScale = new Scale(Orientation.Horizontal, 0.1,1000,0.1);
            TimestepScale.Value = 50;
            RScale = new Scale(Orientation.Horizontal, 1, 1000, 1);
            RScale.Value = 100;
            LineScale = new Scale(Orientation.Horizontal, 50, 1000, 1);
            LineScale.Value = 100;


            radio0 = new RadioButton("Standard Solar System");
            radio1 = new RadioButton(radio0,"Standard Solar System with Black Hole");
            radio2 = new RadioButton(radio1,"Inner Solar System");
            radio3 = new RadioButton(radio2,"Binary Star System");
            radio4 = new RadioButton(radio3,"Custom Solar System");
            var sysL1 = new Label("Name");
            var sysL2 = new Label("Parent");
            var sysL3 = new Label("Log Mass (10^22kg)");
            var sysL4 = new Label("Radius (km)");
            var sysL5 = new Label("Semi-major axis (AU)");
            var sysL6 = new Label("Eccentricity");
            var sysL7 = new Label("Inclination (deg)");
            var sysL8 = new Label("Longitude of Ascending Node (deg)");
            var sysL9 = new Label("Argument of Periapsis (deg)");
            var sysL10= new Label("True Anomaly (deg)");
            var lBox = new HBox();
            lBox.PackStart(sysL1,true,true,3);
            lBox.PackStart(sysL2,true,true,3);
            lBox.PackStart(sysL3,true,true,3);
            lBox.PackStart(sysL4,true,true,3);
            lBox.PackStart(sysL5,true,true,3);
            lBox.PackStart(sysL6,true,true,3);
            lBox.PackStart(sysL7,true,true,3);
            lBox.PackStart(sysL8,true,true,3);
            lBox.PackStart(sysL9,true,true,3);
            lBox.PackStart(sysL10,true,true,3);

            var addBox = new HBox();

            var addButton = new Button("Add");
            addButton.Clicked += new EventHandler(OnAddClick);
            filename = new Entry();
            var saveButton = new Button("Save");
            saveButton.Clicked += new EventHandler(OnSaveClick);
            loadButton = new Button("Load");
            loadButton.Clicked += new EventHandler(OnLoadClick);
            bCombo = new ComboBoxText();
            bCombo.AppendText("Custom");
            foreach (Body b in Menu.std_bodies) {
                bCombo.AppendText(b.name);
            } 
            bCombo.Active = 0; // Default to Custom body
            addBox.PackStart(bCombo, true, false, 3);
            addBox.PackStart(addButton, true, false, 3);
            addBox.PackStart(filename, true, false, 3);
            addBox.PackStart(saveButton, true, false, 3);
            addBox.PackStart(loadButton, true, false, 3);
            var doneButton = new Button("Done");
            doneButton.Clicked += new EventHandler (OnDoneClick);
            
            var optionsbox = new HBox(homogeneous: false, spacing: 3);
            var optionbox1 = new VBox(homogeneous: false, spacing: 3);
            var optionbox2 = new VBox(homogeneous: false, spacing: 3);
            var optionbox3 = new VBox(homogeneous: false, spacing: 3);
            optionbox1.PackStart(l1, true, true, 3);
            optionbox1.PackStart(TimestepScale, true, true, 3);
            optionbox2.PackStart(l2, true, true, 3);
            optionbox2.PackStart(RScale, true, true, 3);
            optionbox3.PackStart(l3, true, true, 3);
            optionbox3.PackStart(LineScale, true, true, 3);
            optionsbox.PackStart(optionbox1, true, true, 3);
            optionsbox.PackStart(optionbox2, true, true, 3);
            optionsbox.PackStart(optionbox3, true, true, 3);

            radiobox.PackStart(optionsbox, false, false, 3);
            radiobox.PackStart(TimestepScale, false, false, 3);
            radiobox.PackStart(radio0, false, false, 3);
            radiobox.PackStart(radio1, false, false, 3);
            radiobox.PackStart(radio2, false, false, 3);
            radiobox.PackStart(radio3, false, false, 3);
            radiobox.PackStart(radio4, false, false, 3);
            radiobox.PackStart(addButton, false, false, 3);
            radiobox.PackStart(addBox, false, false, 3);
            radiobox.PackStart(lBox, false, false, 3);
            systemscrollbox.Add(systembox);
            donebox.PackStart(doneButton, false, false, 3);

            containerbox.PackStart(radiobox, false, false, 3);
            containerbox.PackStart(systemscrollbox, true, true, 3);
            containerbox.PackStart(donebox, false, false, 3);
            this.Add(containerbox);
            this.ShowAll();
        }
        private void OnDoneClick(object obj, EventArgs args) {
            Program.RadioOptions = new List<Boolean>(){radio0.Active,radio1.Active,radio2.Active,radio3.Active,radio4.Active};
            Program.CustomBodies.Clear();
            Program.CustomCenters.Clear();
            centers.Clear();
            foreach (BodyBox b in new_bodies) {
                b.Set();
                Program.CustomBodies.Add(b.body);
                centers.Add(b.CenterButton.Active);

            }
            Program.CustomCenters = centers;
            Program.radius_multiplier = RScale.Value;
            Program.line_max = (int)LineScale.Value;
            Program.STEP = TimestepScale.Value;
            Program.Start();
            this.Destroy();
        }
        private void OnAddClick(object obj, EventArgs args) {

            var bodyBox = new BodyBox();
            String bString = bCombo.ActiveText;
            if (bString != "Custom") {
                var body = Examples.solar_system.bodies.First(b => b.name == bString);
                if (!new_bodies.Exists(b => b.name.Text == body.parent.name)) {
                    body = std_bodies.First(b => b.name == bString);
                }
                bodyBox.body = body;
                bodyBox.ReverseSet();
            }
            
            bodyBox.name.Text = bCombo.ActiveText;
            systembox.PackStart(bodyBox, true, true, 3);
            this.radio4.Active = true;
            new_bodies.Add(bodyBox);
            foreach (BodyBox b in new_bodies) {
                b.ResetParents();
            }
            this.ShowAll();
        }
        private void OnSaveClick(object obj, EventArgs args) {
            System.Xml.Serialization.XmlSerializer writer =   
                new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
            if (File.Exists(Environment.CurrentDirectory + "//" + filename.Text + ".xml")) {
                File.Delete(Environment.CurrentDirectory + "//" + filename.Text + ".xml");
            }
            FileStream file = File.Create(
                Environment.CurrentDirectory + "//" + filename.Text + ".xml");
            var bodies = new List<Body>();
            if (centers == null) centers = new List<bool>();
            centers.Clear();
            foreach (BodyBox b in new_bodies) {
                b.Set();
                bodies.Add(b.body);
                centers.Add(b.CenterButton.Active);
            }
            var data = new SaveData() {
                bodies = bodies,
                STEP = TimestepScale.Value,
                centers = centers,
                radius_multiplier = RScale.Value,
                line_max = LineScale.Value,
            };
            writer.Serialize(file, data);
            file.Close();
        }
        private void OnLoadClick(object obj, EventArgs args) {
            System.Xml.Serialization.XmlSerializer reader =   
                new System.Xml.Serialization.XmlSerializer(typeof(SaveData));
            try {
                SaveData data;
                if (temp_savedata != null) {
                    data = temp_savedata;
                } else {
                    var file = new StreamReader(Environment.CurrentDirectory + "//" + filename.Text + ".xml");
                    data = (SaveData)reader.Deserialize(file);
                }
                RScale.Value = data.radius_multiplier;
                LineScale.Value = data.line_max;
                TimestepScale.Value = data.STEP;
                new_bodies.Clear();
                foreach (Widget w in systembox.Children) {
                    if (w is BodyBox) systembox.Remove (w);
                }

                for (int i = 0; i < data.bodies.Count; i++) {
                    var bbox = new BodyBox() {
                        body = data.bodies[i],
                    };
                    bbox.CenterButton.Active = data.centers[i];
                    bbox.ReverseSet();
                    new_bodies.Add(bbox);
                    systembox.PackStart(bbox, true, true, 3);
                }
                foreach (BodyBox b in new_bodies) {
                    b.ResetParents();
                    b.ReverseSet();
                }

                this.radio4.Active = true;
            } catch (IOException) { filename.Text = "Not Found"; }
            catch (InvalidOperationException e) { 
                Console.WriteLine(e.Message);
                filename.Text = "Invalid File";}
            this.ShowAll();
        }
    }
    class BodyBox : HBox {
        public Structures.Body body {get; set;}
        public Entry name;
        public ComboBoxText parent = new ComboBoxText();
        public Scale MassScale;
        public Scale RadiusScale;
        public Scale SMAScale;
        public Scale EScale;
        public Scale IncScale;
        public Scale ANLScale;
        public Scale PAScale;
        public Scale TAScale;
        public Scale RScale;
        public Scale GScale;
        public Scale BScale;
        public CheckButton CenterButton;
        public Button DeleteButton;
        public BodyBox() {
            body = new Structures.Body();
            name = new Entry();
            name.IsEditable = true;
            ResetParents();
            MassScale = new Scale(Orientation.Vertical, 0.1,50,0.01);
            RadiusScale = new Scale(Orientation.Vertical, 0.1,1000000,0.1);
            SMAScale = new Scale(Orientation.Vertical, 0.1,50,0.01);
            EScale = new Scale(Orientation.Vertical, 0,10,0.001);
            IncScale = new Scale(Orientation.Vertical, 0,90,0.01);
            ANLScale = new Scale(Orientation.Vertical, 0,360,0.01);
            PAScale = new Scale(Orientation.Vertical, 0,360,0.01);
            TAScale = new Scale(Orientation.Vertical, 0,360,0.01);
            RScale = new Scale(Orientation.Horizontal, 0, 1, 0.01);
            GScale = new Scale(Orientation.Horizontal, 0, 1, 0.01);
            BScale = new Scale(Orientation.Horizontal, 0, 1, 0.01);
            CenterButton = new CheckButton("Focusable");
            DeleteButton = new Button("Delete");


            parent.Changed += new EventHandler(OnParentChange);
            MassScale.Inverted = true;
            RadiusScale.Inverted = true;
            SMAScale.Inverted = true;
            EScale.Inverted = true;
            IncScale.Inverted = true;
            ANLScale.Inverted = true;
            PAScale.Inverted = true;
            TAScale.Inverted = true;
            
            this.PackStart(name, true, true, 3);
            this.PackStart(parent, false, false, 3);
            this.PackStart(MassScale, true, true, 3);
            this.PackStart(RadiusScale, true, true, 3);
            this.PackStart(SMAScale, true, true, 3);
            this.PackStart(EScale, true, true, 3);
            this.PackStart(IncScale, true, true, 3);
            this.PackStart(ANLScale, true, true, 3);
            this.PackStart(PAScale, true, true, 3);
            this.PackStart(TAScale, true, true, 3);

            var colorbox = new VBox(homogeneous: false, spacing: 3);
            colorbox.PackStart(RScale, true, true, 3);
            colorbox.PackStart(GScale, true, true, 3);
            colorbox.PackStart(BScale, true, true, 3);
            
            this.PackStart(colorbox, true, true, 3);
            var optionsbox = new VBox(homogeneous: false, spacing: 3);
            optionsbox.PackStart(CenterButton, true, true, 3);
            optionsbox.PackStart(DeleteButton, true, true, 3);
            this.PackStart(optionsbox, true, true, 3);

        }
        private void OnParentChange(object obj, EventArgs args) {
            try {
                var parentBody = Menu.new_bodies.FirstOrDefault(b => b.body.name == parent.ActiveText).body;
                double hillrad = parentBody.HillRadius()/AU;
                this.SMAScale.Digits = Math.Max(0,8);//3-(int)Math.Log(hillrad/100000));
                this.SMAScale.SetIncrements(Math.Pow(10,-this.SMAScale.Digits),hillrad/100000);
                this.SMAScale.SetRange(Math.Pow(10,-this.SMAScale.Digits),hillrad);
            } catch (NullReferenceException) {} // no parent, don't set values
        }
        public void Set() {
            if (parent.ActiveText != this.name.Text && parent.Active != -1) {
                var elements = new Structures.OrbitalElements() {
                    semimajoraxis = SMAScale.Value*AU,
                    eccentricity = EScale.Value,
                    inclination = IncScale.Value*deg,
                    ascendingNodeLongitude = ANLScale.Value*deg,
                    periapsisArgument = PAScale.Value*deg,
                    trueAnomaly = TAScale.Value*deg
                };
                body = new Structures.Body(Menu.new_bodies.FirstOrDefault(b => b.body.name == parent.ActiveText).body,elements);
            }
            body.name = this.name.Text;
            body.stdGrav = Math.Pow(Math.E,MassScale.Value)*Constants.G*1e22;
            body.radius = RadiusScale.Value*1e3;
            body.reflectivity = new Vector3(RScale.Value, GScale.Value, BScale.Value);
        }
        public void ReverseSet() {
            try {
                parent.Active  = Menu.new_bodies.FindIndex(b => b.name.Text == body.parent.name);
                var elements   = new OrbitalElements(body.position-body.parent.position,body.velocity-body.parent.velocity,body.parent.stdGrav);
                SMAScale.Value = elements.semimajoraxis/AU;
                EScale.Value   = elements.eccentricity;
                IncScale.Value = elements.inclination/deg;
                ANLScale.Value = elements.ascendingNodeLongitude/deg;
                PAScale.Value  = elements.periapsisArgument/deg;
                TAScale.Value  = elements.trueAnomaly/deg;
            } catch (NullReferenceException) {} // if body has no parent
            name.Text = body.name;
            MassScale.Value = Math.Log((body.stdGrav/Constants.G)/1e22);
            RadiusScale.Value = body.radius/1e3;
            RScale.Value = body.reflectivity.x;
            GScale.Value = body.reflectivity.y;
            BScale.Value = body.reflectivity.z;
        }
        public void ResetParents() {
            parent.RemoveAll();
            foreach (BodyBox b in Menu.new_bodies) {
                parent.AppendText(b.name.Text);
            }
            try {
                parent.Active = Menu.new_bodies.FindIndex(b => b.name.Text == body.parent.name);
            } catch (NullReferenceException) {} // parent no longer exists

        }
    }
    [Serializable()]
    public class SaveData {
        public List<Body> bodies {get; set;}
        public List<bool> centers {get; set;}
        public double STEP {get; set;}
        public double radius_multiplier {get; set;}
        public double line_max {get; set;}
    }
}