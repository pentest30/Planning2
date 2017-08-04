using System;
using Microsoft.Reporting.WinForms;
using Planing.Reporting;

namespace Planing.Views
{
    /// <summary>
    /// Interaction logic for AffReportWind.xaml
    /// </summary>
    public partial class AffReportWind
    {
        public AffReportWind(System.Data.DataTable table , ReportType type)
        {
            InitializeComponent();
            ReportViewer.LocalReport.ReportPath = (type == ReportType.PlanningReport) ? ReportViewer.LocalReport.ReportPath = Environment.CurrentDirectory + @"\Reporting\ReportAff.rdlc" : Environment.CurrentDirectory + @"\Reporting\ReportEnsg.rdlc";
            ReportViewer.LocalReport.DataSources.Clear();
            ReportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",
                table));
            ReportViewer.RefreshReport();
            ReportViewer.Show();
        }
    }
}
