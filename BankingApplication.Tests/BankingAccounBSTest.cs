﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankingApplication.BLL.BusinessServicesInterfaces;
using Moq;
using BankingApplication.DAL;
using BankingApplication.Entities;
using System.Data.Entity;
using System.Linq;
using BankingApplication.BLL.BusinessServices;
using BankingApplication.Common;

namespace BankingApplication.Tests
{

    [TestClass]
    public class BankingAccounBSTest
    {

        private IBankingAccount _bankingAccount;
        Mock<ApplicationDbContext> _mockContext;
        Mock<DbContextFactory> _mockContextFactory;
        Mock<DbSet<Transaction>> _mockSet;

        IList<Transaction> transactions;

        [TestInitialize]
        public void Initialize()
        {
            transactions = new List<Transaction>()
            {
                new Transaction {Amount = 400,DateTime = DateTime.Now.AddDays(1),UserFrom = null, UserTo = "user1@test.com" },
                new Transaction { Amount = 100,DateTime = DateTime.Now.AddDays(1),UserFrom = "user1@test.com", UserTo = null },
                new Transaction {Amount = 100,DateTime = DateTime.Now.AddDays(1),UserFrom = "user1@test.com", UserTo = "user2@test.com" },
            };

            _mockSet = new Mock<DbSet<Transaction>>();

            _mockSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transactions.AsQueryable().Provider);
            _mockSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transactions.AsQueryable().Expression);
            _mockSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transactions.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transactions.GetEnumerator());
            _mockSet.Setup(m => m.Add(It.IsAny<Transaction>())).Callback<Transaction>(transactions.Add);


            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.Transactions).Returns(_mockSet.Object);
            
            _mockContextFactory = new Mock<DbContextFactory>();

            _bankingAccount = new BankingAccountBS(_mockContextFactory.Object);
        }

        [TestMethod]
        public void Get_User_Data()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);

            //Act
            var result = _bankingAccount.GetUserData("user1@test.com");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AvailableAmount, 200);
            Assert.AreEqual(result.History.Count, 3);
        }

        [TestMethod]
        public void Deposit_Money_Service()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);

            //Act
             _bankingAccount.DepositMoney("user1@test.com",100);

            //Assert
            Assert.AreEqual(transactions.Count(), 4);
            Assert.AreEqual(transactions.Last().Amount,100 );
            Assert.AreEqual(transactions.Last().UserTo, "user1@test.com");
        }

        [TestMethod]
        public void Withdraw_Money_Service()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);


            //Act
            _bankingAccount.WithdrawMoney("user1@test.com", 100);

            //Assert
            Assert.AreEqual(transactions.Count(), 4);
            Assert.AreEqual(transactions.Last().Amount, 100);
            Assert.AreEqual(transactions.Last().UserFrom, "user1@test.com");
        }

        [TestMethod]
        [ExpectedException(typeof(NotEnoughMoneyException))]
        public void Withdraw_Money_Service_Not_Enough_Money()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);

            //Act
            _bankingAccount.WithdrawMoney("user1@test.com", 500);

            //Assert
        }

        [TestMethod]
        public void Send_Money_Service()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);


            //Act
            _bankingAccount.SendMoney("user1@test.com", "user2@test.com", 100,"test");

            //Assert
            Assert.AreEqual(transactions.Count(), 4);
            Assert.AreEqual(transactions.Last().Amount, 100);
            Assert.AreEqual(transactions.Last().UserFrom, "user1@test.com");
            Assert.AreEqual(transactions.Last().UserTo, "user2@test.com");
        }

        [TestMethod]
        [ExpectedException(typeof(NotEnoughMoneyException))]
        public void Send_Money_Service_Not_Enough_Money()
        {
            //Arrange
            _mockContextFactory.Setup(x => x.Create()).Returns(_mockContext.Object);

            //Act
            _bankingAccount.SendMoney("user1@test.com", "user2@test.com", 500, "test");

            //Assert
        }
    }
}
