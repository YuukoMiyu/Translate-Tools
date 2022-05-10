using System.Text.RegularExpressions;
using TranslateTools.UserForm.SupportTools;

namespace TranslateTools {
    public partial class SupportToolsControl : MyUserControl, IMyUserControl {

        // Declare
        private delegate void WriteText(string text);
        private readonly ISupportTool Tool;
        private CancellationTokenSource FindingCancellationTokenSource = new();
        public SupportToolsControl(ISupportTool tool) {
            InitializeComponent();
            Tool = tool;
            WriteOutput(tool.DefaultText);
            tool.FinishOneWork += () => GlobalHelper.InvokeSpecificControl(progressBar1, PerformStepProgressBar);

            _name = Tool.DisplayName;
            _path = Tool.Path;
        }

        // Function
        public void Search(string f) {
            ((string, int)[] fs, bool doExtra) = SupportToolHelpers.DecodeInput(f);
            if(fs.Length == 0) {
                return;
            }

            FindingCancellationTokenSource = new();
            ResetProgressBar();


            Tool.SearchAsync(fs, doExtra, FindingCancellationTokenSource.Token).ContinueWith((text) => {

                ReplaceOutput(text.Result);
                ClearProgressBar();
                MoveCursorToRight();

                if(Tool.WantHighlight) {
                    HighlightOutputedText(fs.Select(t => t.Item1).ToArray());
                }    
                Tool.QueueSaveOutput(f, richTextBox1.Text);

            }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(false);
        }

        // TextBox
        private void ReplaceInput(string text) {
            textBox1.Text = text;
        }
        private void MoveCursorToRight() {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }

        // RichTextBox
        protected string GetOutput() {
            return richTextBox1.Text;
        }
        protected void ClearOutput() {
            richTextBox1.Clear();
        }
        protected void WriteOutput(string text) {
            richTextBox1.AppendText(text);
        }
        protected void ReplaceOutput(string text) {
            richTextBox1.Text = text;
        }
        protected void HighlightOutputedText(IEnumerable<string> highlightTexts) {
            if(!highlightTexts.Any())
                return;

            Match[] hightlighIndex = Regex.Matches(
                                        GetOutput(),
                                        @"\b" + String.Join("|", highlightTexts) + @"\b",
                                        RegexOptions.IgnoreCase).ToArray();

            GlobalHelper.InvokeSpecificControl(richTextBox1, () => {
                foreach(Match? i in hightlighIndex) {
                    richTextBox1.SelectionStart = i.Index;
                    richTextBox1.SelectionLength = i.Length;
                    richTextBox1.SelectionBackColor = Color.Yellow;
                }
            });
        }

        // Progress Bar
        protected void ResetProgressBar() {
            progressBar1.Value = 10;
        }
        protected void PerformStepProgressBar() {
            progressBar1.PerformStep();
        }
        protected void ClearProgressBar() {
            progressBar1.Value = 0;
        }

        // Event
        private void KeyInput(object sender, KeyEventArgs e) {
            try {
                switch(e.KeyCode) {
                    case Keys.Enter: {
                        Search(textBox1.Text);
                        return;
                    }

                    case Keys.Up: {
                        (string input, string output)? save = Tool.GetPreviousSave;
                        if(save != null) {
                            ReplaceInput(save.Value.input);
                            ReplaceOutput(save.Value.output);
                            if(Tool.WantHighlight) {
                                ((string, int)[] fs, bool _) = SupportToolHelpers.DecodeInput(save.Value.input);
                                HighlightOutputedText(fs.Select(t => t.Item1).ToArray());
                                GlobalHelper.InvokeSpecificControl(textBox1, () => {
                                    textBox1.SelectionStart = textBox1.Text.Length;
                                    textBox1.SelectionLength = 0;
                                });
                            }
                        }
                        return;
                    }

                    case Keys.Down: {
                        (string input, string output)? save = Tool.GetNextSave;
                        if(save != null) {
                            ReplaceInput(save.Value.input);
                            ReplaceOutput(save.Value.output);
                            if(Tool.WantHighlight) {
                                ((string, int)[] fs, bool _) = SupportToolHelpers.DecodeInput(save.Value.input);
                                HighlightOutputedText(fs.Select(t => t.Item1).ToArray());
                                GlobalHelper.InvokeSpecificControl(textBox1, () => {
                                    textBox1.SelectionStart = textBox1.Text.Length;
                                    textBox1.SelectionLength = 0;
                                });
                            }
                        }
                        return;
                    }

                    case Keys.Escape: {
                        ClearOutput();
                        WriteOutput(Tool.DefaultText);
                        break;
                    }

                }
            } catch {
                // Do Nothing            
            }
        }
        private void SelectText(object sender, EventArgs e) {
            TriggerSelectText(((RichTextBox)sender).SelectedText);
        }
        private void CancelFinding(object sender, EventArgs e) {
            if(!FindingCancellationTokenSource.IsCancellationRequested)
                FindingCancellationTokenSource.Cancel();
        }

        // IMyUserControl
        public override void FocusMe() {
            textBox1.Focus();
        }
        public override string GetSaveInfo() {
            return $"{FromPath}&|>>>>{Tool.GetToolSaveString()}";
        }
        public override bool ImportSaveInfo(string saveInfoStr) {
            if(Tool.ImportFromString(saveInfoStr)) {
                (string input, string output)? save = Tool.GetPreviousSave;
                if(save != null) {
                    ReplaceInput(save.Value.input);
                    ReplaceOutput(save.Value.output);
                }
                return true;
            }

            return false;
        }
    }
}