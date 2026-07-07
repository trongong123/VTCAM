using log4net.Appender;
using log4net.Core;
using System.IO;

namespace EQX.UI
{
    public class CustomRollingFileAppender : RollingFileAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (File != null && Layout.Header != null)
            {
                if (!System.IO.File.Exists(File))
                {
                    using (StreamWriter writer = new StreamWriter(System.IO.File.Create(File)))
                    {
                        writer.WriteLine(Layout.Header);
                    }
                }

                string fileContent = System.IO.File.ReadAllText(File);
                string header = Layout.Header;

                if (fileContent.EndsWith(header))
                {
                    base.Append(loggingEvent);
                    WriteFooter();
                }
                else
                {
                    RemoveExistingFooter();
                    base.Append(loggingEvent);
                    WriteFooter();
                }
            }

        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (File != null && Layout.Header != null)
            {

                string fileContent = System.IO.File.ReadAllText(File);
                string header = Layout.Header;

                if (fileContent.EndsWith(header))
                {
                    base.Append(loggingEvents);
                    WriteFooter();
                }
                else
                {
                    RemoveExistingFooter();
                    base.Append(loggingEvents);
                    WriteFooter();
                }
            }

        }

        protected override void RollOverRenameFiles(string baseFileName)
        {
            if (File != null && Layout?.Footer != null && System.IO.File.Exists(File))
            {
                try
                {
                    lock (this)
                    {
                        string footer = Layout.Footer;
                        if (!string.IsNullOrEmpty(footer) && System.IO.File.Exists(File))
                        {
                            string fileContent = null;

                            using (var fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                long length = fs.Length;
                                int readSize = Math.Min(footer.Length + 50, (int)length);
                                byte[] buffer = new byte[readSize];

                                fs.Seek(-readSize, SeekOrigin.End);
                                fs.Read(buffer, 0, readSize);

                                fileContent = System.Text.Encoding.UTF8.GetString(buffer);
                            }

                            if (!fileContent.EndsWith(footer))
                            {
                                using (var fs = new FileStream(File, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                                using (var sw = new StreamWriter(fs))
                                {
                                    sw.Write(footer);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error("Failed to write footer before rolling", ex);
                }
            }

            base.RollOverRenameFiles(baseFileName);
        }

        private void WriteFooter()
        {
            if (File != null && Layout.Footer != null)
            {
                RemoveExistingFooter();
                lock (this)
                {
                    using (var stream = new FileStream(File, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(Layout.Footer);
                    }
                }
            }
        }

        private void RemoveExistingFooter()
        {
            if (File != null && Layout.Footer != null)
            {
                string fileContent = System.IO.File.ReadAllText(File);
                string footer = Layout.Footer;
                if (fileContent.EndsWith(footer))
                {
                    fileContent = fileContent.Substring(0, fileContent.Length - footer.Length);
                    System.IO.File.WriteAllText(File, fileContent);
                }
            }
        }

        protected override void WriteHeader()
        {
            if (Layout is not null && QuietWriter is not null && !QuietWriter.Closed)
            {
                if (File == null) return;

                if (Layout.Header is string h)
                {
                    string fileContent = System.IO.File.ReadAllText(File);
                    if (fileContent.Contains(h)) return;

                    lock (this)
                    {
                        using (var stream = new FileStream(File, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.Write(h);
                        }
                    }
                }
            }
        }

        protected override void WriteFooterAndCloseWriter()
        {
            try
            {
                if (Layout != null && QuietWriter != null && !QuietWriter.Closed && File != null)
                {
                    string footer = Layout.Footer;
                    if (!string.IsNullOrEmpty(footer) && System.IO.File.Exists(File))
                    {
                        string fileContent = null;

                        using (var fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            long length = fs.Length;
                            int readSize = Math.Min(footer.Length + 50, (int)length); 
                            byte[] buffer = new byte[readSize];

                            fs.Seek(-readSize, SeekOrigin.End);
                            fs.Read(buffer, 0, readSize);

                            fileContent = System.Text.Encoding.UTF8.GetString(buffer);
                        }

                        if (!fileContent.EndsWith(footer))
                        {
                            using (var fs = new FileStream(File, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                            using (var sw = new StreamWriter(fs))
                            {
                                sw.Write(footer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Failed to write footer on close", ex);
            }
            finally
            {
                CloseWriter();
            }
        }
    }
}
