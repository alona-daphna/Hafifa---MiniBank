﻿using MiniBank.Controllers;
using MiniBank.Models;
using Serilog;

namespace MiniBank.Utils
{
    internal class SessionManager
    {
        internal User LoggedUser { get; set; }
        internal bool IsUserLoggedIn => LoggedUser != null;
        private ColorWriter ColorWriter { get; set; } = new ColorWriter();
        private ILogger Logger { get; set; } = MiniBankLogger.GetInstance().Logger;


        internal void Authorize(Action action)
        {
            if (!IsUserLoggedIn)
            {
                ColorWriter.DisplayErrorMessage("You must login first.");

                return;
            }

            action();
        }

        internal void Authenticate()
        {
            if (IsUserLoggedIn)
            {
                LogOut();
            } else
            {
                LogIn();
            }
        }


        private void LogIn()
        { 
            var userID = ColorWriter.GetValidInputString("Enter User ID: ");

            var password = ColorWriter.GetValidInputString("Enter password: ");

            var (status, user, _) = new UserController().GetByID(userID);

            if (status == Enums.OperationStatus.Success)
            {
                if (new PasswordManager().VerifyPassword(password, user.Password))
                {
                    LoggedUser = user;
                    Logger.Information("User {id} logged in", user.ID);
                } else
                {
                    Logger.Information("Failed login attempt for user {id}", user.ID);
                    ColorWriter.DisplayErrorMessage("Incorrect password.");
                }
            } else
            {
                ColorWriter.DisplayErrorMessage("User does not exist.");
            }
        }


        private void LogOut()
        {
            LoggedUser = null;
            Logger.Information("User {id} logged out", LoggedUser.ID);
        }
    }
}