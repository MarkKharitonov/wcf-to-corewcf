using System;
using WCF.SampleService.Contracts;
using CoreWCF;
using CoreWCF.SampleService.BL;

namespace WCF.SampleService.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ISomeBL m_someBL;

        public CalculatorService(ISomeBL someBL) 
        {
            m_someBL = someBL;
        }

        public int Add(int n1, int n2)
        {
            return n1 + n2;
        }

        public int Subtract(int n1, int n2)
        {
            return n1 - n2;
        }

        public int Multiply(int n1, int n2)
        {
            return n1 * n2;
        }

        public int Divide(int n1, int n2)
        {
            try
            {
                return n1 / n2;
            }
            catch (DivideByZeroException)
            {
                throw new FaultException(m_someBL.DivideByZeroErrorMessage);
            }
        }

        public int Factorial(int n)
        {
            if (n < 1)
                throw new FaultException("Invalid Argument: The argument must be greater than zero.");
            int factorial = 1;
            for (int i = 1; i <= n; i++)
            {
                factorial = factorial * i;
            }

            return factorial;
        }
    }
}