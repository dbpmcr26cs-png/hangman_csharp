using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;

//LOAD LEXICON
string path = "/Users/vasco1/Documents/Game Dev/C# Practice/hangman/";

string[] words = File.ReadAllText($"{path}1k_lexicon.txt")
    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);


//GAME START
static int gameStart()
{
    bool gameDiff = false;
    int diffSetting = 0;

    Console.Clear();

    while (!gameDiff)
    {
        Console.WriteLine("Choose game difficulty:");
        Console.WriteLine("    1.) Easy");
        Console.WriteLine("    2.) Medium");
        Console.WriteLine("    3.) Hard");
        string input = Console.ReadLine();
        if (int.TryParse(input, out diffSetting) && diffSetting is 1 or 2 or 3) {
            gameDiff = true;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Must choose a valid number between 1 and 3.\n");
        }
    }
    return diffSetting;
}

// SELECT WORD BASED ON DIFFICULTY
int diffSetting = gameStart();

static string selectWord(int diffSetting, string[] words)
{
    switch(diffSetting)
    {
        case 1:
            IEnumerable<string> easyLexicon =
                words.Where(word => word.Length >= 5 && word.Length < 7);
            return easyLexicon.ElementAt(Random.Shared.Next(easyLexicon.Count()));
        case 2:
            IEnumerable<string> midLexicon =
                words.Where(word => word.Length >= 7 && word.Length < 9);
            return midLexicon.ElementAt(Random.Shared.Next(midLexicon.Count()));
        case 3:
            IEnumerable<string> hardLexicon =
                words.Where(word => word.Length >= 9);
            return hardLexicon.ElementAt(Random.Shared.Next(hardLexicon.Count()));
        default:
            throw new ArgumentException("Difficulty must be 1, 2, or 3!");
    }
}

string guess = selectWord(diffSetting, words);

Console.WriteLine($"Your word is: {guess}");

// string selectedWord = fiveToSix.ElementAt(Random.Shared.Next(fiveToSix.Count()));

//GAME LOOP
// while livesLeft(lives) > 0://