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
        public VBox systembox {get; private set;}
        public VBox donebox {get; private set;}
        public Scale TimestepScale {get; private set;}
        public RadioButton radio0;
        public RadioButton radio1;
        public RadioButton radio2;
        public RadioButton radio3;
        public RadioButton radio4;
        private ComboBoxText bCombo;
        public Entry filename;
        public static List<Structures.Body> std_bodies = Examples.solar_system.bodies;
        private List<BodyBox> new_bodies = new List<BodyBox>();
        
        public Menu(Gtk.WindowType s = Gtk.WindowType.Toplevel) : base(s) { // weird inheritancy stuff, don't change
            this.SetDefaultSize(300,400);
			this.DeleteEvent += delegate { Application.Quit (); };
            containerbox = new VBox(homogeneous: false, spacing: 3);
            radiobox = new VBox(homogeneous: false, spacing: 3);
            systembox = new VBox(homogeneous: false, spacing: 3);
            donebox = new VBox(homogeneous: false, spacing: 3);

            
            var l1 = new Label("Mechanics Timestep:");
            TimestepScale = new Scale(Orientation.Horizontal, 0.1,1000,0.1);

            radio0 = new RadioButton("Standard Solar System");
            radio1 = new RadioButton(radio0,"Standard Solar System with Black Hole");
            radio2 = new RadioButton(radio1,"Inner Solar System");
            radio3 = new RadioButton(radio2,"Binary Star System");
            radio4 = new RadioButton(radio3,"Custom Solar System");
            var sysL1 = new Label("Name");
            var sysL2 = new Label("Parent");
            var sysL3 = new Label("Log Mass (10^22kg)");
            var sysL4 = new Label("Radius(10^6m)");
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
            var loadButton = new Button("Load");
            loadButton.Clicked += new EventHandler(OnLoadClick);
            bCombo = new ComboBoxText();
            foreach (Body b in Menu.std_bodies) {
                bCombo.AppendText(b.name);
            } 
            bCombo.AppendText("Custom");
            addBox.PackStart(bCombo, true, false, 3);
            addBox.PackStart(addButton, true, false, 3);
            addBox.PackStart(filename, true, false, 3);
            addBox.PackStart(saveButton, true, false, 3);
            addBox.PackStart(loadButton, true, false, 3);
            var doneButton = new Button("Done");
            doneButton.Clicked += new EventHandler (OnDoneClick);
            
            radiobox.PackStart(l1, false, false, 3);
            radiobox.PackStart(TimestepScale, false, false, 3);
            radiobox.PackStart(radio0, false, false, 3);
            radiobox.PackStart(radio1, false, false, 3);
            radiobox.PackStart(radio2, false, false, 3);
            radiobox.PackStart(radio3, false, false, 3);
            radiobox.PackStart(radio4, false, false, 3);
            radiobox.PackStart(addButton, false, false, 3);
            systembox.PackStart(addBox, false, false, 3);
            systembox.PackStart(lBox, false, false, 3);
            donebox.PackStart(doneButton, false, false, 3);

            containerbox.PackStart(radiobox, false, false, 3);
            containerbox.PackStart(systembox, true, true, 3);
            containerbox.PackStart(donebox, false, false, 3);
            this.Add(containerbox);
            //this.Add(systembox);
            //this.Add(donebox);

            this.ShowAll();
        }
        private void OnDoneClick(object obj, EventArgs args) {
            Program.RadioOptions = new List<Boolean>(){radio0.Active,radio1.Active,radio2.Active,radio3.Active,radio4.Active};
            foreach (BodyBox b in new_bodies) {
                
                b.Set();
                Program.CustomBodies.Add(b.body);
            }
            Program.STEP = TimestepScale.Value;
            Program.Start();
            this.Destroy();
        }
        private void OnAddClick(object obj, EventArgs args) {
            var bodyBox = new BodyBox();
            String bString = bCombo.ActiveText;
            if (bString != "Custom") {
                var body = std_bodies.First(b => b.name == bString);
                bodyBox.body = body;
                bodyBox.ReverseSet();
            }
            new_bodies.Add(bodyBox);
            systembox.PackStart(bodyBox, true, true, 3);
            this.ShowAll();
        }
        private void OnSaveClick(object obj, EventArgs args) {
            System.Xml.Serialization.XmlSerializer writer =   
                new System.Xml.Serialization.XmlSerializer(typeof(List<Body>));
            FileStream file = File.Create(
                Environment.CurrentDirectory + "//" + filename.Text + ".xml");
            var bodies = new List<Body>();
            foreach (BodyBox b in new_bodies) {
                b.Set();
                bodies.Add(b.body);
            }
            writer.Serialize(file, bodies);  
            file.Close();
        }
        private void OnLoadClick(object obj, EventArgs args) {
            System.Xml.Serialization.XmlSerializer reader =   
                new System.Xml.Serialization.XmlSerializer(typeof(List<Body>));
            try {
                StreamReader file = new StreamReader(
                    Environment.CurrentDirectory + "//" + filename.Text + ".xml");
                var bodies = (List<Body>)reader.Deserialize(file);
                new_bodies.Clear();
                foreach (Widget w in systembox.Children) {
                    if (w is BodyBox) systembox.Remove (w);
                }

                foreach (Body b in bodies) {
                    var bbox = new BodyBox() {
                        body = b
                    };
                    bbox.ReverseSet();
                    new_bodies.Add(bbox);
                    systembox.PackStart(bbox, true, true, 3);
                }
            } catch (IOException) { filename.Text = "Not Found"; }
            this.ShowAll();
        }
    }
    class BodyBox : HBox {
        public Structures.Body body {get; set;}
        public Entry name;
        public ComboBoxText parent;
        public Scale MassScale;
        public Scale RadiusScale;
        public Scale SMAScale;
        public Scale EScale;
        public Scale IncScale;
        public Scale ANLScale;
        public Scale PAScale;
        public Scale TAScale;
        public BodyBox() {
            body = new Structures.Body();
            name = new Entry();
            name.IsEditable = true;
            parent = new ComboBoxText();//.NewText();
            foreach (Body b in Menu.std_bodies) {
                parent.AppendText(b.name);
            }
            MassScale = new Scale(Orientation.Vertical, 0.1,50,0.01);
            RadiusScale = new Scale(Orientation.Vertical, 0.1,10000,0.1);
            MassScale.Inverted = true;
            RadiusScale.Inverted = true;
            SMAScale = new Scale(Orientation.Vertical, 0.1,50,0.01);
            EScale = new Scale(Orientation.Vertical, 0,10,0.001);
            IncScale = new Scale(Orientation.Vertical, 0,90,0.01);
            ANLScale = new Scale(Orientation.Vertical, 0,360,0.01);
            PAScale = new Scale(Orientation.Vertical, 0,360,0.01);
            TAScale = new Scale(Orientation.Vertical, 0,360,0.01);
            
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

            // use ComboBoxText with existing bodies in some sort of list
        }
        public void Set() {
            //if (!Menu.std_bodies.Contains(body)) {
                try { 
                    var elements = new Structures.OrbitalElements() {
                        semimajoraxis = SMAScale.Value*AU,
                        eccentricity = EScale.Value,
                        inclination = IncScale.Value*deg,
                        ascendingNodeLongitude =ANLScale.Value*deg,
                        periapsisArgument = PAScale.Value*deg,
                        trueAnomaly = TAScale.Value*deg
                    };
                    body = new Structures.Body(Menu.std_bodies.FirstOrDefault(b => b.name == parent.ActiveText),elements);
                } catch (InvalidOperationException) {} // no parent
                body.name = this.name.Text;
                body.stdGrav = Math.Pow(Math.E,MassScale.Value)*Constants.G*1e22;
                body.radius = RadiusScale.Value*1e6;
            //}
        }
        public void ReverseSet() {
            name.Text = body.name;
            try {
                parent.Active  = Menu.std_bodies.FindIndex(b => b.name == body.parent.name);
                var elements   = new OrbitalElements(body.position,body.velocity,body.parent.stdGrav);
                SMAScale.Value = elements.semimajoraxis/AU;
                EScale.Value   = elements.eccentricity;
                IncScale.Value = elements.inclination/deg;
                ANLScale.Value = elements.ascendingNodeLongitude/deg;
                PAScale.Value  = elements.periapsisArgument/deg;
                TAScale.Value  = elements.trueAnomaly/deg;
            } catch (NullReferenceException) {} // if body has no parent
            MassScale.Value = Math.Log((body.stdGrav/Constants.G)/1e22);
            RadiusScale.Value = body.radius/1e6;
        }
    }
}