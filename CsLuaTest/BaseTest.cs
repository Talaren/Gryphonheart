﻿namespace CsLuaTest
{
    using System;
    using CsLua.Collection;
    using CsLua;
    using Lua;

    public abstract class BaseTest : ITestSuite
    {
        public string Name = "Unnamed";
        public static string Output = "";
        public const bool ContinueOnError = false;
        public static int TestCount;
        public static int FailCount;

        public CsLuaDictionary<string, Action> Tests
        {
            get; protected set;
        }

        public BaseTest()
        {
            this.Tests = new CsLuaDictionary<string, Action>();
        }

        public void PerformTests(IndentedLineWriter lineWriter)
        {
            lineWriter.WriteLine(Name);
            lineWriter.indent++;
            foreach (var testName in this.Tests.Keys)
            {
                var test = this.Tests[testName];

                if (ContinueOnError)
                { 
                    try
                    {
                        TestCount++;
                        ResetOutput();
                        test();
                        lineWriter.WriteLine(testName + " Success");
                    }
                    catch (CsException ex)
                    {
                        FailCount++;
                        lineWriter.WriteLine(testName + " Failed");
                        lineWriter.indent++;
                        lineWriter.WriteLine(ex.Message);
                        lineWriter.indent--;
                    }
                }
                else
                {
                    lineWriter.WriteLine(testName);
                    TestCount++;
                    ResetOutput();
                    test();
                }
            }
            lineWriter.indent--;
        }

        protected static void ResetOutput()
        {
            Output = "";
        }

        protected static void Assert(object expectedValueObj, object actualValueObj)
        {
            var expectedValue = Strings.tostring(expectedValueObj);
            var actualValue = Strings.tostring(actualValueObj);

            if (expectedValue != actualValue)
            {
                throw new CsException(Strings.format("Incorrect value. Expected: '{0}' got: '{1}'.", expectedValue, actualValue));
            }
        }
    }
}