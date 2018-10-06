using AspNetInsight.Dto;

namespace AspNetInsight
{
    /// <summary>
    /// Html widget generator.
    /// </summary>
    public interface IResponseBanner
    {
        string Prefix { get; }
        string Suffix { get; }
        string TemplateText { get; }
        string GeneratedBanner(BasicSts Data);
    }
}