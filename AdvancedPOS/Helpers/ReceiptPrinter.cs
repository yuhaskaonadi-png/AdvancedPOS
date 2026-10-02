using AdvancedPOS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace AdvancedPOS.Helpers
{
    public class ReceiptPrinter
    {
        private Sale _sale;
        private List<SaleDetail> _details;
        private List<SalePayment> _payments;

        // Thermal Printer එකේ කොළයේ පළල අනුව Font සකස් කරගැනීම
        private Font headerFont = new Font("Courier New", 12, FontStyle.Bold);
        private Font regularFont = new Font("Courier New", 9, FontStyle.Regular);
        private Font boldFont = new Font("Courier New", 9, FontStyle.Bold);

        public ReceiptPrinter(Sale sale, List<SaleDetail> details, List<SalePayment> payments)
        {
            _sale = sale;
            _details = details;
            _payments = payments;
        }

        public void PrintReceipt()
        {
            PrintDocument printDoc = new PrintDocument();
            // Thermal Printer එකේ නම මෙතනින් ලබා දිය හැක (Default Printer එක භාවිතා වේ)
            // printDoc.PrinterSettings.PrinterName = "Your_Thermal_Printer_Name";

            printDoc.PrintPage += new PrintPageEventHandler(PrintPage_Event);

            try
            {
                // Print වීමට පෙර Print Preview එකක් පෙන්වීම (Testing සඳහා ඉතා වැදගත්)
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = printDoc;
                previewDialog.ShowDialog();

                // කෙලින්ම Print වීමට අවශ්‍ය නම් ඉහත පේළි 3 මකා පහත පේළිය පමණක් තබන්න:
                // printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Printer Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintPage_Event(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            int startX = 10;
            int startY = 10;
            int offset = 0;

            // 1. Header (ආයතනයේ නම සහ විස්තර)
            string shopName = "ADVANCED POS (PVT) LTD";
            string address = "123, Main Street, Colombo";
            string phone = "Tel: 011-1234567";

            // මැදට (Center) එන සේ Print කිරීම
            g.DrawString(shopName, headerFont, Brushes.Black, startX + 30, startY + offset);
            offset += 25;
            g.DrawString(address, regularFont, Brushes.Black, startX + 30, startY + offset);
            offset += 15;
            g.DrawString(phone, regularFont, Brushes.Black, startX + 50, startY + offset);
            offset += 20;

            // ඉරි කෑල්ලක් (Dashed Line)
            string dashedLine = "--------------------------------------";
            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;

            // 2. Invoice අංකය සහ දිනය
            string invoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss"); // තාවකාලික Invoice අංකයක්
            g.DrawString("Invoice No: " + invoiceNo, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;
            g.DrawString("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;
            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 20;

            // 3. භාණ්ඩ ලැයිස්තුව (Column Headers)
            g.DrawString("Item", boldFont, Brushes.Black, startX, startY + offset);
            g.DrawString("Qty", boldFont, Brushes.Black, startX + 130, startY + offset);
            g.DrawString("Price", boldFont, Brushes.Black, startX + 170, startY + offset);
            g.DrawString("Total", boldFont, Brushes.Black, startX + 230, startY + offset);
            offset += 15;
            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;

            // භාණ්ඩ එකින් එක Print කිරීම
            foreach (var item in _details)
            {
                // භාණ්ඩයේ නම දිග වැඩි නම් අකුරු 15කට සීමා කිරීම (Thermal Paper එකේ ඉඩ මදි වන නිසා)
                string prodName = "Item #" + item.ProductID; // ඔබගේ Grid එකේ නම ඇත්නම් එය මෙතනට දෙන්න
                if (prodName.Length > 15) prodName = prodName.Substring(0, 15);

                g.DrawString(prodName, regularFont, Brushes.Black, startX, startY + offset);
                g.DrawString(item.Quantity.ToString(), regularFont, Brushes.Black, startX + 135, startY + offset);
                g.DrawString(item.UnitPrice.ToString("0.00"), regularFont, Brushes.Black, startX + 160, startY + offset);
                g.DrawString(item.TotalPrice.ToString("0.00"), regularFont, Brushes.Black, startX + 220, startY + offset);
                offset += 20;
            }

            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;

            // 4. මුළු මුදල සහ වට්ටම් (Totals)
            g.DrawString("Sub Total:", regularFont, Brushes.Black, startX + 100, startY + offset);
            g.DrawString(_sale.SubTotal.ToString("0.00"), regularFont, Brushes.Black, startX + 220, startY + offset);
            offset += 15;

            g.DrawString("Discount:", regularFont, Brushes.Black, startX + 100, startY + offset);
            g.DrawString("-" + _sale.Discount.ToString("0.00"), regularFont, Brushes.Black, startX + 220, startY + offset);
            offset += 15;

            g.DrawString("Grand Total:", boldFont, Brushes.Black, startX + 100, startY + offset);
            g.DrawString(_sale.GrandTotal.ToString("0.00"), boldFont, Brushes.Black, startX + 220, startY + offset);
            offset += 20;

            // 5. ගෙවීම් විස්තර (Payment Details)
            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;

            foreach (var pay in _payments)
            {
                g.DrawString(pay.PaymentMethod + " Paid:", regularFont, Brushes.Black, startX + 100, startY + offset);
                g.DrawString(pay.Amount.ToString("0.00"), regularFont, Brushes.Black, startX + 220, startY + offset);
                offset += 15;
            }

            g.DrawString("Change:", boldFont, Brushes.Black, startX + 100, startY + offset);
            g.DrawString(_sale.ChangeAmount.ToString("0.00"), boldFont, Brushes.Black, startX + 220, startY + offset);
            offset += 20;

            // 6. Footer (ස්තුති පණිවිඩය)
            g.DrawString(dashedLine, regularFont, Brushes.Black, startX, startY + offset);
            offset += 15;
            g.DrawString("Thank You! Come Again.", boldFont, Brushes.Black, startX + 50, startY + offset);
            offset += 15;
            g.DrawString("System by AdvancedPOS", new Font("Courier New", 7, FontStyle.Italic), Brushes.Gray, startX + 70, startY + offset);
        }
    }
}