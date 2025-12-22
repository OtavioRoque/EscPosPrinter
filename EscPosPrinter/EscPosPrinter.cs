using System.Text;

namespace EscPosPrinter
{
    #region Enums

    public enum Alignment : byte
    {
        Left = 0x00,
        Center = 0x01,
        Right = 0x02
    }

    [Flags]
    public enum Style : byte
    {
        None = 0x00,
        Bold = 0x08,
        DoubleHeight = 0x10,
        DoubleWidth = 0x20,
        Underline = 0x80
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
        None = 0x00,
        Above = 0x01,
        Below = 0x02
    }

    #endregion

    public class EscPosPrinter
    {
        private const byte ESC = 0x1B;
        private const byte CMD_INIT = 0x40;
        private const byte CMD_ALIGN = 0x61;
        private const byte CMD_STYLE = 0x21;
        private const byte LF = 0x0A;
        private const byte GS = 0x1D;
        private const byte CMD_BARCODE_HRI = 0x48;
        private const byte CMD_BARCODE_WIDTH = 0x77;
        private const byte CMD_BARCODE_HEIGHT = 0x68;
        private const byte CMD_BARCODE_PRINT = 0x6B;
        private const byte BARCODE_CODE128 = 0x49;

        private readonly string _printerName;
        private readonly int _columns;
        private readonly List<byte[]> _buffer = [];
        private readonly Encoding _encoding = Encoding.GetEncoding(437);

        public EscPosPrinter(string printerName, int columns = 48)
        {
            _printerName = printerName;
            _columns = columns;

            InitializePrinter();
        }

        #region Public Methods

        public void Write(string text, Style style = Style.None, Alignment alignment = Alignment.Left, bool lineFeed = true)
        {
            ApplyAlignment(alignment);
            ApplyStyle(style);

            SendCommand(_encoding.GetBytes(text));

            if (lineFeed)
                SendCommand(LF);

            ResetStyle();
        }

        public void WriteLeftRight(string left, string right, Style style = Style.None)
        {
            int spaces = _columns - left.Length - right.Length;
            if (spaces < 1)
                spaces = 1;

            string line = left + new string(' ', spaces) + right;

            Write(line, style);
        }

        public void WriteBarcode(string content, BarcodeWidth width = BarcodeWidth.Medium, BarcodeHeight height = BarcodeHeight.Medium, BarcodeHri hri = BarcodeHri.None)
        {
            ApplyAlignment(Alignment.Center);

            SendCommand(GS, CMD_BARCODE_HRI, (byte)hri);
            SendCommand(GS, CMD_BARCODE_WIDTH, (byte)width);
            SendCommand(GS, CMD_BARCODE_HEIGHT, (byte)height);

            byte[] prefix = [0x7B, 0x42];
            byte[] data = _encoding.GetBytes(content);
            byte[] payload = prefix.Concat(data).ToArray();

            SendCommand(GS, CMD_BARCODE_PRINT, BARCODE_CODE128, (byte)payload.Length);
            SendCommand(payload);
            SendCommand(LF);

            ApplyAlignment(Alignment.Left);
        }

        public void WriteSplitter(char sep = '-')
        {
            string line = new(sep, _columns);
            Write(line);
        }

        #endregion

        #region Private Methods

        private void SendCommand(params byte[] cmd)
        {
            _buffer.Add(cmd);
        }

        public void InitializePrinter()
        {
            SendCommand(ESC, CMD_INIT);
        }

        private void ApplyStyle(Style style)
        {
            SendCommand(ESC, CMD_STYLE, (byte)style);
        }

        private void ResetStyle()
        {
            ApplyStyle(Style.None);
        }

        private void ApplyAlignment(Alignment alignment)
        {
            SendCommand(ESC, CMD_ALIGN, (byte)alignment);
        }

        #endregion
    }
}
