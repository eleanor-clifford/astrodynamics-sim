using System;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using Cairo;
using static Program;
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
            TimestepScale = new Scale(Orientation.Horizontal, 0.1,100,0.1);

            radio0 = new RadioButton("Standard Solar System");
            radio1 = new RadioButton(radio0,"Standard Solar System with Black Hole");
            radio2 = new RadioButton(radio1,"Inner Solar System");
            radio3 = new RadioButton(radio2,"Binary Star System");
            radio4 = new RadioButton(radio3,"Custom Solar System");
            var sysL1 = new Label("Name");
            var sysL2 = new Label("Parent");
            var sysL3 = new Label("Mass (10^22kg)");
            var sysL4 = new Label("Radius(10^6m)");
            var lBox = new HBox();
            lBox.PackStart(sysL1,true,true,3);
            lBox.PackStart(sysL2,true,true,3);
            lBox.PackStart(sysL3,true,true,3);
            lBox.PackStart(sysL4,true,true,3);

            var addBox = new HBox();

            var addButton = new Button("Add");
            addButton.Clicked += new EventHandler(OnAddClick);
            bCombo = new ComboBoxText();
            foreach (Body b in Menu.std_bodies) {
                bCombo.AppendText(b.name);
            } 
            bCombo.AppendText("Custom");
            addBox.PackStart(bCombo, true, false, 3);
            addBox.PackStart(addButton, true, false, 3);
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
            Console.WriteLine(new_bodies.Count);
            foreach (BodyBox b in new_bodies) {
                
                b.Set();
                Program.CustomBodies.Add(b.body);
            }
            Program.STEP = TimestepScale.Value; // must be set last since main program checks step value
            this.Destroy();
        }
        private void OnAddClick(object obj, EventArgs args) {
            var bodyBox = new BodyBox();
            String bString = bCombo.ActiveText;
            Console.WriteLine(bString);
            if (bString != "Custom") {
                var body = std_bodies.First(b => b.name == bString);
                Console.WriteLine(body.name);
                bodyBox.body = body;
                bodyBox.ReverseSet();
            }
            new_bodies.Add(bodyBox);
            systembox.PackStart(bodyBox, true, true, 3);
            this.ShowAll();
        }
    }
    class BodyBox : HBox {
        public Structures.Body body {get; set;}
        public Entry name;
        public ComboBoxText parent;
        public Scale MassScale;
        public Scale RadiusScale;
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
            this.PackStart(name, true, true, 3);
            this.PackStart(parent, false, false, 3);
            this.PackStart(MassScale, true, true, 3);
            this.PackStart(RadiusScale, true, true, 3);
            // use ComboBoxText with existing bodies in some sort of list
        }
        public void Set() {
            body.name = this.name.Text;
            body.stdGrav = Math.Pow(MassScale.Value,Math.E)*Constants.G*1e22;
            body.radius = RadiusScale.Value;
            try {
                body.parent = Menu.std_bodies.First(b => b.name == parent.ActiveText);
            } catch (InvalidOperationException) {} // if parent has not been set
        }
        public void ReverseSet() {
            name.Text = body.name;
            try {
                parent.Active = Menu.std_bodies.FindIndex(b => b.name == body.parent.name);
            } catch (NullReferenceException) {} // if body has no parent
            MassScale.Value = Math.Log((body.stdGrav/Constants.G)/1e22);
            RadiusScale.Value = body.radius/1e6;
        }
    }
}