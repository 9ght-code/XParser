using Spectre.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace XParser
{
    internal class Program
    {
       static bool is_dir_mode = false;

        static private void init()
        {
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");
            
            if(!Directory.Exists("input"))
                Directory.CreateDirectory("input");
        }

        static private void settings_menu()
        {
            while (true)
            {
                Console.Clear();
                var settings = AnsiConsole.Prompt
            (
                new SelectionPrompt<string>()
                .Title("\nНастройки после перезапуска [red]сбрасываются[/]")
                .PageSize(10)
                .AddChoices(new[] { "Добавить ограничения по длине", "Изменить количество строк для нарезки", "Удалить все ограничения по длине", 
                    "Изменить максимальное ограничение длины", "Вернуться" })
                .HighlightStyle(Color.Yellow)

            );

                try
                {
                    switch (settings)
                    {
                        case "Вернуться":
                            return;



                        case "Добавить ограничения по длине":
                            var limits_panel = new Panel($"Ограничения: [blue]{String.Join(", ", Options.endpoints)}[/]\n" +
                                $"Максимальное ограничение длины: [red]{Options.ingoing_file_length}[/]");

                            AnsiConsole.Write(limits_panel);
                            Thread.Sleep(500);

                            var limits = AnsiConsole.Ask<string>("Укажите ограничения через <пробел>:");
                            Thread.Sleep(500);

                            foreach (string item in limits.Split(' ')) { if (Convert.ToInt32(item) <= Options.ingoing_file_length) Options.endpoints.Add(Convert.ToInt32(item)); }
                            Console.Clear();

                            AnsiConsole.Markup($"Ограничения: [blue]{String.Join(", ", Options.endpoints)}[/]\n");
                            AnsiConsole.Markup($"Успешно");
                            Thread.Sleep(2000);
                            break;

                        case "Изменить количество строк для нарезки":
                            var count_string_panel = new Panel($"Ограничение по нарезке: [blue]{Options.point}[/]");
                            AnsiConsole.Write(count_string_panel);
                            Thread.Sleep(500);

                            var count_string_limit = AnsiConsole.Ask<Int32>("Укажите ограничение:");
                            Options.point = count_string_limit;
                            Thread.Sleep(500);

                            Console.Clear();

                            AnsiConsole.Markup($"Ограничение: [blue]{Options.point}[/]\n");
                            AnsiConsole.Markup($"Успешно");
                            Thread.Sleep(2000);
                            break;

                        case "Удалить все ограничения по длине":
                            Options.endpoints.Clear();
                            AnsiConsole.Write(new Panel("Успешно"));
                            Thread.Sleep(2000);
                            break;

                        case "Изменить максимальное ограничение длины":
                            AnsiConsole.Write(new Panel($"Максимальное ограничение длины: [red]{Options.ingoing_file_length}[/]"));
                            Thread.Sleep(500);

                            var max_limit = AnsiConsole.Ask<Int32>("Укажите ограничение:");
                            Options.ingoing_file_length = max_limit;
                            Thread.Sleep(500);

                            Console.Clear();

                            AnsiConsole.Markup($"Максимальное ограничение длины: [red]{Options.ingoing_file_length}[/]\n");

                            foreach ( var item in Options.endpoints ) {if (item > max_limit) { Options.endpoints.Remove(item); break; } }

                            AnsiConsole.Markup($"Успешно");
                            Thread.Sleep(2000);
                            break;
                    }
                }

                catch (Exception ex) {Console.Clear(); Console.WriteLine("Вы забыли, как писать цифры?"); AnsiConsole.Write(new Panel(ex.Message));  Thread.Sleep(2000); }
            }
        }

        static private string mode()
        {
            string ingoing = @"";

            while (true)
            {
                Console.Clear();

                var mode = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<string>()
                        .Title("Выберите [green]режим[/]")
                        .PageSize(10)
                        .AddChoices(new[] { "Указать файл", "Использовать папку input"})
                        .HighlightStyle(Color.Yellow)
                    );

                switch (mode)
                {
                    case "Указать файл":
                        var path = AnsiConsole.Ask<string>("Укажите [green]путь[/] к файлу:");

                        var path_interptretation = new TextPath(path)
                                .RootColor(Color.Red)
                                .SeparatorColor(Color.Green)
                                .StemColor(Color.Blue)
                                .LeafColor(Color.Yellow);

                        var panel = new Panel(path_interptretation);

                        AnsiConsole.Write(panel);

                        Thread.Sleep(1000);

                        if (File.Exists(path)) { ingoing = path; AnsiConsole.Markup("\nфайл [green]найден[/]"); Thread.Sleep(500); Console.Clear(); break; }

                        else { AnsiConsole.Markup("Такого файла [red]не существует[/]"); Thread.Sleep(1000); continue; }

                    case "Использовать папку input":
                        is_dir_mode = true;
                        break;
                }

                return ingoing;
            }

        }

        static void Main(string[] args)
        {
            init();

            while (true)
            {
                Console.Clear();

                AnsiConsole.Write(
                    new FigletText("XParser")
                        .LeftJustified()
                        .Color(Color.Red));

                AnsiConsole.Write(new Rule("[green]by 9ght[/]") { Justification = Justify.Left });

                var menu = AnsiConsole.Prompt
                    (
                        new SelectionPrompt<string>()
                        .Title("\nВыберите [green]опцию[/] (не забудьте закрыть файлы для [blue]чтения[/] и [blue]записи[/])")
                        .PageSize(10)
                        .AddChoices(new[] { "Запустить парсер", "Настройки", "Выйти" })
                        .HighlightStyle(Color.Yellow)

                    );

                switch (menu)
                {
                    case "Выйти":
                        AnsiConsole.Markup("[green]До свидания![/]");
                        Thread.Sleep(1000);

                        Environment.Exit(0);
                        break;

                    case "Запустить парсер":
                        try
                        {
                            Parser parser = new Parser(mode());

                            if (is_dir_mode == false)
                            {
                                int work = 1;


                                AnsiConsole.Status()
                                    .Start("Выполняю...", ctx => 
                                    { 
                                        work = parser.Parse(); Thread.Sleep(2000);
                                        var p = Process.Start("Converter.exe");
                                        ctx.Status("Конвертирую");
                                        p.WaitForExit();
                                    });

                                if (work == 0)
                                    AnsiConsole.Markup("[green]Успешно[/]");

                                else AnsiConsole.Markup("[red]неудача[/]");
                                Thread.Sleep(1000);
                                break;
                            }

                            else
                            {
                                AnsiConsole.Status()
                                    .Start("Считываю файлы...", ctx =>
                                    {
                                        Thread.Sleep(1500);

                                        foreach (var file in Directory.GetFiles(@"input/"))
                                        {
                                            Parser pars = new Parser(file);

                                            if (pars.Parse() == 0) { ctx.Status($"{Path.GetFileName(file)} успешно отработан"); }

                                            else { ctx.Status($"при отбратотке файла '{Path.GetFileName(file)}' произошла ошибка"); }

                                            Thread.Sleep(500);
                                        }

                                        var p = Process.Start("Converter.exe");
                                        ctx.Status("Конвертирую");
                                        p.WaitForExit();
                                        Thread.Sleep (1000);
                                    });

                                is_dir_mode = false;
                                AnsiConsole.Markup("[green]Успешно[/]");
                                Thread.Sleep(1000);
                            }

                            break;
                        }

                        catch (Exception ex)
                        {
                            AnsiConsole.Markup($"Похоже, у вас открыты файлы для [blue]чтения[/] или [blue]записи[/], произошла ошибка\n\n");
                            var error_panel = new Panel(ex.Message);
                            AnsiConsole.Write(error_panel);

                            Thread.Sleep(2000);
                            break;
                        }


                    case "Настройки":
                        settings_menu();
                        break;
                        
                }
            }

        }
    }
}
