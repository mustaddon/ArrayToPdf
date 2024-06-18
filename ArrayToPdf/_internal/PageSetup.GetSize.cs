using MigraDoc.DocumentObjectModel;
namespace ArrayToPdf._internal;

internal static partial class PageSetupExt
{
    internal static Unit GetWidth(this PageSetup pageSetup)
        => pageSetup.Orientation == Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;

    internal static Unit GetHeight(this PageSetup pageSetup)
        => pageSetup.Orientation != Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;
}
