using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

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

string secretWord = selectWord(diffSetting, words).ToLower();

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

    _ _ _ _ _ _ _ _
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


// Wrong Guesses


// Game State - draws the gallow based on gamestate (number of wrong guesses)
static void makeGallow(string[][] gamestate, List<char> wrongGuess)
{
    Console.Clear();
    Console.WriteLine("\n");
    // Gamestate and gallow drawing is based on number of wrong guesses so far
    foreach (string segment in gamestate[wrongGuess.Count])
    {
        Console.WriteLine($"{segment}");
    }
    return;
}


//REMEMBER TO ACCOUNT FOR DOUBLE LETTERS IN WORD
// Game is won when correctGuesses matches all UNIQUE letters in secretword

//GAME LOOP
List<char> wordAnswer = new List<char> { };
List<char> wrongGuess = new List<char> { };

// Call after running code to populate wordAnswer with "_"
bool gameover(List<char> wrongGuess, List<char> wordAnswer, string[][] gamestate)
{
    if (wordAnswer.Count > 0 && !wordAnswer.Contains('_'))
    {
        Console.WriteLine("Congratulations! You Won!");
        return true;
    }
    else if (wrongGuess.Count >= gamestate.Length-1)
    {
        Console.WriteLine("Sorry, you lost!");
        return true;
    }
    else
    {
        return false;
    }
}


void mainGame()
{
    //Initial Set-up of Gameboard
    makeGallow(gamestate, wrongGuess);
    wordAnswer = generateAnsweredWord(secretWord, wordAnswer);

    // Main Game Loop
    while (!gameover(wrongGuess, wordAnswer, gamestate))
    {
        //Player Letter Guess
        char letterGuess = getLetterGuess(wordAnswer, wrongGuess);

        // Redraw Board w/Guess and Answered Word Updated
        (wordAnswer, wrongGuess) = updateGamestate(letterGuess, secretWord, wordAnswer, wrongGuess);
        makeGallow(gamestate, wrongGuess);        
        //logic is wrong because it generates anaswered word draws it and then makes gallow after
        generateSpacesAndWriteToConsole(wordAnswer);
        
        // still need to print incorrect guesses at bottom
        printWrongGuesses(wrongGuess);
    }
    return;
}


void printWrongGuesses(List<char> wrongGuess)
{
    Console.Write("\n\nIncorrect Guesses: ");
    foreach (char l in wrongGuess)
    {
        Console.Write($"{l}, ");
    }
    Console.WriteLine("");
    return;
}

// Take user's input for letter guess and checks for valid input --> outputs character
static char getLetterGuess(List<char> wordAnswer, List<char> wrongGuess)
{
    while (true)
    {
        Console.WriteLine("\nEnter a valid letter for your guess:");
        string letter = Console.ReadLine();
        if (char.TryParse(letter, out char letterGuess) && isValidLetter(letterGuess))
        {
            if (!wordAnswer.Contains(letterGuess) && !wrongGuess.Contains(letterGuess))
            {
                return char.ToLower(letterGuess);
            }
            else
            {
                Console.WriteLine("You already guessed this letter. Try Again!");
            }
        }
        else
        {
            Console.WriteLine("Invalid letter. Try Again!");
        }
    }
}

static bool isValidLetter(char letter)
{
    return (letter >= 'a' && letter <= 'z') ||
        (letter >= 'A' && letter <= 'Z');
}



// UPDATE GAMESTATE - checks to see if valid character is correct guess and updates corresponding wordlist
// guess, secretWord, wordAnswerm wrongGuess -->  wordAnswer, wrongGuess
(List<char>, List<char>) updateGamestate(char letterGuess, string secretWord, List<char> wordAnswer, List<char> wrongGuess)
{
    // CORRECT GUESS: If guess is in secretWord --> updates wordAnswer
    if (secretWord.Contains(letterGuess))
    {
        wordAnswer = generateAnsweredWord(secretWord, wordAnswer, letterGuess);
    }
    // INCORRECT GUESS: Else --> updates wrongGuess & Gamestate (only update gamestate if guess is wrong)
    else
    {
        wrongGuess.Add(letterGuess);
    }
    return (wordAnswer, wrongGuess);
}


// Get indices of letter guess in the secretWord
List<int> letterIndex(char guess, string secretWord)
{
    List<int> letterIndices = new List<int> { };
    for (int i = 0; i < secretWord.Length; i++)
    {
        if (guess == secretWord[i])
        {
            letterIndices.Add(i);
        }
    }
    return letterIndices;
}

// Used to generate spaces between letters
// when displaying correct guesses on screen
void generateSpacesAndWriteToConsole(List<char> wordAnswer)
{
    Console.WriteLine("\n   ");
    if (wordAnswer.Count == 0 || !wordAnswer.Any())
    {
        return;
    }
    else
    {
        foreach (char c in wordAnswer)
        {
            Console.Write($"{c} ");
        }
        return;
    }
}

// Updates wordAnswer and prints to console
List<char> generateAnsweredWord(string secretWord, List<char> wordAnswer, char? guess = null)
{
    // First time through - generates blank word spaces
    if (guess == null && (wordAnswer.Count == 0 || !wordAnswer.Any()))
    {
        for (int c = 0; c < secretWord.Length; c++)
        {
            wordAnswer.Add('_');
        }
        generateSpacesAndWriteToConsole(wordAnswer);
        return wordAnswer;
    }
    // Updates wordAndswer and prints to console
    else if (guess != null)
    {
        foreach (int i in letterIndex(guess.Value, secretWord))
        {
            wordAnswer[i] = guess.Value;
        }
        return wordAnswer;
    }
    else
    {
        return wordAnswer;
    }
}

mainGame();