namespace DevPodcast.FSharp

open Microsoft.Extensions.Configuration
open DevPodcast.Services.Core.Interfaces
open System.Threading.Tasks

type internal DataCleaner(configuration : IConfiguration, dbContextFactory : IDbContextFactory) =
    member this.Configuration = configuration
    member this.DbContextFactory  = dbContextFactory
   
        

module Say =
    let hello name =
        printfn "Hello %s" name
