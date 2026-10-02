
using TrainAlert.Api.Providers;

namespace TrainAlert.Tests;

public class TrenordStrikeParserTests
{
  private const string SourceUrl =
      "https://www.trenord.it/news/trenord-informa/avvisi/sciopero/";

  [Fact]
  public void Parse_ValidStrikeHtml_ShouldExtractAllFields()
  {
    var parser = new TrenordStrikeParser();

    var strike = parser.Parse(CreateValidHtml(), SourceUrl);

    Assert.NotNull(strike);

    Assert.Equal(
        "Sciopero nazionale del 2 ottobre 2026",
        strike.Title);

    Assert.Equal(
        new DateTime(2026, 9, 25),
        strike.PublishedAt);

    Assert.Equal(
        new DateTime(2026, 10, 2, 11, 0, 0),
        strike.StartAt);

    Assert.Equal(
        new DateTime(2026, 10, 2, 14, 0, 0),
        strike.EndAt);

    Assert.Contains(
        "sciopero nazionale",
        strike.Description,
        StringComparison.OrdinalIgnoreCase);

    Assert.Equal(
        "204/2026",
        strike.NoticeNumber);

    Assert.Equal(
        SourceUrl,
        strike.SourceUrl);

    Assert.Equal(
        "https://www.trenord.it/fileadmin/contenuti/TRENORD/3-News/Trenord_Informa/Avvisi/2026/AvvisoTrenord_2026_204__Sciopero_2_ott.pdf",
        strike.NoticeUrl);
  }

  [Fact]
  public void Parse_EmptyHtml_ShouldReturnNull()
  {
    var parser = new TrenordStrikeParser();

    var strike = parser.Parse(string.Empty, SourceUrl);

    Assert.Null(strike);
  }

  [Fact]
  public void Parse_WhitespaceHtml_ShouldReturnNull()
  {
    var parser = new TrenordStrikeParser();

    var strike = parser.Parse("   \r\n\t ", SourceUrl);

    Assert.Null(strike);
  }

  [Fact]
  public void Parse_MissingTitle_ShouldReturnNull()
  {
    var parser = new TrenordStrikeParser();

    var html = CreateValidHtml()
    .Replace(
        "Sciopero nazionale del 2 ottobre 2026",
        string.Empty);

    var strike = parser.Parse(html, SourceUrl);

    Assert.Null(strike);
  }

  [Fact]
  public void Parse_MissingStrikeArticle_ShouldReturnNull()
  {
    var parser = new TrenordStrikeParser();

    var html = """
        <html>
            <body>
                <div class="container-foglia-news">
                    <h1 class="uppercase">
                        <b>Sciopero nazionale del 2 ottobre 2026</b>
                    </h1>

                    <p class="date-news">
                        venerdì 25/09/2026
                    </p>
                </div>
            </body>
        </html>
        """;

    var strike = parser.Parse(html, SourceUrl);

    Assert.Null(strike);
  }

  [Fact]
  public void Parse_DifferentTimeSeparator_ShouldParseCorrectly()
  {
    var parser = new TrenordStrikeParser();

    var html = CreateValidHtml()
        .Replace("11.00", "11:00")
        .Replace("14:00", "14.00");

    var strike = parser.Parse(html, SourceUrl);

    Assert.NotNull(strike);

    Assert.Equal(
        new DateTime(2026, 10, 2, 11, 0, 0),
        strike.StartAt);

    Assert.Equal(
        new DateTime(2026, 10, 2, 14, 0, 0),
        strike.EndAt);
  }



  [Fact]
  public void Parse_NoticeLinkMissing_ShouldStillReturnStrike()
  {
    var parser = new TrenordStrikeParser();

    var html = CreateValidHtml();

    var document = new HtmlAgilityPack.HtmlDocument();
    document.LoadHtml(html);

    var noticeLink = document.DocumentNode.SelectSingleNode(
        "//a[contains(normalize-space(.), 'Avviso')]");

    Assert.NotNull(noticeLink);

    noticeLink.Remove();

    using var writer = new StringWriter();
    document.Save(writer);

    var strike = parser.Parse(writer.ToString(), SourceUrl);

    Assert.NotNull(strike);

    Assert.Null(strike.NoticeNumber);
    Assert.Null(strike.NoticeUrl);

    Assert.Equal(
        "Sciopero nazionale del 2 ottobre 2026",
        strike.Title);
  }



  [Fact]
  public void Parse_NestedHtmlInsideArticle_ShouldExtractCleanDescription()
  {
    var parser = new TrenordStrikeParser();

    var html = CreateValidHtml()
        .Replace(
            "sciopero nazionale",
            "<strong>sciopero nazionale</strong>");

    var strike = parser.Parse(html, SourceUrl);

    Assert.NotNull(strike);

    Assert.Contains(
        "sciopero nazionale",
        strike.Description,
        StringComparison.OrdinalIgnoreCase);

    Assert.DoesNotContain(
        "<strong>",
        strike.Description,
        StringComparison.OrdinalIgnoreCase);
  }

  private static string CreateValidHtml()
  {
    return """
        <html>
            <body>

                <div class="breadcrumb-container free-breadcrumbs">
                    <ul>
                        <li>Sciopero</li>
                    </ul>
                </div>

                <div class="container-fluid grid-margin container-foglia-news">
                    <div class="container">
                        <div class="container-fluid containers">
                            <div class="container-sticky stickem-container">
                                <div class="content">
                                    <div class="container-content grid-margin">
                                        <div class="col-l">

                                            <div class="text align-middle">
                                                <h1 class="uppercase">
                                                    <b>Sciopero nazionale del 2 ottobre 2026</b>
                                                </h1>

                                                <p class="subtitle_news_detail"></p>

                                                <p class="green date-news">
                                                    venerdì 25/09/2026
                                                </p>

                                                <p></p>
                                            </div>

                                            <div
                                                id="c14692"
                                                class="frame frame-default frame-type-trenordtheme_simpletextmedia frame-layout-0">

                                                <div class="container-content col-l">
                                                    <div class="text align-middle">

                                                        <p>
                                                            I sindacati Csle e FI-SI hanno indetto
                                                            uno sciopero nazionale di tutti i settori
                                                            pubblici e privati che, per il settore ferroviario,
                                                            si effettuerà dalle ore 11.00 alle ore 14:00
                                                            del 2 ottobre 2026 che potrà generare
                                                            ripercussioni sulla circolazione ferroviaria
                                                            in Lombardia.
                                                        </p>

                                                        <p>
                                                            Le fasce orarie di garanzia non sono interessate
                                                            dallo sciopero.
                                                        </p>

                                                        <p>
                                                            Nel caso di
                                                            <strong>cancellazione dei treni del servizio aeroportuale</strong>,
                                                            saranno istituiti
                                                            <strong>bus senza fermate intermedie</strong>.
                                                        </p>

                                                    </div>
                                                </div>
                                            </div>

                                            <div
                                                id="c18757"
                                                class="frame frame-default frame-type-trenordtheme_simpletextmedia frame-layout-0">

                                                <div class="container-content col-l">
                                                    <div class="text align-middle">

                                                        <p>
                                                            <a
                                                                href="/fileadmin/contenuti/TRENORD/3-News/Trenord_Informa/Avvisi/2026/AvvisoTrenord_2026_204__Sciopero_2_ott.pdf"
                                                                target="_blank">
                                                                Avviso 204/2026 [.pdf]
                                                            </a>
                                                        </p>

                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </body>
        </html>
        """;
  }
}
