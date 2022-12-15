using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace CodeRabbits.AspNetCore.Razor.TagHelpers.Test;

public class SvgTagHelperTest
{
    [Fact]
    public async void RenderWithOption()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.Configure<SvgTagHelperOption>(option =>
        {
            option.BasePath = "./svg";
        });

        builder.Services.AddTransient<SvgTagHelper>();

        var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var svgTagHelper = scope.ServiceProvider.GetRequiredService<SvgTagHelper>();

        var tagHelperContext = new TagHelperContext(
            new TagHelperAttributeList {
                new TagHelperAttribute("src", "-/test.svg"),
                new TagHelperAttribute("fill", "#FFF"),
            },
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var tagHelperOutput = new TagHelperOutput("svg",
            new TagHelperAttributeList(),
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });


        svgTagHelper.Src = "-/test.svg";
        await svgTagHelper.ProcessAsync(tagHelperContext, tagHelperOutput);

        Assert.Equal("svg", tagHelperOutput.TagName);
        Assert.Equal(new TagHelperAttributeList
        {
            new TagHelperAttribute("viewBox", "0 0 100 100"),
            new TagHelperAttribute("xmlns", "http://www.w3.org/2000/svg"),
            new TagHelperAttribute("fill", "#FFF"),
        }, tagHelperOutput.Attributes);
        Assert.Equal("<circle cx=\"50\" cy=\"50\" r=\"50\" />", tagHelperOutput.Content.GetContent());
    }

    [Fact]
    public async void RenderWithoutOption()
    {
        var builder = WebApplication.CreateBuilder();  
        builder.Services.AddTransient<SvgTagHelper>();

        var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var svgTagHelper = scope.ServiceProvider.GetRequiredService<SvgTagHelper>();

        var tagHelperContext = new TagHelperContext(
            new TagHelperAttributeList {
                new TagHelperAttribute("src", "-/svg/test.svg"),
                new TagHelperAttribute("fill", "#FFF"),
            },
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var tagHelperOutput = new TagHelperOutput("svg",
            new TagHelperAttributeList(),
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });


        svgTagHelper.Src = "-./svg/test.svg";
        await svgTagHelper.ProcessAsync(tagHelperContext, tagHelperOutput);

        Assert.Equal("svg", tagHelperOutput.TagName);
        Assert.Equal(new TagHelperAttributeList
        {
            new TagHelperAttribute("viewBox", "0 0 100 100"),
            new TagHelperAttribute("xmlns", "http://www.w3.org/2000/svg"),
            new TagHelperAttribute("fill", "#FFF"),
        }, tagHelperOutput.Attributes);
        Assert.Equal("<circle cx=\"50\" cy=\"50\" r=\"50\" />", tagHelperOutput.Content.GetContent());
    }
}
