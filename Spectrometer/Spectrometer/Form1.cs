using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raman
{
    public partial class Form1 : Form
    {
        ClassFileIO FileIO;
        RegExWrapper Re;

        public frmTerminal _FrmTerminal_ = new frmTerminal();
        public Form1()
        {
            InitializeComponent();
            FileIO = new ClassFileIO();
            Re = new RegExWrapper();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

    }

        private void button1_Click(object sender, EventArgs e)
        {
            _FrmTerminal_.Hide(); _FrmTerminal_.Hide(); _FrmTerminal_.Show(); _FrmTerminal_.WindowState = FormWindowState.Normal; _FrmTerminal_.WindowState = FormWindowState.Normal;
            _FrmTerminal_.ShowInTaskbar = true;
   
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            string rdStr, parseStr;
            btnRead.Enabled = false;
            
            _FrmTerminal_.readReg("r", out rdStr);
            parseStr = Re.RepRegex("\t", rdStr, ",");
            parseStr = Re.RepRegex("\r\r", parseStr, "\r");
            FileIO.WriteAll(parseStr, "raw_"+ DateTime.Now.Ticks +".csv");
            fillChart(rdStr);
            btnRead.Enabled = true;
        }

        private void fillChart(string DataV)
        {

            string[] strArray = DataV.Split('\n');
            string strLine;
            string[] strXY;
            Int16  X;
            double Min, Max, Y;
            Min = 99; Max = 0;
            if(strArray.Length < 3647)
            {
                return;
            }

            // Set fast line chart type
            chart1.Series.Clear();
            chart1.Series.Add("CCD");
            chart1.Series["CCD"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;

            for(int i = 2; i< 3648; i++) // first 2 samp are 0
            {
                strLine = strArray[i];
                strXY = strLine.Split('\t');
                if(strXY.Length < 2)
                {
                    //log err
                    continue;
                }
                //check bounds i = xval
                X = Convert.ToInt16(strXY[0]);
                Y = Convert.ToDouble(strXY[1]);
                chart1.Series["CCD"].Points.AddXY(X, Y);
                if (Y < Min)
                    Min = Y;
                if (Y > Max)
                    Max = Y;
            }
            //chart1.ChartAreas[0].RecalculateAxesScale();



        }


    }
}
