using online_mr_certi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfDocument = QuestPDF.Fluent.Document;

namespace online_mr_certi.Services;

public sealed class CertificatePdfService : ICertificatePdfService
{
    static CertificatePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public void GenerateCertificatePdf(MarriageApplication application, Stream output)
    {
        var certNo = $"MC-{application.Id:D6}-{application.SubmissionDate:yyyy}";

        PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(15);
                page.PageColor(Colors.White);

                // 1. Border-ka qurxoon ee Shahaadada
                page.Content().Border(3).BorderColor("#1a4d2e").Padding(5).Border(1).Column(column =>
                {
                    // Header Section
                    column.Item().PaddingTop(20).AlignCenter().Column(headerCol =>
                    {
                        headerCol.Item().Text("بِسْمِ اللهِ الرَّحْمٰنِ الرَّحِيْمِ").FontSize(20).Bold();
                        headerCol.Item().Text("JAMHUURIYADDA FEDERAALKA SOOMAALIYA").FontSize(14).Bold();
                        headerCol.Item().Text("Wasaaradda Cadaaladda iyo Arrimaha Diinta").FontSize(11).Italic();

                        headerCol.Item().PaddingVertical(10).AlignCenter().Text("SHAHAADADA GUURKA").FontSize(26).Bold().FontColor("#1a4d2e");
                        headerCol.Item().AlignCenter().Text("(MARRIAGE CERTIFICATE)").FontSize(13).SemiBold();
                    });

                    // Body Section
                    column.Item().PaddingHorizontal(40).PaddingTop(15).Column(contentCol =>
                    {
                        contentCol.Item().AlignRight().Text($"Lr. (No): {certNo}").Bold();

                        contentCol.Item().PaddingTop(15).Text(text =>
                        {
                            text.Span("Waxaan halkan ku caddaynaynaa in la isku guuriyey ").FontSize(12);
                            text.Span(application.HusbandName).Bold().Underline();
                            text.Span(" iyo ").FontSize(12);
                            text.Span(application.WifeName).Bold().Underline();
                            text.Span(" taariikhdu markay ahayd ").FontSize(12);
                            text.Span(application.MarriageDate.ToString("dd MMMM, yyyy")).Bold();
                            text.Span(", kuna midoobay goobta: ").FontSize(12);
                            text.Span(application.MarriageLocation).Bold();
                            text.Span(".").FontSize(12);
                        });

                        // 2. PARTIES DETAILS (Husband & Wife)
                        contentCol.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            // Husband Info
                            table.Cell().PaddingRight(10).Column(c => {
                                c.Item().BorderBottom(1).PaddingBottom(2).Text("Xogta Ninka (Husband)").SemiBold();
                                c.Item().Text($"Magaca: {application.HusbandName}");
                                c.Item().Text($"Dhalashada: {application.HusbandDob:yyyy-MM-dd}");
                                c.Item().Text($"ID/Pass: {application.HusbandIdNumber}");
                                c.Item().Text($"Tel: {application.HusbandContactNumber}");
                                c.Item().Text($"Addr: {application.HusbandAddress}");
                            });

                            // Wife Info
                            table.Cell().PaddingLeft(10).Column(c => {
                                c.Item().BorderBottom(1).PaddingBottom(2).Text("Xogta Gabadha (Wife)").SemiBold();
                                c.Item().Text($"Magaca: {application.WifeName}");
                                c.Item().Text($"Dhalashada: {application.WifeDob:yyyy-MM-dd}");
                                c.Item().Text($"ID/Pass: {application.WifeIdNumber}");
                                c.Item().Text($"Tel: {application.WifeContactNumber}");
                                c.Item().Text($"Addr: {application.WifeAddress}");
                            });
                        });

                        // 3. WITNESSES SECTION (Markhaatiyaasha)
                        contentCol.Item().PaddingTop(20).Text("Markhaatiyaasha (Witnesses)").SemiBold().Underline();
                        contentCol.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });

                            var witnesses = application.Witnesses.OrderBy(x => x.SortOrder).ToList();
                            for (int i = 0; i < witnesses.Count; i++)
                            {
                                var w = witnesses[i];
                                table.Cell().PaddingRight(i % 2 == 0 ? 10 : 0).PaddingLeft(i % 2 != 0 ? 10 : 0).PaddingBottom(10).Column(c => {
                                    c.Item().Text($"Witness {i + 1}: {w.FullName}").SemiBold().FontSize(10);
                                    c.Item().Text($"DOB: {w.DateOfBirth:yyyy-MM-dd} | ID: {w.IdNumber}").FontSize(9);
                                    c.Item().Text($"Tel: {w.ContactNumber} | Addr: {w.Address}").FontSize(9);
                                });
                            }
                        });
                    });

                    // Footer / Signatures
                    column.Item().AlignBottom().PaddingBottom(30).PaddingHorizontal(40).Row(row =>
                    {
                        row.RelativeItem().Column(c => {
                            c.Item().PaddingTop(40).BorderTop(1).AlignCenter().Text("Saxiixa Labada Qof");
                            c.Item().AlignCenter().Text("(Signatures of Couple)").FontSize(8).Italic();
                        });

                        row.ConstantItem(100).AlignCenter().Column(c => {
                            c.Item().Width(65).Height(65).Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text("STAMP").FontSize(8).FontColor(Colors.Grey.Lighten1);
                        });

                        row.RelativeItem().Column(c => {
                            c.Item().PaddingTop(40).BorderTop(1).AlignCenter().Text("Saxiixa iyo Shaabadda");
                            c.Item().AlignCenter().Text("(Sign & Official Stamp)").FontSize(8).Italic();
                        });
                    });
                });
            });
        }).GeneratePdf(output);
    }
}