using System.Text.RegularExpressions;

namespace TranslateTools.UserForm.SupportTools {
    public class TranslateLookUp : SupportTool, ISupportTool {

        private (string raw, string sub)[] Data = Array.Empty<(string raw, string trans)>();

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
            Match[] matches = Regex.Matches(fileText, ".+?\\n.+").ToArray();

            Data = matches.Select(match => {

                string[] splitter = match.Value.Split("\n", 2);

                return (raw: splitter[0], sub: splitter[1]);
            }).ToArray();

            if(Data.Length == 0) {
                _ = MessageBox.Show("File trống");
                return false;
            }

            return true;
        }

        protected override string Search((string, int)[] fs, bool doExtra, CancellationToken token) {
            int count = 0;
            string writeStr = "";
            foreach((string raw, string trans) in Data) {
                string text = raw + "\n" + trans;
                if(SupportToolHelpers.CustomSearch(text, fs)) {
                    writeStr += text;
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
