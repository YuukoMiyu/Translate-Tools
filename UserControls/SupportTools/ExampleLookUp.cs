using System.Text.RegularExpressions;

namespace TranslateTools.UserForm.SupportTools {
    public class ExampleLookUp : SupportTool, ISupportTool {

        private static string[] Data = Array.Empty<string>();

        public override int DataLength {
            get {
                return Data.Length;
            }
        }

        public override string DefaultText {
            get {
                return $"Tìm thấy ({DataLength})";
            }
        }

        public override bool WantHighlight {
            get {
                return true;
            }
        }

        protected override bool LoadData(string path) {
            string fileText = File.ReadAllText(path, System.Text.Encoding.UTF8);
            Data = Regex.Split(fileText, "[\\n.]+");

            if(Data.Length == 0) {
                _ = MessageBox.Show("File trống");
                return false;
            }

            return true;
        }

        protected override string Search((string, int)[] fs, bool doExtra, CancellationToken token) {
            int count = 0;
            string writeStr = "";
            foreach(string t in Data) {
                if(SupportToolHelpers.CustomSearch(t, fs)) {
                    writeStr += $"- {t}\n\n";
                    count += 1;
                }

                AddToProgressBar();
                if(token.IsCancellationRequested)
                    break;
            }

            return $"{writeStr}\n\nTìm thấy: ({count})";
        }
    }
}
