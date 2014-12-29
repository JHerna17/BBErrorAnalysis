using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


namespace ErrorAnalysis
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            while(true){
                // Configure open file dialog box
                OpenFileDialog dlg = new OpenFileDialog();               
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

                // Show open file dialog box                
                dlg.ShowDialog();                
                string filename = dlg.FileName;
                File.Delete(Path.GetDirectoryName(filename) + "\\Results.csv");
                try{
                    using(StreamReader reader = new StreamReader(filename)){
                        string line;

                        while((line = reader.ReadLine()) != null){

                            using(StreamWriter writter = new StreamWriter(Path.GetDirectoryName(filename) + "\\Results.csv", true)){
                                string[] split = Regex.Split(line, @"[^0-9\.\-]+");
                                string lineout="";
                                for (int i = 0; i < split.Length; i++) {
                                    if (i + 2 == split.Length && split.Length > 3 && split[i] != "") {
                                        lineout += split[i] + "e" + split[i + 1];
                                        break;
                                    }
                                    else if(split[i] != "" && split.Length > 3) {
                                        lineout += split[i] + ",";
                                    }
                                }
                                if (split.Length > 3) {
                                    lineout.Trim();
                                    lineout += "," + Convert.ToDouble(split[2]) * Convert.ToDouble(split[3]); 
                                    writter.WriteLine(lineout);
                                }
                            }
                        }
                    }
                    
                }
                    
                catch {
                    Console.WriteLine("error");
                }

                break;   
            }
        }
    }
}
