//-----------------------------------------------------------------------
// <copyright file="PreProcessor.IncludeFileTests.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// <summary>Test how Candle handles preprocessing include files.</summary>
//-----------------------------------------------------------------------

namespace WixTest.Tests.Tools.Candle.PreProcessor
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WixTest;
    
    /// <summary>
    /// Test how Candle handles preprocessing include files.
    /// </summary>
    [TestClass]
    public class IncludeFileTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\Tools\Candle\PreProcessor\IncludeFileTests");

        [TestMethod]
        [Description("Verify that Candle can search a specified absolute path for include files.")]
        [Priority(2)]
        public void SearchIncludeFilesWithAbsolutePath()
        {
            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"SearchIncludeFiles\Product.wxs");

            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);

            // Specify the include directory to be an absolute path
            string includeDirectory = Path.Combine(IncludeFileTests.TestDataDirectory, "SharedData");
            
            candle.IncludeSearchPaths.Add(includeDirectory);
            candle.Run();

            Verifier.VerifyWixObjProperty(candle.ExpectedOutputFiles[0], "MyProperty1", "foo");
        }

        [TestMethod]
        [Description("Verify that Candle can search a specified relative path for include files.")]
        [Priority(2)]
        public void SearchIncludeFilesWithRelativePath()
        {
            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"SearchIncludeFiles\Product.wxs");
            string workingDirectory = IncludeFileTests.TestDataDirectory ;

            Candle candle = new Candle();
            candle.WorkingDirectory = workingDirectory;
            candle.SourceFiles.Add(testFile);

            // Specify the include directory to be a relative path
            string includeDirectory = @".\SharedData";
            candle.IncludeSearchPaths.Add(includeDirectory);
            candle.Run();

            Verifier.VerifyWixObjProperty(candle.ExpectedOutputFiles[0], "MyProperty1", "foo");
        }

        [TestMethod]
        [Description("Verify that Candle can handle the case where the include file is missing.")]
        [Priority(2)]
        public void MissingIncludeFiles()
        {
            // Verify that this file does not exist before continuing with the test
            string nonExistentWxiFile =  Path.Combine(IncludeFileTests.TestDataDirectory, @"SearchIncludeFiles\Property1.wxi");

            if (File.Exists(nonExistentWxiFile))
            {
                Assert.Inconclusive("Test cannot continue as Include file {0} exists", nonExistentWxiFile);
            }

            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"SearchIncludeFiles\Product.wxs");
            
            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);
            string outputString = String.Format("The system cannot find the file '{0}' with type 'include'.", Path.GetFileName(nonExistentWxiFile));
            candle.ExpectedWixMessages.Add(new WixMessage(103, outputString, WixMessage.MessageTypeEnum.Error));
            candle.ExpectedExitCode = 103;
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that Candle can preprocess multiple include files included in the wxs file.")]
        [Priority(2)]
        public void MultipleIncludeFiles()
        {
            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"MultipleIncludeFiles\Product.wxs");

            string outputFile = Candle.Compile(testFile);

            Verifier.VerifyWixObjProperty(outputFile, "MyProperty1", "foo");
            Verifier.VerifyWixObjProperty(outputFile, "MyProperty2", "bar");
            Verifier.VerifyWixObjProperty(outputFile, "MyProperty3", "baz");
        }

        [TestMethod]
        [Description("Verify that Candle can preprocess nested include files included in the wxs file.")]
        [Priority(2)]
        public void NestedIncludeFiles()
        {
            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"NestedIncludeFiles\Product.wxs");
            string outputFile = Candle.Compile(testFile);
            Verifier.VerifyWixObjProperty(outputFile, "MyProperty1", "foo");
        }

        [TestMethod]
        [Description("Verify that Candle can preprocess include files that contain environment variables.")]
        [Priority(2)]
        public void IncludeFilesWithEnvVariables()
        {
            string testFile = Path.Combine(IncludeFileTests.TestDataDirectory, @"IncludeFilesWithEnvVariables\Product.wxs");
            string outputFile = Candle.Compile(testFile);
            Verifier.VerifyWixObjProperty(outputFile, "MyProperty1", "foo");
        }
    }
}