# GymApplication
 GymApplication made for the Threading In C#.

 ## How to use the application
 1. Download the "Final release" release from the main branch.
 2. Open the solution in Visual Studio (the application works only on Windows).
 3. Press the play application button (it's labeled with Windows Machine).
 4. The app opens on the first page of the input of country and city. Input the country and city (use English). When done, press the "Next" button.
 5. In the Workout Creation page enter as many exercises you want (the time format is hours:minutes:seconds) and press Add Exercise. This will be your workout exercises.
 6. To save the above exercises into a workout enter the workout name and press the "Save Workout" buttoon.
 7. To add another workout repeat steps 5 and 6.
 8. Select the checkboxes for which workouts you want to do and press "Begin Workout".
 9. In the next screen you can see your workout name, exercise and timer.
 10. Press Start to start the workout and it's also gonna start the songs based on the weather.
 11. Once all workouts are finished the WORKOUTS FINISHED text is displayed and the song changes again to mark the end of the workout. You can leave it on so you can enjoy some free music treatment :) .
 12. Press pause, to pause the workout or stop to stop the workout.

### Known errors:
All known errors can be fixed by restarting the application. So if you encounter any error just restart the application.

Initial launch shows a blank screen why ?
This happens very rarely. We tried fixing it from all the angles. From the AppShell method to offloading methods from the constructor of the WeatherInput page to diving deep into libraries.
We could not find a fix and since this happens rarely we decided to leave it for now. The initial page does not use any threading besides setting two fields of Country and City of WeatherDataModule.
What was sort of a "cure" to this was the fact I added the initial page to the AppShell.
For Design Patterns I have a C# application that uses threading as well and I did not face any of those issues. But from peers I heard it's the other way around for them, their Design Patterns applications hang for no reason,
while the threading made application does not.
We concluded it's something to do with .NET MAUI itself and how it is running the application in various environments. 

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
