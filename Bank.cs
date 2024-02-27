using MiniBank.Controllers;
using MiniBank.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank
{
    internal class Bank
    {
        private AccountView AccountView { get; set; }
        private MainView MainView { get; set; } 
        private Dictionary<int, Action> Actions { get; set; }

        public Bank()
        {
            MainView = new MainView();
            AccountView =  new AccountView();
            Actions = new Dictionary<int, Action>()
            {
                {1, AccountView.DisplayAllAccounts }
            };

        }

        internal void Start()
        {
            // main loop
            MainView.DisplayMenu();
            MainView.SelectAction();

            // call controller method from dictionary that maps int to Action
        }
    }
}
