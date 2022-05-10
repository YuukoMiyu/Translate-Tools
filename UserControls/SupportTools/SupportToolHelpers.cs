using System.Text.RegularExpressions;

namespace TranslateTools.UserForm.SupportTools {
    internal static class SupportToolHelpers {
        public static readonly string[] Section = new string[]{ "danh từ", "động từ", "tính từ", "đại từ", "kết từ", "phụ từ", "cảm từ", "trợ từ",
                                                                "từ láy", "phương ngữ", "khẩu ngữ", "ít dùng", "từ cũ", "trang trọng", "văn chương",
                                                                "nói khái quát", "nói tắt", "thông tục", "hàm ý", "nhưng nghĩa mạnh hơn", "nhưng nghĩa nhẹ hơn", "làm việc gì"};

        public static readonly NotImplementedException notImplementedException = new("Lỗi công cụ chưa được định dạng");

        public static bool CustomSearch(string text, IEnumerable<(string, int)> find, params string[] without) {
            bool currentStat = true;
            foreach((string word, int type) in find) {
                switch(type) {
                    case 0:
                        if(currentStat == false)
                            return false;
                        currentStat &= CustomSearch(text, word, without);
                        break;
                    case 1:
                        currentStat |= CustomSearch(text, word, without);
                        break;
                    case 2:
                        if(currentStat == false)
                            return false;
                        currentStat &= !CustomSearch(text, word, without);
                        break;
                    case 3:
                        currentStat |= !CustomSearch(text, word, without);
                        break;
                    default:
                        _ = MessageBox.Show("Lỗi nhập");
                        return false;
                }
            }
            return currentStat;
        }
        public static bool CustomSearch(string text, string find, params string[] without) {
            string[] splitter = text.Trim().Split("\n");

            foreach(string line in splitter) {
                bool hasSpecialCharacter = find.Any(t => !char.IsLetter(t));
                if(Regex.IsMatch(
                        line,
                        hasSpecialCharacter? find : @"\b" + find + @"\b")
                   && (without.Length == 0 || !Regex.IsMatch(
                       line, 
                       hasSpecialCharacter ? find : @"\b" + String.Join("|", without) + @"\b"))
                   ) {
                    return true;
                }
            }

            return false;
        }
        public static ((string, int)[], bool) DecodeInput(string input) {
            input = "&" + input;
            List<(string, int)> output = new();
            bool doExtra = false;

            Regex regex = new(@"(?:(?<4>\|\!)|(?<3>\&\!|\!)|(?<2>\|)|(?<1>\&))[^\&\|\!]*");
            MatchCollection groupCollection = regex.Matches(input);

            foreach(Match match in groupCollection) {
                string text = Regex.Replace(match.Groups[0].Value, "[&!|]", "").Trim().ToLower();
                string group = match.Groups.Values.Skip(1).First(t => t.Value != "").Name;

                if(text == "*" && (group != "4" || group == "3")) {
                    doExtra = true;
                } if(text == "") {
                    continue;
                } else {
                    output.Add((
                        int.TryParse(text, out int num) ? Section[num - 1] : text,
                        int.Parse(group) - 1
                    ));
                }
            }

            return (output.ToArray(), doExtra);
        }
    }
}