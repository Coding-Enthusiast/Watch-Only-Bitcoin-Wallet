using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonLibrary
{
    /// <summary>
    /// Copied from https://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
