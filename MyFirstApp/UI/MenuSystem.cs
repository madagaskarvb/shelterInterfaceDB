using System;
using System.Collections.Generic;
using System.Threading;

namespace AnimalShelterCLI.UI
{
    // Пункт меню: действие или фабрика подменю
    public class MenuItem
    {
        public string Title { get; set; } = "";
        public Action? Action { get; set; }
        public Func<Menu>? SubMenuFactory { get; set; }

        public bool IsSubMenu => SubMenuFactory != null;
    }

    // Простейшая система меню
    public class Menu
    {
        private readonly List<MenuItem> _items = new();
        public string Title { get; }

        public Menu(string title)
        {
            Title = title ?? "";
        }

        public void AddItem(string title, Action action)
        {
            _items.Add(new MenuItem { Title = title, Action = action });
        }

        public void AddSubMenu(string title, Func<Menu> subMenuFactory)
        {
            _items.Add(new MenuItem { Title = title, SubMenuFactory = subMenuFactory });
        }

        public void Display()
        {
            while (true)
            {
                MenuHelper.PrintHeader(Title);

                // Рендер пунктов
                for (int i = 0; i < _items.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_items[i].Title}");
                }
                Console.WriteLine("0. Выход");

                Console.Write("\nВаш выбор: ");
                var key = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(key))
                {
                    MenuHelper.PrintError("Введите номер пункта.");
                    continue;
                }

                if (!int.TryParse(key, out var num))
                {
                    MenuHelper.PrintError("Неверный ввод. Ожидается число.");
                    continue;
                }

                if (num == 0)
                {
                    // При выходе из верхнего уровня остановим анимацию
                    AsciiAnimator.TryStopIfRoot();
                    return;
                }

                if (num < 0 || num > _items.Count)
                {
                    MenuHelper.PrintError("Неверный номер пункта.");
                    continue;
                }

                var item = _items[num - 1];

                try
                {
                    if (item.IsSubMenu)
                    {
                        var sub = item.SubMenuFactory!.Invoke();
                        sub.Display();
                    }
                    else
                    {
                        // Для экранов действий шапка уже рисует анимацию
                        item.Action?.Invoke();
                        MenuHelper.PressAnyKey();
                    }
                }
                catch (Exception ex)
                {
                    MenuHelper.PrintError($"Ошибка: {ex.Message}");
                    MenuHelper.PressAnyKey();
                }
            }
        }
    }

    // Утилиты оформления
    public static class MenuHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();

            // Запуск анимации (если выключена глобально — ничего не произойдет)
            AsciiAnimator.EnsureRunning();

            // Рамка + заголовок
            var line = new string('═', Math.Max(10, Math.Min(Console.WindowWidth - 2, title.Length + 6)));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔{line}╗");
            Console.WriteLine($"║  {title}  ║");
            Console.WriteLine($"╚{line}╝");
            Console.ResetColor();

            // Отступ под шапкой
            Console.WriteLine();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PressAnyKey()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            // Пока ждем — анимация продолжает работать в шапке
            Console.ReadKey(true);
        }
    }

    // Фоновая ASCII-анимация животных
    internal static class AsciiAnimator
    {
        // Общий флажок на случай необходимости отключить анимацию
        public static bool Enabled = true;

        private static readonly object _sync = new();
        private static Thread? _thread;
        private static CancellationTokenSource? _cts;

        // Сколько строк занимает баннер анимации
        private const int BannerHeight = 6;
        private const int FrameDelayMs = 120;

        // Чтобы не стартовать второй раз при вложенных экранах
        private static int _startCount = 0;

        // Наборы кадров
        private static readonly string[][] Cat =
        {
            new []
            {
                " /\\_/\\ ",
                "( o.o )",
                " > ^ < ",
                "  = =  ",
                "       ",
                "  Кот  "
            },
            new []
            {
                " /\\_/\\ ",
                "( •.• )",
                " > ^ < ",
                "  = =  ",
                "       ",
                "  Кот  "
            }
        };

        private static readonly string[][] Dog =
        {
            new []
            {
                " /‾‾‾\\ ",
                "(•ᴥ• )",
                " /| |\\ ",
                "  | |  ",
                "       ",
                " Собака"
            },
            new []
            {
                " /‾‾‾\\ ",
                "(•ᴥ• )",
                " /| |\\ ",
                "  | |  ",
                "  ✦    ",
                " Собака"
            }
        };

        private static readonly string[][] Bunny =
        {
            new []
            {
                " (\\_/) ",
                " (•.•) ",
                " />🍃  ",
                "       ",
                "       ",
                " Кролик"
            },
            new []
            {
                " (\\_/) ",
                " (•.•) ",
                "  🍃<\\ ",
                "       ",
                "       ",
                " Кролик"
            }
        };

        private static readonly string[][][] Animals = { Cat, Dog, Bunny };

        public static void EnsureRunning()
        {
            if (!Enabled) return;

            lock (_sync)
            {
                _startCount++;
                if (_thread is { IsAlive: true }) return;

                _cts = new CancellationTokenSource();
                var token = _cts.Token;
                _thread = new Thread(() => RunLoop(token))
                {
                    IsBackground = true,
                    Name = "ASCII-Animals-Animator"
                };
                _thread.Start();
            }
        }

        // Вызывается при выходе из корневого меню
        public static void TryStopIfRoot()
        {
            lock (_sync)
            {
                if (_startCount > 0) _startCount--;
                if (_startCount == 0)
                {
                    StopInternal();
                }
            }
        }

        private static void StopInternal()
        {
            try
            {
                _cts?.Cancel();
                _thread?.Join(300);
            }
            catch { /* ignore */ }
            finally
            {
                _cts = null;
                _thread = null;
                ClearBannerArea();
            }
        }

        private static void RunLoop(CancellationToken token)
        {
            var rnd = new Random();
            var animalIndex = rnd.Next(Animals.Length);
            var frame = 0;
            var lastWidth = Console.WindowWidth;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var frames = Animals[animalIndex];
                    DrawFrame(frames[frame]);

                    frame = (frame + 1) % frames.Length;

                    // Периодически меняем животное
                    if (rnd.NextDouble() < 0.02)
                        animalIndex = rnd.Next(Animals.Length);

                    // Если ширина консоли поменялась — очистим область для корректной перерисовки
                    if (Console.WindowWidth != lastWidth)
                    {
                        lastWidth = Console.WindowWidth;
                        ClearBannerArea();
                    }

                    Thread.Sleep(FrameDelayMs);
                }
                catch
                {
                    // Любые ошибки рисования не должны валить приложение
                    Thread.Sleep(200);
                }
            }
        }

        private static void DrawFrame(string[] frame)
        {
            lock (_sync)
            {
                // Сохраним позицию курсора
                int curLeft, curTop;
                try
                {
                    curLeft = Console.CursorLeft;
                    curTop = Console.CursorTop;
                }
                catch
                {
                    curLeft = 0;
                    curTop = BannerHeight + 1;
                }

                var width = MaxWidth(frame) + 2;
                var left = Math.Max(0, Console.WindowWidth - width - 2);
                var top = 0;

                // Рисуем «баннер» справа сверху
                for (int i = 0; i < BannerHeight; i++)
                {
                    var line = i < frame.Length ? frame[i] : "";
                    WriteAt(left, top + i, PadTo(line, width));
                }

                // Восстановим позицию курсора
                TrySetCursor(curLeft, curTop);
            }
        }

        private static void ClearBannerArea()
        {
            lock (_sync)
            {
                var width = Math.Max(10, Console.WindowWidth / 3);
                var left = Math.Max(0, Console.WindowWidth - width - 2);
                for (int i = 0; i < BannerHeight; i++)
                {
                    WriteAt(left, i, new string(' ', width + 2));
                }
            }
        }

        private static void WriteAt(int left, int top, string text)
        {
            TrySetCursor(left, top);
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(text);
            }
            catch { /* ignore */ }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void TrySetCursor(int left, int top)
        {
            try
            {
                left = Math.Max(0, Math.Min(left, Math.Max(0, Console.WindowWidth - 1)));
                top = Math.Max(0, Math.Min(top, Math.Max(0, Console.WindowHeight - 1)));
                Console.SetCursorPosition(left, top);
            }
            catch { /* ignore */ }
        }

        private static int MaxWidth(string[] lines)
        {
            var max = 0;
            foreach (var l in lines)
                if (l.Length > max) max = l.Length;
            return max;
        }

        private static string PadTo(string s, int w)
        {
            if (s.Length >= w) return s;
            return s + new string(' ', w - s.Length);
        }
    }
}
