using System.IO;

namespace NeuroExplorer.LogWriter
{
    class LogStreamer
    {
        private TextWriter writer = null;
        private bool terminated = true;

        public void Init(string path)
        {
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
            }
            writer = new StreamWriter(new BufferedStream(new FileStream(path, FileMode.OpenOrCreate)));
            terminated = false;
        }

        public void Write(string line)
        {
            if (terminated == true || writer == null)
            {
                return;
            }
            writer.WriteLine(line);
        }

        public void Terminate()
        {
            if (writer == null)
            {
                return;
            }
            terminated = true;
            writer.Close();
            writer.Dispose();
        }
    }
}
