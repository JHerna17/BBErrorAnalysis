using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ThresholdAnalysis
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            while (true) {
                // Configure open file dialog box
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".csv"; // Default file extension
                dlg.Filter = "Text documents (.csv)|*.csv"; // Filter files by extension 

                // Show open file dialog box                
                dlg.ShowDialog();
                string filename = dlg.SafeFileName;
                string filenamewithPath = dlg.FileName;
                int NoClusters = 8;
                int NoThresholds = 7;
                List<string>[] ThresholdsPerIteration = new List<string>[NoClusters];
                SortedDictionary<int, double[]>[] ThresholsChangePerIteration_int = new SortedDictionary<int,double[]>[NoClusters];
                for (int i = 0; i < NoClusters; i++) {
                    ThresholdsPerIteration[i] = new List<string>();
                    ThresholsChangePerIteration_int[i] = new SortedDictionary<int,double[]>();
                }

                string[] ThresholdFiles = Directory.GetFiles(Path.GetDirectoryName(filenamewithPath), "*Thresholds.csv");

                string[] FileNames = filename.Split(new string[] {"Thresholds"},StringSplitOptions.RemoveEmptyEntries );
                for (int i = 0; i < NoClusters; i++) {
                    if (File.Exists(Path.GetDirectoryName(filename) + "\\" + "_ThresAnalysis_Cluster" + i + ".csv"))
                        File.Delete(Path.GetDirectoryName(filename) + "\\" + "_ThresAnalysis_Cluster" + i + ".csv");
                }
                    
                try {
                    foreach (string file in ThresholdFiles) {
                        using (StreamReader reader = new StreamReader(file)) {
                            string line;

                            while ((line = reader.ReadLine()) != null) {
                                string[] split = Regex.Split(line, @",");
                                for (int i = 0; i < NoClusters; i++) {
                                    string lineout = split[(NoThresholds * i) + 0] + "," + split[(NoThresholds * i) + 1] + "," + split[(NoThresholds * i) + 2] + "," + split[(NoThresholds * i) + 3] + "," + split[(NoThresholds * i) + 4] + "," + split[(NoThresholds * i) + 5] + "," + split[(NoThresholds * i) + 6];
                                    ThresholdsPerIteration[i].Add(i + "," + split[split.Length - 1] + "," + lineout );
                                }
                            }
                        }
                    }                    

                    for (int i = 0; i < NoThresholds; i++) {
                        string FileToWrite = Path.GetDirectoryName(filenamewithPath) + "\\" +  "_ThresAnalysis_Cluster" + i + ".csv";
                        using (StreamWriter writter = new StreamWriter(FileToWrite)) {
                            foreach(string thres in ThresholdsPerIteration[i]){
                                writter.WriteLine(thres);
                            }
                        }
                    }

                    for (int i = 0; i < NoClusters; i++) {
                        for (int q = 0; q < ThresholdsPerIteration[i].Count - 1; q++ ) {
                            string[] thres1 = ThresholdsPerIteration[i][q].Split(',');
                            double[] value_change = new double[NoThresholds];
                            string[] thres2 = ThresholdsPerIteration[i][q+1].Split(',');
                            for (int x = 0; x < NoThresholds; x++) value_change[x] = Convert.ToDouble(thres1[2 + x]) - Convert.ToDouble(thres2[2+x]);
                            ThresholsChangePerIteration_int[i].Add(Convert.ToInt32(thres1[1]), value_change);
                        }
                    }

                    for (int i = 0; i < NoThresholds; i++) {
                        string FileToWrite = Path.GetDirectoryName(filenamewithPath) + "\\" + "_ThresChangeAnalysis_Cluster" + i + ".csv";
                        using (StreamWriter writter = new StreamWriter(FileToWrite)) {
                            foreach (KeyValuePair<int, double[]> values in ThresholsChangePerIteration_int[i]) {
                                string line = "";
                                for (int y = 0; y < NoThresholds; y++) line += values.Value[y] + ",";
                                    writter.WriteLine(line);
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
