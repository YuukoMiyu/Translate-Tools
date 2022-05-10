namespace TranslateTools {

    internal partial class MyForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.primaryDictionaryInput = new System.Windows.Forms.TextBox();
            this.primaryDictionaryOutput = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // primaryDictionaryInput
            // 
            this.primaryDictionaryInput.Location = new System.Drawing.Point(623, 0);
            this.primaryDictionaryInput.Name = "primaryDictionaryInput";
            this.primaryDictionaryInput.Size = new System.Drawing.Size(175, 23);
            this.primaryDictionaryInput.TabIndex = 2;
            this.primaryDictionaryInput.TextChanged += new System.EventHandler(this.SearchPrimaryDictionary);
            // 
            // primaryDictionaryOutput
            // 
            this.primaryDictionaryOutput.Location = new System.Drawing.Point(623, 29);
            this.primaryDictionaryOutput.Name = "primaryDictionaryOutput";
            this.primaryDictionaryOutput.ReadOnly = true;
            this.primaryDictionaryOutput.Size = new System.Drawing.Size(175, 419);
            this.primaryDictionaryOutput.TabIndex = 3;
            this.primaryDictionaryOutput.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(617, 448);
            this.tabControl1.TabIndex = 4;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileOK);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileOK);
            // 
            // MyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.primaryDictionaryOutput);
            this.Controls.Add(this.primaryDictionaryInput);
            this.Controls.Add(this.tabControl1);
            this.Name = "MyForm";
            this.Text = "Translate Tools";
            this.Load += new System.EventHandler(this.WhenLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public TabControl tabControl1;
        private TextBox primaryDictionaryInput;
        private RichTextBox primaryDictionaryOutput;
        private SaveFileDialog saveFileDialog1;
        private OpenFileDialog openFileDialog1;
    }
}