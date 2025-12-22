using System.Text;

namespace EscPosPrinter
{
    #region Enums

    public enum Alignment
    {
        Left = 0,
        Center = 1,
        Right = 2
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

    #endregion

    public class EscPosPrinter
    {
        private const byte ESC = 0x1B;
        private const byte CMD_INIT = 0x40;
        private const byte CMD_ALIGN = 0x61;
        private const byte CMD_STYLE = 0x21;
        private const byte LF = 0x0A;

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
