// Licensed to the CodeRabbits under one or more agreements.
// The CodeRabbits licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Extensions.Options;

namespace CodeRabbits.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("svg", Attributes = nameof(Src))]
public class SvgTagHelper : TagHelper
{
    private readonly SvgTagHelperOption? _options;

    public string? Src { get; set; }

    public SvgTagHelper(IOptions<SvgTagHelperOption> options)
    {
        _options = options?.Value;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (!(Src?.StartsWith("-") ?? false))
        {
            return;
        }

        var svgPath = _options?.BasePath ?? "";
        var filePath = Src[1..];
        var path = $@"{svgPath}{filePath}";
        if (!File.Exists(path))
        {
            return;
        }

        var svgFIleStream = File.OpenRead(path);
        XDocument svg = await XDocument.LoadAsync(svgFIleStream, LoadOptions.None, CancellationToken.None);
        svgFIleStream.Close();

        XElement svgElement = svg.Root ?? throw new FormatException("Could not find the root of the svg.");
        foreach (var attribute in context.AllAttributes.Where(item => item.Name != "src"))
        {
            svgElement.SetAttributeValue(attribute.Name, attribute.Value);
        }

        XmlWriterSettings settings = new()
        {
            Async = true
        };

        output.Reinitialize("svg", TagMode.StartTagAndEndTag);

        output.Attributes.Clear();
        foreach (var attribute in svgElement.Attributes())
        {
            output.Attributes.Add(attribute.Name.ToString(), attribute.Value);
        }

        output.Content.Clear();
        foreach (var item in svgElement.Elements())
        {
            output.Content.AppendHtml(new XElement(item.Name.LocalName, item.Attributes(), item.Elements()).ToString());
        }
    }
}
