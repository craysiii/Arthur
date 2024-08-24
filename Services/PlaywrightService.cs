using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Arthur.Services;

public class PlaywrightService
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    
    private static string TempPath => Path.GetTempPath();
    
    public PlaywrightService()
    {
        InitializePlayWright().GetAwaiter().GetResult();
    }

    private async Task InitializePlayWright()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(options: new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _context = await _browser.NewContextAsync();
        
        // Blank page to keep browser instance running
        var tempPage = await _context.NewPageAsync();
        await tempPage.GotoAsync("about:blank");
    }

    internal async Task<string> GeneratePdfFromHtmlTemplate(HttpContext context, HtmlRequest request)
    {
        // Setup file paths
        var fileGuid = Guid.NewGuid().ToString();
        var htmlFilePath = Path.Join(TempPath, $"{fileGuid}.html");
        var pdfFilePath = Path.Join(TempPath, $"{fileGuid}.pdf");
        
        // Convert template to html and write to disk
        var html = Base64.FromBase64ToString(request.EncodedTemplate);
        await File.WriteAllTextAsync(htmlFilePath, html);

        return await GeneratePdfFile($"{context.Request.Host}/{fileGuid}", request, pdfFilePath);
    }
    
    internal async Task<string> GeneratePdfFromUrl(UrlRequest request)
    {
        // Setup file path
        var fileGuid = Guid.NewGuid().ToString();
        var pdfFilePath = Path.Join(TempPath, $"{fileGuid}.pdf");

        return await GeneratePdfFile(request.Url, request, pdfFilePath);
    }

    private async Task<string> GeneratePdfFile(string url, PdfDocumentRequest request, string pdfFilePath)
    {
        // Create new page and navigate to html file
        var currentPage = await _context.NewPageAsync();
        await currentPage.GotoAsync(url);
        
        // Generate PDF Options
        var pdfOptions = GeneratePdfOptions(request, currentPage);
        pdfOptions.Path = pdfFilePath;
        
        // Set Emulated media Type
        await currentPage.EmulateMediaAsync(options: new PageEmulateMediaOptions
        {
            Media = Media.Screen
        });
        
        // Optionally wait for page to render according to user input
        await Task.Delay(request.PageRenderDelay ?? 0);
        
        // Generate PDF
        await currentPage.PdfAsync(pdfOptions);

        // Close page
        await currentPage.CloseAsync();
        
        // Return the path to the PDF
        return pdfFilePath;
    }

    private static PagePdfOptions GeneratePdfOptions(PdfDocumentRequest properties, IPage page)
    {
        var pdfOptions = new PagePdfOptions
        {
            Scale = properties.Scale ?? 1.0f,
            Outline = properties.Outline,
            PrintBackground = properties.PrintBackground,
            Tagged = properties.Tagged,
            PreferCSSPageSize = properties.PreferCssPageSize,
            DisplayHeaderFooter = properties.DisplayHeaderFooter,
            FooterTemplate = string.IsNullOrWhiteSpace(properties.FooterTemplate) ? null : Base64.FromBase64ToString(properties.FooterTemplate),
            HeaderTemplate = string.IsNullOrWhiteSpace(properties.HeaderTemplate) ? null : Base64.FromBase64ToString(properties.HeaderTemplate),
            Landscape = properties.Landscape,
            Width = properties.Width,
            Format = properties.Format,
            PageRanges = properties.PageRanges
        };

        GenerateMarginOptions(pdfOptions, properties);
        GenerateHeightOptions(pdfOptions, properties, page);
        
        return pdfOptions;
    }

    private static void GenerateMarginOptions(PagePdfOptions pdfOptions, PdfDocumentRequest properties)
    {
        // Nothing to configure, exit early
        if (
            string.IsNullOrWhiteSpace(properties.Margin) &&
            string.IsNullOrWhiteSpace(properties.MarginTop) &&
            string.IsNullOrWhiteSpace(properties.MarginBottom) &&
            string.IsNullOrWhiteSpace(properties.MarginLeft) &&
            string.IsNullOrWhiteSpace(properties.MarginRight)
            )
        {
            return;
        }

        var margin = new Margin();

        // Margin has the lowest priority, so it gets set first
        if (!string.IsNullOrWhiteSpace(properties.Margin))
        {
            margin.Top = margin.Bottom = margin.Left = margin.Right = properties.Margin;
        }

        // Set all the specific Margin fields
        if (!string.IsNullOrWhiteSpace(properties.MarginTop)) margin.Top = properties.MarginTop;
        if (!string.IsNullOrWhiteSpace(properties.MarginBottom)) margin.Bottom = properties.MarginBottom;
        if (!string.IsNullOrWhiteSpace(properties.MarginLeft)) margin.Left = properties.MarginLeft;
        if (!string.IsNullOrWhiteSpace(properties.MarginRight)) margin.Right = properties.MarginRight;

        // Assign new Margin instance to PagePdfOptions
        pdfOptions.Margin = margin;
    }

    private static void GenerateHeightOptions(PagePdfOptions pdfOptions, PdfDocumentRequest properties, IPage page)
    {
        // SinglePage has the higher precedence, so calculate it first and return if its true
        if (properties.SinglePage == true)
        {
            var height = page.EvaluateAsync("document.body.scrollHeight").GetAwaiter().GetResult();
            pdfOptions.Height = height.ToString();
            pdfOptions.PageRanges = "1";    // No extra pages
            return;
        }

        // Use the provided Height from the request
        pdfOptions.Height = properties.Height;
    }
    
    private async Task DisposePlayWright()
    {
        await _context.DisposeAsync();
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    ~PlaywrightService()
    {
        DisposePlayWright().GetAwaiter().GetResult();
    }
}