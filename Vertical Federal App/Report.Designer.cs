﻿
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
            this.SuspendLayout();
            // 
            // PhysicalAddress
            // 
            this.PhysicalAddress.Location = new System.Drawing.Point(69, 321);
            this.PhysicalAddress.Name = "PhysicalAddress";
            this.PhysicalAddress.Size = new System.Drawing.Size(481, 26);
            this.PhysicalAddress.TabIndex = 1;
            this.PhysicalAddress.TextChanged += new System.EventHandler(this.PhysicalAddress_TextChanged);
            // 
            // BussinessName
            // 
            this.BussinessName.Location = new System.Drawing.Point(69, 255);
            this.BussinessName.Name = "BussinessName";
            this.BussinessName.Size = new System.Drawing.Size(481, 26);
            this.BussinessName.TabIndex = 2;
            this.BussinessName.TextChanged += new System.EventHandler(this.BussinessName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(297, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "NZ Registered Bussiness Name of Client";
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(65, 298);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(129, 20);
            this.AddressLabel.TabIndex = 5;
            this.AddressLabel.Text = "Physical Address";
            // 
            // WriteReportButton
            // 
            this.WriteReportButton.Location = new System.Drawing.Point(69, 502);
            this.WriteReportButton.Name = "WriteReportButton";
            this.WriteReportButton.Size = new System.Drawing.Size(481, 71);
            this.WriteReportButton.TabIndex = 6;
            this.WriteReportButton.Text = "Write Report to File";
            this.WriteReportButton.UseVisualStyleBackColor = true;
            this.WriteReportButton.Click += new System.EventHandler(this.WriteReportButton_Click);
            // 
            // JobNoText
            // 
            this.JobNoText.Location = new System.Drawing.Point(69, 61);
            this.JobNoText.Name = "JobNoText";
            this.JobNoText.Size = new System.Drawing.Size(481, 26);
            this.JobNoText.TabIndex = 8;
            this.JobNoText.Text = "LXXXX/JXXXXX";
            this.JobNoText.TextChanged += new System.EventHandler(this.JobNoText_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "L Number/Job Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(65, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 10;
            // 
            // ReportNumberTextbox
            // 
            this.ReportNumberTextbox.Location = new System.Drawing.Point(69, 128);
            this.ReportNumberTextbox.Name = "ReportNumberTextbox";
            this.ReportNumberTextbox.Size = new System.Drawing.Size(481, 26);
            this.ReportNumberTextbox.TabIndex = 11;
            this.ReportNumberTextbox.Text = "XXXX";
            this.ReportNumberTextbox.TextChanged += new System.EventHandler(this.ReportNumberTextbox_TextChanged);
            // 
            // Report_Number_label
            // 
            this.Report_Number_label.AutoSize = true;
            this.Report_Number_label.Location = new System.Drawing.Point(65, 105);
            this.Report_Number_label.Name = "Report_Number_label";
            this.Report_Number_label.Size = new System.Drawing.Size(118, 20);
            this.Report_Number_label.TabIndex = 12;
            this.Report_Number_label.Text = "Report Number";
            // 
            // ManufacturerTextbox
            // 
            this.ManufacturerTextbox.Location = new System.Drawing.Point(69, 191);
            this.ManufacturerTextbox.Name = "ManufacturerTextbox";
            this.ManufacturerTextbox.Size = new System.Drawing.Size(481, 26);
            this.ManufacturerTextbox.TabIndex = 13;
            this.ManufacturerTextbox.Text = "Enter the manufacturer\'s name";
            this.ManufacturerTextbox.TextChanged += new System.EventHandler(this.ManufacturerTextbox_TextChanged);
            // 
            // ManufacturerLabel
            // 
            this.ManufacturerLabel.AutoSize = true;
            this.ManufacturerLabel.Location = new System.Drawing.Point(65, 168);
            this.ManufacturerLabel.Name = "ManufacturerLabel";
            this.ManufacturerLabel.Size = new System.Drawing.Size(104, 20);
            this.ManufacturerLabel.TabIndex = 14;
            this.ManufacturerLabel.Text = "Manufacturer";
            // 
            // addImageButton
            // 
            this.addImageButton.Location = new System.Drawing.Point(607, 502);
            this.addImageButton.Name = "addImageButton";
            this.addImageButton.Size = new System.Drawing.Size(456, 71);
            this.addImageButton.TabIndex = 15;
            this.addImageButton.Text = "Add Image File";
            this.addImageButton.UseVisualStyleBackColor = true;
            this.addImageButton.Click += new System.EventHandler(this.addImageButton_Click);
            // 
            // imageFileRichTextBox
            // 
            this.imageFileRichTextBox.Location = new System.Drawing.Point(607, 61);
            this.imageFileRichTextBox.Name = "imageFileRichTextBox";
            this.imageFileRichTextBox.Size = new System.Drawing.Size(456, 415);
            this.imageFileRichTextBox.TabIndex = 16;
            this.imageFileRichTextBox.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(603, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(624, 30);
            this.label4.TabIndex = 17;
            this.label4.Text = "Image File List (you can add multiple images to the report)";
            // 
            // ImageFileDialog
            // 
            this.ImageFileDialog.FileName = "ImageFileDialog";
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1125, 611);
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
    }
}