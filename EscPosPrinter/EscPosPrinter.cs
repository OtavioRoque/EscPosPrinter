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
        private readonly string _printerName;
        private readonly int _columns;
        private readonly List<byte[]> _buffer = new();
        private readonly Encoding _encoding = Encoding.GetEncoding(437);

        public EscPosPrinter(string printerName, int columns = 48)
        {
            _printerName = printerName;
            _columns = columns;
            _buffer.Add(new byte[] { 0x1B, 0x40 });
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
            _buffer.Add(new byte[] { 0x1B, 0x61, (byte)alignment });

            byte style = 0x00;
            if (bold)
                style |= 0x08;
            if (underline)
                style |= 0x80;
            if (doubleHeight)
                style |= 0x10;
            if (doubleWidth)
                style |= 0x20;

            _buffer.Add(new byte[] { 0x1B, 0x21, style });

            if (lineBreak)
                _buffer.Add(_encoding.GetBytes(text + "\n"));
            else
                _buffer.Add(_encoding.GetBytes(text));

            ResetStyle();
        }

        public void WriteSplitter(char sep = '-')
        {
            string line = new string(sep, _columns);
            _buffer.Add(_encoding.GetBytes(line + "\n"));
        }

        private void ResetStyle()
        {
            _buffer.Add(new byte[] { 0x1B, 0x21, 0x00 });
        }
    }
}
