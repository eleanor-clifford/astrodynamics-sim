using System;
using Gtk;
using Cairo;
using static Program;
namespace StartupScreen {
    public class Menu {
        public VBox box {get; private set;}
        public Scale TimestepScale {get; private set;}
        
        public Menu() {
            box = new VBox(homogeneous: false, spacing: 3);
            box.PackStart(new Label("Mechanics Timestep:"), false, false, 3);
            TimestepScale = new Scale(Orientation.Horizontal, 0.1,100,0.1);
            box.PackStart(TimestepScale,false, false, 3);
            var done = new Button("Done");
            box.PackStart(done, false, false, 3);
            done.Clicked += new EventHandler (OnClick);

        
        }
        private void OnClick(object obj, EventArgs args) {
            Program.STEP = TimestepScale.Value;
            Console.WriteLine(Program.STEP);
        }
    }
}