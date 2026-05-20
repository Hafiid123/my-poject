using online_mr_certi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PdfDocument = QuestPDF.Fluent.Document;

namespace online_mr_certi.Services;

public sealed class CertificatePdfService : ICertificatePdfService
{
    private const string GreenDark = "#0d3320";
    private const string GreenMid = "#1a5c38";
    private const string GreenAccent = "#2e7d52";
    private const string Gold = "#b8860b";
    private const string GoldLight = "#d4a843";
    private const string GoldPale = "#f5e6a3";
    private const string Cream = "#fdfaf3";
    private const string TextDark = "#1a1a1a";
    private const string TextMid = "#3d3d3d";
    private const string Muted = "#6b6b6b";
    private const string White = "#ffffff";

    static CertificatePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // ════════════════════════════════════════════════════════════════════════
    public void GenerateCertificatePdf(
        MarriageApplication application,
        Stream output,
        string webRootPath)
    {
        var certNo = $"MC-{application.Id:D6}-{application.SubmissionDate:yyyy}";
        var husbandPhoto = GetPhotoBytes(application, DocumentCategories.HusbandPassportPhoto, webRootPath);
        var wifePhoto = GetPhotoBytes(application, DocumentCategories.WifePassportPhoto, webRootPath);
        var logoBytes = GetLogoBytes(webRootPath);

        PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(t => t.FontFamily("Georgia").FontColor(TextDark).FontSize(10));
                page.PageColor(Cream);

                // ── Double gold border fills full page as background ──────
                page.Background()
                    .Padding(10)
                    .Border(4).BorderColor(Gold)
                    .Padding(3)
                    .Border(1).BorderColor(GoldLight)
                    .Background(White);

                // ── Content fills top, footer pinned to bottom inside border ─
                page.Content()
                    .Padding(22)
                    .Layers(layers =>
                    {
                        // Primary layer: all content stacked top-down
                        layers.PrimaryLayer().Column(doc =>
                        {
                            BuildHeader(doc, certNo, application, logoBytes);
                            BuildThinRule(doc);
                            BuildTitleBlock(doc, certNo, application);
                            BuildThinRule(doc);
                            BuildDeclaration(doc, application);
                            BuildThinRule(doc);
                            BuildPersonsRow(doc, application, husbandPhoto, wifePhoto);
                            BuildThinRule(doc);
                            BuildWitnesses(doc, application);
                        });

                        // Footer layer: pinned to bottom of the content area
                        layers.Layer()
                              .AlignBottom()
                              .Column(doc =>
                              {
                                  BuildThinRule(doc);
                                  BuildFooterContent(doc, application, logoBytes);
                              });
                    });
            });
        }).GeneratePdf(output);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  HEADER
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildHeader(
        ColumnDescriptor col,
        string certNo,
        MarriageApplication application,
        byte[]? logoBytes)
    {
        col.Item()
           .PaddingVertical(10)
           .Row(row =>
           {
               row.ConstantItem(66).AlignMiddle().AlignCenter().Column(c =>
               {
                   if (logoBytes != null)
                       c.Item().Width(58).Height(58)
                        //.Border(2).BorderColor(Gold
                        .Image(logoBytes).FitArea();
                   else
                       c.Item().Width(58).Height(58)
                        //.Border(2).BorderColor(Gold)
                        .Background(GoldPale)
                        .AlignCenter().AlignMiddle()
                        .Text("SEAL").FontSize(7).FontColor(Gold);
               });

               row.RelativeItem().AlignCenter().Column(h =>
               {
                   h.Item().AlignCenter()
                    .Text("بِسْمِ اللهِ الرَّحْمٰنِ الرَّحِيْمِ")
                    .FontSize(15).Bold().FontColor(GreenDark);
                   h.Item().Height(5);
                   h.Item().AlignCenter()
                    .Text("JAMHUURIYADDA FEDERAALKA SOOMAALIYA")
                    .FontSize(11).Bold().FontColor(GreenDark);
                   h.Item().Height(3);
                   h.Item().AlignCenter()
                    .Text("Wasaaradda Cadaaladda iyo Arrimaha Diinta")
                    .FontSize(8).Bold().FontColor(GreenMid);
               });

               row.ConstantItem(66).AlignMiddle().AlignCenter().Column(c =>
               {
                   if (logoBytes != null)
                       c.Item().Width(58).Height(58)
                        //.Border(2).BorderColor(Gold)
                        .Image(logoBytes).FitArea();
                   else
                       c.Item().Width(58).Height(58)
                        //.Border(2).BorderColor(Gold)
                        .Background(GoldPale)
                        .AlignCenter().AlignMiddle()
                        .Text("SEAL").FontSize(7).FontColor(Gold);
               });
           });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  TITLE BLOCK
    // ════════════════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════════════════
    //  HELPER — Convert SVG signature to PNG bytes for QuestPDF
    // ════════════════════════════════════════════════════════════════════════
    
    private static void BuildTitleBlock(
        ColumnDescriptor col,
        string certNo,
        MarriageApplication application)
    {
        col.Item()
           .PaddingVertical(10)
           .Column(t =>
           {
               t.Item().AlignCenter()
                .Text("SHAHAADADA GUURKA")
                .FontSize(30).Bold().FontColor(GreenDark);
               t.Item().AlignCenter()
                .Text("(MARRIAGE CERTIFICATE)")
                .FontSize(11).FontColor(TextMid);
               t.Item().Height(6);
               t.Item().AlignCenter()
                .Text($"Lr. (No): {certNo}")
                .FontSize(10).FontColor(TextDark);
           });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  DECLARATION
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildDeclaration(ColumnDescriptor col, MarriageApplication application)
    {
        col.Item()
           .PaddingVertical(8)
           .Text(text =>
           {
               text.DefaultTextStyle(t => t.FontSize(10).FontColor(TextMid).LineHeight(1.5f));
               text.Span("Waxaan halkan ku caddaynaynaa in la isku guuriyey ");
               text.Span(application.HusbandName).Bold().FontColor(GreenDark);
               text.Span(" iyo ");
               text.Span(application.WifeName).Bold().FontColor(GreenDark);
               text.Span(" taariikhdu markay ahayd ");
               text.Span(application.MarriageDate.ToString("dd MMMM, yyyy")).Bold().FontColor(GreenDark);
               text.Span(", kuna midoobay goobta: ");
               text.Span(application.MarriageLocation).Bold().FontColor(GreenDark);
               text.Span(".");
               text.Span("");
           });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  PERSONS ROW
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildPersonsRow(
        ColumnDescriptor col,
        MarriageApplication application,
        byte[]? husbandPhoto,
        byte[]? wifePhoto)
    {
        col.Item()
           .PaddingVertical(8)
           .Row(row =>
           {
               row.RelativeItem()
                  .Column(c => BuildPersonCard(c,
                      "Xogta Ninka (Husband)",
                      application.HusbandName,
                      application.HusbandDob,
                      application.HusbandIdNumber,
                      application.HusbandContactNumber,
                      application.HusbandAddress,
                      husbandPhoto));

               row.ConstantItem(1).Background(GoldLight);

               row.RelativeItem().PaddingLeft(14)
                  .Column(c => BuildPersonCard(c,
                      "Xogta Gabadha (Wife)",
                      application.WifeName,
                      application.WifeDob,
                      application.WifeIdNumber,
                      application.WifeContactNumber,
                      application.WifeAddress,
                      wifePhoto));
           });
    }

    private static void BuildPersonCard(
        ColumnDescriptor col,
        string title,
        string name,
        DateTime dob,
        string idNumber,
        string contact,
        string address,
        byte[]? photo)
    {
        col.Item()
           .BorderBottom(1).BorderColor(Gold)
           .PaddingBottom(4)
           .Text(title)
           .FontSize(9).Bold().FontColor(GreenDark);

        col.Item().Height(7);

        col.Item().Row(r =>
        {
            r.ConstantItem(66).Column(p =>
            {
                var box = p.Item()
                            .Width(62).Height(78)
                            .Border(1).BorderColor(White);
                if (photo != null)
                    box.Image(photo).FitArea();
                else
                    box.Background("#eef5f0")
                       .AlignCenter().AlignMiddle()
                       .Column(ph =>
                           ph.Item().AlignCenter()
                             .Text("Sawir").FontSize(7).FontColor(Muted));
            });

            r.RelativeItem().PaddingLeft(10).Column(info =>
            {
                InfoRow(info, "Magaca", name);
                InfoRow(info, "Dhalashada", dob.ToString("yyyy-MM-dd"));
                InfoRow(info, "National ID", idNumber);
                InfoRow(info, "Tel", contact);
                InfoRow(info, "Addr", address);
            });
        });
    }

    private static void InfoRow(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingBottom(4).Row(r =>
        {
            r.ConstantItem(62)
             .Text(label + ":")
             .FontSize(8.5f).Bold().FontColor(TextDark);
            r.RelativeItem()
             .Text(value)
             .FontSize(8.5f).FontColor(TextDark);
        });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  WITNESSES
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildWitnesses(ColumnDescriptor col, MarriageApplication application)
    {
        var witnesses = application.Witnesses.OrderBy(x => x.SortOrder).ToList();
        if (witnesses.Count == 0) return;

        col.Item()
           .PaddingVertical(8)
           .Column(w =>
           {
               w.Item()
                .BorderBottom(1).BorderColor(Gold)
                .PaddingBottom(4)
                .Text("Markhaatiyaasha (Witnesses)")
                .FontSize(9).Bold().FontColor(GreenDark);

               w.Item().Height(7);

               w.Item().Table(table =>
               {
                   table.ColumnsDefinition(c =>
                   {
                       c.RelativeColumn();
                       c.RelativeColumn();
                   });

                   for (int i = 0; i < witnesses.Count; i++)
                   {
                       var wt = witnesses[i];
                       bool lft = i % 2 == 0;

                       table.Cell()
                            .PaddingRight(lft ? 10 : 0)
                            .PaddingLeft(lft ? 0 : 10)
                            .PaddingBottom(7)
                            .Column(c =>
                            {
                                c.Item()
                                 .Text($"Witness {i + 1}: {wt.FullName}")
                                 .FontSize(9).Bold().FontColor(TextDark);
                                c.Item().Height(2);
                                c.Item()
                                 .Text($"DOB: {wt.DateOfBirth:yyyy-MM-dd}")
                                 .FontSize(7.5f).FontColor(TextDark);
                                c.Item()
                                .Text($"National ID: {wt.IdNumber}")
                                .FontSize(7.5f).FontColor(TextDark);
                                c.Item()
                                 .Text($"Tel: {wt.ContactNumber}")
                                 .FontSize(7.5f).FontColor(TextDark);
                                c.Item()
                                .Text($"Addr: {wt.Address}")
                                .FontSize(7.5f).FontColor(TextDark);
                            });
                   }
               });
           });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  FOOTER — directly after witnesses, no gap
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildFooterContent(
        ColumnDescriptor col,
        MarriageApplication application,
        byte[]? logoBytes)
    {
        col.Item().Height(8);

        // ── 3-column signature + stamp row ────────────────────────────────
        col.Item().Row(row =>
        {
            // LEFT — couple signatures
            row.RelativeItem().AlignCenter().Column(c =>
            {
                c.Item().Height(36);
                c.Item()
                 .BorderTop(1).BorderColor(TextDark)
                 .PaddingTop(4)
                 .AlignCenter()
                 .Text("Saxiixa Labada Qof")
                 .FontSize(8).Bold().FontColor(GreenDark);
                c.Item().AlignCenter()
                 .Text("(Signatures of Couple)")
                 .FontSize(7).Italic().FontColor(Muted);
            });

            // CENTRE — official stamp, fixed 100pt wide, centred inside
            row.ConstantItem(100).AlignCenter().Column(stamp =>
            {
                //stamp.Item()
                //     .AlignCenter()
                //     .Width(88).Height(88)
                //     .Border(3).BorderColor(GreenMid)
                //     .Padding(4)
                //     .Border(1).BorderColor(GreenAccent)
                //     .Background(White)
                //     .AlignCenter().AlignMiddle()
                //     .Column(s =>
                //     {
                //         if (logoBytes != null)
                //             s.Item().AlignCenter()
                //              .Width(44).Height(44)
                //              .Image(logoBytes).FitArea();
                //         s.Item().Height(2);
                //         s.Item().AlignCenter()
                //          .Text("SHAABADDA")
                //          .FontSize(5f).Bold().FontColor(GreenDark).LetterSpacing(1f);
                //         s.Item().AlignCenter()
                //          .Text("RASMIGA AH")
                //          .FontSize(5f).Bold().FontColor(GreenDark).LetterSpacing(1f);
                //     });
            });

            // RIGHT — registrar signature
            row.RelativeItem().AlignCenter().Column(c =>
            {
                c.Item().Height(36);
                c.Item()
                 .BorderTop(1).BorderColor(TextDark)
                 .PaddingTop(4)
                 .AlignCenter()
                 .Text("Saxiixa iyo Shaabadda")
                 .FontSize(8).Bold().FontColor(GreenDark);
                c.Item().AlignCenter()
                 .Text("(Seal & Official Stamp)")
                 .FontSize(7).Italic().FontColor(Muted);
            });
        });

        col.Item().Height(8);

        // Bottom legal strip
        col.Item()
           .BorderTop(1).BorderColor(GoldLight)
           .PaddingTop(5)
           .AlignCenter()
           .Text(text =>
           {
               text.DefaultTextStyle(t => t.FontSize(7).FontColor(Muted).Italic());
               text.Span("Shahaadadani waxay ansax u tahay xeerka guurka Soomaaliyeed.  ·  ");
               text.Span("This certificate is valid under Somali Family Law.  ·  ");
               text.Span($"Issued: {application.SubmissionDate:dd MMM yyyy}  ·  Ref: MC-{application.Id:D6}");
           });
    }

    // ════════════════════════════════════════════════════════════════════════
    //  THIN GOLD RULE
    // ════════════════════════════════════════════════════════════════════════
    private static void BuildThinRule(ColumnDescriptor col)
    {
        col.Item()
           .Height(1)
           .Background(GoldLight);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  HELPERS
    // ════════════════════════════════════════════════════════════════════════
    private static byte[]? GetPhotoBytes(
        MarriageApplication application,
        string category,
        string webRootPath)
    {
        var relativePath = application.Documents
            .FirstOrDefault(d => d.Category == category)
            ?.FilePath;

        if (string.IsNullOrEmpty(relativePath)) return null;

        var fullPath = Path.Combine(
            webRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
    }

    /// <summary>
    /// Loads logo from wwwroot/images/logo.jpg (also tries .jpeg / .png).
    /// Returns null silently if not found.
    /// </summary>
    private static byte[]? GetLogoBytes(string webRootPath)
    {
        foreach (var name in new[] { "logo.jpg", "logo.jpeg", "logo.png" })
        {
            var path = Path.Combine(webRootPath, "images", name);
            if (File.Exists(path))
                return File.ReadAllBytes(path);
        }
        return null;
    }
}
