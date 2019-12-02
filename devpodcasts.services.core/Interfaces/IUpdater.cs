using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DevPodcast.Services.Core.Interfaces
{
    public interface IUpdater : IDisposable
    {
        Task UpdateDataAsync();
    }
}