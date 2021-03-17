/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Serialization;
using CyDesigner.Extensions.Gde;
using CyDesigner.Extensions.Common;

namespace SegLCD_P4_v1_31
{
    public enum CyHelperKind
    {
        [XmlEnum(Name = "Segment7")]
        SEGMENT_7,
        [XmlEnum(Name = "Segment14")]
        SEGMENT_14,
        [XmlEnum(Name = "Segment16")]
        SEGMENT_16,
        [XmlEnum(Name = "Bar")]
        BAR,
        [XmlEnum(Name = "Matrix")]
        MATRIX,
        [XmlEnum(Name = "Empty")]
        EMPTY
    } ;

    public partial class CyHelpers : CyEditingWrapperControl
    {
        private enum CyPixelNameErrors
        {
            NONE, WRONG_FORMAT, NOT_UNIQUE, EMPTY
        } ;
        private const int MAX_HELPERS_NUM = 8;

        #region Variables

        private System.Windows.Forms.TextBox m_editBox;

        private readonly Size m_symbolSize = new Size(60, 90);
        private readonly Size m_symbolBarSize = new Size(15, 90);
        private const int SYMBOL_SPACE = 3;

        private int m_currentHelperIndex = -1;
        private int m_selectedSymbolIndex = -1;
        private int m_selectedSegmentIndex = -1;

        private readonly List<CyLCDCharacter> m_symbols;

        #endregion Variables

        //--------------------------------------------------------------------------------------------------------------

        #region Constructors

        public CyHelpers()
        {
            InitializeComponent();
        }

        public CyHelpers(CyLCDParameters parameters)
            : base(parameters)
        {
            InitializeComponent();
            InitDataGridMapCellStyle();
            m_symbols = new List<CyLCDCharacter>();
            m_parameters.m_cyHelpersTab = this;
            labelHint.Text = Resources.TIP_PIXELS_DRAG;
            InitDataGridMap();
            UpdateDataGridMapValues();
            LoadHelpers();
        }

        #endregion Constructors

        #region CyEditingWrapperControl overriden functions
        public override string TabName
        {
            get { return "Helpers"; }
        }
        #endregion CyEditingWrapperControl overriden functions

        //--------------------------------------------------------------------------------------------------------------

        #region Choose Helper

        private void buttonAddHelper_Click(object sender, EventArgs e)
        {
            if (listBoxAvailHelpers.SelectedIndex < 0)
                return;

            // If number of helpers is MAX_HELPERS_NUMBER - don't create a new helper
            const int MAX_HELPERS_NUMBER = 8;
            if (m_parameters.m_helpersConfig.Count - 1 >= MAX_HELPERS_NUMBER)
            {
                MessageBox.Show(String.Format(Resources.HELPERS_LIMIT_MSG, MAX_HELPERS_NUMBER),
                                Resources.WARNING_MSG_TITLE, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            // Check if there are enough commons and segments to handle a helper
            bool fitInSpace = true;
            switch (listBoxAvailHelpers.SelectedIndex)
            {
                case 0:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_7SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    CyHelperInfo.CreateHelper(CyHelperKind.SEGMENT_7, m_parameters);
                    break;
                case 1:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_14SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    CyHelperInfo.CreateHelper(CyHelperKind.SEGMENT_14, m_parameters);
                    break;
                case 2:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_16SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    CyHelperInfo.CreateHelper(CyHelperKind.SEGMENT_16, m_parameters);
                    break;
                case 3:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_BAR)
                    {
                        fitInSpace = false;
                        break;
                    }
                    CyHelperInfo.CreateHelper(CyHelperKind.BAR, m_parameters);
                    break;
                case 4:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_MATRIX)
                    {
                        fitInSpace = false;
                        break;
                    }
                    CyHelperInfo.CreateHelper(CyHelperKind.MATRIX, m_parameters);
                    break;
                default:
                    break;
            }

            // If number of common and segment lines is not enough - don't create a new helper
            if (!fitInSpace)
            {
                MessageBox.Show(Resources.NOT_ENOUGH_SEG_COM_MSG, Resources.INFORMATION_MSG_TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            listBoxAddedHelpers.Items.Add(m_parameters.m_helpersConfig[m_parameters.m_helpersConfig.Count - 1]);
            m_parameters.m_helpersConfig[m_parameters.m_helpersConfig.Count - 1].AddSymbol(
                AddNextSymbolIndex(m_parameters.m_helpersConfig[m_parameters.m_helpersConfig.Count - 1].Kind));

            listBoxAddedHelpers.SelectedIndex = listBoxAddedHelpers.Items.Count - 1;
            m_parameters.CommitHelpers();
            SetBtnAddActive();
        }

        private void buttonRemoveHelper_Click(object sender, EventArgs e)
        {
            if (listBoxAddedHelpers.SelectedIndex < 0)
                return;

            int helperIndex = listBoxAddedHelpers.SelectedIndex + 1;

            if (m_parameters.m_helpersConfig.Count > helperIndex)
            {
                m_parameters.m_colorChooser.PushCl(
                    m_parameters.m_helpersConfig[helperIndex].HelperColor);
                for (int i = 0;
                     i < m_parameters.m_helpersConfig[helperIndex].m_helpSegInfo.Count;
                     i++)
                {
                    RemoveSymbolIndex(
                        m_parameters.m_helpersConfig[helperIndex].m_helpSegInfo[i].
                            m_globalDigitNum,
                        m_parameters.m_helpersConfig[helperIndex].Kind);
                }
                CyHelperInfo.RemoveHelperIndex(
                    m_parameters.m_helpersConfig[helperIndex].m_globalHelperIndex,
                    m_parameters.m_helpersConfig[helperIndex].Kind, m_parameters);
                m_parameters.m_helpersConfig.RemoveAt(helperIndex);
                listBoxAddedHelpers.Items.RemoveAt(listBoxAddedHelpers.SelectedIndex);

                if (listBoxAddedHelpers.Items.Count > 0)
                    listBoxAddedHelpers.SelectedIndex = 0;
            }
            m_parameters.CommitHelpers();
            SetBtnAddActive();
        }

        public void LoadHelpers()
        {
            for (int i = 1; i < m_parameters.m_helpersConfig.Count; i++)
            {
                listBoxAddedHelpers.Items.Add(m_parameters.m_helpersConfig[i]);
            }
            if (listBoxAddedHelpers.Items.Count > 0)
                listBoxAddedHelpers.SelectedIndex = 0;

            // Remove used colors from the list
            for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                m_parameters.m_colorChooser.PopCl(m_parameters.m_helpersConfig[i].HelperColor);

            SetBtnAddActive();
        }

        private void listBoxAddedHelpers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetSelection();

            if ((listBoxAddedHelpers.Items.Count == 0) || (listBoxAddedHelpers.SelectedIndex < 0))
            {
                panelHelperConfig.Visible = false;
                m_currentHelperIndex = -1;
            }
            else
            {
                panelHelperConfig.Visible = true;
                for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                {
                    if (m_parameters.m_helpersConfig[i].m_name == listBoxAddedHelpers.SelectedItem.ToString())
                    {
                        if (m_currentHelperIndex != i)
                        {
                            m_currentHelperIndex = i;
                            OpenNewHelper();
                            break;
                        }
                    }
                }
            }

            UpdateDataGridMapValues();
            UpdateLCDChars();
        }

        private void ResetSelection()
        {
            // Reset selection
            m_selectedSymbolIndex = -1;
            m_selectedSegmentIndex = -1;
            textBoxSegmentTitle.Enabled = false;
            textBoxSegmentTitle.Text = "";
        }

        #endregion Choose Helper

        //--------------------------------------------------------------------------------------------------------------

        #region Helper Configuration

        private void OpenNewHelper()
        {
            for (int i = m_symbols.Count - 1; i >= 0; i--)
            {
                RemoveSymbol(i);
            }

            if (m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount > 0)
            {
                for (int i = 0; i < m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount; i++)
                    AddSymbol();
            }
            else
            {
                if (AddSymbol())
                    m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount++;
            }
        }


        //--------------------------------------------------------------------------------------------------------------

        private void buttonAddSymbol_Click(object sender, EventArgs e)
        {
            if (m_currentHelperIndex < 0)
                return;

            if ((m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount >=
                 m_parameters.m_helpersConfig[m_currentHelperIndex].MaxSymbolsCount) ||
                (m_parameters.m_helpersConfig[m_currentHelperIndex].Kind == CyHelperKind.EMPTY))
                return;

            bool fitInSpace = true;
            switch (m_parameters.m_helpersConfig[m_currentHelperIndex].Kind)
            {
                case CyHelperKind.SEGMENT_7:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_7SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    break;
                case CyHelperKind.SEGMENT_14:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_14SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    break;
                case CyHelperKind.SEGMENT_16:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_16SEG)
                    {
                        fitInSpace = false;
                        break;
                    }
                    break;
                case CyHelperKind.BAR:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_BAR)
                    {
                        fitInSpace = false;
                        break;
                    }
                    break;
                case CyHelperKind.MATRIX:
                    if (m_parameters.NumCommonLines*m_parameters.NumSegmentLines -
                        CyHelperInfo.GetTotalPixelNumber(m_parameters) < CyHelperInfo.PIXELS_COUNT_MATRIX)
                    {
                        fitInSpace = false;
                        break;
                    }
                    break;
                default:
                    break;
            }

            // If number of common and segment lines is not enough - don't add a new symbol
            if (!fitInSpace)
            {
                MessageBox.Show(Resources.NOT_ENOUGH_SEG_COM_MSG, Resources.INFORMATION_MSG_TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_parameters.m_helpersConfig[m_currentHelperIndex].AddSymbol(
                AddNextSymbolIndex(m_parameters.m_helpersConfig[m_currentHelperIndex].Kind));
            m_parameters.CommitHelpers();
            AddSymbol();
        }

        private int AddNextSymbolIndex(CyHelperKind kind)
        {
            int index = 0;
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    while (m_parameters.m_symbolIndexes_7SEG.Contains(index))
                        index++;
                    m_parameters.m_symbolIndexes_7SEG.Add(index);
                    break;
                case CyHelperKind.SEGMENT_14:
                    while (m_parameters.m_symbolIndexes_14SEG.Contains(index))
                        index++;
                    m_parameters.m_symbolIndexes_14SEG.Add(index);
                    break;
                case CyHelperKind.SEGMENT_16:
                    while (m_parameters.m_symbolIndexes_16SEG.Contains(index))
                        index++;
                    m_parameters.m_symbolIndexes_16SEG.Add(index);
                    break;
                case CyHelperKind.BAR:
                    while (m_parameters.m_symbolIndexes_BAR.Contains(index))
                        index++;
                    m_parameters.m_symbolIndexes_BAR.Add(index);
                    break;
                case CyHelperKind.MATRIX:
                    while (m_parameters.m_symbolIndexes_MATRIX.Contains(index))
                        index++;
                    m_parameters.m_symbolIndexes_MATRIX.Add(index);
                    break;
                default:
                    break;
            }
            return index;
        }

        private void RemoveSymbolIndex(int index, CyHelperKind kind)
        {
            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    m_parameters.m_symbolIndexes_7SEG.Remove(index);
                    break;
                case CyHelperKind.SEGMENT_14:
                    m_parameters.m_symbolIndexes_14SEG.Remove(index);
                    break;
                case CyHelperKind.SEGMENT_16:
                    m_parameters.m_symbolIndexes_16SEG.Remove(index);
                    break;
                case CyHelperKind.BAR:
                    m_parameters.m_symbolIndexes_BAR.Remove(index);
                    break;
                case CyHelperKind.MATRIX:
                    m_parameters.m_symbolIndexes_MATRIX.Remove(index);
                    break;
                default:
                    break;
            }
        }

        private bool AddSymbol()
        {
            int segCount = m_parameters.m_helpersConfig[m_currentHelperIndex].SegmentCount;
            string[] segmentTitles = new string[segCount];
            string pixelName, defaultName;
            for (int i = 0; i < segCount; i++)
            {
                pixelName =
                    m_parameters.m_helpersConfig[m_currentHelperIndex].GetPixelBySymbolSegment(m_symbols.Count, i).Name;
                defaultName = m_parameters.m_helpersConfig[m_currentHelperIndex].GetDefaultSymbolName(0);
                Regex defaultNamePattern = new Regex(defaultName.TrimEnd('_', '0') + "[0-9]+_");
                if (m_parameters.m_helpersConfig[m_currentHelperIndex].Kind == CyHelperKind.BAR)
                    defaultNamePattern = new Regex(defaultName);

                if (defaultNamePattern.IsMatch(pixelName))
                    segmentTitles[i] = pixelName.Remove(0, defaultName.Length);
                else
                    segmentTitles[i] = pixelName;
            }
            if (m_symbols.Count < m_parameters.m_helpersConfig[m_currentHelperIndex].MaxSymbolsCount)
            {
                Size symSize = m_symbolSize;
                //Size depends on symbol kind
                if (segCount == 1)
                    symSize = m_symbolBarSize;
                CyLCDCharacter symbol = new CyLCDCharacter(symSize,
                                                           m_parameters.m_helpersConfig[m_currentHelperIndex].Kind,
                                                           segmentTitles,
                                                           m_parameters.m_helpersConfig[m_currentHelperIndex].
                                                               HelperColor);
                symbol.m_pBox.Location = new Point(SYMBOL_SPACE + (SYMBOL_SPACE + symSize.Width)*m_symbols.Count,
                                                   SYMBOL_SPACE);
                symbol.SegmentSelected += new EventHandler(symbol_SegmentSelected);
                symbol.m_pBox.ContextMenuStrip = contextMenuPixels;
                m_symbols.Add(symbol);
                labelCharsNum.Text = m_symbols.Count.ToString();
                panelDisplay.Controls.Add(symbol.m_pBox);
                symbol.RedrawAll();
            }
            return true;
        }


        private void buttonRemoveSymbol_Click(object sender, EventArgs e)
        {
            if (m_currentHelperIndex < 0)
                return;

            if (m_symbols.Count > 1)
            {
                for (int i = 0; i < m_parameters.m_helpersConfig[m_currentHelperIndex].m_helpSegInfo.Count; i++)
                {
                    if (m_parameters.m_helpersConfig[m_currentHelperIndex].m_helpSegInfo[i].m_digitNum ==
                        m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount - 1)
                    {
                        RemoveSymbolIndex(
                            m_parameters.m_helpersConfig[m_currentHelperIndex].m_helpSegInfo[i].m_globalDigitNum,
                            m_parameters.m_helpersConfig[m_currentHelperIndex].Kind);
                        m_parameters.m_helpersConfig[m_currentHelperIndex].m_helpSegInfo.RemoveAt(i--);
                    }
                }

                m_parameters.m_helpersConfig[m_currentHelperIndex].SymbolsCount--;
                RemoveSymbol(m_symbols.Count - 1);
                UpdateDataGridMapValues();
                m_parameters.CommitHelpers();
            }
        }

        private void RemoveSymbol(int k)
        {
            //Check if it was selected
            if (m_symbols[k].m_selectedSegment >= 0)
                symbol_SegmentSelected(null, EventArgs.Empty);

            m_symbols[k].m_pBox.Dispose();
            m_symbols.RemoveAt(k);
            labelCharsNum.Text = m_symbols.Count.ToString();
        }

        private void symbol_SegmentSelected(object sender, EventArgs e)
        {
            if (sender == null)
            {
                for (int i = 0; i < m_symbols.Count; i++)
                {
                    if (m_symbols[i].m_selectedSegment >= 0)
                    {
                        m_symbols[i].DeselectSegment();
                    }
                }

                textBoxSegmentTitle.Text = "";
                textBoxSegmentTitle.Enabled = false;
            }
            else
            {
                m_selectedSymbolIndex = m_symbols.IndexOf((CyLCDCharacter)sender);
                m_selectedSegmentIndex = ((CyLCDCharacter) sender).m_selectedSegment;
                for (int i = 0; i < m_symbols.Count; i++)
                {
                    if ((m_symbols[i].m_selectedSegment >= 0) && (i != m_selectedSymbolIndex))
                    {
                        m_symbols[i].DeselectSegment();
                    }
                }
                if (m_selectedSegmentIndex < 0)
                {
                    textBoxSegmentTitle.Text = "";
                    textBoxSegmentTitle.Enabled = false;
                }
                else
                {
                    textBoxSegmentTitle.Enabled = true;
                    if ((m_currentHelperIndex >= 0) && (m_currentHelperIndex < m_parameters.m_helpersConfig.Count))
                    {
                        CyHelperSegmentInfo pixel = m_parameters.m_helpersConfig[m_currentHelperIndex].
                            GetPixelBySymbolSegment(
                            m_selectedSymbolIndex, m_selectedSegmentIndex);
                        if (pixel != null)
                            textBoxSegmentTitle.Text = pixel.Name;
                    }
                }
            }
        }

        private void SetBtnAddActive()
        {
            buttonAddHelper.Enabled = m_parameters.m_helpersConfig.Count < MAX_HELPERS_NUM + 1;
        }

        //--------------------------------------------------------------------------------------------------------------

        private void textBoxSegmentTitle_TextChanged(object sender, EventArgs e)
        {
            if (m_currentHelperIndex < 0 || m_selectedSymbolIndex < 0 || m_selectedSegmentIndex < 0) return;
            CySegmentInfo pixel =
                    m_parameters.m_helpersConfig[m_currentHelperIndex].GetPixelBySymbolSegment(m_selectedSymbolIndex,
                                                                                               m_selectedSegmentIndex);
            if (pixel == null) return;

            // Name was not changed
            if (pixel.Name == textBoxSegmentTitle.Text)
            {
                errorProvider.SetError(textBoxSegmentTitle, String.Empty);
            }
            // Name was changed
            else
            {
                CyPixelNameErrors error = ValidatePixelName(textBoxSegmentTitle.Text);
                // Name is incorrect
                if (error != CyPixelNameErrors.NONE)
                {
                    errorProvider.SetError(textBoxSegmentTitle, GetPixelNameErrorText(error, textBoxSegmentTitle.Text));
                }
                // Name is correct
                else
                {
                    errorProvider.SetError(textBoxSegmentTitle, String.Empty);

                    pixel.Name = textBoxSegmentTitle.Text.ToUpper();

                    //Update title of LCD character's segments. (For Matrix helper - always empty).
                    string defaultName = m_parameters.m_helpersConfig[m_currentHelperIndex].GetDefaultSymbolName(0);
                    Regex defaultNamePattern = new Regex(defaultName.TrimEnd('_', '0') + "[0-9]+_");
                    if (m_parameters.m_helpersConfig[m_currentHelperIndex].Kind == CyHelperKind.BAR)
                        defaultNamePattern = new Regex(defaultName);

                    if (defaultNamePattern.IsMatch(pixel.Name))
                        m_symbols[m_selectedSymbolIndex].m_segments[m_selectedSegmentIndex].
                            m_title =
                            pixel.Name.Remove(0, defaultName.Length);
                    else
                        m_symbols[m_selectedSymbolIndex].m_segments[m_selectedSegmentIndex].
                            m_title =
                            pixel.Name;

                    m_symbols[m_selectedSymbolIndex].DrawSegment(m_selectedSegmentIndex);
                }
            }
        }

        private void textBoxSegmentTitle_Validated(object sender, EventArgs e)
        {
            UpdateDataGridMapValues();
            m_parameters.CommitHelpers();
        }

        private string GetPixelNameErrorText(CyPixelNameErrors error, string text)
        {
            string res = "";
            if (error == CyPixelNameErrors.WRONG_FORMAT)
                res = Resources.WRONG_FORMAT_PARAM_MSG;
            else if (error == CyPixelNameErrors.NOT_UNIQUE)
                res = String.Format(Resources.NOT_UNIQUE_NAME_MSG, text);
            else if (error == CyPixelNameErrors.EMPTY)
                res = Resources.EMPTY_NAME_MSG;
            return res;
        }

        #endregion Helper Configuration

        //--------------------------------------------------------------------------------------------------------------

        #region DataGridMap

        private DataGridViewCellStyle m_cellCommonStyle;
        private DataGridViewCellStyle m_cellSegmentStyle;
        private DataGridViewCellStyle m_cellDisabledStyle;
        private DataGridViewCellStyle m_cellBusyStyle;
        private DataGridViewCellStyle m_cellAssignedStyle;
        private DataGridViewCellStyle m_cellHighlightedStyle;

        private void InitDataGridMapCellStyle()
        {
            dataGridMap.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 7);
            dataGridMap.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            m_cellCommonStyle = new DataGridViewCellStyle();
            m_cellCommonStyle.BackColor = Color.LightSteelBlue;

            m_cellSegmentStyle = new DataGridViewCellStyle();
            m_cellSegmentStyle.BackColor = Color.LightSteelBlue;

            m_cellDisabledStyle = new DataGridViewCellStyle();
            m_cellDisabledStyle.BackColor = Color.FromArgb(200, 200, 200);
            m_cellDisabledStyle.ForeColor = Color.Gray;

            m_cellBusyStyle = new DataGridViewCellStyle();
            m_cellBusyStyle.BackColor = Color.LightGreen;

            m_cellAssignedStyle = new DataGridViewCellStyle();
            m_cellAssignedStyle.BackColor = Color.Lime;

            m_cellHighlightedStyle = new DataGridViewCellStyle();
            m_cellHighlightedStyle.BackColor = Color.Yellow;
        }

        private void UpdateDataGridMapValues()
        {
            for (int i = 1; i < dataGridMap.ColumnCount; i++)
                for (int j = 1; j < dataGridMap.RowCount; j++)
                {
                    dataGridMap[i, j].Value = null;
                    dataGridMap[i, j].Style = dataGridMap.DefaultCellStyle;
                }
            for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                for (int j = 0; j < m_parameters.m_helpersConfig[i].m_helpSegInfo.Count; j++)
                {
                    if ((m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Common >= 0) &&
                        (m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Segment >= 0))
                    {
                        dataGridMap[m_parameters.NumCommonLines - m_parameters.m_helpersConfig[i].
                                    m_helpSegInfo[j].Common,
                                    m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Segment + 1].Value =
                            m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Name;
                        if (i != 0)
                        {
                            dataGridMap[
                                m_parameters.NumCommonLines - m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Common,
                                m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Segment + 1].Style =
                                new DataGridViewCellStyle(m_cellBusyStyle);
                            dataGridMap[
                                m_parameters.NumCommonLines - m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Common,
                                m_parameters.m_helpersConfig[i].m_helpSegInfo[j].Segment + 1].Style.BackColor =
                                m_parameters.m_helpersConfig[i].HelperColor;
                        }
                    }
                }
        }

        public void ComSegLinesNumChanged()
        {
            CyHelperInfo.UpdateEmptyHelper(m_parameters);

            for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                for (int j = 0; j < m_parameters.m_helpersConfig[i].m_helpSegInfo.Count; j++)
                {
                    CyHelperSegmentInfo pixel = m_parameters.m_helpersConfig[i].m_helpSegInfo[j];
                    if ((pixel.Common >= m_parameters.NumCommonLines) ||
                        (pixel.Segment >= m_parameters.NumSegmentLines))
                    {
                        pixel.Common = -1;
                        pixel.Segment = -1;
                    }
                }

            UpdateLCDChars();
            InitDataGridMap();
            UpdateDataGridMapValues();
            m_parameters.CommitHelpers();
        }

        private void InitDataGridMap()
        {
            dataGridMap.Rows.Clear();
            dataGridMap.Columns.Clear();
            for (int i = 0; i < m_parameters.NumCommonLines + 1; i++)
            {
                dataGridMap.Columns.Add(i.ToString(), "-");
            }
            for (int j = 0; j < m_parameters.NumSegmentLines + 1; j++)
            {
                dataGridMap.Rows.Add();
            }

            dataGridMap.Rows[0].Frozen = true;
            dataGridMap.Columns[0].Frozen = true;

            dataGridMap.Columns[0].Width = 45;
            for (int i = 0; i < m_parameters.NumCommonLines; i++)
            {
                dataGridMap.Columns[i + 1].Width = 65;
                dataGridMap[i + 1, 0].Value = "Com" + (m_parameters.NumCommonLines - i - 1);
                dataGridMap[i + 1, 0].Style = m_cellCommonStyle;
            }
            for (int j = 0; j < m_parameters.NumSegmentLines; j++)
            {
                dataGridMap[0, j + 1].Value = "Seg" + (j).ToString();
                dataGridMap[0, j + 1].Style = m_cellSegmentStyle;
            }

            //Change dataGridMap size
            int w = 0;
            for (int i = 0; i < dataGridMap.ColumnCount; i++)
                w += dataGridMap.Columns[i].Width;
            dataGridMap.Width = w + 10;

            int h = 0;
            for (int i = 0; i < dataGridMap.RowCount; i++)
                h += dataGridMap.Rows[i].Height;
            dataGridMap.Height = h + 10;
        }

        private void dataGridMap_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.ColumnIndex > 0) && (e.RowIndex > 0) && (dataGridMap[e.ColumnIndex, e.RowIndex].Value != null))
            {
                string pixelName = (string) dataGridMap[e.ColumnIndex, e.RowIndex].Value;
                for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                {
                    CyHelperInfo helper = m_parameters.m_helpersConfig[i];
                    for (int j = 0; j < helper.m_helpSegInfo.Count; j++)
                    {
                        CyHelperSegmentInfo pixel = helper.m_helpSegInfo[j];
                        if (pixel.Name == pixelName)
                        {
                            if (m_parameters.m_helpersConfig.IndexOf(helper) > 0)
                            {
                                listBoxAddedHelpers.SelectedIndex = m_parameters.m_helpersConfig.IndexOf(helper) - 1;
                                //Select segment
                                ((CyLCDCharacter) m_symbols[pixel.m_digitNum]).SelectSegment(pixel.m_relativePos);
                            }
                        }
                    }
                }
            }
        }

        private void dataGridMap_DragLeave(object sender, EventArgs e)
        {
            for (int i = 1; i < dataGridMap.ColumnCount; i++)
                for (int j = 1; j < dataGridMap.RowCount; j++)
                {
                    if (dataGridMap[i, j].Style == m_cellHighlightedStyle)
                        dataGridMap[i, j].Style = dataGridMap.DefaultCellStyle;
                }
        }

        private void listBoxAddedHelpers_DoubleClick(object sender, EventArgs e)
        {
            CreateEditBox(sender);
        }

        private void dataGridMap_DragOver(object sender, DragEventArgs e)
        {
            Point localPoint = dataGridMap.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = dataGridMap.HitTest(localPoint.X, localPoint.Y);

            //Remove selection
            for (int i = 1; i < dataGridMap.ColumnCount; i++)
                for (int j = 1; j < dataGridMap.RowCount; j++)
                {
                    if (dataGridMap[i, j].Style == m_cellHighlightedStyle)
                        dataGridMap[i, j].Style = dataGridMap.DefaultCellStyle;
                }

            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                if ((hit.RowIndex > 0) && (hit.ColumnIndex > 0) && dataGridMap[hit.ColumnIndex, hit.RowIndex].Style !=
                                                                   m_cellDisabledStyle)
                {
                    e.Effect = e.Data.GetDataPresent(typeof (CyLCDCharacter))
                                   ? DragDropEffects.Move
                                   : DragDropEffects.None;
                    if (dataGridMap[hit.ColumnIndex, hit.RowIndex].Style != m_cellHighlightedStyle)
                    {
                        for (int i = 1; i < dataGridMap.ColumnCount; i++)
                            for (int j = 1; j < dataGridMap.RowCount; j++)
                            {
                                if (dataGridMap[i, j].Style == m_cellHighlightedStyle)
                                    dataGridMap[i, j].Style = dataGridMap.DefaultCellStyle;
                            }
                        if (dataGridMap[hit.ColumnIndex, hit.RowIndex].Style == dataGridMap.DefaultCellStyle)
                            dataGridMap[hit.ColumnIndex, hit.RowIndex].Style = m_cellHighlightedStyle;
                    }
                }
            }

            //Scroll vertical
            if ((localPoint.Y >= dataGridMap.Height - 10) &&
                (dataGridMap.CurrentCell.RowIndex < dataGridMap.RowCount - 1)) //if moving downwards
            {
                dataGridMap.CurrentCell =
                    dataGridMap[dataGridMap.CurrentCell.ColumnIndex, dataGridMap.CurrentCell.RowIndex + 1];
            }
            else if ((localPoint.Y < 10) && (dataGridMap.CurrentCell.RowIndex > 0)) //if moving upwards
            {
                dataGridMap.CurrentCell =
                    dataGridMap[dataGridMap.CurrentCell.ColumnIndex, dataGridMap.CurrentCell.RowIndex - 1];
            }
            //Scroll horizontal
            if ((localPoint.X >= dataGridMap.Width - 20) &&
                (dataGridMap.CurrentCell.ColumnIndex < dataGridMap.ColumnCount - 1)) //if moving to the right
            {
                dataGridMap.CurrentCell =
                    dataGridMap[dataGridMap.CurrentCell.ColumnIndex + 1, dataGridMap.CurrentCell.RowIndex];
            }
            else if ((localPoint.X < 20) && (dataGridMap.CurrentCell.ColumnIndex > 0)) //if moving to the left
            {
                dataGridMap.CurrentCell =
                    dataGridMap[dataGridMap.CurrentCell.ColumnIndex - 1, dataGridMap.CurrentCell.RowIndex];
            }
        }

        private void dataGridMap_DragDrop(object sender, DragEventArgs e)
        {
            Point localPoint = dataGridMap.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = dataGridMap.HitTest(localPoint.X, localPoint.Y);
            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                if ((hit.RowIndex > 0) && (hit.ColumnIndex > 0) && (dataGridMap[hit.ColumnIndex, hit.RowIndex].Style !=
                                                                    m_cellDisabledStyle))
                {
                    CyLCDCharacter lcdChar = (CyLCDCharacter) e.Data.GetData(typeof (CyLCDCharacter));
                    CySegmentInfo pixel1 =
                        m_parameters.m_helpersConfig[m_currentHelperIndex].GetPixelBySymbolSegment(
                            m_selectedSymbolIndex,
                            m_selectedSegmentIndex);
                    ResetPixelCommonSegment(pixel1.Name);
                    lcdChar.DrawSegment(lcdChar.m_selectedSegment, true);
                    dataGridMap[hit.ColumnIndex, hit.RowIndex].Style = new DataGridViewCellStyle(m_cellBusyStyle);
                    dataGridMap[hit.ColumnIndex, hit.RowIndex].Style.BackColor =
                        m_parameters.m_helpersConfig[m_currentHelperIndex].HelperColor;
                    //Set common and segment for pixel
                    pixel1.Common = m_parameters.NumCommonLines - hit.ColumnIndex;
                    pixel1.Segment = hit.RowIndex - 1;
                    if (dataGridMap[hit.ColumnIndex, hit.RowIndex].Value != null)
                    {
                        ResetPixelCommonSegment(dataGridMap[hit.ColumnIndex, hit.RowIndex].Value.ToString());
                    }
                    dataGridMap[hit.ColumnIndex, hit.RowIndex].Value = pixel1.Name;

                    m_parameters.CommitHelpers();
                }
            }
        }

        private void ResetPixelCommonSegment(string name)
        {
            // Omit first helper (Empty helper). It's values never reset.
            for (int i = 1; i < m_parameters.m_helpersConfig.Count; i++)
            {
                CySegmentInfo pixel2 = m_parameters.m_helpersConfig[i].GetPixelByName(name);
                if (pixel2 != null)
                {
                    pixel2.Common = -1;
                    pixel2.Segment = -1;
                    m_parameters.CommitHelpers();
                    UpdateLCDChars();
                    UpdateDataGridMapValues();
                    break;
                }
            }
        }

        private void UpdateLCDChars()
        {
            if (m_currentHelperIndex < 0) return;

            for (int i = 0; i < m_symbols.Count; i++)
                for (int j = 0; j < m_symbols[i].m_segmentsCount; j++)
                {
                    m_symbols[i].RedrawAll();
                    CySegmentInfo pixel1 =
                        m_parameters.m_helpersConfig[m_currentHelperIndex].GetPixelBySymbolSegment(i, j);
                    if ((pixel1 != null) && ((pixel1.Common >= 0) && (pixel1.Segment >= 0)))
                        m_symbols[i].DrawSegment(j, true);
                    else
                        m_symbols[i].DrawSegment(j, false);
                }
        }

        private void dataGridMap_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridMap.SelectedCells.Count > 0)
                dataGridMap.SelectedCells[0].Selected = false;
        }

        private void contextMenuPixels_Opening(object sender, CancelEventArgs e)
        {
            resetToolStripMenuItem.Visible = true;

            //Check if any pixel is selected
            if (contextMenuPixels.SourceControl is PictureBox)
            {
                bool isPixelSelected = false;
                for (int i = 0; i < m_symbols.Count; i++)
                {
                    if (m_symbols[i].m_selectedSegment >= 0)
                    {
                        isPixelSelected = true;
                        break;
                    }
                }
                if (!isPixelSelected)
                {
                    e.Cancel = true;
                }
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuPixels.SourceControl is PictureBox)
            {
                for (int i = 0; i < m_symbols.Count; i++)
                {
                    if (m_symbols[i].m_selectedSegment >= 0)
                    {
                        CySegmentInfo pixel1 =
                            m_parameters.m_helpersConfig[m_currentHelperIndex].GetPixelBySymbolSegment(i,
                                         m_symbols[i].m_selectedSegment);
                        if (pixel1 != null)
                            ResetPixelCommonSegment(pixel1.Name);
                    }
                }
            }
            else if (contextMenuPixels.SourceControl is DataGridView)
            {
                Point localPoint = dataGridMap.PointToClient(contextMenuPixels.Location);
                DataGridView.HitTestInfo hit = dataGridMap.HitTest(localPoint.X, localPoint.Y);

                if (hit.Type == DataGridViewHitTestType.Cell)
                {
                    if ((hit.RowIndex > 0) && (hit.ColumnIndex > 0) &&
                        (dataGridMap[hit.ColumnIndex, hit.RowIndex].Value != null))
                    {
                        ResetPixelCommonSegment((string) dataGridMap[hit.ColumnIndex, hit.RowIndex].Value);
                    }
                }
            }
        }

        private void dataGridMap_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) > 0)
            {
                Point localPoint = new Point(e.X, e.Y);
                DataGridView.HitTestInfo hit = dataGridMap.HitTest(localPoint.X, localPoint.Y);
                if (hit.Type == DataGridViewHitTestType.Cell)
                {
                    if ((hit.RowIndex > 0) && (hit.ColumnIndex > 0) && (dataGridMap[hit.ColumnIndex, hit.RowIndex].Style
                                                                        != m_cellDisabledStyle))
                    {
                        string pixelName = (string) dataGridMap[hit.ColumnIndex, hit.RowIndex].Value;
                        for (int i = 0; i < m_parameters.m_helpersConfig.Count; i++)
                        {
                            CyHelperInfo helper = m_parameters.m_helpersConfig[i];
                            for (int j = 0; j < helper.m_helpSegInfo.Count; j++)
                            {
                                CyHelperSegmentInfo pixel = helper.m_helpSegInfo[j];
                                if (pixel.Name == pixelName)
                                {
                                    if (m_parameters.m_helpersConfig.IndexOf(helper) > 0)
                                    {
                                        CyLCDCharacter lcdChar = m_symbols[pixel.m_digitNum];
                                        dataGridMap.DoDragDrop(lcdChar, DragDropEffects.Move);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dataGridMap_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if ((e.ColumnIndex == 0) || (e.RowIndex == 0))
            {
                e.Cancel = true;
            }
        }

        private void dataGridMap_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Check if cell is not empty
            if (dataGridMap[e.ColumnIndex, e.RowIndex].Value != null)
            {
                dataGridMap[e.ColumnIndex, e.RowIndex].Value =
                    dataGridMap[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();

                // Look up for pixel that corresponds this cell and assign a new value to it.
                int common = m_parameters.NumCommonLines - e.ColumnIndex;
                int segment = e.RowIndex - 1;
                if ((e.ColumnIndex > 0) && (e.RowIndex > 0))
                {
                    bool found = false;
                    // Firstly, look up between helpers
                    for (int i = 1; i < m_parameters.m_helpersConfig.Count; i++)
                    {
                        CyHelperInfo helper = m_parameters.m_helpersConfig[i];
                        for (int j = 0; j < helper.m_helpSegInfo.Count; j++)
                        {
                            CyHelperSegmentInfo pixel = helper.m_helpSegInfo[j];
                            if ((pixel.Common == common) && (pixel.Segment == segment))
                            {
                                textBoxSegmentTitle.Text = dataGridMap[e.ColumnIndex, e.RowIndex].Value.ToString();
                                m_parameters.CommitHelpers();
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                    // Secondly, look up between unused pixels
                    if (!found)
                    {
                        CyHelperInfo helper = m_parameters.m_helpersConfig[0];
                        for (int j = 0; j < helper.m_helpSegInfo.Count; j++)
                        {
                            CyHelperSegmentInfo pixel = helper.m_helpSegInfo[j];
                            if ((pixel.Common == common) && (pixel.Segment == segment))
                            {
                                pixel.Name = dataGridMap[e.ColumnIndex, e.RowIndex].Value.ToString();
                                m_parameters.CommitHelpers();
                            }
                        }
                    }
                }
            }
        }

        private void dataGridMap_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridMap[e.ColumnIndex, e.RowIndex].IsInEditMode)
            {
                try
                {
                    string value = e.FormattedValue.ToString();
                    if (dataGridMap[e.ColumnIndex, e.RowIndex].Value.ToString() != value.ToUpper())
                    {
                        CyPixelNameErrors error = ValidatePixelName(value);
                        if (error != CyPixelNameErrors.NONE)
                        {
                        e.Cancel = true;
                            dataGridMap.Rows[e.RowIndex].ErrorText = GetPixelNameErrorText(error, value);
                    }
                        else
                    {
                            dataGridMap[e.ColumnIndex, e.RowIndex].ErrorText = "";
                        dataGridMap[e.ColumnIndex, e.RowIndex].Value = value.ToUpper();
                    }
                }
                    else
                    {
                        dataGridMap[e.ColumnIndex, e.RowIndex].ErrorText = "";
                    }
                }
                catch
                {
                    e.Cancel = true;
                }
                dataGridMap.Refresh();
            }
        }

        private void dataGridMap_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridMap.BeginEdit(true);
        }

        #endregion

        #region Other events

        private void textBoxSegmentTitle_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = errorProvider.GetError(textBoxSegmentTitle) != String.Empty;
                if (textBoxSegmentTitle.Text != textBoxSegmentTitle.Text.ToUpper())
                {
                    textBoxSegmentTitle.Text = textBoxSegmentTitle.Text.ToUpper();
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        private CyPixelNameErrors ValidatePixelName(string value)
        {
            CyPixelNameErrors res = CyPixelNameErrors.NONE;

            if (value == "")
            {
                res = CyPixelNameErrors.EMPTY;
            }
            else
            {
                if (!CyHelperInfo.CheckPixelUniqueName(m_parameters, value, 0))
                {
                    res = CyPixelNameErrors.NOT_UNIQUE;
                }
                bool wrongChars = false;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!(Char.IsLetterOrDigit(value[i]) || (value[i] == '_')))
                    {
                        wrongChars = true;
                        break;
                    }
                }
                if (wrongChars)
                {
                    res = CyPixelNameErrors.WRONG_FORMAT;
                }
            }
            return res;
        }

        private void textBoxSegmentTitle_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((Char.IsLetterOrDigit(c)) || (c == '_') || Char.IsControl(c)))
            {
                e.Handled = true;
            }
        }

        private void labelCharsNum_TextChanged(object sender, EventArgs e)
        {
            int symbolCount = 0;
            Int32.TryParse(labelCharsNum.Text, out symbolCount);
            if (symbolCount < 2)
            {
                buttonAddSymbol.Focus();
            }
            buttonRemoveSymbol.Enabled = (symbolCount > 1);
        }
        #endregion Other events

        //--------------------------------------------------------------------------------------------------------------

        #region Printing

        private enum CyPixelCellStyle
        {
            HEADER,
            VALUE
        } ;

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            try
            {
                printDocumentPixelMap.DefaultPageSettings.Landscape = true;

                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocumentPixelMap.Print();
                }
            }
            catch
            {
                MessageBox.Show(Resources.PRINT_ERROR, Resources.ERROR_MSG_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int m_lastPrintedColumn, m_lastPrintedRow;
        private int m_columnsPerPage, m_rowsPerPage;

        private void printDocumentPixelMap_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.PageSettings.Landscape = true;

            try
            {
                // Initial calculations
                if (m_columnsPerPage == 0)
                {
                    m_columnsPerPage =
                        (int) ((e.MarginBounds.Right - e.MarginBounds.Left)/(dataGridMap[1, 1].Size.Width*1.5)) - 1;
                    m_rowsPerPage =
                        (int) ((e.MarginBounds.Bottom - e.MarginBounds.Top)/(dataGridMap[1, 1].Size.Height*1.3)) - 1;
                    m_lastPrintedColumn = 0;
                    m_lastPrintedRow = 0;
                }

                int x1, x2, y1, y2;
                x1 = m_lastPrintedRow + 1;
                x2 = Math.Min(x1 + m_rowsPerPage, dataGridMap.RowCount - 1);
                y1 = m_lastPrintedColumn + 1;
                y2 = Math.Min(y1 + m_columnsPerPage, dataGridMap.ColumnCount - 1);

                PrintBlockTable(x1, x2, y1, y2, e.Graphics);

                if (x2 < dataGridMap.RowCount - 1)
                {
                    m_lastPrintedRow = x2;
                    e.HasMorePages = true;
                }
                else if (y2 < dataGridMap.ColumnCount - 1)
                {
                    m_lastPrintedColumn = y2;
                    m_lastPrintedRow = 0;
                    e.HasMorePages = true;
                }
                else
                {
                    m_columnsPerPage = 0;
                    m_rowsPerPage = 0;
                    m_lastPrintedColumn = 0;
                    m_lastPrintedRow = 0;
                    e.HasMorePages = false;
                }
            }
            catch (Exception)
            {
                m_columnsPerPage = 0;
                m_rowsPerPage = 0;
                m_lastPrintedColumn = 0;
                m_lastPrintedRow = 0;
                e.HasMorePages = false;
                MessageBox.Show(Resources.PRINT_TABLE_ERROR_MSG, Resources.ERROR_MSG_TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintBlockTable(int x1, int x2, int y1, int y2, Graphics g)
        {
            Point cellOrigin = new Point(0, 0);
            Size cellSize = new Size();
            int verticalShift = 0;
            Rectangle boundRect;
            for (int i = 0; i <= y2; i++)
            {
                if (i == 1)
                    i = y1;

                cellOrigin.Y = verticalShift;
                cellSize.Width = (int) (dataGridMap.Columns[i].Width*1.5);
                for (int j = 0; j <= x2; j++)
                {
                    if (j == 1)
                        j = x1;

                    cellSize.Height = (int) (dataGridMap[i, j].Size.Height*1.3);

                    if (dataGridMap[i, j].Value != null)
                    {
                        if ((i == 0) || (j == 0))
                            PrintCell(g, cellOrigin, cellSize, dataGridMap[i, j].Value.ToString(),
                                      CyPixelCellStyle.HEADER);
                        else
                            PrintCell(g, cellOrigin, cellSize, dataGridMap[i, j].Value.ToString(),
                                      CyPixelCellStyle.VALUE);
                    }
                    cellOrigin.Y += cellSize.Height;
                }
                cellOrigin.X += cellSize.Width;
            }
            boundRect = new Rectangle(0, verticalShift, cellOrigin.X, cellOrigin.Y - verticalShift);
            using (Pen pen = new Pen(Color.LightGray, 2))
            {
                g.DrawRectangle(pen, boundRect);
            }
        }

        private void PrintCell(Graphics g, Point origin, Size size, string text, CyPixelCellStyle style)
        {
            using (Font font = new Font("Arial", 10))
            using (StringFormat format = new StringFormat())
            using (Pen pen = new Pen(Color.LightGray, 2))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Rectangle boundRect = new Rectangle(origin, size);

                if (style == CyPixelCellStyle.HEADER)
                    g.FillRectangle(Brushes.LightSteelBlue, boundRect);
                else if (style == CyPixelCellStyle.VALUE)
                    g.FillRectangle(Brushes.White, boundRect);
                g.DrawRectangle(pen, boundRect);
                g.DrawString(text, font, Brushes.Black, boundRect, format);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Helpers list colors and items editing

        private void CyHelpers_Load(object sender, EventArgs e)
        {
            listBoxAddedHelpers.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxAddedHelpers.DrawItem += new DrawItemEventHandler(DrawItemHandler);
            listBoxAddedHelpers.MeasureItem += new MeasureItemEventHandler(MeasureItemHandler);

            //EditBox
            m_editBox = new System.Windows.Forms.TextBox();
            m_editBox.Location = new Point(0, 0);
            m_editBox.Size = new Size(0, 0);
            m_editBox.Hide();
            listBoxAddedHelpers.Controls.Add(m_editBox);
            m_editBox.Text = "";
            m_editBox.BorderStyle = BorderStyle.FixedSingle;
            m_editBox.KeyPress += new KeyPressEventHandler(editBox_KeyPress);
            m_editBox.LostFocus += new EventHandler(editBox_LostFocus);
            m_editBox.Validating += new CancelEventHandler(editBox_Validating);
        }

        private void editBox_Validating(object sender, CancelEventArgs e)
        {
            if (!CyHelperInfo.CheckHelperUniqueName(m_parameters, m_editBox.Text))
            {
                // Omit case when the name wasn't changed
                if (((CyHelperInfo) listBoxAddedHelpers.Items[listBoxAddedHelpers.SelectedIndex]).m_name !=
                    m_editBox.Text)
                {
                    MessageBox.Show(String.Format(Resources.NOT_UNIQUE_NAME_MSG, m_editBox.Text),
                                    Resources.WARNING_MSG_TITLE, MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    e.Cancel = true;
                    m_editBox.Show();
                }
            }
        }

        private void editBox_LostFocus(object sender, EventArgs e)
        {
            ((CyHelperInfo) listBoxAddedHelpers.Items[listBoxAddedHelpers.SelectedIndex]).m_name = m_editBox.Text;
            m_parameters.CommitHelpers();
            m_editBox.Hide();
            listBoxAddedHelpers.Refresh();
        }

        private void editBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ((CyHelperInfo) listBoxAddedHelpers.Items[listBoxAddedHelpers.SelectedIndex]).m_name = m_editBox.Text;
                m_parameters.CommitHelpers();
                m_editBox.Hide();
                listBoxAddedHelpers.Refresh();
            }
        }

        private void CreateEditBox(object sender)
        {
            listBoxAddedHelpers = (ListBox)sender;
            int itemSelected = listBoxAddedHelpers.SelectedIndex;
            if (itemSelected > -1)
            {
                Rectangle r = listBoxAddedHelpers.GetItemRectangle(itemSelected);
                const int DELTA = 5;
                string itemText = listBoxAddedHelpers.Items[itemSelected].ToString();

                m_editBox.Location = new System.Drawing.Point(r.X + DELTA, r.Y + DELTA);
                m_editBox.Size = new System.Drawing.Size(r.Width - 10, r.Height - DELTA);
                m_editBox.Show();
                m_editBox.Text = itemText;
                m_editBox.Focus();
                m_editBox.SelectAll();
            }
        }


        private void DrawItemHandler(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                CyHelperInfo item = (CyHelperInfo)(((ListBox)sender).Items[e.Index]);

                Color foreColor = e.ForeColor;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.DrawBackground();
                }
                else
                {
                    using (SolidBrush bbr = new SolidBrush(item.HelperColor))
                        e.Graphics.FillRectangle(bbr, e.Bounds);
                    foreColor = Color.Black;
                }
                using (SolidBrush solidBrush = new SolidBrush(foreColor))
                    e.Graphics.DrawString(item.ToString(), e.Font, solidBrush, e.Bounds, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
                e.Graphics.DrawRectangle(Pens.White, e.Bounds);
            }
        }

        private void MeasureItemHandler(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 16;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region 'Enter' key override
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion 'Enter' key override
    }
    //==================================================================================================================

    #region CyLCDCharacter class

    /// <summary>
    /// Class that defines a character that is drawn on the bitmap
    /// </summary>
    internal class CyLCDCharacter : IDisposable
    {
        public event EventHandler SegmentSelected;

        public int m_segmentsCount;
        public CyLCDSegment[] m_segments;
        private readonly Bitmap m_bmp;
        public PictureBox m_pBox;
        public int m_selectedSegment = -1;
        public Point m_mouseClickLocation;
        private bool m_isDisposed = false;

        private readonly Size m_characterSize;

        public CyLCDCharacter(Size characterSize, CyHelperKind kind, string[] titles, Color highlightedColor)
        {
            m_characterSize = characterSize;

            m_pBox = new PictureBox();
            m_bmp = new Bitmap(m_characterSize.Width, m_characterSize.Height);
            m_pBox.Size = m_characterSize;
            m_pBox.BorderStyle = BorderStyle.None;
            m_pBox.Image = m_bmp;

            switch (kind)
            {
                case CyHelperKind.SEGMENT_7:
                    m_segmentsCount = CyHelperInfo.PIXELS_COUNT_7SEG;
                    m_segments = new CyLCDSegment[m_segmentsCount];
                    for (int i = 0; i < m_segmentsCount; i++)
                    {
                        CyLCDSegment newSegment = new CyLCDSegment(CreateSegment7(i), titles[i], highlightedColor);
                        m_segments[i] = newSegment;
                    }
                    break;
                case CyHelperKind.SEGMENT_14:
                    m_segmentsCount = CyHelperInfo.PIXELS_COUNT_14SEG;
                    m_segments = new CyLCDSegment[m_segmentsCount];
                    for (int i = 0; i < m_segmentsCount; i++)
                    {
                        CyLCDSegment newSegment = new CyLCDSegment(CreateSegment14(i), titles[i], highlightedColor);
                        m_segments[i] = newSegment;
                    }
                    break;
                case CyHelperKind.SEGMENT_16:
                    m_segmentsCount = CyHelperInfo.PIXELS_COUNT_16SEG;
                    m_segments = new CyLCDSegment[m_segmentsCount];
                    for (int i = 0; i < m_segmentsCount; i++)
                    {
                        CyLCDSegment newSegment = new CyLCDSegment(CreateSegment16(i), titles[i], highlightedColor);
                        m_segments[i] = newSegment;
                    }
                    break;
                case CyHelperKind.BAR:
                    m_segmentsCount = CyHelperInfo.PIXELS_COUNT_BAR;
                    m_segments = new CyLCDSegment[m_segmentsCount];
                    for (int i = 0; i < m_segmentsCount; i++)
                    {
                        CyLCDSegment newSegment = new CyLCDSegment(CreateSegmentBar(), titles[i], highlightedColor);
                        m_segments[i] = newSegment;
                    }
                    break;
                case CyHelperKind.MATRIX:
                    m_segmentsCount = CyHelperInfo.PIXELS_COUNT_MATRIX;
                    m_segments = new CyLCDSegment[m_segmentsCount];
                    for (int i = 0; i < m_segmentsCount; i++)
                    {
                        CyLCDSegment newSegment = new CyLCDSegment(CreateSegmentMatrix(CyHelperInfo.MATRIX_WIDTH,
                            CyHelperInfo.MATRIX_HEIGHT, i), "", highlightedColor);
                        m_segments[i] = newSegment;
                    }
                    break;
                default:
                    break;
            }

            m_pBox.MouseDown += new MouseEventHandler(PBox_MouseDown);
            m_pBox.MouseMove += new MouseEventHandler(PBox_MouseMove);
            m_pBox.Show();
        }

        #region Methods
        /// <summary>
        /// Sets highlighting of a pixel and calls a function that redraws it
        /// </summary>
        /// <param name="num"> Index of the pixel </param>
        /// <param name="isLight"> If pixel is highlighted </param>
        public void DrawSegment(int num, bool isLight)
        {
            if (num < m_segmentsCount)
            {
                m_segments[num].IsHighlighted = isLight;
                DrawSegment(num);
            }
        }

        /// <summary>
        /// Draws a pixel
        /// </summary>
        /// <param name="num"> Index of the pixel </param>
        public void DrawSegment(int num)
        {
            if (num < m_segmentsCount)
            {
                m_segments[num].DrawSegment(m_bmp);
                if (num == m_selectedSegment)
                    m_segments[num].DrawSegmentBorder(m_bmp, true);
                m_pBox.Invalidate();
            }
        }

        /// <summary>
        /// Redraws a character
        /// </summary>
        public void RedrawAll()
        {
            Color backColor = Color.WhiteSmoke;
            Graphics g = Graphics.FromImage(m_bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(backColor);
            for (int i = 0; i < m_segmentsCount; i++)
            {
                m_segments[i].DrawSegment(m_bmp);
            }
            m_pBox.Invalidate();
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if any pixel was selected and highlights it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PBox_MouseDown(object sender, MouseEventArgs e)
        {
            m_pBox.Select();
            if (m_pBox.Focused)
            {
                m_selectedSegment = SelectSegment(e.Location);
                SegmentSelected(this, EventArgs.Empty);

                m_mouseClickLocation = e.Location;
            }
        }

        /// <summary>
        /// Performs drag & drop of the selected pixel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) > 0)
            {
                if (Math.Abs(e.X - m_mouseClickLocation.X) + Math.Abs(e.Y - m_mouseClickLocation.Y) > 3)
                {
                    if (m_selectedSegment >= 0) //Drag only if any segment is selected
                        m_pBox.DoDragDrop(this, DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// Redraws a character when pixel is deselected
        /// </summary>
        public void DeselectSegment()
        {
            m_selectedSegment = -1;
            RedrawAll();
        }

        /// <summary>
        /// Defines which pixel is selected and highlights it (by point)
        /// </summary>
        /// <param name="p"> Point of the mouse click </param>
        /// <returns> Index of selected pixel </returns>
        private int SelectSegment(Point p)
        {
            int selectedIndex = -1;

            for (int i = 0; i < m_segmentsCount; i++)
            {
                if (m_segments[i].IsPointInSegment(p))
                {
                    selectedIndex = i;
                    for (int j = 0; j < m_segmentsCount; j++)
                    {
                        if ((m_segments[j].IsSelected) && (i != j))
                        {
                            RedrawAll();
                        }
                    }
                    m_segments[i].IsSelected = true;
                    m_segments[i].DrawSegmentBorder(m_bmp, true);
                    m_pBox.Invalidate();
                    break;
                }
                else
                {
                    //Deselect all
                    RedrawAll();
                    m_pBox.Invalidate();
                }
            }

            return selectedIndex;
        }

        /// <summary>
        /// Highlights a selected pixel.
        /// </summary>
        /// <param name="segmentNum"> Selected pixel </param>
        public void SelectSegment(int segmentNum)
        {
            m_selectedSegment = segmentNum;
            for (int j = 0; j < m_segmentsCount; j++)
            {
                if ((m_segments[j].IsSelected) && (segmentNum != j))
                {
                    RedrawAll();
                }
            }
            m_segments[segmentNum].IsSelected = true;
            m_segments[segmentNum].DrawSegmentBorder(m_bmp, true);
            m_pBox.Invalidate();
            SegmentSelected(this, EventArgs.Empty);
        }

        //--------------------------------------------------------------------------------------------------------------
        #region Create Segments

        private GraphicsPath CreateSegment7(int segment)
        {
            int segWidth = m_bmp.Width/6;
            int fullWidth = m_bmp.Width - 2 - segWidth/2;
            int fullHeight = m_bmp.Height - 2;
            int space = 1;

            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[6];

            switch (segment)
            {
                    //Top
                case 0:
                    points[0] = new Point(segWidth/2 + space, segWidth/2);
                    points[1] = new Point(segWidth + space, 0);
                    points[2] = new Point(fullWidth - (segWidth + space), 0);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), segWidth);
                    points[5] = new Point(segWidth + space, segWidth);
                    break;
                    //Right upper
                case 1:
                    points[0] = new Point(fullWidth - segWidth/2, segWidth/2 + space);
                    points[1] = new Point(fullWidth, segWidth + space);
                    points[2] = new Point(fullWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(fullWidth - segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(fullWidth - segWidth, segWidth + space);
                    break;
                    //Right lower
                case 2:
                    points[0] = new Point(fullWidth - segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(fullWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(fullWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(fullWidth - segWidth, fullHeight - (segWidth + space));
                    points[5] = new Point(fullWidth - segWidth, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Bottom
                case 3:
                    points[0] = new Point(segWidth/2 + space, fullHeight - segWidth/2);
                    points[1] = new Point(segWidth + space, fullHeight - segWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight - segWidth);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight - segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight);
                    points[5] = new Point(segWidth + space, fullHeight);
                    break;
                    //Left lower
                case 4:
                    points[0] = new Point(segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(segWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(segWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(0, fullHeight - (segWidth + space));
                    points[5] = new Point(0, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Left upper
                case 5:
                    points[0] = new Point(segWidth/2, segWidth/2 + space);
                    points[1] = new Point(segWidth, segWidth + space);
                    points[2] = new Point(segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(0, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(0, segWidth + space);
                    break;
                    //Horizontal
                case 6:
                    points[0] = new Point(segWidth/2 + space, fullHeight/2);
                    points[1] = new Point(segWidth + space, fullHeight/2 - segWidth/2);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight/2 - segWidth/2);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight/2 + segWidth/2);
                    points[5] = new Point(segWidth + space, fullHeight/2 + segWidth/2);
                    break;

                default:
                    break;
            }
            // Circle
            if (segment == 7)
            {
                fullWidth += segWidth/2;
                path.AddEllipse(fullWidth - segWidth, fullHeight - segWidth, segWidth, segWidth);
            }
            else
                path.AddPolygon(points);

            return path;
        }

        //--------------------------------------------------------------------------------------------------------------

        private GraphicsPath CreateSegment14(int segment)
        {
            int segWidth = m_bmp.Width/7;
            int diagsegWidth = (int) (segWidth*Math.Sqrt(2));
            int fullWidth = m_bmp.Width - 2;
            int fullHeight = m_bmp.Height - 2;
            int space = 1;

            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[6];

            switch (segment)
            {
                    //Top
                case 0:
                    points[0] = new Point(segWidth/2 + space, segWidth/2);
                    points[1] = new Point(segWidth + space, 0);
                    points[2] = new Point(fullWidth - (segWidth + space), 0);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), segWidth);
                    points[5] = new Point(segWidth + space, segWidth);
                    break;
                    //Right upper
                case 1:
                    points[0] = new Point(fullWidth - segWidth/2, segWidth/2 + space);
                    points[1] = new Point(fullWidth, segWidth + space);
                    points[2] = new Point(fullWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(fullWidth - segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(fullWidth - segWidth, segWidth + space);
                    break;
                    //Right lower
                case 2:
                    points[0] = new Point(fullWidth - segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(fullWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(fullWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(fullWidth - segWidth, fullHeight - (segWidth + space));
                    points[5] = new Point(fullWidth - segWidth, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Bottom
                case 3:
                    points[0] = new Point(segWidth/2 + space, fullHeight - segWidth/2);
                    points[1] = new Point(segWidth + space, fullHeight - segWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight - segWidth);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight - segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight);
                    points[5] = new Point(segWidth + space, fullHeight);
                    break;
                    //Left lower
                case 4:
                    points[0] = new Point(segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(segWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(segWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(0, fullHeight - (segWidth + space));
                    points[5] = new Point(0, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Left upper
                case 5:
                    points[0] = new Point(segWidth/2, segWidth/2 + space);
                    points[1] = new Point(segWidth, segWidth + space);
                    points[2] = new Point(segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(0, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(0, segWidth + space);
                    break;
                    //Horizontal left
                case 6:
                    points[0] = new Point(segWidth/2 + space, fullHeight/2);
                    points[1] = new Point(segWidth + space, fullHeight/2 - segWidth/2);
                    points[2] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight/2 - segWidth/2);
                    points[3] = new Point(fullWidth/2 - space, fullHeight/2);
                    points[4] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight/2 + segWidth/2);
                    points[5] = new Point(segWidth + space, fullHeight/2 + segWidth/2);
                    break;
                    //Diagonal upper left
                case 7:
                    points[0] = new Point(fullWidth/2 - segWidth/2 - space, fullHeight/2 - (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 - segWidth/2 - space,
                                          fullHeight/2 - (segWidth/2 + space) - diagsegWidth);
                    points[2] = new Point(segWidth + space, segWidth + space);
                    points[3] = new Point(segWidth + space, segWidth + space + diagsegWidth);
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Vertical upper
                case 8:
                    points[0] = new Point(fullWidth/2 + segWidth/2, segWidth + space);
                    points[1] = new Point(fullWidth/2 + segWidth/2, fullHeight/2 - (segWidth/2 + space));
                    points[2] = new Point(fullWidth/2, fullHeight/2 - space);
                    points[3] = new Point(fullWidth/2 - segWidth/2, fullHeight/2 - (segWidth/2 + space));
                    points[4] = new Point(fullWidth/2 - segWidth/2, segWidth + space);
                    points[5] = points[0];
                    break;
                    //Diagonal upper right
                case 9:
                    points[0] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 - (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space,
                                          fullHeight/2 - (segWidth/2 + space) - diagsegWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), segWidth + space);
                    points[3] = new Point(fullWidth - (segWidth + space), segWidth + space + diagsegWidth);
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Horizontal right
                case 10:
                    points[0] = new Point(fullWidth/2 + space, fullHeight/2);
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 - segWidth/2);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight/2 - segWidth/2);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight/2 + segWidth/2);
                    points[5] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 + segWidth/2);
                    break;
                    //Diagonal lower right
                case 11:
                    points[0] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 + (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space,
                                          fullHeight/2 + (segWidth/2 + space) + diagsegWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth - (segWidth + space), fullHeight -
                                                                          (segWidth + space + diagsegWidth));
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Vertical lower
                case 12:
                    points[0] = new Point(fullWidth/2, fullHeight/2 + space);
                    points[1] = new Point(fullWidth/2 + segWidth/2, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(fullWidth/2 + segWidth/2, fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth/2 - segWidth/2, fullHeight - (segWidth + space));
                    points[4] = new Point(fullWidth/2 - segWidth/2, fullHeight/2 + segWidth/2 + space);
                    points[5] = points[0];
                    break;
                    //Diagonal lower left
                case 13:
                    points[0] = new Point(fullWidth/2 - segWidth/2 - space, fullHeight/2 + (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 - segWidth/2 - space,
                                          fullHeight/2 + (segWidth/2 + space) + diagsegWidth);
                    points[2] = new Point(segWidth + space, fullHeight - (segWidth + space));
                    points[3] = new Point(segWidth + space, fullHeight - (segWidth + space + diagsegWidth));
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                default:
                    break;
            }
            path.AddPolygon(points);
            return path;
        }

        //--------------------------------------------------------------------------------------------------------------

        private GraphicsPath CreateSegment16(int segment)
        {
            int segWidth = m_bmp.Width/7;
            int diagsegWidth = (int) (segWidth*Math.Sqrt(2));
            int fullWidth = m_bmp.Width - 2;
            int fullHeight = m_bmp.Height - 2;
            int space = 1;

            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[6];

            switch (segment)
            {
                    //Top left
                case 0:
                    points[0] = new Point(segWidth/2 + space, segWidth/2);
                    points[1] = new Point(segWidth + space, 0);
                    points[2] = new Point(fullWidth/2 - (segWidth/2 + space), 0);
                    points[3] = new Point(fullWidth/2 - space, segWidth/2);
                    points[4] = new Point(fullWidth/2 - (segWidth/2 + space), segWidth);
                    points[5] = new Point(segWidth + space, segWidth);
                    break;
                    //Top right
                case 1:
                    points[0] = new Point(fullWidth/2 + space, segWidth/2);
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space, 0);
                    points[2] = new Point(fullWidth - (segWidth + space), 0);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), segWidth);
                    points[5] = new Point(fullWidth/2 + segWidth/2 + space, segWidth);
                    break;
                    //Right upper
                case 2:
                    points[0] = new Point(fullWidth - segWidth/2, segWidth/2 + space);
                    points[1] = new Point(fullWidth, segWidth + space);
                    points[2] = new Point(fullWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(fullWidth - segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(fullWidth - segWidth, segWidth + space);
                    break;
                    //Right lower
                case 3:
                    points[0] = new Point(fullWidth - segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(fullWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(fullWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth - segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(fullWidth - segWidth, fullHeight - (segWidth + space));
                    points[5] = new Point(fullWidth - segWidth, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Bottom right
                case 4:
                    points[0] = new Point(fullWidth/2 + space, fullHeight - segWidth/2);
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight - segWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight - segWidth);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight - segWidth/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight);
                    points[5] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight);
                    break;
                    //Bottom left
                case 5:
                    points[0] = new Point(segWidth/2 + space, fullHeight - segWidth/2);
                    points[1] = new Point(segWidth + space, fullHeight - segWidth);
                    points[2] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight - segWidth);
                    points[3] = new Point(fullWidth/2 - space, fullHeight - segWidth/2);
                    points[4] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight);
                    points[5] = new Point(segWidth + space, fullHeight);
                    break;
                    //Left lower
                case 6:
                    points[0] = new Point(segWidth/2, fullHeight/2 + space);
                    points[1] = new Point(segWidth, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(segWidth, fullHeight - (segWidth + space));
                    points[3] = new Point(segWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(0, fullHeight - (segWidth + space));
                    points[5] = new Point(0, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Left upper
                case 7:
                    points[0] = new Point(segWidth/2, segWidth/2 + space);
                    points[1] = new Point(segWidth, segWidth + space);
                    points[2] = new Point(segWidth, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(segWidth/2, fullHeight/2 - space);
                    points[4] = new Point(0, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(0, segWidth + space);
                    break;
                    //Horizontal left
                case 8:
                    points[0] = new Point(segWidth/2 + space, fullHeight/2);
                    points[1] = new Point(segWidth + space, fullHeight/2 - segWidth/2);
                    points[2] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight/2 - segWidth/2);
                    points[3] = new Point(fullWidth/2 - space, fullHeight/2);
                    points[4] = new Point(fullWidth/2 - (segWidth/2 + space), fullHeight/2 + segWidth/2);
                    points[5] = new Point(segWidth + space, fullHeight/2 + segWidth/2);
                    break;
                    //Diagonal upper left
                case 9:
                    points[0] = new Point(fullWidth/2 - segWidth/2 - space, fullHeight/2 - (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 - segWidth/2 - space,
                                          fullHeight/2 - (segWidth/2 + space) - diagsegWidth);
                    points[2] = new Point(segWidth + space, segWidth + space);
                    points[3] = new Point(segWidth + space, segWidth + space + diagsegWidth);
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Vertical upper
                case 10:
                    points[0] = new Point(fullWidth/2, segWidth/2 + space);
                    points[1] = new Point(fullWidth/2 + segWidth/2, segWidth + space);
                    points[2] = new Point(fullWidth/2 + segWidth/2, fullHeight/2 - (segWidth/2 + space));
                    points[3] = new Point(fullWidth/2, fullHeight/2 - space);
                    points[4] = new Point(fullWidth/2 - segWidth/2, fullHeight/2 - (segWidth/2 + space));
                    points[5] = new Point(fullWidth/2 - segWidth/2, segWidth + space);
                    break;
                    //Diagonal upper right
                case 11:
                    points[0] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 - (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space,
                                          fullHeight/2 - (segWidth/2 + space) - diagsegWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), segWidth + space);
                    points[3] = new Point(fullWidth - (segWidth + space), segWidth + space + diagsegWidth);
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Horizontal right
                case 12:
                    points[0] = new Point(fullWidth/2 + space, fullHeight/2);
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 - segWidth/2);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight/2 - segWidth/2);
                    points[3] = new Point(fullWidth - (segWidth/2 + space), fullHeight/2);
                    points[4] = new Point(fullWidth - (segWidth + space), fullHeight/2 + segWidth/2);
                    points[5] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 + segWidth/2);
                    break;
                    //Diagonal lower right
                case 13:
                    points[0] = new Point(fullWidth/2 + segWidth/2 + space, fullHeight/2 + (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 + segWidth/2 + space,
                                          fullHeight/2 + (segWidth/2 + space) + diagsegWidth);
                    points[2] = new Point(fullWidth - (segWidth + space), fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth - (segWidth + space), fullHeight - (
                                                                                           segWidth + space +
                                                                                           diagsegWidth));
                    points[4] = points[0];
                    points[5] = points[0];
                    break;
                    //Vertical lower
                case 14:
                    points[0] = new Point(fullWidth/2, fullHeight/2 + space);
                    points[1] = new Point(fullWidth/2 + segWidth/2, fullHeight/2 + segWidth/2 + space);
                    points[2] = new Point(fullWidth/2 + segWidth/2, fullHeight - (segWidth + space));
                    points[3] = new Point(fullWidth/2, fullHeight - (segWidth/2 + space));
                    points[4] = new Point(fullWidth/2 - segWidth/2, fullHeight - (segWidth + space));
                    points[5] = new Point(fullWidth/2 - segWidth/2, fullHeight/2 + segWidth/2 + space);
                    break;
                    //Diagonal lower left
                case 15:
                    points[0] = new Point(fullWidth/2 - segWidth/2 - space, fullHeight/2 + (segWidth/2 + space));
                    points[1] = new Point(fullWidth/2 - segWidth/2 - space,
                                          fullHeight/2 + (segWidth/2 + space) + diagsegWidth);
                    points[2] = new Point(segWidth + space, fullHeight - (segWidth + space));
                    points[3] = new Point(segWidth + space, fullHeight - (segWidth + space + diagsegWidth));
                    points[4] = points[0];
                    points[5] = points[0];
                    break;

                default:
                    break;
            }
            path.AddPolygon(points);
            return path;
        }

        //--------------------------------------------------------------------------------------------------------------

        private GraphicsPath CreateSegmentBar()
        {
            int fullWidth = m_bmp.Width - 1;
            int fullHeight = m_bmp.Height - 1;
            int segWidth = fullWidth - 2;
            int segHeight = fullHeight - 2;

            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[4];

            points[0] = new Point(0, 0);
            points[1] = new Point(segWidth, 0);
            points[2] = new Point(segWidth, segHeight);
            points[3] = new Point(0, segHeight);

            path.AddPolygon(points);
            return path;
        }

        //--------------------------------------------------------------------------------------------------------------

        private GraphicsPath CreateSegmentMatrix(int totalSegX, int totalSegY, int segment)
        {
            int fullWidth = m_bmp.Width - 1;
            int fullHeight = m_bmp.Height - 1;
            int space = 1;
            int segWidth = (fullWidth - space*(totalSegX - 1))/totalSegX;
            int segHeight = (fullHeight - space*(totalSegY - 1))/totalSegY;
            int segX = segment%totalSegX;
            int segY = segment/totalSegX;

            GraphicsPath path = new GraphicsPath();
            Point[] points = new Point[4];

            points[0] = new Point((segWidth + space)*segX, (segHeight + space)*segY);
            points[1] = new Point((segWidth + space)*segX + segWidth, (segHeight + space)*segY);
            points[2] = new Point((segWidth + space)*segX + segWidth, (segHeight + space)*segY + segHeight);
            points[3] = new Point((segWidth + space)*segX, (segHeight + space)*segY + segHeight);

            path.AddPolygon(points);
            return path;
        }
        #endregion Create Segments

        #endregion Methods

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    if (m_bmp != null)
                        m_bmp.Dispose();
                    if (m_pBox != null)
                        m_pBox.Dispose();
                    if (m_segments != null)
                    {
                        for (int i = 0; i < m_segments.Length; i++)
                        {
                            m_segments[i].Dispose();
                        }
                    }
                }
            }
            m_isDisposed = true;
        }
        #endregion Dispose
    }

    #endregion

    //==================================================================================================================

    #region CyLCDSegment class

    /// <summary>
    /// CyLCDSegment class represents one segment of the display.
    /// Its purpose is to draw itself and define if the mouse click was on it.
    /// </summary>
    internal class CyLCDSegment : IDisposable
    {
        public string m_title;
        public GraphicsPath m_segmentPath;
        private bool m_isHighlighted;
        private bool m_isSelected;
        private bool m_isMouseOver;
        private readonly Color m_segOnColor = Color.LightGreen;
        private bool m_isDisposed = false;

        public bool IsHighlighted
        {
            get { return m_isHighlighted; }
            set { m_isHighlighted = value; }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; }
        }

        public bool IsMouseOver
        {
            get { return m_isMouseOver; }
            set { m_isMouseOver = value; }
        }

        public CyLCDSegment(GraphicsPath path, string title, Color highlightedColor)
        {
            m_segmentPath = path;
            m_title = title;
            m_segOnColor = highlightedColor;
            IsHighlighted = false;
            IsSelected = false;
        }

        /// <summary>
        /// Draws one pixel of the symbol (highlighted or not)
        /// </summary>
        /// <param name="bmp">Bitmap on which to draw</param>
        public void DrawSegment(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color SegOffColor = Color.LightGray;

            if (IsHighlighted)
            {
                using (SolidBrush brush = new SolidBrush(m_segOnColor))
                {
                    g.FillPath(brush, m_segmentPath);
                }
                IsHighlighted = true;
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(SegOffColor))
                {
                    g.FillPath(brush, m_segmentPath);
                }
                IsHighlighted = false;
            }
            DrawTitle(g);
        }

        /// <summary>
        /// Draws a pixel border (depends on if the pixel is selected).
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="isLight"></param>
        public void DrawSegmentBorder(Bitmap bmp, bool isLight)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color SegPressColor = Color.FromArgb(70, 70, 70);

            if (isLight)
            {
                using (Pen pen = new Pen(SegPressColor, 2))
                {
                    g.DrawPath(pen, m_segmentPath);
                }
                IsSelected = true;
            }
            else
            {
                using (Pen pen = new Pen(m_segOnColor, 2))
                {
                    g.DrawPath(pen, m_segmentPath);
                }
                IsSelected = false;
            }
        }

        /// <summary>
        /// Draw the name of the pixel.
        /// </summary>
        /// <param name="g"></param>
        private void DrawTitle(Graphics g)
        {
            Color textColor = IsHighlighted ? Color.Black : Color.DarkGray;

            using (Font fnt = new Font("Arial", 8))
            using (SolidBrush txtBrush = new SolidBrush(textColor))
            using ( StringFormat drawFormat = new StringFormat())
            {
                RectangleF segBoundRect = m_segmentPath.GetBounds();
                drawFormat.Alignment = StringAlignment.Center;
                drawFormat.LineAlignment = StringAlignment.Center;
                g.DrawString(m_title, fnt, txtBrush, segBoundRect, drawFormat);
            }
        }

        /// <summary>
        /// Defines if a point belongs to the pixel (used when user clicks on the pixel).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsPointInSegment(Point p)
        {
            bool result = false;
            using (Region segRegion = new Region(m_segmentPath))
            {
                if (segRegion.IsVisible(p))
                {
                    result = true;
                }
            }
            return result;
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                    if (m_segmentPath != null)
                        m_segmentPath.Dispose();
                }
            }
            m_isDisposed = true;
        }
        #endregion Dispose
    }

    #endregion
}
