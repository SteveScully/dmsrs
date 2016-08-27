using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace Raman
{
    public partial class frmTerminal : Form
    {
        public SerialPort port;
        RegExWrapper Re = new RegExWrapper() ;
        DateTime TimeLastNotificationNoHardware;// = new DateTime();


        const int DefaultComPortDelay =6;//11-27-2013 10;
        const int ReadByteDelay = 6;
        private int ComPortDelay = DefaultComPortDelay; //DefaultComPortDelay gets used a c
        public string FirmwareVersionReply ="";

               

        public frmTerminal( )
        {            
            InitializeComponent();
       //     checkBoxAutoScanAndOpen.Checked = Settings.Default._checkBoxAutoScanAndOpen;    
            port = new SerialPort();
            EnumerateComPorts(); //TODO move this to evkit jumper form
           // FillDeviceOpts();
           
        }


      string SavedPortName;
      public RStatus CheckPortOK()
      {
          foreach (string portName in SerialPort.GetPortNames())
          {
              if (portName == SavedPortName)
                  return RStatus.Good;

          }
          PortClose();  //Someone unpluged me!

          return RStatus.ComError;

      }
        
        public void EnumerateComPorts()
        {
            comboBoxComPorts.Items.Clear();
            foreach (string portName in SerialPort.GetPortNames())
            {
                comboBoxComPorts.Items.Add(portName);
            }
            if (comboBoxComPorts.Items.Count>0)    comboBoxComPorts.SelectedIndex = 0;
            CheckAndSetPorts();
        }

        private void CheckAndSetPorts()
        {
            if (checkBoxAutoScanAndOpen.Checked == false)
                return;
            string reply;
            foreach (var item in comboBoxComPorts.Items)
            {
                comboBoxComPorts.SelectedItem = item;
                reply = btnOpenPort_Click_Hook();
                if (Re.TstRegEx("Raman", reply) == true)
                    break;
                //Close port, ready for next
                port.Close();
                btnOpenPort.Text = "Open Port";
                panel1.Enabled = false;
            }

            GetString();
            SendString("i"); //print info
            System.Threading.Thread.Sleep(50);
            reply = GetString();
            int L= reply.IndexOf ("meridian");
            if (L == -1)
                LabelEVkitInfo.Text = "Error, did not recognize reply to 'VER' command";
            else
            {
                ShowPortOpenInfo(reply);
                reply = reply.Substring(L);               
                
            }

        }


        private void frmTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

   
        private void Form1_Load(object sender, EventArgs e)
        {
      //      checkBoxAutoScanAndOpen.Checked = Settings.Default._checkBoxAutoScanAndOpen;
          //  FillDeviceOpts();      
        }
        public RStatus GetDataValue(string strIn, out UInt32 DataValue)
        {
            string str;
            DataValue = 0;
            str = strIn.ToUpper();
            str = Re.Str0ExeRegEx("[\r\n]+[0-9A-F ]+[\r\n]+", str);
            str = Re.Str0ExeRegEx("[0-9A-F ]+", str);
            if (str.Length  < 0)
                return RStatus.ParseErr ;

            str = str.Trim();
            str = str.Split(' ')[0]; 
         //   str = str.TrimEnd('\r', '\n');
            str = Re.RepRegex("[^0-9A-F]+", str, "");
            if (str.Length == 0)
                return RStatus.ParseErr ;
            
            DataValue = Convert.ToUInt32(str, 16);
            return RStatus.Good;
        }
           
        public void ChangeBaudRate(string Rate)
        {
            SendString("SETBAUDRATE  1 " + Rate);
        }

        public RStatus readReg(string RdCmd,  out string DataValue )
        {
            string rv = ""; DataValue = "";
            if (port.IsOpen == false)
            {
                PortClose(true);                return RStatus.ComError ;
            }
            for (int i = 0; i < 2; i++)
            {
                GetString(); //flush
                System.Threading.Thread.Sleep(1);
                if (port.BytesToRead == 0)
                    break;
            }
             

            SendString(RdCmd );
            int emptyCnt = 0;
            for (int i = 0; i < 999; i++)
            {
                System.Threading.Thread.Sleep(ReadByteDelay);
                string subStr = GetString();                
                rv = rv + subStr.ToUpper()  ;

                if (subStr.Length == 0)
                    emptyCnt++;
                else
                    emptyCnt = 0;
                if (emptyCnt > 200)
                    break;

                if(Re.TstRegEx ("ERROR",rv) == true)
                    return RStatus.ComError ;
                int pos = rv.IndexOf (RdCmd);
                if (pos != -1) //clip of any leading info
                {
                    rv = rv.Substring(pos);                   
                    if (Re.TstRegEx("[\r\n]+[0-9A-F ]+[\r\n]+", rv) == true) //Found , and the EVKit has nothing more to say
                        break;
                }
            }

            if (rv.IndexOf('\n') > 0)
            {
                DataValue = rv;
                return RStatus.Good; // GetDataValue(rv, out DataValue);
            }
            else
                return RStatus.ParseErr;
        }

  
        public RStatus  SendString(string Str)
        {
            if (port.IsOpen == false)
            {                
                PortClose(true);
                return RStatus.ComError ;
            }            
            port.Write(Str + "\n");
            return RStatus.Good ;
        }
        public string GetString()
        {
            if (port.IsOpen == false)
            {                
                PortClose(true); 
                return "ERROR";
            }
            string txt;
            System.Threading.Thread.Sleep(ComPortDelay);
            txt = port.ReadExisting();
            txt = Re.RepRegex("\r\n", txt, Environment.NewLine);
            txt = Re.RepRegex("\n\r", txt, Environment.NewLine);
            txt = Re.RepRegex("\n", txt, Environment.NewLine);
       
            return txt;
        }

        private void ShowPortClosedInfo()
        {
            LabelEVkitInfo.Text = "Com Port is closed" + Environment.NewLine + "Please connect EVkit to PC with USB cable and click '" + buttonScanPorts.Text + "'";
            FirmwareVersionReply = "No Hardware";
    
        }
        private void ShowPortOpenInfo(string reply)
        {
            int pos = reply.IndexOf("meridian");
            if(pos>0)
                reply = reply.Substring(pos);
            LabelEVkitInfo.Text = reply;
            FirmwareVersionReply = reply;
           
        }


        public void NotifyEvkitNotConnected()
        {
            TimeSpan ts;
            const int TimeBetweenWarnings = 10;


            if(TimeLastNotificationNoHardware == null )
            {
              TimeLastNotificationNoHardware = new DateTime();
              TimeLastNotificationNoHardware = DateTime.Now;              
            }
            else
            {
                ts =  DateTime.Now - TimeLastNotificationNoHardware ;
                if (ts.TotalSeconds < TimeBetweenWarnings) //Not ts.Seconds;
                    return;
            }
            
            MessageBox.Show("not connected - select 'Configuration' menu item to connect");
            TimeLastNotificationNoHardware = DateTime.Now;     
        }
        
        private void PortClose(bool Show=false)
        {
            try
            {
                port.Close();
            }
            catch
            {

            }
            btnOpenPort.Text = "Open Port";
            ShowPortClosedInfo();
            panel1.Enabled = false;
            if (Show)
            {
           
            }
        }
        private void btnOpenPort_Click(object sender, EventArgs e){ btnOpenPort_Click_Hook();}
        private string btnOpenPort_Click_Hook()
        {
            LabelEVkitInfo.Text = "";
            if (port.IsOpen)
            {
                PortClose();
                return "";
            }

            port.PortName = (string)comboBoxComPorts.SelectedItem;
            SavedPortName = port.PortName;
            port.Parity = Parity.None;
            //port.BaudRate = 9600; (actualy the boot loader is slow, real firmware is fastest)
            port.BaudRate = 921500;
            port.StopBits = StopBits.One;
            try { port.Open(); }
            catch { ShowPortClosedInfo(); return ""; }
            if (port.IsOpen)
            {
                panel1.Enabled = true;
                btnOpenPort.Text = "Close Port";                
                //TODO
                //This would be a good place to get version / check that its the right board
                GetString();
                SendString("i");
                System.Threading.Thread.Sleep(50);
                return GetString();

            }
            return "";
        }

        private void frmTerminal_FormClosing_1(object sender, FormClosingEventArgs e) { e.Cancel = true; frmTerminal_FormClosing_1_Hook(); }
        public void frmTerminal_FormClosing_1_Hook()
        {

            this.Hide() ;            
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
   
        }

        

        private void tabWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //already done in this close _FrmTab_.Hide(); _FrmTab_.Show(); _FrmTab_.WindowState = FormWindowState.Normal; _FrmTab_.ShowInTaskbar = true;
            this.Close(); 
        }

        private void buttonScanPorts_Click(object sender, EventArgs e)
        {
            EnumerateComPorts();
        }

        private void checkBoxAutoScanAndOpen_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
    this.Close(); 
        }

        private void frmTerminal_Shown(object sender, EventArgs e)
        {
            this.Refresh();

        }

        public void ComPortSetDelay(int Delay = DefaultComPortDelay)
        {
            ComPortDelay = Delay;
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //ReportComError Options:
        //Display in message box
        //Display only in Log (with error tag put into status)
        //
        public void ReportComError(string FirstLine, RStatus R)
        {
            string Rs = R.ToString();
            string ErrorStr = FirstLine + Environment.NewLine + "\"" + Rs + "\"" + Environment.NewLine + "Operation Canceled";
            MessageBox.Show(ErrorStr);
          }

    
    } //end class    
} //end namespace

public enum RStatus
{
    Good,
    ComError,
    ParseErr,
    Unknown,
    NoAck
}