using CommunityToolkit.Mvvm.ComponentModel;
using log4net;

namespace EQX.Core.Common
{
    public class ViewModelBase : ObservableObject, IDisposable, ILogable
    {
        public string Name { get; set; }
        public ILog Log { get; init; }

        // Example: Object need to be disposed
        //private SqlConnection? _Connection = new SqlConnection();

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to Dispose objects in Child ViewModel
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Example: Dispose object
                //if (_Connection != null)
                //{
                //    _Connection.Dispose();
                //    _Connection = null;
                //}
            }
        }
    }
}
