using System;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AdvancedPOS.Helpers
{
    public static class ExportHelper
    {
        // ==========================================
        // 1. DataGridView එක Excel (.xlsx) File එකකට Export කිරීම
        // ==========================================
        public static void ExportToExcel(DataGridView dgv, string title)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export.", "Empty Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                sfd.FileName = title.Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(title.Length > 31 ? title.Substring(0, 31) : title);

                        // Header (Column Titles) ලියීම
                        for (int col = 0; col < dgv.Columns.Count; col++)
                        {
                            if (!dgv.Columns[col].Visible) continue;
                            var cell = worksheet.Cell(1, col + 1);
                            cell.Value = dgv.Columns[col].HeaderText;
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(43, 43, 54);
                            cell.Style.Font.FontColor = XLColor.White;
                        }

                        // දත්ත (Rows) ලියීම
                        int rowIndex = 2;
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.IsNewRow) continue;

                            int colIndex = 1;
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (!col.Visible) continue;

                                object value = row.Cells[col.Index].Value;
                                var cell = worksheet.Cell(rowIndex, colIndex);

                                if (value != null)
                                {
                                    if (value is DateTime dt)
                                        cell.Value = dt;
                                    else if (decimal.TryParse(value.ToString(), out decimal num))
                                        cell.Value = num;
                                    else
                                        cell.Value = value.ToString();
                                }
                                colIndex++;
                            }
                            rowIndex++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // 2. DataGridView එක PDF File එකකට Export කිරීම
        // ==========================================
        public static void ExportToPdf(DataGridView dgv, string title)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export.", "Empty Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF Files (*.pdf)|*.pdf";
                sfd.FileName = title.Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    // Visible Columns ගණන අනුව Page Orientation තෝරාගැනීම (Columns වැඩිනම් Landscape)
                    int visibleColCount = 0;
                    foreach (DataGridViewColumn col in dgv.Columns)
                        if (col.Visible) visibleColCount++;

                    Rectangle pageSize = visibleColCount > 4
                        ? PageSize.A4.Rotate()
                        : PageSize.A4;

                    using (Document doc = new Document(pageSize, 25, 25, 30, 30))
                    {
                        PdfWriter.GetInstance(doc, new System.IO.FileStream(sfd.FileName, System.IO.FileMode.Create));
                        doc.Open();

                        // මාතෘකාව
                        Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                        Paragraph heading = new Paragraph(title, titleFont);
                        heading.SpacingAfter = 5f;
                        doc.Add(heading);

                        Font subFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY);
                        Paragraph sub = new Paragraph("Generated on: " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt"), subFont);
                        sub.SpacingAfter = 15f;
                        doc.Add(sub);

                        // Table එක
                        PdfPTable table = new PdfPTable(visibleColCount);
                        table.WidthPercentage = 100;

                        Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                        Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8.5f, BaseColor.BLACK);
                        BaseColor headerBg = new BaseColor(43, 43, 54);

                        // Header Row
                        foreach (DataGridViewColumn col in dgv.Columns)
                        {
                            if (!col.Visible) continue;
                            PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                            cell.BackgroundColor = headerBg;
                            cell.Padding = 6;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);
                        }

                        // Data Rows
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.IsNewRow) continue;

                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (!col.Visible) continue;

                                object val = row.Cells[col.Index].Value;
                                string text = val != null ? val.ToString() : "";

                                PdfPCell cell = new PdfPCell(new Phrase(text, cellFont));
                                cell.Padding = 5;
                                table.AddCell(cell);
                            }
                        }

                        doc.Add(table);
                        doc.Close();
                    }

                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}