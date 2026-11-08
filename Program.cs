using System.IO;

//IMPORT LEXICON
string lexicon_path = Path.Combine(AppContext.BaseDirectory, "1k_lexicon.txt");

string[] words = File.ReadAllText(lexicon_path)
    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);


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


///////////////////////////
///// GAME CODE BELOW /////
///////////////////////////


List<char> wordAnswer = new List<char> { };
List<char> wrongGuess = new List<char> { };
string secretWord = "";

// MAIN GAME CODE //
void mainGame()
{
    //Initial Set-up of Gameboard
    makeGallow(gamestate, wrongGuess);
    wordAnswer = generateAnsweredWord(secretWord, wordAnswer);

    // INNER GAME LOOP
    while (!gameover(wrongGuess, wordAnswer, gamestate, secretWord))
    {
        //Player Letter Guess
        char letterGuess = getLetterGuess(wordAnswer, wrongGuess);

        // Redraw Board w/Guess and Answered Word Updated
        (wordAnswer, wrongGuess) = updateGamestate(letterGuess, secretWord, wordAnswer, wrongGuess);
        makeGallow(gamestate, wrongGuess);
        // Writes out the correct guesses and unknown letters so far
        generateSpacesAndWriteToConsole(wordAnswer);

        // Prints Wrong Guesses at bottom of console window
        printWrongGuesses(wrongGuess);
    }
    return;
}

/// START OF GAME ///
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
        string? input = Console.ReadLine();
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

// Select word based on difficulty
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

// USER INPUT & VALDIATION //
// Take user's input for letter guess and checks for valid input --> outputs character
static char getLetterGuess(List<char> wordAnswer, List<char> wrongGuess)
{
    while (true)
    {
        Console.WriteLine("\nEnter a valid letter for your guess:");
        string? letter = Console.ReadLine();
        if (char.TryParse(letter, out char letterGuess) && isValidLetter(letterGuess))
        {
            letterGuess = char.ToLower(letterGuess); //converts letterGuess to lower character
            if (!wordAnswer.Contains(letterGuess) && !wrongGuess.Contains(letterGuess))
            {
                return letterGuess;
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

// Checks if character is valid input (true/false)
static bool isValidLetter(char letter)
{
    return (letter >= 'a' && letter <= 'z') ||
        (letter >= 'A' && letter <= 'Z');
}




// UPDATE GAME DATA //

// UPDATE GAMESTATE - checks to see if valid character is correct guess and updates corresponding wordlist
// guess, secretWord, wordAnswerm wrongGuess -->  wordAnswer, wrongGuess
(List<char>, List<char>) updateGamestate(char letterGuess, string secretWord, List<char> wordAnswer, List<char> wrongGuess)
{
    // CORRECT GUESS: If guess is in secretWord --> updates wordAnswer
    if (secretWord.Contains(letterGuess))
    {
        wordAnswer = generateAnsweredWord(secretWord, wordAnswer, letterGuess);
    }
    // INCORRECT GUESS: Else --> updates wrongGuess
    else
    {
        wrongGuess.Add(letterGuess);
    }
    return (wordAnswer, wrongGuess);
}

// Updates wordAnswer and prints to console
List<char> generateAnsweredWord(string secretWord, List<char> wordAnswer, char? guess = null)
{
    // First time through - generates blank word spaces
    if (guess == null && wordAnswer.Count == 0)
    {
        for (int c = 0; c < secretWord.Length; c++)
        {
            wordAnswer.Add('_');
        }
        generateSpacesAndWriteToConsole(wordAnswer);
        return wordAnswer;
    }
    // Updates wordAnswer
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

// GAME OVER CONDITIONS //
// Gameover conditon that will end main gameloop
bool gameover(List<char> wrongGuess, List<char> wordAnswer, string[][] gamestate, string secretWord)
{
    if (wordAnswer.Count > 0 && !wordAnswer.Contains('_'))
    {
        Console.WriteLine("Congratulations! You Won!");
        return true;
    }
    else if (wrongGuess.Count >= gamestate.Length - 1)
    {
        Console.WriteLine("Sorry, you lost!");
        Console.WriteLine($"\nThe secret word was: {secretWord.ToUpper()}");
        return true;
    }
    else
    {
        return false;
    }
}


// UPDATE GAME DISPLAY //
// Draws the gallow based on gamestate (number of wrong guesses)
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

// Used to generate spaces between letters
// when displaying correct guesses on screen
void generateSpacesAndWriteToConsole(List<char> wordAnswer)
{
    Console.Write("   ");
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

// Prints incorrect guesses at bottom of console
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



//REPLAY GAME
bool replayGameLoop()
{
    while (true)
    {
        Console.WriteLine("\nStart new game? (y/n)");
        string? letter = Console.ReadLine();
        if (char.TryParse(letter, out char contGame) && isValidLetter(contGame))
        {
            contGame = char.ToLower(contGame);
            if (contGame == 'y')
            {
                wordAnswer.Clear();
                wrongGuess.Clear();

                return true;
            }
            else if (contGame == 'n')
            {
                return false;
            }
            else
            {
                Console.Clear();
                continue;
            }
        }
        Console.WriteLine("Invalid Choice. Please choose only 'y' or 'n'!");
    }
}


// CODE TO RUN GAME //
do
{
    int diffSetting = gameStart();
    secretWord = selectWord(diffSetting, words).ToLower();

    //Just for debugging:
    // Console.WriteLine($"Your word is: {secretWord}");

    mainGame();
}
while (replayGameLoop());