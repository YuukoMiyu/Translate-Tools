using System;
using System.Text.RegularExpressions;
using Translate_Tools.Workspace;
using TranslateTools.UserForm.SupportTools;

namespace TranslateTools {
    internal partial class MyForm {

        private void ImplementNewMyUserControl(IMyUserControl myUserControl) {
            myUserControl.SelectedText += ChangePrimaryDictionaryInput;
        }

        private void ClearImplementedMyUserControl(IMyUserControl myUserControl) {
            myUserControl.SelectedText -= ChangePrimaryDictionaryInput;
        }

        private void AddNewTabPageWithControl(IMyUserControl control) {
            TabPage newTabPage = new() {
                Location = new Point(4, 24),
                Padding = new Padding(3),
                Size = new Size(609, 420),
                Text = control.DisplayName,
                UseVisualStyleBackColor = true
            };
            newTabPage.Controls.Add(control.ToControl());
            this.tabControl1.Controls.Add(newTabPage);

            ImplementNewMyUserControl(control);
        }

        private InputControl AddNewEmptyTabPage() {
            InputControl newInputControl = new() {
                Location = new Point(0, 0),
                Size = new Size(609, 420),
                TabIndex = 0,
            };
            newInputControl.RequestReplaceUserControl += ReplaceUserControl;
            AddNewTabPageWithControl(newInputControl);
            return newInputControl;
        }

        private void DeleteTabPage(int index) {
            TabPage tabPage = tabControl1.TabPages[index];
            foreach(MyUserControl control in tabPage.Controls) {
                control.Dispose();
            }
            tabPage.Dispose();
        }

        private void ReplaceUserControl(object? sender, (IMyUserControl where, IMyUserControl replace) controls) {
            Control whereControl = controls.where.ToControl(),
                    replaceControl = controls.replace.ToControl();

            foreach(Control tabPage in tabControl1.Controls) {
                if(tabPage.Controls.Contains(whereControl)) {
                    tabPage.Controls.Remove(whereControl);
                    tabPage.Controls.Add(replaceControl);

                    ImplementNewMyUserControl(controls.replace);
                    tabPage.Text = controls.replace.DisplayName;
                    controls.replace.FocusMe();
                    controls.where.Dispose();
                }
            }


        }

        private void ChangePrimaryDictionaryInput(object? sender, string text) {
            primaryDictionaryInput.Text = text;
        }

        private void SearchPrimaryDictionary(object? sender, EventArgs e) {
            _ = Task.Run(() => {

                string input = ((TextBox)sender!).Text.Trim(' ', ',', '.').ToLower();
                if(input == "") {
                    return;
                }

                string text = PrimaryDictionary.GetValueOrDefault(input, "Không tìm thấy")
                                               .Replace("...???", "Thiếu giải thích");

                for(int i = 0; i < 8; i++) {
                    text = Regex.Replace(
                        input: text,
                        pattern: "\n  " + SupportToolHelpers.Section[i],
                        replacement: "\n\n" + SupportToolHelpers.Section[i]);
                }

                _ = GlobalHelper.InvokeSpecificControlAsync(primaryDictionaryOutput, () => {
                    primaryDictionaryOutput.Text = text;
                });

            });
        }

        private void SaveFileOpenDialog() {
            this.saveFileDialog1.InitialDirectory = Application.StartupPath + "save\\";
            this.saveFileDialog1.Filter = "*.dict.txt; *.sub.txt; *.eg.txt; *.ws.conf|*.dict.txt; *.sub.txt; *.eg.txt; *.ws.conf";
            this.saveFileDialog1.ShowDialog();
        }

        private void ImportFileOpenDialog() {
            this.openFileDialog1.InitialDirectory = Application.StartupPath + "save\\";
            this.openFileDialog1.Filter = "*.dict.txt; *.sub.txt; *.eg.txt; *.ws.conf|*.dict.txt; *.sub.txt; *.eg.txt; *.ws.conf";
            this.openFileDialog1.ShowDialog();
        }

        private void SaveWorkspaceOpenDialog() {
            this.openFileDialog1.InitialDirectory = Application.StartupPath + "save\\";
            this.saveFileDialog1.DefaultExt = "*.ws.txt|*.ws.txt";
            this.saveFileDialog1.FileName = "workspace1.ws.txt";
            this.saveFileDialog1.ShowDialog();
        }

        private void ImportWorkspaceOpenDialog() {
            this.openFileDialog1.InitialDirectory = Application.StartupPath + "save\\";
            this.openFileDialog1.Filter = "*.ws.txt|*.ws.txt";
            this.openFileDialog1.ShowDialog();
        }

        private async Task<bool> SaveWorkspace(string path) {
            Workspace workspace = new();
            IMyUserControl[] AllMyUserControl = this.tabControl1.Controls.Cast<TabPage>()
                                                                         .Select(t => t.Controls.Cast<IMyUserControl>())
                                                                         .SelectMany(t => t)
                                                                         .Distinct()
                                                                         .ToArray();

            workspace.MyUserControlList.AddRange(AllMyUserControl);
            return await workspace.Save(path);
        }

        private async Task<bool> ImportWorkspace(string path) {
            Workspace workspace = new();
            IMyUserControl[]? AllMyUserControl = await workspace.Import(path);

            if(AllMyUserControl != null) {
                foreach(IMyUserControl myUserControl in AllMyUserControl) {
                    AddNewTabPageWithControl(myUserControl);
                }
            }

            return false;
        }
    }
}
