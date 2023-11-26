using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AirlineOrevine;
using ClosedXML.Excel;
using Common.Utilities;
using DbContext.Database;
using Novacode;
using DocX = Novacode.DocX;
using PassengerDb = DbContext.Models.Passenger;
using Table = Novacode.Table;
using Window = System.Windows.Window;

namespace Document.Windows;

public partial class ExportFirstDocumentWindow : Window
{
    public List<PassengerDb> Passengers { get; set; }
    public DataGrid DocumentGrid { get; set; }

    public ExportFirstDocumentWindow(AirlineOrevineDbContext dbContext, DataGrid documentGrid)
    {        
        InitializeComponent();
        DocumentGrid = documentGrid;
    }

    private bool ValidateNameFile()
    {
        if (string.IsNullOrEmpty(NameFileTextBox.Text.Trim()))
        {
            return true;
        }

        return false;
    }

    private List<ExportPassenger> GetDataToExport()
    {
        if (DocumentGrid.ItemsSource == null)
        {
            return new List<ExportPassenger>();
        }

        var l = new List<PassengerDb>((IEnumerable<PassengerDb>)DocumentGrid.ItemsSource);

        var myExportList = new List<ExportPassenger>();

        foreach (var item in l)
        {
            myExportList.Add(new ExportPassenger()
            {
                Id = item.Id.ToString(),
                FirstName = item.FirstName,
                LastName = item.LastName,
                Patronymic = item.Patronymic,
                PassportSeries = item.PassportSeries.ToString(),
                PassportId = item.PassportId.ToString(),
                IssuedBy = item.IssuedBy,
                DateOfIssue = item.DateOfIssue.ToString(),
            });
        }

        myExportList.OrderBy(x => x.Id);
        return myExportList;
    }

    private void ExportPassengerToExcelButton_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateNameFile())
        {
            MessageBox.Show("Введите название файла", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var myExportList = GetDataToExport();

        if (!myExportList.Any())
        {
            MessageBox.Show("Нет данных для экспорта", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string absolutePath = Path.Combine(rootPath, "Documents", NameFileTextBox.Text);
        string xlsxFilePath = $"{absolutePath}.xlsx";
        ExportToExcel(myExportList, xlsxFilePath);
        OpenDir(absolutePath);
    }

    private void ExportPassengerToWordButton_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateNameFile())
        {
            MessageBox.Show("Введите название файла", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        
        var myExportList = GetDataToExport();

        if (!myExportList.Any())
        {
            MessageBox.Show("Нет данных для экспорта", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string absolutePath = Path.Combine(rootPath, "Documents", NameFileTextBox.Text);
        string docxFilePath = $"{absolutePath}.docx";
        ExportToWord(myExportList, docxFilePath);
        OpenDir(absolutePath);
    }

    private void OpenDir(string absolutePath)
    {
        string directoryPath = Path.GetDirectoryName(absolutePath);

        try
        {
            Process.Start("explorer.exe", directoryPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при открытии проводника: " + ex.Message);
        }

        MessageBox.Show("Успешно", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        NameFileTextBox.Clear();
    }


    private void ExportToExcel(List<ExportPassenger> myExportList, string xlsxFilePath)
    {
        using (XLWorkbook wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("PassengerData");

            var headers = new[]
                { "ID", "Имя", "Фамилия", "Отчество", "Серия паспорта", "Номер паспорта", "Дата выдачи", "Кем выдан" };
            for (int col = 0; col < headers.Length; col++)
            {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            for (int row = 0; row < myExportList.Count; row++)
            {
                var passenger = myExportList[row];

                ws.Cell(row + 2, 1).Value = passenger.Id;
                ws.Cell(row + 2, 2).Value = passenger.FirstName;
                ws.Cell(row + 2, 3).Value = passenger.LastName;
                ws.Cell(row + 2, 4).Value = passenger.Patronymic;
                ws.Cell(row + 2, 5).Value = passenger.PassportSeries;
                ws.Cell(row + 2, 6).Value = passenger.PassportId;
                ws.Cell(row + 2, 7).Value = passenger.DateOfIssue;
                ws.Cell(row + 2, 8).Value = passenger.IssuedBy;
            }

            wb.SaveAs(xlsxFilePath);
        }
    }

    private void ExportToWord(List<ExportPassenger> myExportList, string docxFilePath)
    {
        using (DocX doc = DocX.Create(docxFilePath))
        {
            int rowCount = myExportList.Count + 1;
            int columnCount = 8;

            Table table = doc.AddTable(rowCount, columnCount);
            table.Design = TableDesign.None;

            for (int col = 0; col < columnCount; col++)
            {
                table.Rows[0].Cells[col].Paragraphs.First().Append(GetColumnName(col));
            }

            for (int row = 1; row < rowCount; row++)
            {
                ExportPassenger passenger = myExportList[row - 1];

                table.Rows[row].Cells[0].Paragraphs.First().Append(passenger.Id);
                table.Rows[row].Cells[1].Paragraphs.First().Append(passenger.FirstName);
                table.Rows[row].Cells[2].Paragraphs.First().Append(passenger.LastName);
                table.Rows[row].Cells[3].Paragraphs.First().Append(passenger.Patronymic);
                table.Rows[row].Cells[4].Paragraphs.First().Append(passenger.PassportSeries);
                table.Rows[row].Cells[5].Paragraphs.First().Append(passenger.PassportId);
                table.Rows[row].Cells[6].Paragraphs.First().Append(passenger.DateOfIssue);
                table.Rows[row].Cells[7].Paragraphs.First().Append(passenger.IssuedBy);
            }

            doc.InsertTable(table);
            doc.Save();
        }
    }

    private string GetColumnName(int columnIndex)
    {
        switch (columnIndex)
        {
            case 0:
                return "ID";
            case 1:
                return "Имя";
            case 2:
                return "Фамилия";
            case 3:
                return "Отчество";
            case 4:
                return "Серия паспорта";
            case 5:
                return "Номер паспорта";
            case 6:
                return "Дата выдачи";
            case 7:
                return "Кем выдан";
            default:
                return string.Empty;
        }
    }
}