namespace Common.Utilities;

public class ExcelExportPassenger
{
    public string Идентификатор { get; set; }
    public string Имя { get; set; }

    public string Фамилия { get; set; }

    public string Отчество { get; set; }

    public string Серия { get; set; }

    public string Номер { get; set; }

    public string Дата_выдачи { get; set; }

    public string Кем_выдан { get; set; }

    public ExcelExportPassenger(ExportPassenger vacancy)
    {
        Идентификатор = vacancy.Id;
        Имя = vacancy.FirstName;
        Фамилия = vacancy.LastName;
        Отчество = vacancy.Patronymic;
        Серия = vacancy.PassportSeries;
        Номер = vacancy.PassportId;
        Дата_выдачи = vacancy.DateOfIssue;
        Кем_выдан = vacancy.IssuedBy;
    }
}