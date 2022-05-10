using System.Text.RegularExpressions;

namespace TranslateTools {


    public delegate void DelegateVoidString(string text);
    public delegate void DelegateVoidVoid();


    internal partial class MyForm : Form {

        private Dictionary<string, string> PrimaryDictionary = new();

        public MyForm() {
            InitializeComponent();
        }

        private void WhenLoad(object sender, EventArgs e) {

            AddDataForPrimaryDictionary("data\\dictionary.dict.txt").ContinueWith(t => {
                if(t.Result == false) {
                    _ = MessageBox.Show("Lỗi đọc tập tin từ điển");
                }
            }).ConfigureAwait(false);

            AddNewEmptyTabPage();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch(keyData) {
                
                // Tab Control
                case Keys.Control | Keys.W: {
                    AddNewEmptyTabPage();
                    break;
                }

                case Keys.Control | Keys.E when tabControl1.HasChildren: {
                    DeleteTabPage(tabControl1.SelectedIndex);
                    break;
                }

                case Keys.Control | Keys.Q when tabControl1.HasChildren: {
                    int index = tabControl1.SelectedIndex;
                    if(index - 1 >= 0) {
                        tabControl1.SelectedIndex = index - 1;
                    }
                    break;
                }

                case Keys.Control | Keys.R when tabControl1.HasChildren: {
                    int index = tabControl1.SelectedIndex;
                    if(index + 1 < tabControl1.TabCount) {
                        tabControl1.SelectedIndex = index + 1;
                    }
                    break;
                }

                /*                // Command
                                case Keys.Control | Keys.S: {
                                    SaveFileOpenDialog();
                                    break;
                                }

                                case Keys.Control | Keys.A: {
                                    ImportFileOpenDialog();
                                    break;
                                }
                */
/*                case Keys.Control | Keys.Shift | Keys.S: {
                    SaveWorkspaceOpenDialog();
                    break;
                }

                case Keys.Control | Keys.Shift | Keys.A: {
                    ImportWorkspaceOpenDialog();
                    break;
                }*/
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async Task<bool> AddDataForPrimaryDictionary(string path) {
            return await Task.Run(() => {
                try {
                    string fileText = File.ReadAllText(path, System.Text.Encoding.UTF8);
                    Match[] matches = Regex.Matches(fileText, "[^@]+").ToArray();
                    IEnumerable<(string, string)>? data = matches.Select(match => {
                        string[] splitter = match.Value.Split("\n", 2);
                        return (splitter[0], splitter[1]);
                    });

                    if(!data.Any()) {
                        _ = MessageBox.Show("Tập tin từ điển trống");
                        return true;
                    }

                    PrimaryDictionary = data.ToDictionary(t => t.Item1, t => t.Item2);
                    return true;
                } catch {
                    return false;
                }

            });
        }

        private async void SaveFileOK(object sender, System.ComponentModel.CancelEventArgs e) {
            string path = ((SaveFileDialog)sender).FileName;
            await SaveWorkspace(path);
        }

        private async void OpenFileOK(object sender, System.ComponentModel.CancelEventArgs e) {
            string path = ((OpenFileDialog)sender).FileName;
            await ImportWorkspace(path);
        }
    }
}