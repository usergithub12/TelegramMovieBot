using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient("your client id");

            var me = Bot.GetMeAsync().Result;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQuery;
            Bot.StartReceiving();
            Console.WriteLine(me.FirstName);
            Console.ReadLine();
            Bot.StopReceiving();

        }

        private async static void BotOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} вибрав {buttonText}");
            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Ви обрали: {buttonText}");
        }


        public class Movie
        {
            public string Title { get; set; }

        }
        private async static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                return;
            }
            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} : {message.Text}");

            var keyboard = new ReplyKeyboardMarkup(
    new[] {
        new[]{
            new KeyboardButton("/movie-top-rated-1"),
        },
        new[]{
            new KeyboardButton("/movie-top-rated-2"),
        },
        new[]{
            new KeyboardButton("/movies-coming-soon"),
        },
        new[]{
            new KeyboardButton("/movies-in-theaters"),
        },
        new[]{
            new KeyboardButton("/top-rated-indian-movies-01"),
        },
        new[]{
            new KeyboardButton("/top-rated-indian-movies-02"),
        },
    });
            await Bot.SendTextMessageAsync(message.From.Id, "-----------", replyMarkup: keyboard);
            switch (message.Text)
            {
                case "/start":
                    string text = @"Список команд: 
                                    /start - запуск бота
                                    /menu - вивод меню
                                    /movie-top-rated-1 
                                    /movie-top-rated-2 
                                    /movies-coming-soon
                                    /movies-in-theaters
                                    /top-rated-indian-movies-01
                                    /top-rated-indian-movies-02
                                    ";
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;
                case "/menu":
                    var buttonItem = new[] {  "/movie-top-rated-1",
                                    "/movie-top-rated-2",
                                    "/movies-coming-soon",
                                    "/movies-in-theaters",
                                    "/top-rated-indian-movies-01",
                                    "/top-rated-indian-movies-02" };
                    var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(buttonItem));
                    await Bot.SendTextMessageAsync(message.From.Id, "Виберіть: ", replyMarkup: keyboardMarkup);
                    break;
                case "/movie-top-rated-1":
                    await SendMessageWithMovieAsync("./movie-json-data/json/top-rated-movies-01.json", message.From.Id);
                    break;
                case "/movie-top-rated-2":
                    await SendMessageWithMovieAsync("./movie-json-data/json/top-rated-movies-02.json", message.From.Id);
                    break;
                case "/movies-coming-soon":
                    await SendMessageWithMovieAsync("./movie-json-data/json/movies-coming-soon.json", message.From.Id);
                    break;
                case "/movies-in-theaters":
                    await SendMessageWithMovieAsync("./movie-json-data/json/movies-in-theaters.json", message.From.Id);
                    break;
                case "/top-rated-indian-movies-01":
                    await SendMessageWithMovieAsync("./movie-json-data/json/top-rated-indian-movies-01.json", message.From.Id);
                    break;
                case "/top-rated-indian-movies-02":
                    await SendMessageWithMovieAsync("./movie-json-data/json/top-rated-indian-movies-02.json", message.From.Id);
                    break;
                  
                default:
                  
                    break;
            }
        }

        public static Item LoadJson(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                Random rnd = new Random();
                int randomNumber = rnd.Next(0, items.Count);
                return items[randomNumber];
            }

        }

        public static async Task SendMessageWithMovieAsync(string path, long from)
        {
            var movie = LoadJson(path);
            await Bot.SendTextMessageAsync(from, movie.title);
            if (!String.IsNullOrEmpty(movie.posterurl))
            {
                try
                {
                    await Bot.SendPhotoAsync(from, movie.posterurl);
                }
                catch (Exception)
                {
                    
                }
              
            }

        }

        public class Item
        {
            public string title;
            public string year;
           // public string[] genres;
           // public string[] ratings;
            public string poster;
           // public DateTime releaseDate;
            public string storyline;
            public string imdbRating;
            public string posterurl;
        }



        private static InlineKeyboardButton[][] GetInlineKeyboard(string[] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[1][];
            var keyboardButtons = new InlineKeyboardButton[stringArray.Length];
            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton
                {
                    Text = stringArray[i],
                    CallbackData = "Some Callback Data",
                };
            }
            keyboardInline[0] = keyboardButtons;
            return keyboardInline;
        }
    }
}
