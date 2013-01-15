namespace DemoWithDiagnostics.Tests
{
    using System;
    using System.Threading;

    using NUnit.Framework;

    public class Tests
    {
        private static Random random = new Random();
         
        [Test]
        public void CanConnectToDb()
        {
            Thread.Sleep(random.Next(0,3000));

            throw new System.Data.DataException("Unable to connect to our rest feed");
        }

        [Test]
        public void CanConnectToRestService()
        {
            Thread.Sleep(random.Next(0,3000));
        }
    }
}