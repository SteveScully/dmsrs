using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Windows.Forms;

namespace Raman
{
    class ClassFileIO
    {
        public SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        public OpenFileDialog openFileDialog1 = new OpenFileDialog();
        /// <summary>
        /// Returns OK if file is not locked.  (note that file doesnt exist will still make file return ok)
        /// Gives Message box on error, then returns nonOK value
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public DialogResult CheckThatFileIsNotLocked(string FileName)
        {
            try
            {               
                    FileStream fileStream = File.OpenWrite(FileName);
                    fileStream.Close();
                    return DialogResult.OK ;  
            }
            catch
            {
                MessageBox.Show("File is locked, please check if the file is open in an external application and try again");
                return DialogResult.Abort;
            }
        }

        public DialogResult WriteAll(string TextToWr, string Fname)
        { // Create a file to write to. 
            if (CheckThatFileIsNotLocked(Fname) != DialogResult.OK)
                return DialogResult.Abort;            
            File.WriteAllText(Fname, TextToWr);
            return DialogResult.OK;
        }

        public DialogResult ShowDialogGetOpenFileName(out string FileName, string filter = "Comma Seperated Values |*.CSV")
        {
            FileName = "";

        beginning:
            // Show the dialog and get result.
            openFileDialog1.Filter = "Comma Seperated Values |*.CSV";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) // Test result.
            {
                return result;
            }

            FileName = openFileDialog1.FileName;
            if (openFileDialog1.CheckFileExists == false)
            {
                goto beginning;
            }



            return DialogResult.OK;
        }

        public DialogResult ShowDialogGetSaveFileName(out string FileName, string filter = "Comma Seperated Values |*.CSV")
        {
            FileName = "";
        beginning:
            // Show the dialog and get result.
            saveFileDialog1.Filter = filter;
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) // Test result.
            {
                return DialogResult.Abort ;
            }

            FileName = saveFileDialog1.FileName;
            if (saveFileDialog1.CheckFileExists == true)
            {
                DialogResult dialogResult = MessageBox.Show("File exists, overwrite?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    goto beginning;
            }

            if (DialogResult.OK == this.CheckThatFileIsNotLocked(FileName))
                File.Delete(FileName);
            else
                return DialogResult.Abort;

            return DialogResult.OK;

        }



    }
}
