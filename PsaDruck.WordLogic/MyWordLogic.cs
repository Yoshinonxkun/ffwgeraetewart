using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Member.Data;
using Member.Data.Interfaces;
using OpenXmlPowerTools;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;

namespace PsaDruck.WordLogic
{
    public class MyWordLogic
    {
        private readonly string _pathExcel;
        private readonly string _pathOut;
        private readonly string _pathTemplateArbeitskleidung;
        private readonly string _pathTemplateEinsatzkleidung;
        private readonly string _pathTemplateHandschuhe;
        private readonly string _pathTemplateHelm;
        private readonly string _pathTemplateKopfschutzhaube;
        private readonly string _pathTemplateSchuhe;
        private readonly string _pathTmp;

        // Constructur
        public MyWordLogic()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _pathOut = currentPath + @"\docs\out.docx";
            _pathTmp = currentPath + @"\docs\tmp\";
            _pathExcel = currentPath + @"\docs\out.xlsx";
            _pathTemplateArbeitskleidung = currentPath + @"\docs\template_arbeitskleidung.docx";
            _pathTemplateEinsatzkleidung = currentPath + @"\docs\template_einsatzkleidung.docx";
            _pathTemplateHandschuhe = currentPath + @"\docs\template_handschuhe.docx";
            _pathTemplateHelm = currentPath + @"\docs\template_helm.docx";
            _pathTemplateKopfschutzhaube = currentPath + @"\docs\template_kopfschutzhaube.docx";
            _pathTemplateSchuhe = currentPath + @"\docs\template_schuhe.docx";

            if (File.Exists(_pathOut)) File.Delete(_pathOut);
        }

        private void MyRegex(Dictionary<string, string> replacement, string template, string output)
        {
            string docText = null;

            // Kopieren des zu bearbeitenden Files
            File.Copy(template, output);

            using (var wordDoc = WordprocessingDocument.Open(output, true))
            {
                using (var sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (var variable in replacement)
                {
                    var regexText = new Regex(variable.Key);
                    docText = regexText.Replace(docText, variable.Value);
                }


                using (var sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }

        public void GetSingle(string type, Member.Data.Member member, Psa psa)
        {
            string template = null;
            var tmp = _pathTmp + type + member.MemberId + ".docx";
            var replace = new Dictionary<string, string>();

            switch (type)
            {
                case "arbeitskleidung":
                    template = _pathTemplateArbeitskleidung;
                    replace.Add("numj", psa.ArbeitsJacke.ToString());
                    replace.Add("numh", psa.ArbeitsHose.ToString());
                    break;

                case "einsatzkleidung":
                    template = _pathTemplateEinsatzkleidung;
                    replace.Add("numj", psa.EinsatzJacke.ToString());
                    replace.Add("numh", psa.EinsatzHose.ToString());
                    break;

                case "handschuhe":
                    template = _pathTemplateHandschuhe;
                    replace.Add("num", psa.Handschuhe.ToString());
                    break;

                case "helm":
                    template = _pathTemplateHelm;
                    replace.Add("num", psa.Helm.ToString());
                    replace.Add("yearh", psa.HelmDate.Month + "." + psa.HelmDate.Year);
                    break;

                case "kopfschutzhaube":
                    template = _pathTemplateKopfschutzhaube;
                    replace.Add("num", psa.Kopfschutzhaube.ToString());
                    break;

                case "schuhe":
                    template = _pathTemplateSchuhe;
                    replace.Add("num", psa.Schuhe.ToString());
                    break;
            }

            replace.Add("lastname", member.Name);
            replace.Add("firstname", member.Surname);
            replace.Add("year", DateTime.Now.Year.ToString());

            MyRegex(replace, template, tmp);
            File.Copy(tmp, _pathOut);
            File.Delete(tmp);
        }

        public void GetAllByMember(Member.Data.Member member, Psa psa)
        {
            var first = true;
            var main = _pathTmp + "main.docx";

            var types = new List<string>();
            types.Add("arbeitskleidung");
            types.Add("einsatzkleidung");
            types.Add("handschuhe");
            types.Add("helm");
            types.Add("kopfschutzhaube");
            types.Add("schuhe");

            foreach (var type in types)
            {
                GetSingle(type, member, psa);

                if (first)
                {
                    File.Copy(_pathOut, main);
                    first = false;
                }
                else
                {
                    var sources = new List<Source>
                    {
                        new Source(new WmlDocument(main), true),
                        new Source(new WmlDocument(_pathOut), true)
                    };

                    DocumentBuilder.BuildDocument(sources, main);
                }

                File.Delete(_pathOut);
            }

            File.Copy(main, _pathOut);
            File.Delete(main);
        }

        public void GetAll(IEnumerable<Member.Data.Member> members, IPsaRepository psaRepository)
        {
            var first = true;
            var main = _pathTmp + "mainall.docx";

            foreach (var member in members)
            {
                GetAllByMember(member, psaRepository.GetPsaByMember(member));

                if (first)
                {
                    File.Copy(_pathOut, main);
                    first = false;
                }
                else
                {
                    var sources = new List<Source>
                    {
                        new Source(new WmlDocument(main), true),
                        new Source(new WmlDocument(_pathOut), true)
                    };

                    DocumentBuilder.BuildDocument(sources, main);
                }

                File.Delete(_pathOut);
            }

            File.Copy(main, _pathOut);
            File.Delete(main);
        }

        public void GetExcelList(IEnumerable<Member.Data.Member> members, IPsaRepository psaRepository)
        {
            if (File.Exists(_pathExcel)) File.Delete(_pathExcel);

            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            var spreadsheetDocument = SpreadsheetDocument.Create(_pathExcel, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            var workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            var sheet = new Sheet
                {Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet"};
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            var cellName = InsertCellInWorksheet("A", 1, worksheetPart);
            cellName.CellValue = new CellValue("Name");
            cellName.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellSurname = InsertCellInWorksheet("B", 1, worksheetPart);
            cellSurname.CellValue = new CellValue("Vorname");
            cellSurname.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellArbeitsJacke = InsertCellInWorksheet("C", 1, worksheetPart);
            cellArbeitsJacke.CellValue = new CellValue("Arbeitsjacke");
            cellArbeitsJacke.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellArbeitsHose = InsertCellInWorksheet("D", 1, worksheetPart);
            cellArbeitsHose.CellValue = new CellValue("Arbeitshose");
            cellArbeitsHose.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellEinsatzJacke = InsertCellInWorksheet("E", 1, worksheetPart);
            cellEinsatzJacke.CellValue = new CellValue("Einsatzjacke");
            cellEinsatzJacke.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellEinsatzHose = InsertCellInWorksheet("F", 1, worksheetPart);
            cellEinsatzHose.CellValue = new CellValue("Einsatzhose");
            cellEinsatzHose.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellHelm = InsertCellInWorksheet("G", 1, worksheetPart);
            cellHelm.CellValue = new CellValue("Helm");
            cellHelm.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellHelmDatum = InsertCellInWorksheet("H", 1, worksheetPart);
            cellHelmDatum.CellValue = new CellValue("Helm-Datum");
            cellHelmDatum.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellKopfschutz = InsertCellInWorksheet("I", 1, worksheetPart);
            cellKopfschutz.CellValue = new CellValue("Kopfschutzhaube");
            cellKopfschutz.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellHandschuhe = InsertCellInWorksheet("J", 1, worksheetPart);
            cellHandschuhe.CellValue = new CellValue("Handschuhe");
            cellHandschuhe.DataType = new EnumValue<CellValues>(CellValues.String);

            var cellSchuhe = InsertCellInWorksheet("K", 1, worksheetPart);
            cellSchuhe.CellValue = new CellValue("Schuhe");
            cellSchuhe.DataType = new EnumValue<CellValues>(CellValues.String);

            var rowindex = 2;
            foreach (var member in members)
            {
                var psa = psaRepository.GetPsaByMember(member);

                cellName = InsertCellInWorksheet("A", (uint) rowindex, worksheetPart);
                cellName.CellValue = new CellValue(member.Name);
                cellName.DataType = new EnumValue<CellValues>(CellValues.String);

                cellSurname = InsertCellInWorksheet("B", (uint) rowindex, worksheetPart);
                cellSurname.CellValue = new CellValue(member.Surname);
                cellSurname.DataType = new EnumValue<CellValues>(CellValues.String);

                cellArbeitsJacke = InsertCellInWorksheet("C", (uint) rowindex, worksheetPart);
                cellArbeitsJacke.CellValue = new CellValue(psa.ArbeitsJacke.ToString());
                cellArbeitsJacke.DataType = new EnumValue<CellValues>(CellValues.String);

                cellArbeitsHose = InsertCellInWorksheet("D", (uint) rowindex, worksheetPart);
                cellArbeitsHose.CellValue = new CellValue(psa.ArbeitsHose.ToString());
                cellArbeitsHose.DataType = new EnumValue<CellValues>(CellValues.String);

                cellEinsatzJacke = InsertCellInWorksheet("E", (uint) rowindex, worksheetPart);
                cellEinsatzJacke.CellValue = new CellValue(psa.EinsatzJacke.ToString());
                cellEinsatzJacke.DataType = new EnumValue<CellValues>(CellValues.String);

                cellEinsatzHose = InsertCellInWorksheet("F", (uint) rowindex, worksheetPart);
                cellEinsatzHose.CellValue = new CellValue(psa.EinsatzHose.ToString());
                cellEinsatzHose.DataType = new EnumValue<CellValues>(CellValues.String);

                cellHelm = InsertCellInWorksheet("G", (uint) rowindex, worksheetPart);
                cellHelm.CellValue = new CellValue(psa.Helm.ToString());
                cellHelm.DataType = new EnumValue<CellValues>(CellValues.String);

                cellHelmDatum = InsertCellInWorksheet("H", (uint) rowindex, worksheetPart);
                cellHelmDatum.CellValue = new CellValue(psa.HelmDate.Month + "." + psa.HelmDate.Year);
                cellHelmDatum.DataType = new EnumValue<CellValues>(CellValues.String);

                cellKopfschutz = InsertCellInWorksheet("I", (uint) rowindex, worksheetPart);
                cellKopfschutz.CellValue = new CellValue(psa.Kopfschutzhaube.ToString());
                cellKopfschutz.DataType = new EnumValue<CellValues>(CellValues.String);

                cellHandschuhe = InsertCellInWorksheet("J", (uint) rowindex, worksheetPart);
                cellHandschuhe.CellValue = new CellValue(psa.Handschuhe.ToString());
                cellHandschuhe.DataType = new EnumValue<CellValues>(CellValues.String);

                cellSchuhe = InsertCellInWorksheet("K", (uint) rowindex, worksheetPart);
                cellSchuhe.CellValue = new CellValue(psa.Schuhe.ToString());
                cellSchuhe.DataType = new EnumValue<CellValues>(CellValues.String);

                rowindex++;
            }

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();

            var startInfo = new ProcessStartInfo
            {
                Arguments = _pathExcel,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row {RowIndex = rowIndex};
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();

            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell refCell = null;
            foreach (var cell in row.Elements<Cell>())
                if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                {
                    refCell = cell;
                    break;
                }

            var newCell = new Cell {CellReference = cellReference};
            row.InsertBefore(newCell, refCell);

            worksheet.Save();
            return newCell;
        }

        public void GetFile()
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = _pathOut,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        private static void Main()
        {
        }
    }
}