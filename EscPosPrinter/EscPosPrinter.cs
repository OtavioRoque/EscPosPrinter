using System.Text;

namespace EscPosPrinter
{
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
    }
}
