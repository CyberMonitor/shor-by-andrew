using System;
using Microsoft.Quantum.Simulation.Simulators;

namespace Shor
{
    public class Factoriser
    {
        // Find r from the dyadic numerator using continued fractions
        // If r is odd pick a new a
        // If a ^ (r / 2) is -1 mod n then pick a new a
        // Find GCD of (a^(r/2) and N) and of (a^(r/2) and N) these are the factors
        private GreatestCommonDenominator gcd;
        private ContinuedFractions cf;
        public Factoriser() {
            gcd = new GreatestCommonDenominator();
            cf = new ContinuedFractions();
        }
        internal (int, int) factorise(int numberToFactorise)
        {
            if (numberToFactorise < 2)
            {
                throw new ArgumentException();
            }
            else if (numberToFactorise % 2 == 0)
            {
                return (2, numberToFactorise / 2);
            }
            else 
            {
                return factoriseWithShors(numberToFactorise);
            }
        }

        private (int, int) factoriseWithShors(int numberToFactorise)
        {
            int s = 1;
            int a = new Random().Next(3, numberToFactorise);
            int r = 0;
            int gcdOfAAndN = gcd.findGCD(a, numberToFactorise);
            if (gcdOfAAndN != 1) {
                return (gcdOfAAndN, numberToFactorise / gcdOfAAndN);
            }
            else {
                while ((int) Math.Pow(a, s) % numberToFactorise != 1)
                {
                    Console.WriteLine("modulo loop");
                    while (r == 0)
                    {
                        Console.WriteLine("Inside r == 0 loop");
                        using (var qsim = new QuantumSimulator())
                        {
                            r = (int)FindNumerator.Run(qsim, a, numberToFactorise).Result;
                        }
                    }
                    s = cf.findS(r, (int) Math.Pow(2, 2 * Math.Ceiling(Math.Log(numberToFactorise, 2))), numberToFactorise);
                }

                Console.WriteLine(s);
                Console.WriteLine(a);

                return (gcd.findGCD((int) Math.Pow(a, s / 2) - 1, numberToFactorise), gcd.findGCD((int) Math.Pow(a, s / 2) + 1,  numberToFactorise));
            }
        }
    }
}