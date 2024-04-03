# GymApplication
 GymApplication made for the Threading In C#.

 ## How to use the application
 1. Download the "Final release" release from the main branch.
 2. Open the solution in Visual Studio (the application works only on Windows)


## Threading techniques used
- TPL (Task Parallel Library/Async & Await)
- Semaphore (SemaphoreSlim)
- PLINQ (Parallel LINQ)
- Asynchronous I/O

### Threading techniques explained
1. TPL (Task Parallel Library/Async & Await):
   TPL it's going to be seen throughout the whole application. It's function is to enable non-blocking operations, from which those operations are useful for I/O bound tasks such as file operations.
   Example from the application. In WeatherDataModule.cs, the SearchCsvAsync method:
   ```
     public async Task<string> SearchCsvAsync(string filePath, string country, string city, string weatherConditions) {...}
   ```
   The above method, uses async-await in order to perform asynchronous file reading operations.

2. Semaphore(SemaphoreSlim):
   A semaphore(SemaphoreSlim) purpose is to limit the number of threads that can access a resource (or a pool of resources concurently).
   Example from the application. In GymApplication.xaml.cs, loadSongsSemaphore and later used in the LoadSongsAsync method(more explanation in there in the comments):
   ```
    private SemaphoreSlim loadSongsSemaphore = new SemaphoreSlim(1, 1)
   ```
  The above semaphore is used to make sure that only one task at a time can execute the LoadSongsAsync method which purpose is to load songs from a directory.

3. PLINQ(Parallel LINQ):
   PLINQ(Parallel LINQ purpose is to parallelize queries which can be processed in parallel for better performance.
   Example from the application. In GymApplication.xaml.cs, inside the FilterSongsAsync method:
   ```
     return _songs.AsParallel().Where(song => song.Genre == genre && song != _previousSong).ToList();
   ```
   The above uses PLINQ to filter songs by genre in parallel.

4. Asynchronous I/O:
   Asynchronous I/O purpose is to parallelize queries that can be processed in parallel for better performance.
   Example from the application. In WeatherDataModule.cs:
   ```
     var data = (from line in await File.ReadAllLinesAsync(filePath).ConfigureAwait(false) ...
   ```
   The above reads the CSV file asynchronously.
   
   ```
     using HttpResponseMessage response = await client.GetAsync(url);
   ```
   The above is used for asynchronous HTTP requests from the API.
