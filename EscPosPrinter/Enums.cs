namespace EscPosPrinter
{
    [Flags]
    public enum Style : byte
    {
        None = 0,
        Bold = 8,
        DoubleHeight = 16,
        DoubleWidth = 32,
        Underline = 128
    }

    public enum Alignment : byte
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum BarcodeWidth : byte
    {
        Narrow = 2,
        Medium = 3,
        Wide = 4
    }

    public enum BarcodeHeight : byte
    {
        Short = 60,
        Medium = 120,
        Tall = 180
    }

    public enum BarcodeHri : byte
    {
        None = 0,
        Above = 1,
        Below = 2
    }
}
