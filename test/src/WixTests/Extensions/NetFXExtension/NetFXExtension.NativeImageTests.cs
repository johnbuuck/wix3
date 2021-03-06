//-----------------------------------------------------------------------
// <copyright file="NetFXExtension.NativeImageTests.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// <summary>NetFX Extension NativeImage tests</summary>
//-----------------------------------------------------------------------

namespace WixTest.Tests.Extensions.NetFXExtension
{
    using System;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;

    using WixTest;
    using WixTest.Verifiers;
    using WixTest.Verifiers.Extensions;

    /// <summary>
    /// NetFX extension NativeImage element tests
    /// </summary>
    [TestClass]
    public class NativeImageTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\Extensions\NetFXExtension\NativeImageTests");

        [TestMethod]
        [Description("Verify that the (NetFxNativeImage,CustomAction) Tables are created in the MSI and have expected data.")]
        [Priority(1)]
        public void NativeImage_VerifyMSITableData()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            Verifier.VerifyCustomActionTableData(msiFile,
                new CustomActionTableData("NetFxScheduleNativeImage", 1, "NetFxCA", "SchedNetFx"),
                new CustomActionTableData("NetFxExecuteNativeImageInstall", 3137, "NetFxCA", "ExecNetFx"),
                new CustomActionTableData("NetFxExecuteNativeImageCommitInstall", 3649, "NetFxCA", "ExecNetFx"),
                new CustomActionTableData("NetFxExecuteNativeImageUninstall", 3137, "NetFxCA", "ExecNetFx"),
                new CustomActionTableData("NetFxExecuteNativeImageCommitUninstall", 3649, "NetFxCA", "ExecNetFx"));

            Verifier.VerifyTableData(msiFile, MSITables.NetFxNativeImage,
                new TableRow(NetFxNativeImageColumns.NetFxNativeImage.ToString(), "private_assembly"),
                new TableRow(NetFxNativeImageColumns.File_.ToString(), "TestNetFxProductFile"),
                new TableRow(NetFxNativeImageColumns.Priority.ToString(), "3", false),
                new TableRow(NetFxNativeImageColumns.Attributes.ToString(), "24", false),
                new TableRow(NetFxNativeImageColumns.File_Application.ToString(), string.Empty),
                new TableRow(NetFxNativeImageColumns.Directory_ApplicationBase.ToString(), string.Empty));

            Verifier.VerifyTableData(msiFile, MSITables.NetFxNativeImage,
               new TableRow(NetFxNativeImageColumns.NetFxNativeImage.ToString(), "gac_assembly"),
               new TableRow(NetFxNativeImageColumns.File_.ToString(), "TestNetFxProductFile2"),
               new TableRow(NetFxNativeImageColumns.Priority.ToString(), "0", false),
               new TableRow(NetFxNativeImageColumns.Attributes.ToString(), "8", false),
               new TableRow(NetFxNativeImageColumns.File_Application.ToString(), string.Empty),
               new TableRow(NetFxNativeImageColumns.Directory_ApplicationBase.ToString(), string.Empty));

            Verifier.VerifyTableData(msiFile, MSITables.NetFxNativeImage,
             new TableRow(NetFxNativeImageColumns.NetFxNativeImage.ToString(), "private_assembly2"),
             new TableRow(NetFxNativeImageColumns.File_.ToString(), "TestNetFxProductFile3"),
             new TableRow(NetFxNativeImageColumns.Priority.ToString(), "3", false),
             new TableRow(NetFxNativeImageColumns.Attributes.ToString(), "15", false),
             new TableRow(NetFxNativeImageColumns.File_Application.ToString(), string.Empty),
             new TableRow(NetFxNativeImageColumns.Directory_ApplicationBase.ToString(), string.Empty));
        }

        [TestMethod]
        [Description("Verify that the file was ngened and exists in the nativeimage folder.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void NativeImage_Install()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
            
            // make sure all the assymblies have been ngened
            NgenQueuedBinaries(NetFXVerifier.FrameworkArch.x86);

            Assert.IsTrue(NetFXVerifier.NativeImageExists("WixTasks.dll", NetFXVerifier.FrameworkVersion.NetFX20, NetFXVerifier.FrameworkArch.x86), "Native Image '{0}' was not created on Install", "WixTasks.dll");
            // Verify actions in the log file 
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageCommitUninstall"), "Could not find NetFxExecuteNativeImageUninstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageUninstall"), "Could not find NetFxExecuteNativeImageUninstall Skipping message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageCommitInstall"), "Could not find NetFxExecuteNativeImageInstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageInstall"), "Could not find NetFxExecuteNativeImageInstall Skipping message in log file: '{0}'.", logFile);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            Assert.IsFalse(NetFXVerifier.NativeImageExists("WixTasks.dll", NetFXVerifier.FrameworkVersion.NetFX20, NetFXVerifier.FrameworkArch.x86), "Native Image '{0}' was not removed on Uninstall", "WixTasks.dll");
        }

        [TestMethod]
        [Description("Verify that the file was ngened and exists in the 64 bit nativeimage folder.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        [TestProperty("Is64BitSpecificTest", "true")]
        public void NativeImage_Install_64bit()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product_64.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // make sure all the assymblies have been ngened
            NgenQueuedBinaries(NetFXVerifier.FrameworkArch.x64);
            Assert.IsTrue(NetFXVerifier.NativeImageExists("WixTasks.dll", NetFXVerifier.FrameworkVersion.NetFX20, NetFXVerifier.FrameworkArch.x64), "Native Image '{0}' was not created on Install", "WixTasks.dll");

            // Verify actions in the log file 
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageCommitUninstall"), "Could not find NetFxExecuteNativeImageUninstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageUninstall"), "Could not find NetFxExecuteNativeImageUninstall Skipping message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageCommitInstall"), "Could not find NetFxExecuteNativeImageInstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageInstall"), "Could not find NetFxExecuteNativeImageInstall Skipping message in log file: '{0}'.", logFile);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            Assert.IsFalse(NetFXVerifier.NativeImageExists("WixTasks.dll", NetFXVerifier.FrameworkVersion.NetFX20, NetFXVerifier.FrameworkArch.x64), "Native Image '{0}' was not removed on Uninstall", "WixTasks.dll");
        }

        [TestMethod]
        [Description("Verify using the msilog that the correct actions were skipped and performed.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void NativeImage_DisableWindowsInstallerRollback_Install()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            //Disable Windows Installer RollBack
            NativeImageTests.DisableWindowsInstallerRollBack();

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Verify actions in the log file 
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageUninstall"), "Could not find NetFxExecuteNativeImageUninstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageCommitUninstall"), "Could not find NetFxExecuteNativeImageCommitUninstall Skipping message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Doing action: NetFxExecuteNativeImageInstall"), "Could not find NetFxExecuteNativeImageInstall Doing message in log file: '{0}'.", logFile);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, "Skipping action: NetFxExecuteNativeImageCommitInstall"), "Could not find NetFxExecuteNativeImageCommitInstall Skipping message in log file: '{0}'.", logFile);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }
        
        [TestMethod]
        [Description("Verify that the netfx roolback removes the nativeimages.")]
        [Priority(3)]
        [TestProperty("IsRuntimeTest", "true")]
        public void NativeImage_InstallFailure()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product_fail.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.ERROR_INSTALL_FAILURE);

            // make sure all the assymblies have been ngened
            NgenQueuedBinaries(NetFXVerifier.FrameworkArch.x86);

            Assert.IsFalse(NetFXVerifier.NativeImageExists("WixTasks.dll", NetFXVerifier.FrameworkVersion.NetFX20, NetFXVerifier.FrameworkArch.x86), "Native Image '{0}' was not removed on Rollback", "WixTasks.dll");
        }

        [TestMethod]
        [Description("Verify using the log that the NGEN is picked from the latest frmework folder.")]
        [Priority(3)]
        [TestProperty("IsRuntimeTest", "true")]
        public void NativeImage_VerifyNgenPath()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            string expectedPath = GetNgenPath(NetFXVerifier.FrameworkArch.x86);
            Assert.IsTrue(LogVerifier.MessageInLogFile(logFile, expectedPath), "Could not find expected ngen path in log file: '{0}'.", logFile);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify using the log that the NGEN command line contains the following�/Debug /Profile /NoDependencies.")]
        [Priority(3)]
        [TestProperty("IsRuntimeTest", "true")]
        public void NativeImage_VerifyCommandLineParameters()
        {
            string sourceFile = Path.Combine(NativeImageTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixNetFXExtension");

            string logFile = MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            Assert.IsTrue(LogVerifier.MessageInLogFileRegex(logFile, @"ngen[\.]exe\sinstall(.*)WiXTasks[\.]dll(.*)/Debug\s/Profile\s/NoDependencies"), "Could not find expected CommandLine paramters in log file: '{0}'.", logFile);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestCleanup]
        public override void CleanUp()
        {
            NativeImageTests.EnableWindowsInstallerRollBack();
            // make sure to call the base class cleanup method
            base.CleanUp();
        }

        #region Helper Methods

        /// <summary>
        /// Disable Windows Installer Roll Back
        /// </summary>
        private static void DisableWindowsInstallerRollBack()
        {
            string registryKeyPath = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\Installer";
            string subKey = "DisableRollback";
            Registry.SetValue(registryKeyPath, subKey, 1);
        }

        /// <summary>
        /// Enable Windows Installer Roll Back
        /// </summary>
        private static void EnableWindowsInstallerRollBack()
        {
            string registryKeyPath = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\Installer";
            string subKey = "DisableRollback";
            Registry.SetValue(registryKeyPath, subKey, 0);
        }

        /// <summary>
        /// Call Ngen with executeQueuedItems parameters to make sure that all assemblies in the queue have been processed
        /// </summary>
        /// <param name="arch">64 bit or 32 bit</param>
        private static void NgenQueuedBinaries(NetFXVerifier.FrameworkArch arch)
        {
            string ngenPath = GetNgenPath(arch);

            // run ngen
            Tool ngen = new Tool(ngenPath, " executeQueuedItems ");
            ngen.WorkingDirectory = Environment.CurrentDirectory;
            Result result = ngen.Run();
            Console.WriteLine(result.ToString());
        }

        /// <summary>
        /// Find the latest Ngen.exe to use
        /// </summary>
        /// <param name="arch">64 bit or 32 bit</param>
        /// <returns>Path to the latest ngen.exe</returns>
        private static string GetNgenPath(NetFXVerifier.FrameworkArch arch)
        {
            string frameworkFolderName;
            if (NetFXVerifier.FrameworkArch.x86 == arch)
            {
                frameworkFolderName = Environment.ExpandEnvironmentVariables(@"%windir%\Microsoft.NET\Framework");
            }
            else if (NetFXVerifier.FrameworkArch.x64 == arch)
            {
                frameworkFolderName = Environment.ExpandEnvironmentVariables(@"%windir%\Microsoft.NET\Framework64");
            }
            else
            {
                return null;
            }

            DirectoryInfo frameworkFolder = new DirectoryInfo(frameworkFolderName);
            FileInfo[] ngenFileList = frameworkFolder.GetFiles("ngen.exe", SearchOption.AllDirectories);

            if (null == ngenFileList || ngenFileList.Length < 1)
            {
                return null;
            }

            FileInfo latestNgenFile = ngenFileList[0];
            string version = FileVersionInfo.GetVersionInfo(latestNgenFile.FullName).ProductVersion;
            foreach (FileInfo ngenFile in ngenFileList)
            {
                if (FileVersionInfo.GetVersionInfo(ngenFile.FullName).ProductVersion.CompareTo(version) >0)
                {
                    latestNgenFile = ngenFile;
                    version = FileVersionInfo.GetVersionInfo(ngenFile.FullName).ProductVersion;
                }
            }

            return latestNgenFile.FullName;
        }
        #endregion
    }
}
