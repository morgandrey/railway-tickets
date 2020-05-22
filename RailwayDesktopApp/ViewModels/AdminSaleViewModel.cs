using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Mvvm;
using RailwayDesktopApp.Data;
using RailwayDesktopApp.Models;

namespace RailwayDesktopApp.ViewModels {
    public class AdminSaleViewModel : BindableBase {

        #region Properties
        private DateTime timeFrom;
        public DateTime TimeFrom {
            get => timeFrom;
            set => SetProperty(ref timeFrom, value);
        }
        private DateTime timeTo;
        public DateTime TimeTo {
            get => timeTo;
            set => SetProperty(ref timeTo, value);
        }
        private ObservableCollection<Sale> sales;
        public ObservableCollection<Sale> Sales {
            get => sales;
            set {
                SetProperty(ref sales, value);
                CreateReportCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        private DelegateCommand findSaleCommand;
        public DelegateCommand FindSaleCommand =>
            findSaleCommand ??= new DelegateCommand(ExecuteFindSaleCommand);

        private DelegateCommand createReportCommand;
        public DelegateCommand CreateReportCommand =>
            createReportCommand ??= new DelegateCommand(ExecuteCreateReportCommand, CanExecuteCreateReportCommand);
        #endregion

        public AdminSaleViewModel() {
            TimeFrom = DateTime.Now.AddDays(-7);
            TimeTo = DateTime.Now.AddDays(1);
        }
        void ExecuteCreateReportCommand() {
            try {
                var dest = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                           $"/Отчёт по продажам от {TimeFrom:d}.pdf";
                var file = new FileInfo(dest);
                file.Directory.Create();
                var writer = new PdfWriter(dest);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                var font = PdfFontFactory.CreateFont(ShellViewModel.FONT, PdfEncodings.IDENTITY_H);
                float[] columnWidths = { 1, 5, 5 };
                var table = new Table(UnitValue.CreatePercentArray(columnWidths));

                var cell = new Cell(1, 3)
                    .Add(new Paragraph($"Отчёт по продажам {TimeFrom:d} - {TimeTo:d}"))
                    .SetFont(font)
                    .SetFontSize(13)
                    .SetFontColor(DeviceGray.WHITE)
                    .SetBackgroundColor(DeviceGray.BLACK)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                table.AddHeaderCell(cell);

                Cell[] headerFooter = {
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("#")),
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("Дата продажи")),
                    new Cell().SetFont(font).SetBackgroundColor(new DeviceGray(0.75f)).Add(new Paragraph("Цена"))
                };

                foreach (var hfCell in headerFooter) {
                    table.AddHeaderCell(hfCell);
                }

                for (int i = 0; i < Sales.Count; i++) {
                    table.AddCell(new Cell().SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(font).Add(new Paragraph((i + 1).ToString())));
                    table.AddCell(new Cell().SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(font).Add(new Paragraph(sales[i].SaleDate.ToString("g"))));
                    table.AddCell(new Cell().SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(font).Add(new Paragraph(sales[i].TotalPrice.ToString(CultureInfo.InvariantCulture))));
                    if (i == Sales.Count - 1) {
                        table.AddCell(new Cell());
                        table.AddCell(new Cell().SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(font).Add(new Paragraph("Итог:")));
                        table.AddCell(new Cell().SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(font).Add(new Paragraph($"{sales.Sum(x => x.TotalPrice)}")));
                    }
                }
                document.Add(table);
                var process = new Process {
                    StartInfo = new ProcessStartInfo(dest) {
                        UseShellExecute = true
                    }
                };
                process.Start();
                document.Close();
                MessageBox.Show("Билет сохранен в документах");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CanExecuteCreateReportCommand() {
            if (Sales == null) {
                return false;
            }
            return Sales.Count > 0;
        }

        void ExecuteFindSaleCommand() {
            LoadSales(TimeFrom, TimeTo);
        }
        private void LoadSales(DateTime from, DateTime to) {
            try {
                using var dbContext = new RailwaydbContext();
                Sales = new ObservableCollection<Sale>(dbContext.Sale
                    .Include(user => user.IdPassengerNavigation.IdUserNavigation)
                    .Where(x => x.SaleDate >= from && x.SaleDate <= to)
                    .ToList());
                if (Sales.Count == 0) {
                    MessageBox.Show("Не было найдено продаж за данный период");
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}