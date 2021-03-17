/*******************************************************************************
* Copyright 2013, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions, 
* disclaimers, and limitations in the end user license agreement accompanying 
* the software package with which this file was provided.
********************************************************************************/


namespace SegLCD_P4_v1_31
{
    partial class CyBasicConfiguration
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CyBasicConfiguration));
            this.comboBoxFrameRate = new System.Windows.Forms.ComboBox();
            this.comboBoxWaveformType = new System.Windows.Forms.ComboBox();
            this.labelFrameRate = new System.Windows.Forms.Label();
            this.labelWaveformType = new System.Windows.Forms.Label();
            this.labelBiasType = new System.Windows.Forms.Label();
            this.numUpDownNumSegmentLines = new System.Windows.Forms.NumericUpDown();
            this.labelNumSegmentLines = new System.Windows.Forms.Label();
            this.labelNumCommonLines = new System.Windows.Forms.Label();
            this.numUpDownNumCommonLines = new System.Windows.Forms.NumericUpDown();
            this.comboBoxBiasType = new System.Windows.Forms.ComboBox();
            this.comboBoxLCDMode = new System.Windows.Forms.ComboBox();
            this.labeLCDMode = new System.Windows.Forms.Label();
            this.comboBoxDrivingMode = new System.Windows.Forms.ComboBox();
            this.labelDrivingMode = new System.Windows.Forms.Label();
            this.comboBoxContrast = new System.Windows.Forms.ComboBox();
            this.labelContrast = new System.Windows.Forms.Label();
            this.groupBoxWarning = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.labelNoClock = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNumSegmentLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNumCommonLines)).BeginInit();
            this.groupBoxWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxFrameRate
            // 
            this.comboBoxFrameRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFrameRate.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxFrameRate, "comboBoxFrameRate");
            this.comboBoxFrameRate.Name = "comboBoxFrameRate";
            this.comboBoxFrameRate.SelectedIndexChanged += new System.EventHandler(this.comboBoxFrameRate_SelectedIndexChanged);
            // 
            // comboBoxWaveformType
            // 
            this.comboBoxWaveformType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWaveformType.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxWaveformType, "comboBoxWaveformType");
            this.comboBoxWaveformType.Name = "comboBoxWaveformType";
            this.comboBoxWaveformType.SelectedIndexChanged += new System.EventHandler(this.comboBoxWaveformType_SelectedIndexChanged);
            // 
            // labelFrameRate
            // 
            resources.ApplyResources(this.labelFrameRate, "labelFrameRate");
            this.labelFrameRate.Name = "labelFrameRate";
            // 
            // labelWaveformType
            // 
            resources.ApplyResources(this.labelWaveformType, "labelWaveformType");
            this.labelWaveformType.Name = "labelWaveformType";
            // 
            // labelBiasType
            // 
            resources.ApplyResources(this.labelBiasType, "labelBiasType");
            this.labelBiasType.Name = "labelBiasType";
            // 
            // numUpDownNumSegmentLines
            // 
            resources.ApplyResources(this.numUpDownNumSegmentLines, "numUpDownNumSegmentLines");
            this.numUpDownNumSegmentLines.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numUpDownNumSegmentLines.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numUpDownNumSegmentLines.Name = "numUpDownNumSegmentLines";
            this.numUpDownNumSegmentLines.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numUpDownNumSegmentLines.ValueChanged += new System.EventHandler(this.numUpDownNumLines_ValueChanged);
            // 
            // labelNumSegmentLines
            // 
            resources.ApplyResources(this.labelNumSegmentLines, "labelNumSegmentLines");
            this.labelNumSegmentLines.Name = "labelNumSegmentLines";
            // 
            // labelNumCommonLines
            // 
            resources.ApplyResources(this.labelNumCommonLines, "labelNumCommonLines");
            this.labelNumCommonLines.Name = "labelNumCommonLines";
            // 
            // numUpDownNumCommonLines
            // 
            resources.ApplyResources(this.numUpDownNumCommonLines, "numUpDownNumCommonLines");
            this.numUpDownNumCommonLines.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numUpDownNumCommonLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownNumCommonLines.Name = "numUpDownNumCommonLines";
            this.numUpDownNumCommonLines.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numUpDownNumCommonLines.ValueChanged += new System.EventHandler(this.numUpDownNumLines_ValueChanged);
            // 
            // comboBoxBiasType
            // 
            this.comboBoxBiasType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBiasType.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxBiasType, "comboBoxBiasType");
            this.comboBoxBiasType.Name = "comboBoxBiasType";
            this.comboBoxBiasType.SelectedIndexChanged += new System.EventHandler(this.comboBoxBiasType_SelectedIndexChanged);
            // 
            // comboBoxLCDMode
            // 
            this.comboBoxLCDMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLCDMode.DropDownWidth = 180;
            this.comboBoxLCDMode.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxLCDMode, "comboBoxLCDMode");
            this.comboBoxLCDMode.Name = "comboBoxLCDMode";
            this.comboBoxLCDMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxLCDMode_SelectedIndexChanged);
            // 
            // labeLCDMode
            // 
            resources.ApplyResources(this.labeLCDMode, "labeLCDMode");
            this.labeLCDMode.Name = "labeLCDMode";
            // 
            // comboBoxDrivingMode
            // 
            this.comboBoxDrivingMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDrivingMode.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxDrivingMode, "comboBoxDrivingMode");
            this.comboBoxDrivingMode.Name = "comboBoxDrivingMode";
            this.comboBoxDrivingMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrivingMode_SelectedIndexChanged);
            // 
            // labelDrivingMode
            // 
            resources.ApplyResources(this.labelDrivingMode, "labelDrivingMode");
            this.labelDrivingMode.Name = "labelDrivingMode";
            // 
            // comboBoxContrast
            // 
            this.comboBoxContrast.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxContrast.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxContrast, "comboBoxContrast");
            this.comboBoxContrast.Name = "comboBoxContrast";
            this.comboBoxContrast.SelectedIndexChanged += new System.EventHandler(this.comboBoxContrast_SelectedIndexChanged);
            // 
            // labelContrast
            // 
            resources.ApplyResources(this.labelContrast, "labelContrast");
            this.labelContrast.Name = "labelContrast";
            // 
            // groupBoxWarning
            // 
            resources.ApplyResources(this.groupBoxWarning, "groupBoxWarning");
            this.groupBoxWarning.Controls.Add(this.label1);
            this.groupBoxWarning.Name = "groupBoxWarning";
            this.groupBoxWarning.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // errProvider
            // 
            this.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProvider.ContainerControl = this;
            // 
            // labelNoClock
            // 
            resources.ApplyResources(this.labelNoClock, "labelNoClock");
            this.labelNoClock.Name = "labelNoClock";
            // 
            // CyBasicConfiguration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelNoClock);
            this.Controls.Add(this.groupBoxWarning);
            this.Controls.Add(this.comboBoxContrast);
            this.Controls.Add(this.labelContrast);
            this.Controls.Add(this.comboBoxDrivingMode);
            this.Controls.Add(this.labelDrivingMode);
            this.Controls.Add(this.comboBoxLCDMode);
            this.Controls.Add(this.labeLCDMode);
            this.Controls.Add(this.comboBoxBiasType);
            this.Controls.Add(this.comboBoxFrameRate);
            this.Controls.Add(this.comboBoxWaveformType);
            this.Controls.Add(this.labelFrameRate);
            this.Controls.Add(this.labelWaveformType);
            this.Controls.Add(this.labelBiasType);
            this.Controls.Add(this.numUpDownNumSegmentLines);
            this.Controls.Add(this.labelNumSegmentLines);
            this.Controls.Add(this.labelNumCommonLines);
            this.Controls.Add(this.numUpDownNumCommonLines);
            this.Name = "CyBasicConfiguration";
            this.Load += new System.EventHandler(this.CyBasicConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNumSegmentLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNumCommonLines)).EndInit();
            this.groupBoxWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFrameRate;
        private System.Windows.Forms.ComboBox comboBoxWaveformType;
        private System.Windows.Forms.Label labelFrameRate;
        private System.Windows.Forms.Label labelWaveformType;
        private System.Windows.Forms.Label labelBiasType;
        private System.Windows.Forms.NumericUpDown numUpDownNumSegmentLines;
        private System.Windows.Forms.Label labelNumSegmentLines;
        private System.Windows.Forms.Label labelNumCommonLines;
        private System.Windows.Forms.NumericUpDown numUpDownNumCommonLines;
        private System.Windows.Forms.ComboBox comboBoxBiasType;
        private System.Windows.Forms.ComboBox comboBoxLCDMode;
        private System.Windows.Forms.Label labeLCDMode;
        private System.Windows.Forms.ComboBox comboBoxDrivingMode;
        private System.Windows.Forms.Label labelDrivingMode;
        private System.Windows.Forms.ComboBox comboBoxContrast;
        private System.Windows.Forms.Label labelContrast;
        private System.Windows.Forms.GroupBox groupBoxWarning;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errProvider;
        private System.Windows.Forms.Label labelNoClock;
    }
}
