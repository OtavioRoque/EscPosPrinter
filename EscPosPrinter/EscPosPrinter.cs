using System.Text;

namespace EscPosPrinter
{
    public enum Alignment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public class EscPosPrinter
    {
        private const byte ESC = 0x1B;
        private const byte CMD_INIT = 0x40;
        private const byte CMD_ALIGN = 0x61;
        private const byte CMD_STYLE = 0x21;

        private readonly string _printerName;
        private readonly int _columns;
        private readonly List<byte[]> _buffer = [];
        private readonly Encoding _encoding = Encoding.GetEncoding(437);

        public EscPosPrinter(string printerName, int columns = 48)
        {
            _printerName = printerName;
            _columns = columns;
            _buffer.Add([ESC, CMD_INIT]);
        }

        public void Write(
            string text,
            Alignment alignment = Alignment.Left,
            bool bold = false,
            bool underline = false,
            bool doubleHeight = false,
            bool doubleWidth = false,
            bool lineBreak = true)
        {
            _buffer.Add([ESC, CMD_ALIGN, (byte)alignment]);

            byte style = 0x00;
            if (bold)
                style |= 0x08;
            if (underline)
                style |= 0x80;
            if (doubleHeight)
                style |= 0x10;
            if (doubleWidth)
                style |= 0x20;

            _buffer.Add([ESC, CMD_STYLE, style]);

            if (lineBreak)
                _buffer.Add(_encoding.GetBytes(text + "\n"));
            else
                _buffer.Add(_encoding.GetBytes(text));

            ResetStyle();
        }

        public void WriteSplitter(char sep = '-')
        {
            string line = new(sep, _columns);
            _buffer.Add(_encoding.GetBytes(line + "\n"));
        }

        private void ResetStyle()
        {
            _buffer.Add([ESC, CMD_STYLE, 0x00]);
        }
    }
}
