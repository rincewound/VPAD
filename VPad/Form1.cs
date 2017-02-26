using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpLib.Hid;

namespace VPad
{
    public partial class Form1 : Form
    {
        HIDDevice dev;
        GamepadMapping mapping;
        KeypadMapping kpMapping;

        public Form1()
        {
            InitializeComponent();
            mapping = Newtonsoft.Json.JsonConvert.DeserializeObject<GamepadMapping>(System.IO.File.ReadAllText("./Gamepads/Speedlink.json"));
            kpMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<KeypadMapping>(System.IO.File.ReadAllText("./Layouts/WingCommander4.json"));

            var pads = System.IO.Directory.EnumerateFiles(@".\Gamepads", "*.json");
            var mappings = System.IO.Directory.EnumerateFiles(@".\Layouts", "*.json");

            foreach(var k in mappings)
                this.lstKeys.Items.Add(k);
            foreach(var p in pads)
                this.lstPads.Items.Add(p);
        }

        private void lstKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstKeys.SelectedIndex == -1)
                return;
            kpMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<KeypadMapping>(System.IO.File.ReadAllText(lstKeys.SelectedItem.ToString()));
        }

        private void lstPads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPads.SelectedIndex == -1)
                return;
            mapping = Newtonsoft.Json.JsonConvert.DeserializeObject<GamepadMapping>(System.IO.File.ReadAllText(lstPads.SelectedItem.ToString()));
        }

        private void HandleHidEvent(object aSender, Event aHidEvent)
        {
            if(!aHidEvent.IsRepeat)
                this.Invoke(new Action(() => { this.Text = " " ; }));

            foreach (var button in mapping.buttonMapping)
            {
                if((aHidEvent.InputReport[button.inputReportIndex] & button.inputReportMask) != 0 
                    && !aHidEvent.IsRepeat && !aHidEvent.IsStray)
                {
                    System.Diagnostics.Trace.WriteLine(button.button);

                    // Find mapping for button:
                    var key = kpMapping.keymapping.FirstOrDefault(x => x.Key == button.button);
                    if (key.Key == null)
                        continue;

                    // We found a valid mapping -> emit keystrike!
                    Keyboard.Key k = new Keyboard.Key(key.Value);
                    k.PressForeground();

                    this.Invoke( new Action( () => { this.Text = " Key Active :" + key.Value.ToString(); }));

                }
            }

        }

        protected override void WndProc(ref Message m)
        {
            Boolean handled = false; m.Result = IntPtr.Zero;

            if(m.Msg == 255)
            {
                handled = true;
                dev.InjectEvent(ref m);
            }          

            if (handled) DefWndProc(ref m); else base.WndProc(ref m);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            dev = new HIDDevice(this.Handle, this.HandleHidEvent);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


    }
}
