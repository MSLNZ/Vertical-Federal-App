
namespace Vertical_Federal_App
{
    partial class Report
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PhysicalAddress = new System.Windows.Forms.TextBox();
            this.BussinessName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.WriteReportButton = new System.Windows.Forms.Button();
            this.JobNoText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ReportNumberTextbox = new System.Windows.Forms.TextBox();
            this.Report_Number_label = new System.Windows.Forms.Label();
            this.ManufacturerTextbox = new System.Windows.Forms.TextBox();
            this.ManufacturerLabel = new System.Windows.Forms.Label();
            this.addImageButton = new System.Windows.Forms.Button();
            this.imageFileRichTextBox = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ImageFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.sn_note = new System.Windows.Forms.Label();
            this.serial_Number_Note = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // PhysicalAddress
            // 
            this.PhysicalAddress.Location = new System.Drawing.Point(46, 429);
            this.PhysicalAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PhysicalAddress.Name = "PhysicalAddress";
            this.PhysicalAddress.Size = new System.Drawing.Size(322, 20);
            this.PhysicalAddress.TabIndex = 1;
            this.PhysicalAddress.TextChanged += new System.EventHandler(this.PhysicalAddress_TextChanged);
            // 
            // BussinessName
            // 
            this.BussinessName.Location = new System.Drawing.Point(47, 381);
            this.BussinessName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BussinessName.Name = "BussinessName";
            this.BussinessName.Size = new System.Drawing.Size(322, 20);
            this.BussinessName.TabIndex = 2;
            this.BussinessName.TextChanged += new System.EventHandler(this.BussinessName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 366);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "NZ Registered Bussiness Name of Client";
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(44, 414);
            this.AddressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(87, 13);
            this.AddressLabel.TabIndex = 5;
            this.AddressLabel.Text = "Physical Address";
            // 
            // WriteReportButton
            // 
            this.WriteReportButton.Location = new System.Drawing.Point(47, 464);
            this.WriteReportButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.WriteReportButton.Name = "WriteReportButton";
            this.WriteReportButton.Size = new System.Drawing.Size(321, 46);
            this.WriteReportButton.TabIndex = 6;
            this.WriteReportButton.Text = "Write Report to File";
            this.WriteReportButton.UseVisualStyleBackColor = true;
            this.WriteReportButton.Click += new System.EventHandler(this.WriteReportButton_Click);
            // 
            // JobNoText
            // 
            this.JobNoText.Location = new System.Drawing.Point(46, 40);
            this.JobNoText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.JobNoText.Name = "JobNoText";
            this.JobNoText.Size = new System.Drawing.Size(322, 20);
            this.JobNoText.TabIndex = 8;
            this.JobNoText.Text = "LXXXX/JXXXXX";
            this.JobNoText.TextChanged += new System.EventHandler(this.JobNoText_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "L Number/Job Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 60);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 10;
            // 
            // ReportNumberTextbox
            // 
            this.ReportNumberTextbox.Location = new System.Drawing.Point(46, 88);
            this.ReportNumberTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ReportNumberTextbox.Name = "ReportNumberTextbox";
            this.ReportNumberTextbox.Size = new System.Drawing.Size(322, 20);
            this.ReportNumberTextbox.TabIndex = 11;
            this.ReportNumberTextbox.Text = "XXXX";
            this.ReportNumberTextbox.TextChanged += new System.EventHandler(this.ReportNumberTextbox_TextChanged);
            // 
            // Report_Number_label
            // 
            this.Report_Number_label.AutoSize = true;
            this.Report_Number_label.Location = new System.Drawing.Point(43, 73);
            this.Report_Number_label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Report_Number_label.Name = "Report_Number_label";
            this.Report_Number_label.Size = new System.Drawing.Size(79, 13);
            this.Report_Number_label.TabIndex = 12;
            this.Report_Number_label.Text = "Report Number";
            // 
            // ManufacturerTextbox
            // 
            this.ManufacturerTextbox.Location = new System.Drawing.Point(45, 134);
            this.ManufacturerTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ManufacturerTextbox.Name = "ManufacturerTextbox";
            this.ManufacturerTextbox.Size = new System.Drawing.Size(322, 20);
            this.ManufacturerTextbox.TabIndex = 13;
            this.ManufacturerTextbox.Text = "Enter the manufacturer\'s name";
            this.ManufacturerTextbox.TextChanged += new System.EventHandler(this.ManufacturerTextbox_TextChanged);
            // 
            // ManufacturerLabel
            // 
            this.ManufacturerLabel.AutoSize = true;
            this.ManufacturerLabel.Location = new System.Drawing.Point(43, 119);
            this.ManufacturerLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ManufacturerLabel.Name = "ManufacturerLabel";
            this.ManufacturerLabel.Size = new System.Drawing.Size(70, 13);
            this.ManufacturerLabel.TabIndex = 14;
            this.ManufacturerLabel.Text = "Manufacturer";
            // 
            // addImageButton
            // 
            this.addImageButton.Location = new System.Drawing.Point(405, 464);
            this.addImageButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.addImageButton.Name = "addImageButton";
            this.addImageButton.Size = new System.Drawing.Size(304, 46);
            this.addImageButton.TabIndex = 15;
            this.addImageButton.Text = "Add Image File";
            this.addImageButton.UseVisualStyleBackColor = true;
            this.addImageButton.Click += new System.EventHandler(this.addImageButton_Click);
            // 
            // imageFileRichTextBox
            // 
            this.imageFileRichTextBox.Location = new System.Drawing.Point(405, 40);
            this.imageFileRichTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.imageFileRichTextBox.Name = "imageFileRichTextBox";
            this.imageFileRichTextBox.Size = new System.Drawing.Size(305, 409);
            this.imageFileRichTextBox.TabIndex = 16;
            this.imageFileRichTextBox.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(402, 25);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(276, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Image File List (you can add multiple images to the report)";
            // 
            // ImageFileDialog
            // 
            this.ImageFileDialog.FileName = "ImageFileDialog";
            // 
            // sn_note
            // 
            this.sn_note.AutoSize = true;
            this.sn_note.Location = new System.Drawing.Point(44, 244);
            this.sn_note.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.sn_note.Name = "sn_note";
            this.sn_note.Size = new System.Drawing.Size(99, 13);
            this.sn_note.TabIndex = 18;
            this.sn_note.Text = "Serial Number Note";
            // 
            // serial_Number_Note
            // 
            this.serial_Number_Note.Location = new System.Drawing.Point(46, 260);
            this.serial_Number_Note.Name = "serial_Number_Note";
            this.serial_Number_Note.Size = new System.Drawing.Size(321, 92);
            this.serial_Number_Note.TabIndex = 19;
            this.serial_Number_Note.Text = "The gauge block set has the serial number XXXX.   Each gauge block is marked with" +
    " a nominal size and a unique serial number.";
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 544);
            this.Controls.Add(this.serial_Number_Note);
            this.Controls.Add(this.sn_note);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.imageFileRichTextBox);
            this.Controls.Add(this.addImageButton);
            this.Controls.Add(this.ManufacturerLabel);
            this.Controls.Add(this.ManufacturerTextbox);
            this.Controls.Add(this.Report_Number_label);
            this.Controls.Add(this.ReportNumberTextbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.JobNoText);
            this.Controls.Add(this.WriteReportButton);
            this.Controls.Add(this.AddressLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BussinessName);
            this.Controls.Add(this.PhysicalAddress);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Report";
            this.Text = "Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox PhysicalAddress;
        private System.Windows.Forms.TextBox BussinessName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Button WriteReportButton;
        private System.Windows.Forms.TextBox JobNoText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ReportNumberTextbox;
        private System.Windows.Forms.Label Report_Number_label;
        private System.Windows.Forms.TextBox ManufacturerTextbox;
        private System.Windows.Forms.Label ManufacturerLabel;
        private System.Windows.Forms.Button addImageButton;
        private System.Windows.Forms.RichTextBox imageFileRichTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog ImageFileDialog;
        private System.Windows.Forms.Label sn_note;
        private System.Windows.Forms.RichTextBox serial_Number_Note;
    }
}