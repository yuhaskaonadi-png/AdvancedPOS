using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AdvancedPOS.Helpers
{
    public static class ThemeManager
    {
        // App එක පුරාම මතක තියාගන්න වත්මන් Theme එක (Session එකට විතරයි, App Restart කළොත් Light වෙනස් වෙනවා)
        public static bool IsDarkMode { get; set; } = false;

        // ==========================================
        // Dashboard Shell වලට පාවිච්චි කරන Color Tokens
        // ==========================================
        public static Color SidebarColor => IsDarkMode ? Color.FromArgb(20, 20, 26) : Color.FromArgb(43, 43, 54);
        public static Color SidebarHoverColor => IsDarkMode ? Color.FromArgb(40, 40, 50) : Color.FromArgb(63, 63, 80);

        public static Color MainBackground => IsDarkMode ? Color.FromArgb(24, 26, 32) : Color.FromArgb(240, 242, 245);
        public static Color HeaderBackground => IsDarkMode ? Color.FromArgb(30, 32, 40) : Color.White;

        public static Color CardBackground => IsDarkMode ? Color.FromArgb(34, 36, 45) : Color.White;
        public static Color CardTitleColor => IsDarkMode ? Color.FromArgb(150, 155, 170) : Color.Gray;

        public static Color PrimaryTextColor => IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(43, 43, 54);
        public static Color SecondaryTextColor => IsDarkMode ? Color.FromArgb(180, 185, 195) : Color.Gray;

        public static Color GridHeaderColor => IsDarkMode ? Color.FromArgb(20, 20, 26) : Color.FromArgb(43, 43, 54);
        public static Color GridBackground => IsDarkMode ? Color.FromArgb(34, 36, 45) : Color.White;
        public static Color GridSelectionColor => IsDarkMode ? Color.FromArgb(50, 53, 65) : Color.FromArgb(236, 240, 241);

        public static Color ChartAreaColor => IsDarkMode ? Color.FromArgb(34, 36, 45) : Color.White;
        public static Color ChartAxisColor => IsDarkMode ? Color.FromArgb(90, 95, 105) : Color.LightGray;
        public static Color ChartGridLineColor => IsDarkMode ? Color.FromArgb(55, 58, 68) : Color.Gainsboro;

        public static Color ClockTextColor => IsDarkMode ? Color.FromArgb(140, 145, 155) : Color.Silver;

        // ==========================================
        // ඕනෑම Form එකක Control Tree එකට Theme එක Automatic විදිහට Apply කිරීම
        // ==========================================
        public static void ApplyToForm(Form form, Color formBackColor, Color panelBackColor,
                                        Color textColor, Color subTextColor, Panel primaryPanel = null,
                                        Color? primaryPanelColor = null)
        {
            form.BackColor = formBackColor;
            ApplyToControls(form.Controls, formBackColor, panelBackColor, textColor, subTextColor,
                             primaryPanel, primaryPanelColor);
        }

        private static void ApplyToControls(Control.ControlCollection controls, Color formBackColor,
                                             Color panelBackColor, Color textColor, Color subTextColor,
                                             Panel primaryPanel, Color? primaryPanelColor)
        {
            foreach (Control ctrl in controls)
            {
                // Sidebar/Left Panel එක වෙනම Color එකකින් තියෙනවා නම් (Dark Sidebar), ඒක Skip කරනවා
                bool isPrimaryPanel = (ctrl == primaryPanel);

                switch (ctrl)
                {
                    case DataGridView grid:
                        grid.BackgroundColor = GridBackground;
                        grid.DefaultCellStyle.BackColor = GridBackground;
                        grid.DefaultCellStyle.ForeColor = textColor;
                        grid.DefaultCellStyle.SelectionBackColor = GridSelectionColor;
                        grid.DefaultCellStyle.SelectionForeColor = textColor;
                        grid.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderColor;
                        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                        grid.GridColor = IsDarkMode ? Color.FromArgb(55, 58, 68) : Color.LightGray;
                        break;

                    case Chart chart:
                        chart.BackColor = ChartAreaColor;
                        foreach (ChartArea area in chart.ChartAreas)
                        {
                            area.BackColor = ChartAreaColor;
                            area.AxisX.LineColor = ChartAxisColor;
                            area.AxisY.LineColor = ChartAxisColor;
                            area.AxisY.MajorGrid.LineColor = ChartGridLineColor;
                            area.AxisX.LabelStyle.ForeColor = textColor;
                            area.AxisY.LabelStyle.ForeColor = textColor;
                        }
                        foreach (Title t in chart.Titles) t.ForeColor = textColor;
                        break;

                    case TextBox txt:
                        txt.BackColor = IsDarkMode ? Color.FromArgb(45, 48, 58) : Color.White;
                        txt.ForeColor = textColor;
                        break;

                    case ComboBox cmb:
                        cmb.BackColor = IsDarkMode ? Color.FromArgb(45, 48, 58) : Color.White;
                        cmb.ForeColor = textColor;
                        break;

                    case NumericUpDown nud:
                        nud.BackColor = IsDarkMode ? Color.FromArgb(45, 48, 58) : Color.White;
                        nud.ForeColor = textColor;
                        break;

                    case Label lbl:
                        if (!isPrimaryPanel)
                        {
                            // Gray/Sub-title පාට Labels (Font Size කුඩා නම් Sub-text කියලා සලකනවා) වෙනම Handle කිරීම
                            if (lbl.ForeColor == Color.Gray)
                                lbl.ForeColor = subTextColor;
                            else if (lbl.ForeColor != Color.White && !IsAccentColor(lbl.ForeColor))
                                lbl.ForeColor = textColor;

                            lbl.BackColor = Color.Transparent;
                        }
                        break;

                    case Panel pnl:
                        if (isPrimaryPanel && primaryPanelColor.HasValue)
                        {
                            pnl.BackColor = primaryPanelColor.Value;
                        }
                        else if (pnl.BackColor == Color.White || pnl.BackColor.ToArgb() == Color.FromArgb(240, 242, 245).ToArgb())
                        {
                            pnl.BackColor = pnl.BackColor == Color.White ? panelBackColor : formBackColor;
                        }
                        break;
                }

                if (ctrl.HasChildren)
                {
                    ApplyToControls(ctrl.Controls, formBackColor, panelBackColor, textColor, subTextColor,
                                     primaryPanel, primaryPanelColor);
                }
            }
        }

        // Accent Colors (Green/Red/Orange/Blue වගේ Semantic Colors) මාරු නොකර තියාගන්න
        private static bool IsAccentColor(Color c)
        {
            return c == Color.FromArgb(39, 174, 96) ||   // Green
                   c == Color.FromArgb(231, 76, 60) ||   // Red
                   c == Color.FromArgb(243, 156, 18) ||  // Orange
                   c == Color.FromArgb(41, 128, 185) ||  // Blue
                   c == Color.ForestGreen || c == Color.Crimson || c == Color.DarkRed ||
                   c == Color.DarkOrange || c == Color.DarkGreen;
        }
    }
}