using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using ViewModel.Models;

namespace ViewModel.Helpers
{
    public static class PrintHelper
    {
        public static void PrintViewList(ListView appointmentList)
        {
            PrintDialog pd = new PrintDialog();
            FlowDocument fd = new FlowDocument();
            fd.FontFamily = new FontFamily("Courier New");
            fd.FontSize = 11;
            fd.PagePadding = new Thickness(50);
            fd.ColumnGap = 0;
            fd.ColumnWidth = pd.PrintableAreaWidth;
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Subject",-35}{"Organizer",-15}{"Beginning date",-18}{"Ending date",-18}{"Location",-10}{"User count",-3}"))));
            foreach (var item in appointmentList.ItemContainerGenerator.Items)
            {
                var temp = item as AppointmentModel;
                fd.Blocks.Add(new Paragraph(new Run(String.Format($"{temp.Subject,-35}{temp.AppointmentId,-15}{temp.BeginningDate.ToString("dd-MM-yyyy HH-mm"),-18}{temp.EndingDate.ToString("dd-MM-yyyy HH-mm"),-18}{temp.Room,-15}{temp.Users.Count,-3}"))));
            }
            IDocumentPaginatorSource dps = fd;
            var result = pd.ShowDialog();
            if ((bool)result)
            {
                pd.PrintDocument(dps.DocumentPaginator, "flowdoc");
            }
        }

        public static void PrintAppointment(AppointmentModel appointment)
        {

            PrintDialog pd = new PrintDialog();
            FlowDocument fd = new FlowDocument();
            fd.FontFamily = new FontFamily("Courier New");
            fd.FontSize = 14;
            fd.PagePadding = new Thickness(50);
            fd.ColumnGap = 0;
            fd.ColumnWidth = pd.PrintableAreaWidth;
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Subject:",-15}{appointment.Subject,-50}"))));
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Organizer:",-15}{appointment.AppointmentId,-50}"))));
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Beginning date:",-15}{appointment.BeginningDate.ToString("dd-MM -yyyy HH-mm"),-20}"))));
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Ending date:",-15}{appointment.EndingDate.ToString("dd-MM -yyyy HH-mm"),-20}"))));
            fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Location:",-15}{appointment.Room,-50}"))));
            fd.Blocks.Add(new Paragraph(new Run("Participants")));
            foreach (var item in appointment.Users)
            {
                fd.Blocks.Add(new Paragraph(new Run(String.Format($"{"Name:", -6}{item.Name, -50}"))));
            }
            IDocumentPaginatorSource dps = fd;
            var result = pd.ShowDialog();
            if ((bool)result)
            {
                pd.PrintDocument(dps.DocumentPaginator, "flowdoc");
            }
        }
    }
}
