using _01_Threading;

//BackgroundThread backgroundThread = new BackgroundThread();
//backgroundThread.StartBackgroundThread();

//BackgroundThread_Param backgroundThread_Param = new BackgroundThread_Param();
//backgroundThread_Param.StartBackgroundThread();

//BackgroundThread_Pause backgroundThread_Pause = new BackgroundThread_Pause();
//backgroundThread_Pause.StartBackgroundThread();

//Lock_Scync lock_Scync = new Lock_Scync();
//lock_Scync.Run();

//MonitorScyn monitorScyn = new MonitorScyn();
//monitorScyn.Run();

//Network_scheduler Network_Scheduler = new Network_scheduler();
//Network_Scheduler.Run();

CancelationTokenSource cancelationTokenSource = new CancelationTokenSource();
cancelationTokenSource.Run();