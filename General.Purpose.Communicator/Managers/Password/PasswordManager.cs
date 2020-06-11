using General.Purpose.Communicator.Interfaces;
using System;
using System.Text;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace General.Purpose.Communicator.Managers.Password
{
    /// <summary>
    /// Class to generate password and send it to specified users via TelegramBot
    /// </summary>
    public class PasswordManager : IPasswordManager
    {
        public string Password { get; private set; }

        private readonly Timer _timer;
        private static ITelegramBotClient _botClient;
        private static MessageEventArgs _e;

        public PasswordManager()
        {
            _timer = new Timer
            {
                AutoReset = true,
                Interval = TimeSpan.FromHours(24).TotalMilliseconds
            };

            _timer.Elapsed += CreateNewPassword;
            _timer.Start();

            Password = "test";
        }

        private void CreateNewPassword(Object source, ElapsedEventArgs e)
        {
            const int length = 10;
            const string pool = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+";

            var rnd = new Random();
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[rnd.Next(0, pool.Length)];
                builder.Append(c);
            }

            SendToHexaBotUsers(builder.ToString());
        }

        private void SendToHexaBotUsers(string password)
        {
            Password = password;

            _botClient = new TelegramBotClient("1296798539:AAFIYCRljM80J193Jlk1k4PmxchQog2EumA");
            _botClient.SendTextMessageAsync(-368508167, "Ваш пароль для чата: " + password);
        }
    }
}
