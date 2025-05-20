using MyBackupUtil;

if (!AppUpdater.WindowsUpdate(args))
    ConsoleAppFramework.ConsoleApp.Create().Run(args);