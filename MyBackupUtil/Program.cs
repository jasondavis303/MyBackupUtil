using MyBackupUtil;

if (!AppUpdater.WaitAndUpdate(args))
    ConsoleAppFramework.ConsoleApp.Create().Run(args);