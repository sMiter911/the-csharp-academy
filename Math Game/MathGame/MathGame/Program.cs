/*
1. You need to create a Math game containing the 4 basic operations
2. The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
3. Users should be presented with a menu to choose an operation
4. You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
5. You don't need to record results on a database. Once the program is closed the results will be deleted.
 */

using MathGame;
using System.Diagnostics;

MathGameLogic mathGame = new MathGameLogic();
Random random = new Random();

int firstNumber;
int secondNUmber;
int userMenuSelection;
int score = 0;
bool gameOver = false;

DifficultyLevel _difficultyLevel = DifficultyLevel.Easy;
while (!gameOver)
{
    userMenuSelection = GetUserMenuSelection(mathGame);
    firstNumber = random.Next(1, 101);
    secondNUmber = random.Next(1, 101);

    switch (userMenuSelection)
    {
        case 1:
            score += await PerformOperation(mathGame, firstNumber, secondNUmber, '+', score, _difficultyLevel);
            break;
        case 2:
            score += await PerformOperation(mathGame, firstNumber, secondNUmber, '-', score, _difficultyLevel);
            break;
        case 3:
            score += await PerformOperation(mathGame, firstNumber, secondNUmber, '*', score, _difficultyLevel);
            break;
        case 4:
            while(firstNumber % secondNUmber != 0)
            {
                firstNumber = random.Next(1, 101);
                secondNUmber = random.Next(1, 101);
            }
            score += await PerformOperation(mathGame, firstNumber, secondNUmber, '/', score, _difficultyLevel);
            break;
        case 5:
            int numberOfQuestions = 99;
            Console.WriteLine("Please enter a number of questions you want to attempt: ");
            while(!int.TryParse(Console.ReadLine(), out numberOfQuestions))
            {
                Console.WriteLine("Please enter a number of questions you want to attempt: ");
            }
            while(numberOfQuestions > 0)
            {
                int randomOperation = random.Next(1, 5);
                if(randomOperation == 1)
                {
                    firstNumber = random.Next(1, 101);
                    secondNUmber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNUmber, '+', score, _difficultyLevel);
                } else if(randomOperation == 2)
                {
                    firstNumber = random.Next(1, 101);
                    secondNUmber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNUmber, '-', score, _difficultyLevel);
                } else if(randomOperation == 3)
                {
                    firstNumber = random.Next(1, 101);
                    secondNUmber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNUmber, '*', score, _difficultyLevel);
                } else if(randomOperation == 4)
                {
                    firstNumber = random.Next(1, 101);
                    secondNUmber = random.Next(1, 101);
                    while (firstNumber % secondNUmber != 0)
                    {
                        firstNumber = random.Next(1, 101);
                        secondNUmber = random.Next(1, 101);
                    }
                    score += await PerformOperation(mathGame, firstNumber, secondNUmber, '/', score, _difficultyLevel);
                }
                numberOfQuestions--;
            }
            break;
        case 6:
            Console.WriteLine("GAME HISTORY: \n");
            foreach(var operation in mathGame.GameHistory)
            {
                Console.WriteLine($"{operation}");
            }
            break;
        case 7:
            _difficultyLevel = ChangeDifficulty();
            DifficultyLevel difficultyEnum = (DifficultyLevel)_difficultyLevel;
            Enum.IsDefined(typeof(DifficultyLevel), difficultyEnum);
            Console.WriteLine("Difficulty level changed to: " + _difficultyLevel);
            break;
        case 8:
            gameOver = true;
            Console.WriteLine("Thanks for playing! Your final score is: " + score);
            break;
    }
}

static DifficultyLevel ChangeDifficulty()
{
    int userSelection = 0;

    Console.WriteLine("Please select a difficulty level: ");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");

    while(!int.TryParse(Console.ReadLine(), out userSelection) || (userSelection < 1 || userSelection > 3))
    {
        Console.WriteLine("Please enter a valid number between 1 and 3: ");
    }

    return userSelection switch
    {
        1 => DifficultyLevel.Easy,
        2 => DifficultyLevel.Medium,
        3 => DifficultyLevel.Hard,
        _ => DifficultyLevel.Easy
    };
}

static void DisplayMathGameQuestion(int firstNumber, int secondNumber, char operation)
{
    Console.WriteLine($"What is {firstNumber} {operation} {secondNumber}?");
}

static int GetUserMenuSelection(MathGameLogic mathGame)
{
    int selection = -1;
    mathGame.ShowMenu();
    while (selection < 1 || selection > 8)
    {
        while(!int.TryParse(Console.ReadLine(), out selection))
        {
            Console.WriteLine("Please enter a valid number between 1 and 8: ");
        }

        if(!(selection >= 1 && selection <= 8))
        {
            Console.WriteLine("Please enter a valid number between 1 and 8: ");
        }
    }

    return selection;
}

static async Task<int?> GetUserResponse(DifficultyLevel difficulty)
{
    int response = 0;
    int timeout = (int)difficulty;

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    Task<string?> getUserInputTask = Task.Run(() => Console.ReadLine());

    try
    {
        string? result  = await Task.WhenAny(getUserInputTask, Task.Delay(timeout * 1000)) == getUserInputTask ? await getUserInputTask : null;
        stopwatch.Stop();
        if(result != null && int.TryParse(result, out response))
        {
            Console.WriteLine($"You took {stopwatch.Elapsed.TotalSeconds} seconds to answer.");
            return response;
        }
        else 
        { 
            throw new OperationCanceledException();
        }
    } catch (OperationCanceledException)
    {
        Console.WriteLine($"Time is up!!!");
        return null;
    }
}

static int ValidateResult(int result, int? userResponse, int score)
{
    if(result == userResponse)
    {
        Console.WriteLine("You answered conrrectly! You earned 5 points");
        return score + 5;
    }
    else
    {
        Console.WriteLine("You answered incorrectly!");
        Console.WriteLine($"The correct answer is {result}.");
    }
    return score;
}

static async Task<int> PerformOperation(MathGameLogic mathGame, int firstNumber, int secondNumber, char operation, int score, DifficultyLevel difficulty)
{
    int result;
    int? userResponse;
    DisplayMathGameQuestion(firstNumber, secondNumber, operation);
    result = mathGame.MathOperation(firstNumber, secondNumber, operation);
    userResponse =  await GetUserResponse(difficulty);
    score += ValidateResult(result, userResponse, score);
    return score;
}