# Hangman in C#

A console Hangman project I am building while learning C# and practicing problem-solving.

## Current progress

- Load a word list from a text file.
- Validate the player's difficulty choice.
- Filter words by length for easy, medium, and hard difficulty.
- Select a random word from the chosen difficulty.

The project is in progress. The current version prints the selected word; the guessing loop and win/loss conditions are planned next.

## Running locally

Requires the .NET 10 SDK.

The word-list path in `Program.cs` currently points to my local folder. Update it to your own project folder before running:

```sh
dotnet run
```

## Concepts practiced

- Methods and parameters
- Arrays and `IEnumerable<string>`
- LINQ filtering with `Where`
- `switch` statements
- Input validation with `int.TryParse`
- Random selection and exceptions

## Next steps

- Track guessed letters and remaining lives.
- Display hidden and revealed letters.
- Handle invalid and repeated guesses.
- Detect wins and losses.
