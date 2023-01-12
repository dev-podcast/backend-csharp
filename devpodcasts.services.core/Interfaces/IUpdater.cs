using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace devpodcasts.Services.Core.Interfaces
{
    public interface IUpdater : IDisposable
    {
        Task UpdateDataAsync();
    }
}