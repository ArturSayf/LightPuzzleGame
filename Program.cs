using System;

namespace LightPuzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            LightPuzzleGame game = new LightPuzzleGame();
            game.Run();
        }
    }

    public class LightPuzzleGame
    {
        private char[,] board;
        private bool[,] litLamps;
        private int currentX, currentY;
        private int boardSize;

        public LightPuzzleGame()
        {
            boardSize = 6;
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            board = new char[boardSize, boardSize];
            litLamps = new bool[boardSize, boardSize];

            // Инициализация поля - 0 для лампочки, 1 для стены
            int[,] field = {
                {1, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0},
                {0, 1, 0, 0, 0, 0},
                {0, 1, 0, 1, 0, 0},
                {0, 0, 0, 0, 0, 0},
                {1, 1, 1, 0, 0, 0}
            };

            // Заполняем поле на основе заданной конфигурации
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (field[i, j] == 1)
                    {
                        board[i, j] = '#'; // Стена
                    }
                    else
                    {
                        board[i, j] = 'O'; // Негорящая лампочка
                    }
                    litLamps[i, j] = false;
                }
            }

            // Начальная позиция - всегда в левом верхнем углу (0,0)
            currentX = 1;
            currentY = 4;

            // Зажигаем стартовую лампочку
            litLamps[currentX, currentY] = true;
            board[currentX, currentY] = '☼'; // Горящая лампочка
        }

        public void Run()
        {
            Console.WriteLine("ГОЛОВОЛОМКА С ЛАМПОЧКАМИ");
            Console.WriteLine("Цель: зажечь все лампочки на поле");
            Console.WriteLine("Управление:");
            Console.WriteLine("W - зажечь лампочки вверх");
            Console.WriteLine("S - зажечь лампочки вниз");
            Console.WriteLine("A - зажечь лампочки влево");
            Console.WriteLine("D - зажечь лампочки вправо");
            Console.WriteLine("R - сбросить игру");
            Console.WriteLine("Q - выйти");

            bool playing = true;

            while (playing)
            {
                DisplayBoard();
                DisplayStatus();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                Console.WriteLine();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        LightDirection(-1, 0);
                        break;
                    case ConsoleKey.S:
                        LightDirection(1, 0);
                        break;
                    case ConsoleKey.A:
                        LightDirection(0, -1);
                        break;
                    case ConsoleKey.D:
                        LightDirection(0, 1);
                        break;
                    case ConsoleKey.R:
                        ResetGame();
                        Console.WriteLine("Игра сброшена!");
                        break;
                    case ConsoleKey.Q:
                        playing = false;
                        break;
                    default:
                        Console.WriteLine("Неверная команда!");
                        break;
                }

                if (CheckWinCondition())
                {
                    DisplayBoard();
                    Console.WriteLine("ПОЗДРАВЛЯЕМ! ВЫ ЗАЖГЛИ ВСЕ ЛАМПОЧКИ!");
                    playing = false;
                }
                else if (CheckDeadlock())
                {
                    DisplayBoard();
                    Console.WriteLine("ТУПИК! Нажмите R для сброса игры.");
                }
            }
        }

        private void LightDirection(int dx, int dy)
        {
            int x = currentX + dx;
            int y = currentY + dy;

            while (IsInBounds(x, y))
            {
                if (board[x, y] == '#') // Стена
                {
                    break;
                }

                if (litLamps[x, y]) // Уже горящая лампочка
                {
                    break;
                }

                // Зажигаем лампочку
                litLamps[x, y] = true;
                board[x, y] = '☼';

                // Обновляем текущую позицию
                currentX = x;
                currentY = y;

                x += dx;
                y += dy;
            }
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        private void DisplayBoard()
        {
            Console.Clear();
            Console.WriteLine("ПОЛЕ:");

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (i == currentX && j == currentY)
                    {
                        Console.Write('@'); // Текущая позиция
                    }
                    else
                    {
                        Console.Write(board[i, j]);
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        private void DisplayStatus()
        {
            int totalLamps = 0;
            int litCount = 0;

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == 'O' || board[i, j] == '☼')
                    {
                        totalLamps++;
                        if (litLamps[i, j])
                        {
                            litCount++;
                        }
                    }
                }
            }

            Console.WriteLine($"\nЗажжено лампочек: {litCount}/{totalLamps}");
            Console.WriteLine($"Текущая позиция: [{currentX}, {currentY}]");
        }

        private bool CheckWinCondition()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == 'O') // Если есть негорящая лампочка
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckDeadlock()
        {
            // Проверяем все возможные направления от текущей позиции
            int[][] directions = new int[][]
            {
                new int[] {-1, 0}, // вверх
                new int[] {1, 0},  // вниз
                new int[] {0, -1}, // влево
                new int[] {0, 1}   // вправо
            };

            foreach (var dir in directions)
            {
                int x = currentX + dir[0];
                int y = currentY + dir[1];

                if (IsInBounds(x, y) && board[x, y] == '○')
                {
                    return false; // Есть доступная лампочка
                }
            }

            return true; // Все направления заблокированы
        }

        private void ResetGame()
        {
            InitializeBoard();
        }
    }
}