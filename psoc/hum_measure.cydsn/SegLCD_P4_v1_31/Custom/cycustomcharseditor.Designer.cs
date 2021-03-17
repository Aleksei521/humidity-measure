/*******************************************************************************
* Copyright 2013, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions, 
* disclaimers, and limitations in the end user license agreement accompanying 
* the software package with which this file was provided.
********************************************************************************/

namespace SegLCD_P4_v1_31
{
    partial class CyCustomCharsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CyCustomCharsEditor));
            this.cyCustomCharacter1 = new SegLCD_P4_v1_31.CyCustomCharacter();
            this.listBoxChars = new System.Windows.Forms.ListBox();
            this.toolStripChars = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDefaultList = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonResetAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelCharList = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxCharPreview = new System.Windows.Forms.PictureBox();
            this.labelCharEditor = new System.Windows.Forms.Label();
            this.toolStripChars.SuspendLayout();
            this.panel2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCharPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // cyCustomCharacter1
            // 
            this.cyCustomCharacter1.Activate = false;
            this.cyCustomCharacter1.BorderWidth = 1;
            this.cyCustomCharacter1.Columns = 5;
            this.cyCustomCharacter1.DisplayName = "Custom Character";
            resources.ApplyResources(this.cyCustomCharacter1, "cyCustomCharacter1");
            this.cyCustomCharacter1.MinimumSize = new System.Drawing.Size(5, 5);
            this.cyCustomCharacter1.Name = "cyCustomCharacter1";
            this.cyCustomCharacter1.Rows = 8;
            this.cyCustomCharacter1.Selected = false;
            // 
            // listBoxChars
            // 
            resources.ApplyResources(this.listBoxChars, "listBoxChars");
            this.listBoxChars.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxChars.FormattingEnabled = true;
            this.listBoxChars.MultiColumn = true;
            this.listBoxChars.Name = "listBoxChars";
            this.listBoxChars.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxChars_DrawItem);
            this.listBoxChars.SelectedIndexChanged += new System.EventHandler(this.listBoxChars_SelectedIndexChanged);
            this.listBoxChars.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.listBoxChars_Format);
            // 
            // toolStripChars
            // 
            this.toolStripChars.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripChars.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLoad,
            this.toolStripButtonSave,
            this.toolStripButtonDefaultList,
            this.toolStripSeparator1,
            this.toolStripButtonResetAll,
            this.toolStripButtonReset});
            resources.ApplyResources(this.toolStripChars, "toolStripChars");
            this.toolStripChars.Name = "toolStripChars";
            // 
            // toolStripButtonLoad
            // 
            this.toolStripButtonLoad.Image = global::SegLCD_P4_v1_31.Resources.imopen;
            resources.ApplyResources(this.toolStripButtonLoad, "toolStripButtonLoad");
            this.toolStripButtonLoad.Name = "toolStripButtonLoad";
            this.toolStripButtonLoad.Click += new System.EventHandler(this.toolStripButtonLoad_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Image = global::SegLCD_P4_v1_31.Resources.imsave;
            resources.ApplyResources(this.toolStripButtonSave, "toolStripButtonSave");
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonDefaultList
            // 
            resources.ApplyResources(this.toolStripButtonDefaultList, "toolStripButtonDefaultList");
            this.toolStripButtonDefaultList.Image = global::SegLCD_P4_v1_31.Resources.imdefault;
            this.toolStripButtonDefaultList.Name = "toolStripButtonDefaultList";
            this.toolStripButtonDefaultList.Click += new System.EventHandler(this.toolStripButtonDefaultList_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonResetAll
            // 
            resources.ApplyResources(this.toolStripButtonResetAll, "toolStripButtonResetAll");
            this.toolStripButtonResetAll.Image = global::SegLCD_P4_v1_31.Resources.imundo;
            this.toolStripButtonResetAll.Name = "toolStripButtonResetAll";
            this.toolStripButtonResetAll.Click += new System.EventHandler(this.toolStripButtonResetAll_Click);
            // 
            // toolStripButtonReset
            // 
            resources.ApplyResources(this.toolStripButtonReset, "toolStripButtonReset");
            this.toolStripButtonReset.Image = global::SegLCD_P4_v1_31.Resources.imundo;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelCharList);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // labelCharList
            // 
            resources.ApplyResources(this.labelCharList, "labelCharList");
            this.labelCharList.Name = "labelCharList";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxChars);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.splitContainer1_Panel1_SizeChanged);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.cyCustomCharacter1);
            this.splitContainer1.Panel2.SizeChanged += new System.EventHandler(this.splitContainer1_Panel2_SizeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBoxCharPreview);
            this.panel1.Controls.Add(this.labelCharEditor);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pictureBoxCharPreview
            // 
            resources.ApplyResources(this.pictureBoxCharPreview, "pictureBoxCharPreview");
            this.pictureBoxCharPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCharPreview.Name = "pictureBoxCharPreview";
            this.pictureBoxCharPreview.TabStop = false;
            // 
            // labelCharEditor
            // 
            resources.ApplyResources(this.labelCharEditor, "labelCharEditor");
            this.labelCharEditor.Name = "labelCharEditor";
            // 
            // CyCustomCharsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripChars);
            this.Name = "CyCustomCharsEditor";
            this.Load += new System.EventHandler(this.CyCharsCustomizer_Load);
            this.toolStripChars.ResumeLayout(false);
            this.toolStripChars.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCharPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CyCustomCharacter cyCustomCharacter1;
        private System.Windows.Forms.ListBox listBoxChars;
        private System.Windows.Forms.ToolStrip toolStripChars;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoad;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonResetAll;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripButton toolStripButtonDefaultList;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelCharList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelCharEditor;
        private System.Windows.Forms.PictureBox pictureBoxCharPreview;

    }
}
