/*******************************************************************************
* Copyright 2013 - 2015, Cypress Semiconductor Corporation.  All rights reserved.
* You may use this file only in accordance with the license, terms, conditions,
* disclaimers, and limitations in the end user license agreement accompanying
* the software package with which this file was provided.
********************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SegLCD_P4_v1_31
{
    public partial class CyCustomCharacter : UserControl
    {
        #region Fields, Constants

        private CyBox[,] m_box;
        private int m_boxWidth;
        private int m_boxHeight;
        private const int HIGHLIGHT_BORDER_WIDTH = 1;

        #endregion Fields, Constants

        #region Properties: Brushes, Columns, Rows, Name

        private bool m_selected = false;

        private int m_borderWidth = 1;
        private int m_columns = 5;
        private int m_rows = 8;

        private string m_displayName = "Custom Character";

        // Main property
        public byte[] m_charCode = new byte[5];

        public SolidBrush BorderBrush
        {
            get { return (SolidBrush)Brushes.LightGray; }
        }
        public SolidBrush ActiveBrush
        {
            get { return (SolidBrush)Brushes.Black; }
        }
        public SolidBrush InactiveBrush
        {
            get { return (SolidBrush)Brushes.White; }
        }
        public bool Selected
        {
            get { return m_selected; }
            set { m_selected = value; }
        }
        public int BorderWidth
        {
            get { return m_borderWidth; }
            set
            {
                if (value < this.Size.Height & value < this.Size.Width)
                    m_borderWidth = value;
            }
        }
        public int Columns
        {
            get { return m_columns; }
            set
            {
                m_columns = value;
                CheckBoxArray();
            }
        }
        public int Rows
        {
            get { return m_rows; }
            set
            {
                m_rows = value;
                CheckBoxArray();
            }
        }
        public string DisplayName
        {
            get { return m_displayName;}
            set { m_displayName = value;}
        }

        public bool Activate
        {
            get { return m_activate; }
            set { m_activate = value; }
        }
        #endregion

        #region Events declaration

        public event EventHandler CharChangedEvent;

        protected void OnCharChangedEvent()
        {
            if (CharChangedEvent != null)
                CharChangedEvent(this, EventArgs.Empty);
        }

        #endregion Events declaration

        #region Constructor

        public CyCustomCharacter()
        {
            InitializeComponent();
            CheckBoxArray();
            this.MouseDown += new MouseEventHandler(custArray_MouseDown);
            this.MouseMove += new MouseEventHandler(custArray_MouseMove);
            this.MouseUp += new MouseEventHandler(custArray_MouseUp);
        }

        #endregion Constructor

        #region ToString override

        public override string ToString()
        {
            return this.Name;
        }

        #endregion ToString override

        #region Appearance.  Box size. Colors. OnPaint method.
        // Before runtime, if the number of columns or rows is changed, update.
        // Causes loss of box state data.
        private void CheckBoxArray()
        {
            CheckBoxSize();
            m_box = new CyBox[m_rows, m_columns];
            for (int row = 0; row < m_rows; row++)
            {
                for (int column = 0; column < m_columns; column++)
                {
                    m_box[row, column] = new CyBox();
                }
            }
        }

        // Recalculate box width and box height.
        public void CheckBoxSize()
        {
            m_boxWidth = (Size.Width - m_borderWidth) / m_columns;
            m_boxHeight = (Size.Height - m_borderWidth) / m_rows;
            Invalidate();
        }

        // Paint the boxes based on state
        protected override void OnPaint(PaintEventArgs e)
        {

            // Update Borders: Border Widths : Selection
            Graphics graphics = e.Graphics;
            // Draw Right and Bottom Border
            for (int row = 0; row < m_rows; row++)
            {
                for (int column = 0; column < m_columns; column++)
                {
                    if (m_box[row,column].IsActive)
                    {
                        graphics.FillRectangle(BorderBrush, column*m_boxWidth, row*m_boxHeight, m_boxWidth,
                                               m_boxHeight);
                        graphics.FillRectangle(ActiveBrush, column*m_boxWidth + m_borderWidth,
                                               row*m_boxHeight + m_borderWidth, m_boxWidth - m_borderWidth,
                                               m_boxHeight - m_borderWidth);
                    }
                    else
                    {
                        // Draw box which forms top and left border
                        graphics.FillRectangle(BorderBrush, column*m_boxWidth, row*m_boxHeight, m_boxWidth,
                                               m_boxHeight);
                        // Draw standard box over border box so they overlap
                        graphics.FillRectangle(InactiveBrush, column*m_boxWidth + m_borderWidth,
                                               row*m_boxHeight + m_borderWidth, m_boxWidth - m_borderWidth,
                                               m_boxHeight - m_borderWidth);
                    }

                    using (Pen borderPen = new Pen(ActiveBrush.Color, m_borderWidth))
                    {
                        // Alignment == Inset instead of Center
                        borderPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                        graphics.DrawRectangle(borderPen, 0, 0, m_columns*m_boxWidth + m_borderWidth - 1,
                                               m_rows*m_boxHeight + m_borderWidth - 1);
                    }

                    if (m_selected)
                    {
                        using (Pen pen = new Pen(Color.Blue, HIGHLIGHT_BORDER_WIDTH))
                        {
                            // Alignment == Inset instead of Center
                            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                            graphics.DrawRectangle(pen, 0, 0, m_columns*m_boxWidth + m_borderWidth - 1,
                                                   m_rows*m_boxHeight + m_borderWidth - 1);
                        }
                        // Alignment == Center Code.

                        using (Pen pen = new Pen(Color.DodgerBlue, HIGHLIGHT_BORDER_WIDTH))
                        {
                            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                            graphics.DrawRectangle(pen, HIGHLIGHT_BORDER_WIDTH, HIGHLIGHT_BORDER_WIDTH,
                                                   m_columns*m_boxWidth + m_borderWidth - 2*HIGHLIGHT_BORDER_WIDTH - 1,
                                                   m_rows*m_boxHeight + m_borderWidth - 2*HIGHLIGHT_BORDER_WIDTH - 1);
                        }

                    }
                }
            }
        }
        #endregion

        #region Box Manipulation
        /// <summary>
        /// Given a mouse location on the control GetBoxByLocation calculates which
        /// row and column the box is and returns a reference to that box.
        ///
        /// Returns null for invalid values.
        /// </summary>
        /// <param name="x"> X coordinate of mouse click</param>
        /// <param name="y"> Y coordinate of mouse click</param>
        /// <returns> A Box object if a valid location (inside the control) is passed in.
        /// Otherwise it returns null.</returns>
        public CyBox GetBoxByLocation(int x, int y)
        {

            int pixPerRow = (Size.Height - m_borderWidth) / m_rows;
            int row = y / pixPerRow;
            int pixPerCol = (Size.Width - m_borderWidth) / m_columns;
            int column = x / pixPerCol;
            if (row >= 0 && row < m_rows && column >=0 && column < m_columns)
                return m_box[row, column];
            else
                return null;
        }

        /// <summary>
        /// GetBoxArray returns the 2-Dimensional array of boxes ("cells", "pixels", etc) to
        /// allow the user to process the meaning of the states.
        /// </summary>
        /// <returns> A 2-D array of type Box </returns>
        public CyBox[,] GetBoxArray()
        {
            return this.m_box;
        }

        /// <summary>
        /// Match accepts a CustomCharacter as an input and matches the current CustomCharacter
        /// to the the pixel set of the input CustomCharacter
        /// </summary>
        /// <param name="character"> a CustomCharacter object for the current CustomCharacter to copy.</param>
        public void Match(CyCustomCharacter character)
        {
            this.m_box = character.GetBoxArray();
        }

        /// <summary>
        /// Activates one cell.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void SetCell(int row, int column)
        {
            m_box[row, column].IsActive = true;
            Invalidate();
        }

        public void ResetAll()
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    m_box[i, j].IsActive = false;
            Invalidate();
        }

        #endregion

        #region Mouse Events on Custom Characters
        // Toggles for mouse down events.
        bool m_activate = false;

        private void custArray_MouseDown(object sender, MouseEventArgs e)
        {
            CyCustomCharacter current = (CyCustomCharacter)sender;
            CyBox clickedBox = current.GetBoxByLocation(e.X, e.Y);
            if (clickedBox != null)
            {
                clickedBox.IsActive = !clickedBox.IsActive;
                m_activate = clickedBox.IsActive;
                CharChanged();
                current.Invalidate();
            }
        }

        private void custArray_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))
            {
                CyCustomCharacter current = (CyCustomCharacter)sender;
                CyBox currentBox = current.GetBoxByLocation(e.X, e.Y);
                if (currentBox != null)
                {
                    currentBox.IsActive = m_activate;
                    CharChanged();
                    current.Invalidate();
                }
            }
        }

        private void custArray_MouseUp(object sender, MouseEventArgs e)
        {
            OnCharChangedEvent();
        }

        #endregion

        #region Char Changes handling

        public void ChangeChar()
        {
            ResetAll();
            for (int i = 0; i < Columns; i++)
                for (int j = 0; j < Rows; j++)
                {
                    if ((m_charCode[i] & (1 << j)) != 0)
                    {
                        SetCell(j, i);
                    }
                }
        }

        private void CharChanged()
        {
            // Recalculate character byte array
            for (int i = 0; i < Columns; i++)
            {
                byte colVal = 0;
                for (int j = 0; j < Rows; j++)
                {
                    if (m_box[j,i].IsActive)
                    {
                        colVal += (byte)(1 << j);
                    }
                }
                m_charCode[i] = colVal;
            }
        }

        #endregion Char Changes handling
    }

    public class CyBox
    {
        #region Variables and Properties
        // Properties
        private bool m_isActive = false;
        public bool IsActive
        {
            get { return m_isActive; }
            set { m_isActive = value; }
        }
        #endregion
    }
}
