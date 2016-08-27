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

        public frmTerminal _FrmTerminal_ = new frmTerminal();
        public Form1()
        {
            InitializeComponent();
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
            string rdStr;
            _FrmTerminal_.readReg("r", out rdStr);
            fillChart(rdStr);

        }

        private void fillChart(string DataV)
        {

            string[] strArray = DataV.Split('\n');
            string strLine;
            string[] strXY;

            if(strArray.Length < 3647)
            {
                return;
            }

            // Set fast line chart type
            chart1.Series.Clear();
            chart1.Series.Add("CCD");
            chart1.Series["CCD"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;

            for(int i = 0; i< 3648; i++)
            {
                strLine = strArray[i];
                strXY = strLine.Split('\t');
                if(strXY.Length < 2)
                {
                    //log err
                    continue;
                }
                //check bounds i = xval
                chart1.Series["CCD"].Points.AddXY(Convert.ToInt16(strXY[0]), Convert.ToDouble( strXY[1]));
            }




        }


    }
}
