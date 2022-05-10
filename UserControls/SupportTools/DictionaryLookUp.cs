using System.Text.RegularExpressions;

namespace TranslateTools.UserForm.SupportTools {
    public class DictionaryLookUp : SupportTool, ISupportTool {

        private static (string word, string expl)[] Data = Array.Empty<(string word, string expl)>();

        public override int DataLength {
            get {
                return Data.Length;
            }
        }

        public override string DefaultText {
            get {
                string text = $"Tìm thấy ({DataLength})\n\n";
                for(int i = 0; i < SupportToolHelpers.Section.Length; i++) {
                    text += i + 1 + "/ " + SupportToolHelpers.Section[i] + "\n";
                    if(i == 7 || i == 14)
                        text += "--------------------------\n";
                }
                text += "cdao, tkiều, ts, co, tng, pcch\n\n";
                text += "*/ tìm lần hai";
                return text;
            }
        }

        public override bool WantHighlight {
            get {
                return false;
            }
        }

        protected override bool LoadData(string path) {
            string fileText = File.ReadAllText(path, System.Text.Encoding.UTF8);
            Match[] matches = Regex.Matches(fileText, "[^@]+").ToArray();

            Data = matches.Select(match => {

                string[] splitter = match.Value.Split("\n", 2);

                return (word: splitter[0], expl: splitter[1].TrimEnd('\n'));
            }).ToArray();

            if(Data.Length == 0) {
                _ = MessageBox.Show("File trống");
                return false;
            }

            return true;
        }

        protected override string Search((string, int)[] fs, bool doExtra, CancellationToken token) {
            Workload = DataLength * (doExtra? 3 : 2);
            string writeStr = "";

            List<string> getWord = new();
            foreach((string word, string expl) in Data) {

                if(token.IsCancellationRequested)
                    break;

                if(SupportToolHelpers.CustomSearch(word, fs.Take(1))
                    && SupportToolHelpers.CustomSearch(word + "\n" + expl, fs.Skip(1), word)) {
                    getWord.Add(word);
                    writeStr += word + ", ";
                }
                AddToProgressBar();

            }

            writeStr += $"\n\nTìm từ từ vựng: ({getWord.Count})\n\n";

            if(token.IsCancellationRequested) {
                return $"{writeStr}=> Tổng cộng: ({getWord.Count})";
            }

            List<string> getExpl = new();
            foreach((string word, string expl) in Data.Where(t => !getWord.Equals(t.word))) {
                if(SupportToolHelpers.CustomSearch(word + "\n" + expl, fs)) {
                    writeStr += word + ", ";
                    getExpl.Add(word);
                }

                AddToProgressBar();
                if(token.IsCancellationRequested)
                    break;
            }

            writeStr += $"\n\nTìm từ giải thích: ({getExpl.Count})\n\n";

            if(token.IsCancellationRequested || !doExtra) {
                return $"{writeStr} => Tổng cộng: ({getWord.Count + getExpl.Count})";
            }


            getWord.AddRange(getExpl);
            int rcount = 0;
            foreach((string word, string expl) in Data.Where(t => !getWord.Equals(t.word))) {
                if(getWord.Any(t => SupportToolHelpers.CustomSearch(expl, t))) {
                    writeStr += word + ", ";
                    rcount += 1;
                }

                AddToProgressBar();
                if(token.IsCancellationRequested)
                    break;
            }

            writeStr += $"\n\nTìm thêm: ({rcount})\n\n";

            return $"{writeStr}=> Tổng cộng: ({getWord.Count + rcount})";
        }
    }
}
