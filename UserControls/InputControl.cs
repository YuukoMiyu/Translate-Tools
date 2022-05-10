namespace TranslateTools {
    public partial class InputControl : MyUserControl, IMyUserControl {

        public override string DisplayName {
            get {
                return "Mới";
            }
        }

        public event EventHandler<(IMyUserControl, IMyUserControl)>? RequestReplaceUserControl = null;

        public InputControl() {
            InitializeComponent();
        }

        public new void Focus() {
            button1.Focus();
        }

        private void FindFileButton_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
        }

        private async void OpenFileDialog1_FileOkAsync(object sender, System.ComponentModel.CancelEventArgs e) {
            string path = ((OpenFileDialog)sender).FileName;
            if(File.Exists(path)) {

                string[] fileInfo = new FileInfo(path).Name.Split(".");
                if(fileInfo.Length != 3 && fileInfo[2] == "txt") {
                    label1.Text = "Tập tin không hợp lệ";
                    return;
                }

                IMyUserControl? myUserControl = await SaveAndImportHelpers.TryImportFile(path);
                if(myUserControl != null) {
                    RequestReplaceUserControl?.Invoke(null, (this, myUserControl));
                } else {
                    label1.Text = "Lỗi đọc file";
                } 

            } else {
                label1.Text = "Không tìm thấy tập tin";
            }
            return;
        }

        // IMyUserControl
        public override string? GetSaveInfo() {
            return null;
        }
        public override bool ImportSaveInfo(string fileSaveStr) {
            throw new NotImplementedException();
        }
    }
}