using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;
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
        if (int.TryParse(input, out diffSetting) && diffSetting is 1 or 2 or 3)
        {
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
    switch (diffSetting)
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

string secretWord = selectWord(diffSetting, words);

Console.WriteLine($"Your word is: {secretWord}");

/*
// GAME BOARD

    ________________
    |       |
    |       |
    |       o
    |      /|\
    |      / \
    |_________________
   /|\               |
__/_|_\______________|___
*/

string[] gallow =
["   ________________",
"    |",
"    |",
"    |",
"    |",
"    |",
"    |_________________",
"   /|\\               |",
"__/_|_\\______________|___"];

string[] head = gallow.ToArray();
head[1] = "    |       |";
head[2] = head[1];
head[3] = "    |       o";

string[] body = head.ToArray();
body[4] = body[1];

string[] leftArm = body.ToArray();
leftArm[4] = "    |      /|";

string[] rightArm = leftArm.ToArray();
rightArm[4] = "    |      /|\\";

string[] leftLeg = rightArm.ToArray();
leftLeg[5] = "    |      /";

string[] rightLeg = leftLeg.ToArray();
rightLeg[5] = "    |      / \\";

string[][] gamestate = [gallow, head, body, leftArm, rightArm, leftLeg, rightLeg];



// Game State

foreach (string segment in gamestate[0])
{
    // Console.Clear();
    Console.WriteLine($"{segment}");
}


//REMEMBER TO ACCOUNT FOR DOUBLE LETTERS IN WORD
// Game is won when correctGuesses matches all UNIQUE letters in secretword

//GAME LOOP
int wrongGuess = 0;
List<char> correctGuesses = new List<char> {};

while (wrongGuess < gamestate.Length)
{
    char letterGuess = getLetterGuess(correctGuesses);
    /*
    if checkLetterGuess(letterGuess);
    {
        
        // Show all guessed letters in the game loop, if correctGuesses equals length then game over
    }
    */
    wrongGuess++;
}

//User Input
static char getLetterGuess(List<char> correctGuesses)
{
    while (true) {
        string letter = Console.ReadLine();
        if (char.TryParse(letter, out char letterGuess) && isValidLetter(letterGuess))
        {
            if (!correctGuesses.Contains(letterGuess))
            {
                correctGuesses.Add(letterGuess);
                return letterGuess;
            }
        }
    }
}

static bool isValidLetter(char letter)
{
    return (letter >= 'a' && letter <= 'z') ||
        (letter >= 'A' && letter <= 'Z');
}

bool checkLetterGuess(guess)
{
    correct
}