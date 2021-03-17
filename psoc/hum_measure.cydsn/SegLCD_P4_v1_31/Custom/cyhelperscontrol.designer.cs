/*******************************************************************************
* Copyright 2013, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions, 
* disclaimers, and limitations in the end user license agreement accompanying 
* the software package with which this file was provided.
********************************************************************************/

using System.Drawing;

namespace SegLCD_P4_v1_31
{
    partial class CyHelpers
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
                if (m_symbols != null)
                {
                    for (int i = 0; i < m_symbols.Count; i++)
                    {
                        m_symbols[i].Dispose();
                    }
                }
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
            this.components = new System.ComponentModel.Container();
            this.panelDisplay = new System.Windows.Forms.Panel();
            this.contextMenuPixels = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRemoveSymbol = new System.Windows.Forms.Button();
            this.labelCharsNum = new System.Windows.Forms.Label();
            this.listBoxAddedHelpers = new System.Windows.Forms.ListBox();
            this.labelAddedHelpers = new System.Windows.Forms.Label();
            this.labelHelpers = new System.Windows.Forms.Label();
            this.listBoxAvailHelpers = new System.Windows.Forms.ListBox();
            this.groupBoxHelperConfig = new System.Windows.Forms.GroupBox();
            this.panelHelperConfig = new System.Windows.Forms.Panel();
            this.textBoxSegmentTitle = new System.Windows.Forms.TextBox();
            this.labelHint = new System.Windows.Forms.Label();
            this.labelPixelName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAddSymbol = new System.Windows.Forms.Button();
            this.buttonRemoveHelper = new System.Windows.Forms.Button();
            this.buttonAddHelper = new System.Windows.Forms.Button();
            this.dataGridMap = new System.Windows.Forms.DataGridView();
            this.groupBoxMap = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.printDocumentPixelMap = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panelHidden = new System.Windows.Forms.Panel();
            this.contextMenuPixels.SuspendLayout();
            this.groupBoxHelperConfig.SuspendLayout();
            this.panelHelperConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMap)).BeginInit();
            this.groupBoxMap.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDisplay
            // 
            this.panelDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplay.AutoScroll = true;
            this.panelDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDisplay.Cursor = System.Windows.Forms.Cursors.Default;
            this.panelDisplay.Location = new System.Drawing.Point(5, 26);
            this.panelDisplay.Name = "panelDisplay";
            this.panelDisplay.Size = new System.Drawing.Size(428, 97);
            this.panelDisplay.TabIndex = 0;
            // 
            // contextMenuPixels
            // 
            this.contextMenuPixels.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem});
            this.contextMenuPixels.Name = "contextMenuPixels";
            this.contextMenuPixels.Size = new System.Drawing.Size(103, 26);
            this.contextMenuPixels.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuPixels_Opening);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // buttonRemoveSymbol
            // 
            this.buttonRemoveSymbol.Image = global::SegLCD_P4_v1_31.Resources.imdelete;
            this.buttonRemoveSymbol.Location = new System.Drawing.Point(40, 3);
            this.buttonRemoveSymbol.Name = "buttonRemoveSymbol";
            this.buttonRemoveSymbol.Size = new System.Drawing.Size(29, 23);
            this.buttonRemoveSymbol.TabIndex = 5;
            this.buttonRemoveSymbol.UseVisualStyleBackColor = true;
            this.buttonRemoveSymbol.Click += new System.EventHandler(this.buttonRemoveSymbol_Click);
            // 
            // labelCharsNum
            // 
            this.labelCharsNum.AutoSize = true;
            this.labelCharsNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCharsNum.Location = new System.Drawing.Point(174, 6);
            this.labelCharsNum.Name = "labelCharsNum";
            this.labelCharsNum.Size = new System.Drawing.Size(15, 16);
            this.labelCharsNum.TabIndex = 4;
            this.labelCharsNum.Text = "0";
            this.labelCharsNum.TextChanged += new System.EventHandler(this.labelCharsNum_TextChanged);
            // 
            // listBoxAddedHelpers
            // 
            this.listBoxAddedHelpers.FormattingEnabled = true;
            this.listBoxAddedHelpers.Location = new System.Drawing.Point(280, 23);
            this.listBoxAddedHelpers.Name = "listBoxAddedHelpers";
            this.listBoxAddedHelpers.Size = new System.Drawing.Size(148, 69);
            this.listBoxAddedHelpers.TabIndex = 3;
            this.listBoxAddedHelpers.SelectedIndexChanged += new System.EventHandler(this.listBoxAddedHelpers_SelectedIndexChanged);
            this.listBoxAddedHelpers.DoubleClick += new System.EventHandler(this.listBoxAddedHelpers_DoubleClick);
            // 
            // labelAddedHelpers
            // 
            this.labelAddedHelpers.AutoSize = true;
            this.labelAddedHelpers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelAddedHelpers.Location = new System.Drawing.Point(277, 7);
            this.labelAddedHelpers.Name = "labelAddedHelpers";
            this.labelAddedHelpers.Size = new System.Drawing.Size(89, 13);
            this.labelAddedHelpers.TabIndex = 6;
            this.labelAddedHelpers.Text = "Selected helpers:";
            // 
            // labelHelpers
            // 
            this.labelHelpers.AutoSize = true;
            this.labelHelpers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelHelpers.Location = new System.Drawing.Point(11, 7);
            this.labelHelpers.Name = "labelHelpers";
            this.labelHelpers.Size = new System.Drawing.Size(46, 13);
            this.labelHelpers.TabIndex = 7;
            this.labelHelpers.Text = "Helpers:";
            // 
            // listBoxAvailHelpers
            // 
            this.listBoxAvailHelpers.FormattingEnabled = true;
            this.listBoxAvailHelpers.Items.AddRange(new object[] {
            "7 Segment",
            "14 Segment",
            "16 Segment",
            "Bargraph and Dial",
            "Matrix"});
            this.listBoxAvailHelpers.Location = new System.Drawing.Point(11, 23);
            this.listBoxAvailHelpers.Name = "listBoxAvailHelpers";
            this.listBoxAvailHelpers.Size = new System.Drawing.Size(150, 69);
            this.listBoxAvailHelpers.TabIndex = 0;
            // 
            // groupBoxHelperConfig
            // 
            this.groupBoxHelperConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxHelperConfig.Controls.Add(this.panelHelperConfig);
            this.groupBoxHelperConfig.Location = new System.Drawing.Point(11, 98);
            this.groupBoxHelperConfig.Name = "groupBoxHelperConfig";
            this.groupBoxHelperConfig.Size = new System.Drawing.Size(442, 162);
            this.groupBoxHelperConfig.TabIndex = 11;
            this.groupBoxHelperConfig.TabStop = false;
            this.groupBoxHelperConfig.Text = "Helper function configuration";
            // 
            // panelHelperConfig
            // 
            this.panelHelperConfig.Controls.Add(this.textBoxSegmentTitle);
            this.panelHelperConfig.Controls.Add(this.labelHint);
            this.panelHelperConfig.Controls.Add(this.labelPixelName);
            this.panelHelperConfig.Controls.Add(this.label1);
            this.panelHelperConfig.Controls.Add(this.buttonRemoveSymbol);
            this.panelHelperConfig.Controls.Add(this.panelDisplay);
            this.panelHelperConfig.Controls.Add(this.labelCharsNum);
            this.panelHelperConfig.Controls.Add(this.buttonAddSymbol);
            this.panelHelperConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHelperConfig.Location = new System.Drawing.Point(3, 16);
            this.panelHelperConfig.Name = "panelHelperConfig";
            this.panelHelperConfig.Size = new System.Drawing.Size(436, 143);
            this.panelHelperConfig.TabIndex = 5;
            this.panelHelperConfig.Visible = false;
            // 
            // textBoxSegmentTitle
            // 
            this.textBoxSegmentTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSegmentTitle.Location = new System.Drawing.Point(342, 5);
            this.textBoxSegmentTitle.Name = "textBoxSegmentTitle";
            this.textBoxSegmentTitle.Size = new System.Drawing.Size(72, 20);
            this.textBoxSegmentTitle.TabIndex = 6;
            this.textBoxSegmentTitle.TextChanged += new System.EventHandler(this.textBoxSegmentTitle_TextChanged);
            this.textBoxSegmentTitle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSegmentTitle_KeyPress);
            this.textBoxSegmentTitle.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSegmentTitle_Validating);
            this.textBoxSegmentTitle.Validated += new System.EventHandler(this.textBoxSegmentTitle_Validated);
            // 
            // labelHint
            // 
            this.labelHint.AutoSize = true;
            this.labelHint.Location = new System.Drawing.Point(8, 126);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(26, 13);
            this.labelHint.TabIndex = 18;
            this.labelHint.Text = "Hint";
            // 
            // labelPixelName
            // 
            this.labelPixelName.AutoSize = true;
            this.labelPixelName.Location = new System.Drawing.Point(231, 8);
            this.labelPixelName.Name = "labelPixelName";
            this.labelPixelName.Size = new System.Drawing.Size(105, 13);
            this.labelPixelName.TabIndex = 7;
            this.labelPixelName.Text = "Selected pixel name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Number of symbols:";
            // 
            // buttonAddSymbol
            // 
            this.buttonAddSymbol.Image = global::SegLCD_P4_v1_31.Resources.imadd;
            this.buttonAddSymbol.Location = new System.Drawing.Point(5, 3);
            this.buttonAddSymbol.Name = "buttonAddSymbol";
            this.buttonAddSymbol.Size = new System.Drawing.Size(29, 23);
            this.buttonAddSymbol.TabIndex = 4;
            this.buttonAddSymbol.UseVisualStyleBackColor = true;
            this.buttonAddSymbol.Click += new System.EventHandler(this.buttonAddSymbol_Click);
            // 
            // buttonRemoveHelper
            // 
            this.buttonRemoveHelper.Image = global::SegLCD_P4_v1_31.Resources.imarrowleft;
            this.buttonRemoveHelper.Location = new System.Drawing.Point(185, 59);
            this.buttonRemoveHelper.Name = "buttonRemoveHelper";
            this.buttonRemoveHelper.Size = new System.Drawing.Size(70, 23);
            this.buttonRemoveHelper.TabIndex = 2;
            this.buttonRemoveHelper.UseVisualStyleBackColor = true;
            this.buttonRemoveHelper.Click += new System.EventHandler(this.buttonRemoveHelper_Click);
            // 
            // buttonAddHelper
            // 
            this.buttonAddHelper.Image = global::SegLCD_P4_v1_31.Resources.imarrowright;
            this.buttonAddHelper.Location = new System.Drawing.Point(185, 30);
            this.buttonAddHelper.Name = "buttonAddHelper";
            this.buttonAddHelper.Size = new System.Drawing.Size(70, 23);
            this.buttonAddHelper.TabIndex = 1;
            this.buttonAddHelper.UseVisualStyleBackColor = true;
            this.buttonAddHelper.Click += new System.EventHandler(this.buttonAddHelper_Click);
            // 
            // dataGridMap
            // 
            this.dataGridMap.AllowDrop = true;
            this.dataGridMap.AllowUserToAddRows = false;
            this.dataGridMap.AllowUserToDeleteRows = false;
            this.dataGridMap.AllowUserToResizeColumns = false;
            this.dataGridMap.AllowUserToResizeRows = false;
            this.dataGridMap.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridMap.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMap.ColumnHeadersVisible = false;
            this.dataGridMap.ContextMenuStrip = this.contextMenuPixels;
            this.dataGridMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridMap.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridMap.Location = new System.Drawing.Point(3, 16);
            this.dataGridMap.MultiSelect = false;
            this.dataGridMap.Name = "dataGridMap";
            this.dataGridMap.RowHeadersVisible = false;
            this.dataGridMap.RowTemplate.Height = 18;
            this.dataGridMap.ShowEditingIcon = false;
            this.dataGridMap.Size = new System.Drawing.Size(436, 101);
            this.dataGridMap.TabIndex = 14;
            this.dataGridMap.TabStop = false;
            this.dataGridMap.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridMap_CellBeginEdit);
            this.dataGridMap.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellDoubleClick);
            this.dataGridMap.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellEndEdit);
            this.dataGridMap.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridMap_CellMouseDown);
            this.dataGridMap.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridMap_CellValidating);
            this.dataGridMap.SelectionChanged += new System.EventHandler(this.dataGridMap_SelectionChanged);
            this.dataGridMap.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragDrop);
            this.dataGridMap.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragOver);
            this.dataGridMap.DragLeave += new System.EventHandler(this.dataGridMap_DragLeave);
            this.dataGridMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridMap_MouseMove);
            // 
            // groupBoxMap
            // 
            this.groupBoxMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMap.Controls.Add(this.dataGridMap);
            this.groupBoxMap.Controls.Add(this.groupBox3);
            this.groupBoxMap.Controls.Add(this.groupBox2);
            this.groupBoxMap.Location = new System.Drawing.Point(11, 266);
            this.groupBoxMap.MinimumSize = new System.Drawing.Size(0, 50);
            this.groupBoxMap.Name = "groupBoxMap";
            this.groupBoxMap.Size = new System.Drawing.Size(442, 120);
            this.groupBoxMap.TabIndex = 15;
            this.groupBoxMap.TabStop = false;
            this.groupBoxMap.Text = "Pixel mapping table";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dataGridView3);
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.MinimumSize = new System.Drawing.Size(0, 50);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(442, 117);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pixel mapping table";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowDrop = true;
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.AllowUserToResizeColumns = false;
            this.dataGridView3.AllowUserToResizeRows = false;
            this.dataGridView3.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.ColumnHeadersVisible = false;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView3.Location = new System.Drawing.Point(3, 16);
            this.dataGridView3.MultiSelect = false;
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.RowTemplate.Height = 18;
            this.dataGridView3.ShowEditingIcon = false;
            this.dataGridView3.Size = new System.Drawing.Size(436, 98);
            this.dataGridView3.TabIndex = 14;
            this.dataGridView3.TabStop = false;
            this.dataGridView3.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridMap_CellBeginEdit);
            this.dataGridView3.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellDoubleClick);
            this.dataGridView3.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellEndEdit);
            this.dataGridView3.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridMap_CellMouseDown);
            this.dataGridView3.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridMap_CellValidating);
            this.dataGridView3.SelectionChanged += new System.EventHandler(this.dataGridMap_SelectionChanged);
            this.dataGridView3.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragDrop);
            this.dataGridView3.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragOver);
            this.dataGridView3.DragLeave += new System.EventHandler(this.dataGridMap_DragLeave);
            this.dataGridView3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridMap_MouseMove);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dataGridView2);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.MinimumSize = new System.Drawing.Size(0, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 120);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pixel mapping table";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowDrop = true;
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.ColumnHeadersVisible = false;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView2.Location = new System.Drawing.Point(3, 16);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowTemplate.Height = 18;
            this.dataGridView2.ShowEditingIcon = false;
            this.dataGridView2.Size = new System.Drawing.Size(436, 101);
            this.dataGridView2.TabIndex = 14;
            this.dataGridView2.TabStop = false;
            this.dataGridView2.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridMap_CellBeginEdit);
            this.dataGridView2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellDoubleClick);
            this.dataGridView2.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellEndEdit);
            this.dataGridView2.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridMap_CellMouseDown);
            this.dataGridView2.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridMap_CellValidating);
            this.dataGridView2.SelectionChanged += new System.EventHandler(this.dataGridMap_SelectionChanged);
            this.dataGridView2.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragDrop);
            this.dataGridView2.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragOver);
            this.dataGridView2.DragLeave += new System.EventHandler(this.dataGridMap_DragLeave);
            this.dataGridView2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridMap_MouseMove);
            // 
            // printDocumentPixelMap
            // 
            this.printDocumentPixelMap.OriginAtMargins = true;
            this.printDocumentPixelMap.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocumentPixelMap_PrintPage);
            // 
            // printDialog1
            // 
            this.printDialog1.Document = this.printDocumentPixelMap;
            this.printDialog1.UseEXDialog = true;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPrint.Location = new System.Drawing.Point(11, 388);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(94, 23);
            this.buttonPrint.TabIndex = 16;
            this.buttonPrint.Text = "Print table";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Location = new System.Drawing.Point(11, 266);
            this.groupBox1.MinimumSize = new System.Drawing.Size(0, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(442, 120);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pixel mapping table";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowDrop = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 18;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(436, 101);
            this.dataGridView1.TabIndex = 14;
            this.dataGridView1.TabStop = false;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridMap_CellBeginEdit);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellDoubleClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMap_CellEndEdit);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridMap_CellMouseDown);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridMap_CellValidating);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridMap_SelectionChanged);
            this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragDrop);
            this.dataGridView1.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridMap_DragOver);
            this.dataGridView1.DragLeave += new System.EventHandler(this.dataGridMap_DragLeave);
            this.dataGridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataGridMap_MouseMove);
            // 
            // panelHidden
            // 
            this.panelHidden.Location = new System.Drawing.Point(3, 392);
            this.panelHidden.Name = "panelHidden";
            this.panelHidden.Size = new System.Drawing.Size(10, 10);
            this.panelHidden.TabIndex = 17;
            // 
            // CyHelpers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.groupBoxHelperConfig);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonAddHelper);
            this.Controls.Add(this.groupBoxMap);
            this.Controls.Add(this.buttonRemoveHelper);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxAddedHelpers);
            this.Controls.Add(this.labelHelpers);
            this.Controls.Add(this.listBoxAvailHelpers);
            this.Controls.Add(this.labelAddedHelpers);
            this.Controls.Add(this.panelHidden);
            this.Name = "CyHelpers";
            this.Size = new System.Drawing.Size(456, 414);
            this.Load += new System.EventHandler(this.CyHelpers_Load);
            this.contextMenuPixels.ResumeLayout(false);
            this.groupBoxHelperConfig.ResumeLayout(false);
            this.panelHelperConfig.ResumeLayout(false);
            this.panelHelperConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMap)).EndInit();
            this.groupBoxMap.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelDisplay;
        private System.Windows.Forms.Button buttonAddSymbol;
        private System.Windows.Forms.Button buttonRemoveSymbol;
        private System.Windows.Forms.Label labelCharsNum;
        private System.Windows.Forms.Button buttonRemoveHelper;
        private System.Windows.Forms.Button buttonAddHelper;
        private System.Windows.Forms.ListBox listBoxAddedHelpers;
        private System.Windows.Forms.Label labelAddedHelpers;
        private System.Windows.Forms.Label labelHelpers;
        private System.Windows.Forms.ListBox listBoxAvailHelpers;
        private System.Windows.Forms.GroupBox groupBoxHelperConfig;
        private System.Windows.Forms.Panel panelHelperConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelPixelName;
        private System.Windows.Forms.DataGridView dataGridMap;
        private System.Windows.Forms.ContextMenuStrip contextMenuPixels;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxMap;
        private System.Drawing.Printing.PrintDocument printDocumentPixelMap;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.TextBox textBoxSegmentTitle;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label labelHint;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panelHidden;
    }
}
