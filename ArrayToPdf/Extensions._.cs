using MigraDoc.DocumentObjectModel;

namespace ArrayToPdf;

public static partial class Extensions
{
    internal static Unit GetWidth(this PageSetup pageSetup)
        => pageSetup.Orientation == Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;

    internal static Unit GetHeight(this PageSetup pageSetup)
        => pageSetup.Orientation != Orientation.Landscape ? pageSetup.PageHeight : pageSetup.PageWidth;
}
